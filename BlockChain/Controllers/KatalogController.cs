using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BlockChain.Controllers
{
    public class KatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Katalog/
        public IActionResult Index(string search)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            var produkList = _context.Inventaris
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                produkList = produkList.Where(p =>
                    p.NamaProduk.Contains(search) ||
                    p.Satuan.Contains(search) ||
                    p.User.NamaToko.Contains(search));
            }

            var result = produkList.Select(p => new InventarisViewModel
            {
                Id = p.Id,
                NamaToko = p.User.NamaToko,
                NamaProduk = p.NamaProduk,
                Satuan = p.Satuan,
                HargaSatuan = p.HargaSatuan,
                Stok = p.Stok,
                TanggalExpired = p.TanggalExpired,
                GambarProdukUrl = p.GambarProdukUrl
            }).ToList();

            return View(result);
        }

        // POST: /Katalog/Konfirmasi
        [HttpPost]
        public IActionResult Konfirmasi([FromForm] List<int> selectedIds, [FromForm] Dictionary<string, string> jumlahDibeli)
        {
            if (selectedIds == null || selectedIds.Count == 0)
            {
                return View("Konfirmasi", new List<InventarisViewModel>());
            }

            var produkTerpilih = _context.Inventaris
                .Include(p => p.User)
                .Where(p => selectedIds.Contains(p.Id))
                .ToList();

            var jumlahDibeliParsed = new Dictionary<int, int>();
            foreach (var item in jumlahDibeli)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                {
                    if (int.TryParse(item.Key, out int id) && int.TryParse(item.Value, out int jumlah))
                    {
                        jumlahDibeliParsed[id] = jumlah;
                    }
                }
            }

            // Tampilkan produk untuk dikonfirmasi (tidak membuat notifikasi)
            var viewModel = produkTerpilih.Select(p => new InventarisViewModel
            {
                Id = p.Id,
                NamaToko = p.User.NamaToko,
                NamaProduk = p.NamaProduk,
                Satuan = p.Satuan,
                HargaSatuan = p.HargaSatuan,
                Stok = p.Stok,
                TanggalExpired = p.TanggalExpired,
                GambarProdukUrl = p.GambarProdukUrl
            }).ToList();

            return View("Konfirmasi", viewModel);
        }


        [HttpPost]
        public IActionResult Beli(IFormCollection form)
        {
            var jumlahDibeli = new Dictionary<int, int>();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("jumlah["))
                {
                    var idString = key.Substring(7, key.Length - 8); // Ambil ID dari 'jumlah[ID]'
                    var valueString = form[key];
                    if (!string.IsNullOrWhiteSpace(idString) && !string.IsNullOrWhiteSpace(valueString))
                    {
                        if (int.TryParse(idString, out int id) && int.TryParse(valueString, out int jumlah))
                        {
                            jumlahDibeli[id] = jumlah;
                        }
                    }
                }
            }


            TempData["selectedIds"] = string.Join(",", jumlahDibeli.Keys);
            TempData["jumlahDibeli"] = string.Join(";", jumlahDibeli.Select(kv => $"{kv.Key}:{kv.Value}"));

            return RedirectToAction("KonfirmasiFromTempData");
        }
        [HttpGet]
        public IActionResult KonfirmasiFromTempData()
        {
            if (TempData["selectedIds"] is not string ids || TempData["jumlahDibeli"] is not string jumlahStr)
            {
                return RedirectToAction("Index");
            }

            var selectedIds = ids.Split(',').Select(int.Parse).ToList();
            var jumlahDibeli = jumlahStr.Split(';')
                .Select(s => s.Split(':'))
                .Where(parts => parts.Length == 2 && int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _))
                .ToDictionary(parts => int.Parse(parts[0]), parts => int.Parse(parts[1]));

            // Panggil method Konfirmasi yang kamu buat untuk proses data, tapi jangan return View
            // Sebaiknya buat method internal baru yang proses konfirmasi tanpa return IActionResult
            ProsesKonfirmasi(selectedIds, jumlahDibeli);

            TempData["SuccessMessage"] = "Produk berhasil dikonfirmasi dan notifikasi telah dikirim.";

            return RedirectToAction("Index");
        }

        // Buat method private untuk proses konfirmasi tanpa return view, biar bisa dipanggil ulang
        private void ProsesKonfirmasi(List<int> selectedIds, Dictionary<int, int> jumlahDibeliParsed)
        {
            try
            {
                if (selectedIds == null || selectedIds.Count == 0) return;

                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == null) return;

                var produkTerpilih = _context.Inventaris
                    .Include(p => p.User)
                    .Where(p => selectedIds.Contains(p.Id))
                    .ToList();

                var notifikasiBaru = new Notifikasi
                {
                    Role = "Owner",
                    Tanggal = DateTime.Now,
                    Pesan = "Gudang telah memilih produk untuk dikonfirmasi",
                    Kategori = "Pembelian",
                    UserId = currentUserId.Value
                };

                _context.Notifikasi.Add(notifikasiBaru);
                _context.SaveChanges();

                foreach (var produk in produkTerpilih)
                {
                    if (produk.User == null) continue;

                    int jumlah = jumlahDibeliParsed.ContainsKey(produk.Id) ? jumlahDibeliParsed[produk.Id] : 0;
                    if (jumlah <= 0) continue;

                    var detail = new NotifikasiPembelianDetail
                    {
                        NotifikasiId = notifikasiBaru.Id,
                        NamaProduk = produk.NamaProduk,
                        JumlahDibeli = jumlah,
                        Satuan = produk.Satuan,
                        HargaSatuan = produk.HargaSatuan,
                        TotalHarga = produk.HargaSatuan * jumlah,
                        Stok = produk.Stok,
                        TanggalExpired = produk.TanggalExpired,
                        DistributorId = produk.UserId // FIX foreign key di sini
                    };

                    _context.NotifikasiPembelianDetail.Add(detail);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saat simpan notifikasi: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public JsonResult SearchProduk(string keyword)
        {
            var produkList = _context.Inventaris
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                produkList = produkList.Where(p =>
                    p.NamaProduk.Contains(keyword) ||
                    p.Satuan.Contains(keyword) ||
                    p.User.NamaToko.Contains(keyword));
            }

            var result = produkList.Select(p => new
            {
                p.Id,
                p.NamaProduk,
                p.Satuan,
                p.HargaSatuan,
                p.Stok,
                p.TanggalExpired,
                p.GambarProdukUrl,
                NamaToko = p.User.NamaToko
            }).ToList();

            return Json(result);
        }
    }
}
