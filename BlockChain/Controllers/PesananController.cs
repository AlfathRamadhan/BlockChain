﻿using BlockChain.Models;
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
        private string GenerateNomorPembayaran()
        {
            return "PAY-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }


        public PesananController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userIdNullable = HttpContext.Session.GetInt32("UserId");
            if (!userIdNullable.HasValue)
                return RedirectToAction("Login", "Account");

            int userId = userIdNullable.Value;


            var pesananList = _context.TransaksiKeuangan
                .Where(t => t.DistributorId == userId && t.Status == "Konfirmasi")
                .OrderByDescending(t => t.Tanggal)
                .Select(t => new PesananViewModel
                {
                    Transaksi = t,
                    NamaToko = _context.Users
                        .Where(u => u.Id == t.DistributorId)
                        .Select(u => u.NamaToko)
                        .FirstOrDefault(),
                    NotifikasiDetail = _context.Notifikasi
                        .Include(n => n.NotifikasiPembelianDetail)
                        .Where(n => n.TransaksiKeuanganId == t.ID && n.UserId == userId)
                        .SelectMany(n => n.NotifikasiPembelianDetail)
                        .ToList()

                })
                .ToList(); // jangan lupa ini agar data benar-benar dieksekusi

            ViewBag.UserId = userId;
            ViewBag.TotalPesanan = pesananList.Count;

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
