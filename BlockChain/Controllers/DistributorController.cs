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
    }
}
