using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BlockChain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlockChain.Controllers
{
    public class DashboardKeuanganController : Controller
    {
        // Dummy data untuk contoh transaksi dengan daftar produk
        private static List<TransaksiKeuangan> _dataDummy = new List<TransaksiKeuangan>
        {
            new TransaksiKeuangan
            {
                ID = 1,
                NomorPembayaran = "INV-20250505-001",
                Supplier = "PT. Maju Jaya",
                Tanggal = new DateTime(2025, 5, 5),
                Jumlah = 12450000,
                Status = "Sudah Dibayar",
                MetodePembayaran = "Transfer Bank",
                Produk = new List<ItemProduk>
                {
                    new ItemProduk
                    {
                        NamaProduk = "Produk A",
                        Satuan = "Batang",
                        JumlahUnit = 100,
                        HargaSatuan = 124500
                    }
                }
            },
            new TransaksiKeuangan
            {
                ID = 2,
                NomorPembayaran = "INV-20250505-002",
                Supplier = "Produk Bahan",
                Tanggal = new DateTime(2025, 5, 5),
                Jumlah = 6450000,
                Status = "Belum Dibayar",
                MetodePembayaran = "Tunai",
                Produk = new List<ItemProduk>
                {
                    new ItemProduk
                    {
                        NamaProduk = "Produk B",
                        Satuan = "Sak",
                        JumlahUnit = 60,
                        HargaSatuan = 107500
                    }
                }
            }
        };

        // Menampilkan halaman utama dengan daftar transaksi
        public IActionResult Index()
        {
            return View(_dataDummy);
        }

        // Mengambil detail invoice berdasarkan ID dalam format JSON
        [HttpGet]
        public IActionResult GetInvoiceById(int id)
        {
            var transaksi = _dataDummy.FirstOrDefault(t => t.ID == id);
            if (transaksi == null)
                return NotFound();

            var result = new
            {
                transaksi.ID,
                transaksi.NomorPembayaran,
                transaksi.Supplier,
                transaksi.Tanggal,
                transaksi.Jumlah,
                transaksi.Status,
                transaksi.MetodePembayaran,
                Produk = transaksi.Produk.Select(p => new
                {
                    p.NamaProduk,
                    p.Satuan,
                    p.JumlahUnit,
                    p.HargaSatuan
                }).ToList()
            };

            return Json(result);
        }

        // Upload bukti pembayaran untuk transaksi tertentu
        [HttpPost]
        public IActionResult UploadBuktiPembayaran(int transaksiId, IFormFile buktiPembayaran)
        {
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

                TempData["Message"] = "Bukti pembayaran berhasil diupload!";
            }

            return RedirectToAction("Index");
        }

        // Menampilkan halaman cetak invoice PDF
        [HttpGet]
        public IActionResult PrintInvoicePDF(int id)
        {
            var transaksi = _dataDummy.FirstOrDefault(t => t.ID == id);
            if (transaksi == null)
                return NotFound();

            return View("InvoicePrint", transaksi);
        }
    }
}
