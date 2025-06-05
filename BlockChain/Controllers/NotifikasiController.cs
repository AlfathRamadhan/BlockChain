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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("Role");



            // Ambil notifikasi non-pembelian (kategori bukan Pembelian dan Pesanan)
            var gabunganNotifikasi = _context.Notifikasi
                .Where(n => n.Role == role && n.UserId == userId && n.Kategori != "Pembelian" && n.Kategori != "Pesanan")
                .OrderByDescending(n => n.Tanggal)
                .Include(n => n.NotifikasiPembelianDetail)
                .Select(n => new NotifikasiGabunganItemViewModel
                {
                    Id = n.Id,
                    Tanggal = n.Tanggal,
                    Role = n.Role,
                    Pesan = n.Pesan,
                    Status = n.Status,
                    IsPembelian = false,
                    TransaksiKeuanganId = null,
                    NamaProduk = new List<string> { "" },
                    Stok = new List<int> { 0 },
                    Satuan = new List<string> { "" },
                    HargaSatuan = new List<decimal> { 0m },
                    JumlahDibeli = new List<int> { 0 },
                    TotalHarga = new List<decimal> { 0m },
                    TanggalExpired = new List<DateTime?> { null }
                })
                .ToList();

            // Ambil notifikasi kategori Pembelian (existing)
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
                Status = n.Status,
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


            // Ambil notifikasi kategori Pesanan (untuk Distributor)
            var pesananNotifikasiList = _context.Notifikasi
                .Where(n => n.Role == role && n.Kategori == "Pesanan" && n.UserId == userId)
                .OrderByDescending(n => n.Tanggal)
                .Include(n => n.NotifikasiPembelianDetail)
                .ToList();

            var notifikasiPesanan = pesananNotifikasiList.Select(n => new NotifikasiGabunganItemViewModel
            {
                Id = n.Id,
                Tanggal = n.Tanggal,
                Role = n.Role,
                Pesan = n.Pesan,
                Status = n.Status,
                IsPembelian = true, // bisa set true atau sesuai kebutuhan, supaya detail muncul
                TransaksiKeuanganId = n.NotifikasiPembelianDetail?.FirstOrDefault()?.TransaksiKeuanganId,
                NamaProduk = n.NotifikasiPembelianDetail?.Select(d => d.NamaProduk).ToList() ?? new List<string>(),
                Stok = n.NotifikasiPembelianDetail?.Select(d => d.Stok).ToList() ?? new List<int>(),
                Satuan = n.NotifikasiPembelianDetail?.Select(d => d.Satuan).ToList() ?? new List<string>(),
                HargaSatuan = n.NotifikasiPembelianDetail?.Select(d => d.HargaSatuan).ToList() ?? new List<decimal>(),
                JumlahDibeli = n.NotifikasiPembelianDetail?.Select(d => d.JumlahDibeli).ToList() ?? new List<int>(),
                TotalHarga = n.NotifikasiPembelianDetail?.Select(d => d.TotalHarga).ToList() ?? new List<decimal>(),
                TanggalExpired = n.NotifikasiPembelianDetail?.Select(d => d.TanggalExpired).ToList() ?? new List<DateTime?>()
            }).ToList();

            // Ambil notifikasi kategori Pembelian untuk Gudang
            var gudangPembelianNotifikasi = _context.Notifikasi
                .Where(n => n.Role == role && n.Kategori == "Pembelian" && n.UserId == userId)
                .OrderByDescending(n => n.Tanggal)
                .Include(n => n.NotifikasiPembelianDetail)
                .ToList();

            var notifikasiGudangPembelian = gudangPembelianNotifikasi.Select(n => new NotifikasiGabunganItemViewModel
            {
                Id = n.Id,
                Tanggal = n.Tanggal,
                Role = n.Role,
                Pesan = n.Pesan,
                Status = n.Status,
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


            // Hitung total produk yang perlu konfirmasi
            int totalProdukPerluKonfirmasi = notifikasiPembelian
                .Where(n => n.Status != "Dikonfirmasi" && n.Status != "Ditolak")
                .Sum(n => n.NamaProduk?.Count ?? 0);
            // Ambil notifikasi kategori Pembelian untuk Gudang (agar detail produk muncul)
            // Gabungkan semua notifikasi ke satu list
            gabunganNotifikasi.AddRange(notifikasiPembelian);
            gabunganNotifikasi.AddRange(notifikasiPesanan);
            gabunganNotifikasi.AddRange(notifikasiGudangPembelian);


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
                return Json(new { success = false, message = "Notifikasi tidak ditemukan." });

            notif.Status = "Dikonfirmasi";

            var gudangUser = _context.Users.FirstOrDefault(u => u.Role == "Gudang");
            if (gudangUser == null)
                return Json(new { success = false, message = "User  Gudang tidak ditemukan." });

            try
            {
                _context.SaveChanges();

                // Notifikasi ke Gudang
                var notifGudang = new Notifikasi
                {
                    Role = "Gudang",
                    Kategori = "Pembelian",
                    Pesan = $"Pembelian dengan ID {id} telah dikonfirmasi oleh Owner",
                    Tanggal = DateTime.Now,
                    Status = "Dikonfirmasi",
                    UserId = gudangUser.Id
                };
                _context.Notifikasi.Add(notifGudang);
                _context.SaveChanges(); // Simpan untuk dapatkan notifGudang.Id

                // Salin detail ke notifikasi gudang
                if (notif.NotifikasiPembelianDetail != null && notif.NotifikasiPembelianDetail.Any())
                {
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
                    _context.SaveChanges(); // Simpan detail produk yang baru ditambahkan
                }

                // Kirim notifikasi ke distributor yang terkait
                var distributorGroups = notif.NotifikasiPembelianDetail
                    .GroupBy(d => d.DistributorId)
                    .ToList();

                foreach (var group in distributorGroups)
                {
                    var distributorId = group.Key;
                    var distributorUser = _context.Users.FirstOrDefault(u => u.Id == distributorId);

                    if (distributorUser == null)
                        continue;

                    var notifDistributor = new Notifikasi
                    {
                        Role = "Distributor",
                        Kategori = "Pesanan",
                        Pesan = "Pesanan terbaru untuk toko Anda.",
                        Tanggal = DateTime.Now,
                        Status = "Dikonfirmasi",
                        UserId = distributorUser.Id
                    };
                    _context.Notifikasi.Add(notifDistributor);
                    _context.SaveChanges();

                    foreach (var detail in group) // hanya detail milik distributor ini
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
                }

                // ✅ return di akhir method
                return Json(new { success = true, message = "Pembelian berhasil dikonfirmasi dan dikirim ke Gudang dan Distributor." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gagal mengkonfirmasi pembelian: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TolakPembelian(int id)
        {
            var notif = _context.Notifikasi
                .Include(n => n.NotifikasiPembelianDetail)
                .FirstOrDefault(n => n.Id == id);

            if (notif == null)
                return Json(new { success = false, message = "Notifikasi tidak ditemukan." });

            notif.Status = "Ditolak";
            _context.SaveChanges();

            var notifGudang = new Notifikasi
            {
                Role = "Gudang",
                Kategori = "Pembelian",
                Pesan = $"Pembelian dengan ID {id} telah Ditolak oleh Owner",
                Tanggal = DateTime.Now
            };

            _context.Notifikasi.Add(notifGudang);
            _context.SaveChanges();

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

            return Json(new { success = true, message = "Pembelian berhasil ditolak dan dikirim ke Gudang." });
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
