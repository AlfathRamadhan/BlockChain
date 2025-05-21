using BlockChain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlockChain.Controllers
{
    public class DashboardDistributorController : Controller
    {
        public IActionResult DashboardDistributor()
        {
            // Data dummy untuk testing tampilan
            var transaksi = new List<TransaksiKeuangan>
            {
                new TransaksiKeuangan
                {
                    NomorPembayaran = "INV001",
                    Tanggal = DateTime.Now.AddDays(-2),
                    Jumlah = 150000,
                    Status = "Sudah Dibayar"
                },
                new TransaksiKeuangan
                {
                    NomorPembayaran = "INV002",
                    Tanggal = DateTime.Now.AddDays(-1),
                    Jumlah = 200000,
                    Status = "Belum Dibayar"
                },
                new TransaksiKeuangan
                {
                    NomorPembayaran = "INV003",
                    Tanggal = DateTime.Now,
                    Jumlah = 100000,
                    Status = "Gagal"
                }
            };

            return View(transaksi);
        }
    }
}
