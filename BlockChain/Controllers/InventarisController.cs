using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class InventarisController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventarisController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Errors"] = "User belum login atau sesi habis.";
            return RedirectToAction("Index");
        }

        ViewBag.DebugUserId = userId;

        var items = await _context.Inventaris
            .Include(i => i.User)
            .Where(i => i.UserId == userId) // <-- tambahkan filter ini
            .Select(i => new InventarisViewModel
            {
                Id = i.Id,
                NamaProduk = i.NamaProduk,
                Stok = i.Stok,
                Satuan = i.Satuan,
                HargaSatuan = i.HargaSatuan,
                TanggalExpired = i.TanggalExpired,
                GambarProdukUrl = i.GambarProdukUrl,
                NamaToko = i.User != null ? i.User.NamaToko : "-"
            })
            .ToListAsync();

        return View("Index", items);
    }


    [HttpPost]
    public async Task<IActionResult> Tambah(Inventaris model, IFormFile GambarProduk)
    {
        if (GambarProduk == null || GambarProduk.Length == 0)
        {
            TempData["Errors"] = "Gambar produk wajib di-upload.";
            return RedirectToAction("Index");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(GambarProduk.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await GambarProduk.CopyToAsync(fileStream);
        }

        model.GambarProdukUrl = "/images/" + fileName;

        // Ambil UserId dari sesi (bukan klaim)
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["Errors"] = "User belum login atau sesi habis.";
            return RedirectToAction("Index");
        }

        model.UserId = userId.Value;

        try
        {
            _context.Inventaris.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Produk berhasil ditambahkan.";
        }
        catch (Exception ex)
        {
            TempData["Errors"] = "Gagal menambahkan produk: " + ex.Message;
        }

        return RedirectToAction("Index");
    }


    [HttpPost]
        public async Task<IActionResult> Edit(Inventaris model)
        {
            var item = await _context.Inventaris.FindAsync(model.Id);
            if (item == null)
            {
                return NotFound();
            }

            // Update nilai properti
            item.NamaProduk = model.NamaProduk;
            item.Stok = model.Stok;
            item.Satuan = model.Satuan;
            item.HargaSatuan = model.HargaSatuan;
            item.TanggalExpired = model.TanggalExpired;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Produk berhasil diubah.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Hapus(int id)
        {
            var item = await _context.Inventaris.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(item.GambarProdukUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.GambarProdukUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Inventaris.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Produk berhasil dihapus.";
            return RedirectToAction("Index");
        }
    }

