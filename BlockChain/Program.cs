using BlockChain.Models;
using BlockChain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Tambah layanan MVC
builder.Services.AddControllersWithViews();

// Konfigurasi session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set timeout sesi
    options.Cookie.HttpOnly = true; // Cookie hanya dapat diakses oleh HTTP, bukan JavaScript
    options.Cookie.IsEssential = true; // Menandakan bahwa cookie sesi esensial untuk aplikasi
});

// Konfigurasi koneksi ke database SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfigurasi autentikasi dengan cookie (gunakan jika Anda memerlukan autentikasi berbasis cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Rute untuk login
        options.LogoutPath = "/Account/Logout"; // Rute untuk logout
        options.AccessDeniedPath = "/Account/AccessDenied"; // Rute untuk akses yang ditolak
    });

// Konfigurasi email (jika diperlukan)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Middleware untuk menangani error di luar mode development
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Mengaktifkan HTTP Strict Transport Security
}

// Gunakan pengalihan ke HTTPS jika diperlukan
app.UseHttpsRedirection();

// Gunakan file statis seperti CSS, JS, gambar
app.UseStaticFiles();

// Gunakan routing untuk menentukan kontroler dan aksi
app.UseRouting();

// Middleware untuk sesi (pastikan di sini agar sesi dapat diakses)
app.UseSession();

// Middleware autentikasi untuk memverifikasi pengguna yang telah login
app.UseAuthentication(); // Penting untuk autentikasi berbasis cookie

// Middleware otorisasi untuk membatasi akses ke halaman yang memerlukan autentikasi
app.UseAuthorization();

// Routing ke controller dan aksi sesuai pola URL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Jalankan aplikasi
app.Run();
