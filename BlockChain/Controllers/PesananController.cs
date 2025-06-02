using BlockChain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BlockChain.Controllers
{
    public class PesananController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PesananController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Menampilkan daftar pesanan milik distributor yang sedang login
        public IActionResult Index()
        {


            var username = User.Identity?.Name;

            var pesananList = _context.TransaksiKeuangan
                .Include(t => t.Produk)
                .Where(t => t.Supplier == username) // Diasumsikan 'Supplier' menyimpan username distributor
                .ToList();

            return View(pesananList);
        }

        // Ambil detail invoice berdasarkan ID (JSON)
        [HttpGet]
        public IActionResult GetInvoiceById(int id)
        {
            var pesanan = _context.TransaksiKeuangan
                .Include(p => p.Produk)
                .FirstOrDefault(p => p.ID == id);

            if (pesanan == null)
                return NotFound();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };

            return new JsonResult(pesanan, options);
        }

        // Upload bukti pembayaran
        [HttpPost]
        public IActionResult UploadBuktiPembayaran(int pesananId, IFormFile buktiPembayaran)
        {
            var pesanan = _context.TransaksiKeuangan.FirstOrDefault(p => p.ID == pesananId);
            if (pesanan == null)
                return NotFound("Pesanan tidak ditemukan.");

            if (buktiPembayaran != null && buktiPembayaran.Length > 0)
            {
                var fileName = Path.GetFileName(buktiPembayaran.FileName);
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    buktiPembayaran.CopyTo(stream);
                }

                // Jika ada properti BuktiPembayaranPath, tambahkan ke model
                // pesanan.BuktiPembayaranPath = "/uploads/" + fileName;

                pesanan.Status = "Sudah Dibayar";
                _context.SaveChanges();

                TempData["Message"] = "Bukti pembayaran berhasil diupload.";
            }

            return RedirectToAction("Index");
        }

        // Tampilkan halaman cetak invoice PDF
        [HttpGet]
        public IActionResult PrintPesananPDF(int id)
        {
            var pesanan = _context.TransaksiKeuangan
                .Include(p => p.Produk)
                .FirstOrDefault(p => p.ID == id);

            if (pesanan == null)
                return NotFound();

            var username = User.Identity?.Name;

            // Ambil data distributor dari tabel Users (diasumsikan)
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound("Data pelanggan tidak ditemukan.");

            var pelangganInfo = new PelangganInfoViewModel
            {
                NamaToko = user.NamaToko,
                NamaLengkap = user.NamaLengkap,
                Alamat = user.Alamat,
                NoHp = user.NoHp,
                Email = user.Email,
                Bank = user.Bank,
                NoRekening = user.NoRekening,
                LogoPath = user.LogoPath
            };

            ViewBag.Pesanan = pesanan;
            return View("PesananPrint", pelangganInfo);
        }
    }
}
