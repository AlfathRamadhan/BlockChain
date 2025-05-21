using BlockChain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Controllers
{
    public class PenggunaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public PenggunaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Cek login sebelum menjalankan aksi lain
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        // Tampilkan semua pengguna
        public IActionResult Pengguna(string? search)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect ke halaman login jika tidak ada sesi
            }

            var pengguna = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                pengguna = pengguna
                    .OrderByDescending(u => u.Username.Contains(search)) // yang cocok ditaruh di atas
                    .Where(u => u.Username.Contains(search) || string.IsNullOrEmpty(search));
            }

            return View(pengguna.ToList());
        }

        // Fungsi untuk hash password menggunakan SHA256
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Tambah atau Edit pengguna (POST dari Modal)
        [HttpPost]
        public IActionResult Tambah(User user)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect ke halaman login jika tidak ada sesi
            }

            var usernameExist = _context.Users
                .Any(u => u.Username == user.Username && u.Id != user.Id); // pengecekan selain dirinya sendiri (saat edit)

            if (usernameExist)
            {
                TempData["ErrorMessage"] = "Username sudah digunakan. Silakan gunakan username lain.";
                return RedirectToAction("Pengguna");
            }

            if (user.Id == 0)
            {
                user.KataSandi = _passwordHasher.HashPassword(user, user.KataSandi);
                _context.Users.Add(user);
            }
            else
            {
                var existing = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null)
                {
                    existing.Username = user.Username;
                    existing.NamaToko = user.NamaToko;
                    existing.Role = user.Role;

                    if (!string.IsNullOrEmpty(user.KataSandi))
                    {
                        existing.KataSandi = _passwordHasher.HashPassword(existing, user.KataSandi);
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Pengguna");
        }

        // API untuk ambil data user by ID (untuk modal edit via AJAX)
        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect ke halaman login jika tidak ada sesi
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return Json(user); // kirim dalam format JSON
        }

        // Filter pengguna berdasarkan pencarian
        [HttpGet]
        public IActionResult FilterPengguna(string search)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect ke halaman login jika tidak ada sesi
            }

            var pengguna = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                pengguna = pengguna
                    .OrderByDescending(u => u.Username.Contains(search))
                    .Where(u => u.Username.Contains(search));
            }

            var result = pengguna.Select(u => new
            {
                u.Id,
                u.Username,
                u.NamaToko,
                u.Role
            }).ToList();

            return Json(result);
        }

        // Hapus pengguna
        public IActionResult Hapus(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect ke halaman login jika tidak ada sesi
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Pengguna");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Hapus seluruh sesi user
            HttpContext.Session.Clear();

            // Menghapus cookie sesi
            Response.Cookies.Delete("ASP.NETCore.Session");

            // Redirect ke halaman login dan pastikan browser tidak bisa kembali ke halaman sebelumnya
            return RedirectToAction("Login", "Account");
        }
    }
}
