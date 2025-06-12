using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockChain.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace BlockChain.Controllers
{
    public class NotifikasiDistributorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotifikasiDistributorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Distributor()
        {
            var distributorId = HttpContext.Session.GetInt32("UserId");
            if (distributorId == null)
                return RedirectToAction("Login", "Account");

            var notifikasiList = await _context.Notifikasi
                .Where(n => n.Role == "Distributor" && n.UserId == distributorId)
                .Include(n => n.NotifikasiPembelianDetail)
                .OrderByDescending(n => n.Tanggal)
                .ToListAsync();

            var viewModel = new NotifikasiGabunganViewModel
            {
                NotifikasiGabungan = notifikasiList.Select(n => new NotifikasiGabunganItemViewModel
                {
                    Id = n.Id,
                    Tanggal = n.Tanggal,
                    Role = n.Role,
                    Pesan = n.Pesan,
                    IsPembelian = true,
                    Status = n.Status,
                    TransaksiKeuanganId = n.TransaksiKeuanganId,
                    NamaProduk = n.NotifikasiPembelianDetail.Select(d => d.NamaProduk).ToList(),
                    Stok = n.NotifikasiPembelianDetail.Select(d => d.Stok).ToList(),
                    Satuan = n.NotifikasiPembelianDetail.Select(d => d.Satuan).ToList(),
                    HargaSatuan = n.NotifikasiPembelianDetail.Select(d => d.HargaSatuan).ToList(),
                    JumlahDibeli = n.NotifikasiPembelianDetail.Select(d => d.JumlahDibeli).ToList(),
                    TotalHarga = n.NotifikasiPembelianDetail.Select(d => d.TotalHarga).ToList(),
                    TanggalExpired = n.NotifikasiPembelianDetail.Select(d => d.TanggalExpired).ToList()
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
