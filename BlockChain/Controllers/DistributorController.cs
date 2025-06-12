using Microsoft.AspNetCore.Mvc;
using BlockChain.Models; // Sesuaikan namespace model kamu
using System.Linq;

namespace BlockChain.Controllers
{
    public class DistributorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DistributorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mengambil data distributor
        public IActionResult Distributorowner()
        {
            // Mengambil semua distributor dari tabel Users berdasarkan role
            var distributors = _context.Users
                .Where(u => u.Role == "Distributor")
                .ToList();

            return View(distributors);
        }
        [HttpPost]
        public IActionResult Verifikasi(int id)
        {
            var distributor = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Distributor");
            if (distributor != null)
            {
                distributor.IsVerified = true;
                _context.SaveChanges();
                TempData["status"] = "Distributor berhasil diverifikasi.";
            }
            return RedirectToAction("Distributorowner");
        }

        [HttpPost]
        public IActionResult Tolak(int id)
        {
            var distributor = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Distributor");
            if (distributor != null)
            {
                _context.Users.Remove(distributor);
                _context.SaveChanges();
                TempData["status"] = "Distributor ditolak dan dihapus.";
            }
            return RedirectToAction("Distributorowner");
        }


    }
}
