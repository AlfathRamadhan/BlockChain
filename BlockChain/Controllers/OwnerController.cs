using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Globalization;
using System.Linq;

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
            Response.Headers["Cache-Control"] = "no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            var produkList = _context.Produk
                .OrderByDescending(p => p.TanggalMasuk)
                .Take(5)
                .ToList();

            ViewBag.DataProduk = produkList;

            return View();
        }

        public IActionResult AnotherPage()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult GrafikPemakaianProduk()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public JsonResult DataPemakaianProduk(int tahun, int bulan)
        {

            Console.WriteLine($"Masuk ke DataPemakaianProduk: Tahun={tahun}, Bulan={bulan}");
            var data = _context.PemakaianProduk
                .Include(p => p.Produk)
                .Where(p => p.TanggalPenggunaan.Year == tahun && p.TanggalPenggunaan.Month == bulan)
                .GroupBy(p => p.Produk.Nama)
                .Select(g => new {
                    NamaProduk = g.Key,
                    Total = g.Sum(x => x.JumlahDigunakan),
                    Satuan = g.Select(x => x.Produk.Satuan).FirstOrDefault()
                })
                .ToList();
            Console.WriteLine("Jumlah Data:" + data.Count);

            return Json(data);
        }




        public IActionResult GrafikPengeluaranProduk()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
