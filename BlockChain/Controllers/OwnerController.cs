using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Globalization;

namespace BlockChain.Controllers
{
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            // Menonaktifkan cache agar data selalu fresh
            Response.Headers["Cache-Control"] = "no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            // Cek sesi login
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Ambil jumlah user berdasarkan role dari database
            var keuanganCount = _context.Users.Count(u => u.Role == "Keuangan");
            var gudangCount = _context.Users.Count(u => u.Role == "Gudang");
            var distributorCount = _context.Users.Count(u => u.Role == "Distributor");

            // Ambil 5 produk terbaru dari tabel Produk
            var produkList = _context.Produk
                .OrderByDescending(p => p.TanggalMasuk)
                .Take(5)
                .ToList();

            // Kirim data ke view
            ViewBag.Keuangan = keuanganCount;
            ViewBag.Gudang = gudangCount;
            ViewBag.Distributor = distributorCount;
            ViewBag.DataProduk = produkList;

            return View();
        }
        // Aksi lain yang memerlukan login
        public IActionResult AnotherPage()
        {
            // Cek sesi login
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
