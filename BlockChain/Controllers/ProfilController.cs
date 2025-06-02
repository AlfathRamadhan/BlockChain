using BlockChain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace BlockChain.Controllers
{
    public class ProfilController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ProfilController(IWebHostEnvironment env, ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _env = env;
            _context = context;
            _passwordHasher = passwordHasher;
        }


        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new EditProfileViewModel
            {
                NamaToko = user.NamaToko,
                Email = user.Email,
                NoHp = user.NoHp,
                Alamat = user.Alamat ?? "-",
                Deskripsi = user.Deskripsi ?? "-",
                NoRekening = user.NoRekening ?? "-",
                Bank = user.Bank ?? "-",
                NamaLengkap = user.NamaLengkap ?? "-",
                Username = user.Username,
                PasswordBaru = "", // Untuk keamanan
                LogoPath = !string.IsNullOrEmpty(user.LogoPath) ? "/images/logos/" + user.LogoPath : "/images/logo.png"
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new EditProfileViewModel
            {
                NamaToko = user.NamaToko,
                Email = user.Email,
                NoHp = user.NoHp,
                Alamat = user.Alamat,
                Deskripsi = user.Deskripsi,
                NoRekening = user.NoRekening,
                Bank = user.Bank,
                NamaLengkap = user.NamaLengkap,
                Username = user.Username,
                LogoPath = !string.IsNullOrEmpty(user.LogoPath) ? "/images/logos/" + user.LogoPath : "/images/logo.png"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Update properti
            user.NamaToko = model.NamaToko;
            user.Email = model.Email;
            user.NoHp = model.NoHp;
            user.Alamat = model.Alamat;
            user.Deskripsi = model.Deskripsi;
            user.NoRekening = model.NoRekening;
            user.Bank = model.Bank;
            user.NamaLengkap = model.NamaLengkap;
            user.Username = model.Username;

            if (!string.IsNullOrEmpty(model.PasswordBaru))
            {
                user.KataSandi = _passwordHasher.HashPassword(user, model.PasswordBaru);
            }


            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(model.LogoFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("LogoFile", "Format logo harus JPG, JPEG, PNG, atau GIF.");
                    return View(model);
                }

                var logoFolder = Path.Combine(_env.WebRootPath, "images", "logos");
                if (!Directory.Exists(logoFolder))
                    Directory.CreateDirectory(logoFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(logoFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(fs);
                }

                // Hapus logo lama
                if (!string.IsNullOrEmpty(user.LogoPath))
                {
                    var oldPath = Path.Combine(logoFolder, user.LogoPath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                user.LogoPath = fileName;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil berhasil diperbarui.";

            // Tampilkan ulang dengan data terbaru
            var updatedModel = new EditProfileViewModel
            {
                NamaToko = user.NamaToko,
                Email = user.Email,
                NoHp = user.NoHp,
                Alamat = user.Alamat,
                Deskripsi = user.Deskripsi,
                NoRekening = user.NoRekening,
                Bank = user.Bank,
                NamaLengkap = user.NamaLengkap,
                Username = user.Username,
                LogoPath = !string.IsNullOrEmpty(user.LogoPath) ? "/images/logos/" + user.LogoPath : "/images/logo.png"
            };

            return View(updatedModel);
        }
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    }
}
