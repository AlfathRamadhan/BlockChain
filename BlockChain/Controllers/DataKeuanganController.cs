using BlockChain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BlockChain.Controllers
{
    public class DataKeuanganController : Controller
    {
        // Dummy data transaksi keuangan dengan list produk di setiap transaksi
        private static List<TransaksiKeuangan> _dummyData = new List<TransaksiKeuangan>
        {
            new TransaksiKeuangan
            {
                ID = 1,
                NomorPembayaran = "INV-20240518-002",
                Supplier = "Toko Sumber Rejeki",
                Tanggal = DateTime.Now.AddDays(-3),
                Jumlah = 1250000,
                Status = "Sudah Dibayar",
                MetodePembayaran = "Transfer Bank",
                Produk = new List<ItemProduk>
                {
                    new ItemProduk
                    {
                        NamaProduk = "Cat Tembok",
                        Satuan = "Pcs",
                        JumlahUnit = 10,
                        HargaSatuan = 125000
                    }
                }
            },
            new TransaksiKeuangan
            {
                ID = 2,
                NomorPembayaran = "INV-20240518-003",
                Supplier = "CV Baja Prima",
                Tanggal = DateTime.Now.AddDays(-1),
                Jumlah = 780000,
                Status = "Belum Dibayar",
                MetodePembayaran = "Tunai",
                Produk = new List<ItemProduk>
                {
                    new ItemProduk
                    {
                        NamaProduk = "Semen",
                        Satuan = "Sak",
                        JumlahUnit = 12,
                        HargaSatuan = 65000
                    }
                }
            }
        };

        // Menampilkan halaman utama transaksi keuangan
        public IActionResult Index()
        {
            return View(_dummyData);
        }

        // Mengambil detail invoice berdasarkan ID dalam format JSON dengan PascalCase
        [HttpGet]
        public IActionResult GetInvoiceById(int id)
        {
            var transaksi = _dummyData.FirstOrDefault(t => t.ID == id);
            if (transaksi == null)
                return NotFound();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };

            return new JsonResult(transaksi, options);
        }

        // Upload bukti pembayaran
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

        // Menampilkan halaman cetak invoice
        [HttpGet]
        public IActionResult PrintInvoicePDF(int id)
        {
            var transaksi = _dummyData.FirstOrDefault(t => t.ID == id);
            if (transaksi == null)
                return NotFound();

            return View("InvoicePrint", transaksi);
        }
    }
}
