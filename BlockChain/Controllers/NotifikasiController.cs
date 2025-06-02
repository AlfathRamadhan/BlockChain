using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BlockChain.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace BlockChain.Controllers
{
    public class NotifikasiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotifikasiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Notifikasi()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("Role");

            var gabunganNotifikasi = _context.Notifikasi
                .Where(n => n.Role == role && n.Kategori != "Pembelian")
                .OrderByDescending(n => n.Tanggal)
                .Select(n => new NotifikasiGabunganItemViewModel
                {
                    Id = n.Id,
                    Tanggal = n.Tanggal,
                    Role = n.Role,
                    Pesan = n.Pesan,
                    IsPembelian = false,
                    TransaksiKeuanganId = null,
                    NamaProduk = new List<string>(),
                    Stok = new List<int>(),
                    Satuan = new List<string>(),
                    HargaSatuan = new List<decimal>(),
                    JumlahDibeli = new List<int>(),
                    TotalHarga = new List<decimal>(),
                    TanggalExpired = new List<DateTime?>()
                })
                .ToList();

            var pembelianNotifikasiList = _context.Notifikasi
                .Where(n => n.Role == role && n.Kategori == "Pembelian")
                .OrderByDescending(n => n.Tanggal)
                .Include(n => n.NotifikasiPembelianDetail)
                .ToList();

            var notifikasiPembelian = pembelianNotifikasiList.Select(n => new NotifikasiGabunganItemViewModel
            {
                Id = n.Id,
                Tanggal = n.Tanggal,
                Role = n.Role,
                Pesan = n.Pesan,
                IsPembelian = true,
                TransaksiKeuanganId = n.NotifikasiPembelianDetail?.FirstOrDefault()?.TransaksiKeuanganId,
                NamaProduk = n.NotifikasiPembelianDetail?.Select(d => d.NamaProduk).ToList() ?? new List<string>(),
                Stok = n.NotifikasiPembelianDetail?.Select(d => d.Stok).ToList() ?? new List<int>(),
                Satuan = n.NotifikasiPembelianDetail?.Select(d => d.Satuan).ToList() ?? new List<string>(),
                HargaSatuan = n.NotifikasiPembelianDetail?.Select(d => d.HargaSatuan).ToList() ?? new List<decimal>(),
                JumlahDibeli = n.NotifikasiPembelianDetail?.Select(d => d.JumlahDibeli).ToList() ?? new List<int>(),
                TotalHarga = n.NotifikasiPembelianDetail?.Select(d => d.TotalHarga).ToList() ?? new List<decimal>(),
                TanggalExpired = n.NotifikasiPembelianDetail?.Select(d => d.TanggalExpired).ToList() ?? new List<DateTime?>()
            }).ToList();

            int totalProdukPerluKonfirmasi = notifikasiPembelian.Sum(n => n.NamaProduk?.Count ?? 0);

            gabunganNotifikasi.AddRange(notifikasiPembelian);

            var model = new NotifikasiGabunganViewModel
            {
                NotifikasiGabungan = gabunganNotifikasi.OrderByDescending(n => n.Tanggal).ToList(),
                TotalProdukPerluKonfirmasi = totalProdukPerluKonfirmasi
            };

            return role switch
            {
                "Owner" => View("Notifikasi", model),
                "Distributor" => View("Distributor", model),
                "Gudang" => View("Gudang", model),
                "Keuangan" => View("Keuangan", model),
                _ => View("Default", model),
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KonfirmasiPembelian(int id)
        {
            var notif = _context.Notifikasi
                .Include(n => n.NotifikasiPembelianDetail)
                .FirstOrDefault(n => n.Id == id);
            if (notif == null)
                return NotFound();

            notif.Status = "Dikonfirmasi";
            _context.SaveChanges();

            // Tambahkan notifikasi ke Gudang beserta detail pembelian
            var notifGudang = new Notifikasi
            {
                Role = "Gudang",
                Kategori = "Pembelian", // penting agar dikenali sebagai pembelian
                Pesan = $"Pembelian dengan ID {id} telah dikonfirmasi oleh Owner.",
                Tanggal = DateTime.Now,
                Status = "Dikonfirmasi" // opsional, tapi baik untuk kejelasan
            };

            _context.Notifikasi.Add(notifGudang);
            _context.SaveChanges(); // untuk mendapatkan notifGudang.Id

            // Salin detail pembelian dari notif Owner ke notifGudang
            foreach (var detail in notif.NotifikasiPembelianDetail)
            {
                var detailBaru = new NotifikasiPembelianDetail
                {
                    NotifikasiId = notifGudang.Id,
                    TransaksiKeuanganId = detail.TransaksiKeuanganId,
                    NamaProduk = detail.NamaProduk,
                    Stok = detail.Stok,
                    Satuan = detail.Satuan,
                    HargaSatuan = detail.HargaSatuan,
                    JumlahDibeli = detail.JumlahDibeli,
                    TotalHarga = detail.TotalHarga,
                    TanggalExpired = detail.TanggalExpired
                };
                _context.NotifikasiPembelianDetail.Add(detailBaru);
            }

            _context.SaveChanges();

            var notifDistributor = new Notifikasi
            {
                Role = "Distributor",
                Kategori = "Pesanan",
                Pesan = $"Pesanan Terbaru.",
                Tanggal = DateTime.Now,
                Status = "Dikonfirmasi"
            };
            _context.Notifikasi.Add(notifDistributor);
            _context.SaveChanges();

            foreach (var detail in notif.NotifikasiPembelianDetail)
            {
                var detailBaru = new NotifikasiPembelianDetail
                {
                    NotifikasiId = notifDistributor.Id,
                    TransaksiKeuanganId = detail.TransaksiKeuanganId,
                    NamaProduk = detail.NamaProduk,
                    Stok = detail.Stok,
                    Satuan = detail.Satuan,
                    HargaSatuan = detail.HargaSatuan,
                    JumlahDibeli = detail.JumlahDibeli,
                    TotalHarga = detail.TotalHarga,
                    TanggalExpired = detail.TanggalExpired
                };
                _context.NotifikasiPembelianDetail.Add(detailBaru);
            }

            _context.SaveChanges();


            return Ok(new { message = "Pembelian dikonfirmasi dan notifikasi dikirim ke Gudang dan Distributor." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TolakPembelian(int id)
        {
            var notif = _context.Notifikasi
                .Include(n => n.NotifikasiPembelianDetail)
                .FirstOrDefault(n => n.Id == id);

            if (notif == null)
                return NotFound();

            notif.Status = "Ditolak";
            _context.SaveChanges();

            // Buat notifikasi ke Gudang
            var notifGudang = new Notifikasi
            {
                Role = "Gudang",
                Kategori = "Pembelian",
                Pesan = $"Pembelian dengan ID {id} telah DITOLAK oleh Owner.",
                Tanggal = DateTime.Now
            };

            _context.Notifikasi.Add(notifGudang);
            _context.SaveChanges();

            // Salin detail produk ke notifikasi baru
            if (notif.NotifikasiPembelianDetail != null)
            {
                var detailCopy = notif.NotifikasiPembelianDetail.Select(d => new NotifikasiPembelianDetail
                {
                    NotifikasiId = notifGudang.Id,
                    NamaProduk = d.NamaProduk,
                    Stok = d.Stok,
                    Satuan = d.Satuan,
                    HargaSatuan = d.HargaSatuan,
                    JumlahDibeli = d.JumlahDibeli,
                    TotalHarga = d.TotalHarga,
                    TanggalExpired = d.TanggalExpired,
                    TransaksiKeuanganId = d.TransaksiKeuanganId
                }).ToList();

                _context.NotifikasiPembelianDetail.AddRange(detailCopy);
                _context.SaveChanges();
            }

            return Ok(new { message = "Pembelian ditolak dan notifikasi dikirim ke Gudang dengan detail produk." });
        }


        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNotification(int id)
        {
            var notif = _context.Notifikasi.Include(n => n.NotifikasiPembelianDetail).FirstOrDefault(n => n.Id == id);
            if (notif == null)
                return NotFound();

            if (notif.NotifikasiPembelianDetail != null && notif.NotifikasiPembelianDetail.Any())
            {
                _context.NotifikasiPembelianDetail.RemoveRange(notif.NotifikasiPembelianDetail);
            }

            _context.Notifikasi.Remove(notif);
            _context.SaveChanges();

            return Ok();
        }

    }
}
