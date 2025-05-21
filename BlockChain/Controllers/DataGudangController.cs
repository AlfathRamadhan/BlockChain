using BlockChain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;


public class DataGudangController : Controller
{
    private readonly ApplicationDbContext _context;

    public DataGudangController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search, string sortDate, string filterStok)
    {
        var produkList = _context.Produk.AsQueryable();

        // Filter pencarian nama
        if (!string.IsNullOrEmpty(search))
        {
            var cocok = produkList.Where(p => p.Nama.Contains(search));
            var tidakCocok = produkList.Where(p => !p.Nama.Contains(search));
            produkList = cocok.Concat(tidakCocok);
        }

        // Urutkan stok (bukan filter)
        if (filterStok == "habis")
        {
            produkList = produkList.OrderBy(p => p.Stok);
        }
        else if (filterStok == "banyak")
        {
            produkList = produkList.OrderByDescending(p => p.Stok);
        }

        // Urutan tanggal
        if (sortDate == "terbaru")
        {
            produkList = produkList.OrderByDescending(p => p.TanggalMasuk);
        }
        else if (sortDate == "terlama")
        {
            produkList = produkList.OrderBy(p => p.TanggalMasuk);
        }

        return View(produkList.ToList());
    }

    [HttpGet]
    public JsonResult SearchJson(string term)
    {
        var produkList = _context.Produk
            .OrderByDescending(p => p.TanggalMasuk)
            .ToList();

        if (!string.IsNullOrEmpty(term))
        {
            var cocok = produkList
                .Where(p => p.Nama.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var tidakCocok = produkList
                .Where(p => !p.Nama.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            produkList = cocok.Concat(tidakCocok).ToList();
        }

        var result = produkList.Select(p => new
        {
            p.KodeProduk,
            p.Nama,
            p.Stok,
            TanggalMasuk = p.TanggalMasuk.ToString("dd MMMM yyyy"),
            TanggalExpired = p.TanggalExpired.ToString("dd MMMM yyyy")
        });

        return Json(result);
    }

    public static string GenerateKodeProduk()
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString().Substring(7);
        var rand = new Random().Next(100, 999);
        return $"PRD-{timestamp}{rand}";
    }

    // POST: /Gudang/Tambah
    [HttpPost]
    public IActionResult Tambah(Produk produk)
    {
        if (ModelState.IsValid)
        {
            // Jika kode produk sudah ada, buat ulang
            while (_context.Produk.Any(p => p.KodeProduk == produk.KodeProduk))
            {
                var random = new Random();
                var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString().Substring(7);
                var rand = random.Next(100, 999);
                produk.KodeProduk = $"PRD-{timestamp}{rand}";
            }

            _context.Produk.Add(produk);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        var listProduk = _context.Produk.ToList();
        return View("Index", listProduk);
    }
    // POST: /Gudang/Hapus
    [HttpPost]
    public IActionResult Hapus(int id)
    {
        var produk = _context.Produk.Find(id);
        if (produk != null)
        {
            try
            {
                _context.Produk.Remove(produk);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        return Json(new { success = false, message = "Produk tidak ditemukan" });
    }

}
