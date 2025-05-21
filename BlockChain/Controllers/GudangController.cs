using BlockChain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlockChain.Controllers
{
    public class GudangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GudangController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Gudangowner(string search, string filterStok, string sortDate)
        {
            var query = _context.Produk.AsQueryable();

            // 🔍 Filter pencarian
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Nama.Contains(search));
            }

            // 📦 Urutan stok saja, bukan filter jumlahnya
            if (!string.IsNullOrEmpty(filterStok))
            {
                if (filterStok == "habis")
                {
                    query = query.OrderBy(p => p.Stok); // stok sedikit dulu
                }
                else if (filterStok == "banyak")
                {
                    query = query.OrderByDescending(p => p.Stok); // stok banyak dulu
                }
            }

            // 📅 Urutan tanggal (prioritas urutan bisa diatur)
            if (!string.IsNullOrEmpty(sortDate))
            {
                if (sortDate == "terbaru")
                {
                    query = query.OrderByDescending(p => p.TanggalMasuk);
                }
                else if (sortDate == "terlama")
                {
                    query = query.OrderBy(p => p.TanggalMasuk);
                }
            }

            // JSON jika AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var result = query.Select(p => new {
                    kodeProduk = p.KodeProduk,
                    nama = p.Nama,
                    stok = p.Stok,
                    tanggalMasuk = p.TanggalMasuk,
                    tanggalExpired = p.TanggalExpired
                }).ToList();

                return Json(result);
            }

            // Return View jika bukan AJAX
            return View(query.ToList());
        }

    }
}
