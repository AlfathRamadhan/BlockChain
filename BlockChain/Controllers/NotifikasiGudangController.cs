using Microsoft.AspNetCore.Mvc;
using BlockChain.Models;
using System.Linq;

namespace BlockChain.Controllers
{
    public class NotifikasiGudangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotifikasiGudangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Gudang()
        {
            var notifikasiList = _context.Notifikasi
                .Where(n => n.Role == "Gudang")
                .OrderByDescending(n => n.Tanggal)
                .ToList();

            var gabunganList = notifikasiList.Select(notif =>
            {
                var detailList = _context.NotifikasiPembelianDetail
                    .Where(d => d.NotifikasiId == notif.Id)
                    .ToList();

                return new NotifikasiGabunganItemViewModel
                {
                    Id = notif.Id,
                    Tanggal = notif.Tanggal,
                    Role = notif.Role,
                    Pesan = notif.Pesan,
                    IsPembelian = notif.IsPembelian,
                    Status = notif.Status,
                    TransaksiKeuanganId = notif.TransaksiKeuanganId,
                    NamaProduk = detailList.Select(d => d.NamaProduk).ToList(),
                    Stok = detailList.Select(d => d.Stok).ToList(),
                    Satuan = detailList.Select(d => d.Satuan).ToList(),
                    HargaSatuan = detailList.Select(d => d.HargaSatuan).ToList(),
                    JumlahDibeli = detailList.Select(d => d.JumlahDibeli).ToList(),
                    TotalHarga = detailList.Select(d => d.TotalHarga).ToList(),
                    TanggalExpired = detailList.Select(d => d.TanggalExpired).ToList()
                };
            }).ToList();

            var viewModel = new NotifikasiGabunganViewModel
            {
                NotifikasiGabungan = gabunganList,
                TotalProdukPerluKonfirmasi = gabunganList
                    .Where(n => n.IsPembelian)
                    .Sum(n => n.NamaProduk?.Count ?? 0)
            };

            return View(viewModel);
        }
    }
}
