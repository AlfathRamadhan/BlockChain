using Microsoft.AspNetCore.Mvc;
using BlockChain.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlockChain.Services;
using BlockChain.Models.ViewModels;

namespace BlockChain.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly IEmailSender _emailSender;

        public AccountController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IEmailSender emailSender)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _emailSender = emailSender; // Dependency injection for email service
        }

        // GET: Halaman Login
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Dashboard", "Owner");
            }
            return View();
        }

        // POST: Proses Login
        // POST: Proses Login
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            Username = Username?.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == Username);

            if (user == null)
            {
                ViewBag.Error = "Akun tidak ditemukan.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.KataSandi, Password);

            if (result != PasswordVerificationResult.Success)
            {
                ViewBag.Error = "Kata sandi salah.";
                return View();
            }

            // Simpan data user ke session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("NamaToko", user.NamaToko ?? "");
            HttpContext.Session.SetString("Role", user.Role);  // Menyimpan role ke session

            // Arahkan berdasarkan role pengguna
            if (user.Role == "Owner")
            {
                return RedirectToAction("Dashboard", "Owner");  // Arahkan ke dashboard Owner
            }
            else if (user.Role == "Gudang")
            {
                return RedirectToAction("DashboardGudang", "DashboardGudang");  // Arahkan ke dashboard Gudang
            }
            else if (user.Role == "Distributor")
            {
                return RedirectToAction("DashboardDistributor", "DashboardDistributor");  // Arahkan ke dashboard Distributor
            }
            else if (user.Role == "Keuangan")
            {
                return RedirectToAction("Index", "DashboardKeuangan");  // Arahkan ke dashboard Keuangan
            }

            // Jika role tidak dikenali, redirect ke halaman default atau error
            return RedirectToAction("Account", "Login");
        }



        // GET: Halaman Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Proses Register
        // POST: Proses Register
        [HttpPost]
        [HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // Validasi NamaToko wajib karena default role = Distributor
    if (string.IsNullOrWhiteSpace(model.NamaToko))
    {
        ModelState.AddModelError("NamaToko", "Nama Toko wajib diisi.");
        return View(model);
    }

    // Cek duplikasi Username
    var existingUserByUsername = await _context.Users
        .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower());
    if (existingUserByUsername != null)
    {
        ModelState.AddModelError("Username", "Username sudah digunakan.");
        return View(model);
    }

    // Cek duplikasi Email
    var existingUserByEmail = await _context.Users
        .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
    if (existingUserByEmail != null)
    {
        ModelState.AddModelError("Email", "Email sudah digunakan.");
        return View(model);
    }

    // Upload logo
    string uniqueFileName = null;
    if (model.LogoFile != null)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logos");
        Directory.CreateDirectory(uploadsFolder);
        uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.LogoFile.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await model.LogoFile.CopyToAsync(fileStream);
        }
    }

    // Simpan ke tabel Users
    var user = new User
    {
        NamaToko = model.NamaToko,
        Username = model.Username,
        Email = model.Email,
        NoHp = model.NoHp,
        Kategori = model.Kategori,
        LogoPath = uniqueFileName,
        KataSandi = _passwordHasher.HashPassword(null, model.KataSandi),
        Role = "Distributor" // default role
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return RedirectToAction("Login");
}

        public IActionResult Success()
        {
            return View();
        }

        // GET: Halaman Lupa Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username, string email, string noHp)
        {
            if (string.IsNullOrWhiteSpace(username) || (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(noHp)))
            {
                ModelState.AddModelError("", "Username dan salah satu dari Email atau No HP harus diisi.");
                return View();
            }

            username = username.Trim().ToLower();
            email = email?.Trim().ToLower();
            noHp = noHp?.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username &&
                    (u.Email.ToLower() == email || u.NoHp == noHp));

            if (user == null)
            {
                ViewBag.Error = "Username dan Email atau No HP tidak ditemukan.";
                return View();
            }

            string verificationCode = GenerateVerificationCode();

            string contactInfo = !string.IsNullOrEmpty(email) ? email : user.Email;

            string subject = "Kode Verifikasi Reset Password";
            string message = $@"
        <html>
        <body>
            <p>Halo <strong>{user.Username}</strong>,</p>
            <p>Kode verifikasi Anda:</p>
            <h2>{verificationCode}</h2>
            <p>Jangan bagikan kode ini ke siapa pun.</p>
        </body>
        </html>";

            await _emailSender.SendEmailAsync(contactInfo, subject, message);

            HttpContext.Session.SetString("VerificationCode", verificationCode);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("VerifyCode");
        }
        private string GenerateVerificationCode()
        {
            // Anda bisa menggunakan logika untuk membuat kode verifikasi secara acak
            // Misalnya, kode 6 digit acak
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }


        // GET: Halaman Verifikasi Kode
        // GET: Halaman Verifikasi Kode
        [HttpGet]
        public IActionResult VerifyCode()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email"); // Ambil email dari session
            ViewBag.ExpectedCode = HttpContext.Session.GetString("VerificationCode");
            return View();
        }

        // POST: Proses Verifikasi Kode
        // POST: Proses Verifikasi Kode
        [HttpPost]
        public IActionResult VerifyCode(string[] CodeDigit)
        {
            // Gabungkan semua digit menjadi satu string
            string code = string.Join("", CodeDigit);
            Console.WriteLine($"Kode yang Dimasukkan: {code}");

            // Ambil kode verifikasi yang disimpan di session
            string expectedCode = HttpContext.Session.GetString("VerificationCode");
            Console.WriteLine($"Kode yang Diharapkan: {expectedCode}");

            if (code == expectedCode)
            {
                // Jika kode benar, redirect ke halaman reset password
                return RedirectToAction("ResetPassword", new { email = HttpContext.Session.GetString("Email") });
            }

            // Jika kode salah
            ViewBag.Message = "Kode verifikasi salah.";
            return View();
        }


        // GET: Halaman Reset Password
        // GET: Halaman Reset Password
        // GET: Halaman Reset Password
        [HttpGet]
        public IActionResult ResetPassword()
        {
            string email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // POST: Proses Reset Password
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Email = model.Email;
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (user == null)
            {
                ViewBag.Message = "Pengguna tidak ditemukan.";
                ViewBag.Email = model.Email;
                return View(model);
            }

            user.KataSandi = _passwordHasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            // Clear session setelah reset password untuk memastikan pengguna tidak otomatis login
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }




        // GET: Logout
        // GET: Logout
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
