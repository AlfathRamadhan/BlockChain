using BlockChain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;


namespace BlockChain.Controllers
{
    public class DashboardGudangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardGudangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult DashboardGudang()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            // Ambil data dari database
            var produkList = _context.Produk.ToList();

            ViewBag.ProdukMasuk = produkList.Count;
            ViewBag.ProdukKeluar = produkList.Count(p => p.TanggalExpired < DateTime.Now);
            ViewBag.TotalStok = produkList.Sum(p => p.Stok);

            return View(produkList);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("ASP.NETCore.Session");
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            return RedirectToAction("Login", "Account");
        }
    }
}
