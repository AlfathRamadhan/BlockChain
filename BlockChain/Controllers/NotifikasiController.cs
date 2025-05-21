using Microsoft.AspNetCore.Mvc;
using BlockChain.Models;
using System;
using System.Collections.Generic;

namespace BlockChain.Controllers
{
    public class NotifikasiController : Controller
    {
        public IActionResult Notifikasi()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var notifikasiList = new List<Notifikasi>
            {
                new Notifikasi { Id = 1, Pesan = "Pelacakan Produk 6544246890", Tanggal = new DateTime(2025, 4, 21), Kategori = "Pelacakan" },
                new Notifikasi { Id = 2, Pesan = "Permintaan Verifikasi Pendaftaran Distributor", Tanggal = new DateTime(2025, 4, 21), Kategori = "Pendaftaran" }
            };

            return View(notifikasiList);
        }
    }
}
