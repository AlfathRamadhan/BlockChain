using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BlockChain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BlockChain.Controllers
{
    public class KeuanganController : Controller
    {
        private static List<TransaksiKeuangan> _dataDummy = new List<TransaksiKeuangan>
        {
            new TransaksiKeuangan
            {
                ID = 1,
                NomorPembayaran = "INV-20240518-001",
                Supplier = "Toko Sumber Jaya",
                Tanggal = DateTime.Now.AddDays(-5),
                Jumlah = 250000,
                Status = "Sudah Dibayar",
                MetodePembayaran = "Transfer",
                Produk = new List<ItemProduk>
                {
                    new ItemProduk
                    {
                        NamaProduk = "Beras Medium",
                        Satuan = "Karung",
                        JumlahUnit = 1,
                        HargaSatuan = 250000
                    },

                    new ItemProduk
            {
                NamaProduk = "Gula Pasir",
                Satuan = "Sak",
                JumlahUnit = 3,
                HargaSatuan = 150000
            },
            new ItemProduk
            {
                NamaProduk = "Minyak Goreng",
                Satuan = "Dus",
                JumlahUnit = 4,
                HargaSatuan = 100000
            },
            new ItemProduk
            {
                NamaProduk = "Tepung Terigu",
                Satuan = "Karung",
                JumlahUnit = 2,
                HargaSatuan = 120000
            },
            new ItemProduk
            {
                NamaProduk = "Kopi Bubuk",
                Satuan = "Dus",
                JumlahUnit = 1,
                HargaSatuan = 80000
            }

                    // Anda bisa tambahkan produk lain di sini
                    // new ItemProduk { NamaProduk = "...", Satuan = "...", JumlahUnit = ..., HargaSatuan = ... }
                }
            }
        };

        // Menampilkan halaman utama dengan daftar transaksi
        public IActionResult Keuanganowner()
        {
            return View(_dataDummy);
        }

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

            return RedirectToAction("Keuanganowner");
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
