using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace BlockChain.Controllers
{
    public class NotifikasiOwnerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotifikasiOwnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NotifikasiOwner
        public IActionResult Index()
        {
            var notifikasiList = _context.Notifikasi
                .Where(n => n.Role == "Owner")
                .OrderByDescending(n => n.Tanggal)
                .Include(n => n.NotifikasiPembelianDetail)
                .ToList();

            var gabunganList = new List<NotifikasiGabunganItemViewModel>();

            foreach (var notif in notifikasiList)
            {
                var isPembelian = notif.IsPembelian;
                var detailList = new List<NotifikasiPembelianDetail>();

                if (isPembelian)
                {
                    detailList = notif.NotifikasiPembelianDetail?.ToList() ?? new List<NotifikasiPembelianDetail>();
                }

                var item = new NotifikasiGabunganItemViewModel
                {
                    Id = notif.Id,
                    Tanggal = notif.Tanggal,
                    Role = notif.Role,
                    Pesan = notif.Pesan,
                    IsPembelian = isPembelian,
                    TransaksiKeuanganId = notif.TransaksiKeuanganId,
                    Status = notif.Status,
                    NamaProduk = detailList.Select(d => d.NamaProduk).ToList(),
                    Stok = detailList.Select(d => d.Stok).ToList(),
                    Satuan = detailList.Select(d => d.Satuan).ToList(),
                    HargaSatuan = detailList.Select(d => d.HargaSatuan).ToList(),
                    JumlahDibeli = detailList.Select(d => d.JumlahDibeli).ToList(),
                    TotalHarga = detailList.Select(d => d.TotalHarga).ToList(),
                    TanggalExpired = detailList.Select(d => d.TanggalExpired).ToList()
                };

                gabunganList.Add(item);
            }

            var viewModel = new NotifikasiGabunganViewModel
            {
                NotifikasiGabungan = gabunganList,
                TotalProdukPerluKonfirmasi = gabunganList
                    .Where(n => n.IsPembelian)
                    .Sum(n => n.NamaProduk?.Count ?? 0)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KonfirmasiPembelian(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var notifikasi = await _context.Notifikasi
                    .Include(n => n.NotifikasiPembelianDetail)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (notifikasi == null)
                    return NotFound("Notifikasi tidak ditemukan");

                var detailList = _context.NotifikasiPembelianDetail
                    .Where(d => d.NotifikasiId == id)
                    .ToList();

                if (!detailList.Any())
                    return NotFound("Detail notifikasi pembelian tidak ditemukan");

                // Salin data
                var salinanDetailList = detailList.Select(d => new NotifikasiPembelianDetail
                {
                    NamaProduk = d.NamaProduk,
                    Stok = d.Stok,
                    Satuan = d.Satuan,
                    HargaSatuan = d.HargaSatuan,
                    JumlahDibeli = d.JumlahDibeli,
                    TotalHarga = d.TotalHarga,
                    TanggalExpired = d.TanggalExpired
                }).ToList();

                // Hapus detail lama
                _context.NotifikasiPembelianDetail.RemoveRange(detailList);
                await _context.SaveChangesAsync();

                // Buat notifikasi Gudang
                var notifikasiGudang = new Notifikasi
                {
                    Tanggal = DateTime.Now,
                    Role = "Gudang",
                    Pesan = "Pesanan telah dikonfirmasi oleh Owner",
                    Kategori = "Pembelian",
                    Status = "Menunggu Proses Gudang",
                    TransaksiKeuanganId = notifikasi.TransaksiKeuanganId
                };

                _context.Notifikasi.Add(notifikasiGudang);
                await _context.SaveChangesAsync();

                // Tambah ulang detail ke notifikasi Gudang
                foreach (var detail in salinanDetailList)
                {
                    detail.NotifikasiId = notifikasiGudang.Id;
                    _context.NotifikasiPembelianDetail.Add(detail);
                }

                // Update notifikasi lama
                notifikasi.Status = "Terkonfirmasi";
                _context.Notifikasi.Update(notifikasi);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Pembelian berhasil dikonfirmasi" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan server: {ex.Message}");
            }
        }


        // DELETE: NotifikasiOwner/DeleteNotification/
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var detail = await _context.NotifikasiPembelianDetail.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.NotifikasiPembelianDetail.Remove(detail);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
