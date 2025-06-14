﻿using Microsoft.AspNetCore.Mvc;
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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> KonfirmasiPembelian(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var notifikasi = _context.Notifikasi
                    .Include(n => n.NotifikasiPembelianDetail)
                    .FirstOrDefault(n => n.Id == id);

                //if (notifikasi.TransaksiKeuanganId == null)
                //    return BadRequest("Transaksi keuangan tidak ditemukan pada notifikasi ini");


                var detailList = _context.NotifikasiPembelianDetail
                    .Where(d => d.NotifikasiId == id)
                    .ToList();

                if (!detailList.Any())
                    return NotFound("Detail notifikasi pembelian tidak ditemukan");

                var userId = HttpContext.Session.GetInt32("UserId");

                // Salin data
                var salinanDetailList = detailList.Select(d => new NotifikasiPembelianDetail
                {
                    NamaProduk = d.NamaProduk,
                    Stok = d.Stok,
                    Satuan = d.Satuan,
                    HargaSatuan = d.HargaSatuan,
                    JumlahDibeli = d.JumlahDibeli,
                    TotalHarga = d.TotalHarga,
                    TanggalExpired = d.TanggalExpired,
                    DistributorId = d.DistributorId
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
                    TransaksiKeuanganId = notifikasi.TransaksiKeuanganId,
                    UserId = userId
                };

                _context.Notifikasi.Add(notifikasiGudang);
                await _context.SaveChangesAsync();

                // Tambah ulang detail ke notifikasi Gudang
                foreach (var detail in salinanDetailList)
                {
                    Console.WriteLine($"Detail: {detail.NamaProduk}, {detail.HargaSatuan}");
                    detail.NotifikasiId = notifikasiGudang.Id;
                    _context.NotifikasiPembelianDetail.Add(detail);
                }

                // Dapatkan semua distributor unik dari detail
                var distributorIds = salinanDetailList
                    .Select(d => d.DistributorId)
                    .Distinct()
                    .ToList();

                foreach (var distributorId in distributorIds)
                {
                    // Buat notifikasi untuk Distributor
                    var notifikasiDistributor = new Notifikasi
                    {
                        Tanggal = DateTime.Now,
                        Role = "Distributor",
                        Pesan = "Pesanan Baru",
                        Kategori = "Pembelian",
                        Status = "Konfirmasi",
                        TransaksiKeuanganId = notifikasi.TransaksiKeuanganId,
                        UserId = distributorId
                    };

                    _context.Notifikasi.Add(notifikasiDistributor);
                    await _context.SaveChangesAsync();

                    // Tambahkan detail yang sesuai dengan distributor ini
                    var detailDistributor = salinanDetailList
                        .Where(d => d.DistributorId == distributorId)
                        .Select(d => new NotifikasiPembelianDetail
                        {
                            NamaProduk = d.NamaProduk,
                            Stok = d.Stok,
                            Satuan = d.Satuan,
                            HargaSatuan = d.HargaSatuan,
                            JumlahDibeli = d.JumlahDibeli,
                            TotalHarga = d.TotalHarga,
                            TanggalExpired = d.TanggalExpired,
                            DistributorId = d.DistributorId,
                            NotifikasiId = notifikasiDistributor.Id
                        }).ToList();

                    _context.NotifikasiPembelianDetail.AddRange(detailDistributor);
                    await _context.SaveChangesAsync();
                }


                // Update notifikasi lama
                notifikasi.Status = "Terkonfirmasi";
                _context.Notifikasi.Update(notifikasi);

                await _context.SaveChangesAsync();
                Console.WriteLine($"ID Notifikasi Gudang: {notifikasiGudang.Id}");

                await transaction.CommitAsync();

                return Ok(new { message = "Pembelian berhasil dikonfirmasi" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Terjadi kesalahan server: {ex.Message} \n {ex.InnerException?.Message}");
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
        [HttpPost]
        public IActionResult KonfirmasiProduk(Dictionary<int, int> jumlah)
        {
            var produkDetailList = new List<ProdukDetailViewModel>();
            int totalProdukDigunakan = 0;

            foreach (var pair in jumlah)
            {
                var produk = _context.Produk.FirstOrDefault(p => p.Id == pair.Key);
                if (produk != null && produk.Stok >= pair.Value)
                {
                    produk.Stok -= pair.Value;
                    totalProdukDigunakan++;

                    produkDetailList.Add(new ProdukDetailViewModel
                    {
                        Nama = produk.Nama,
                        Jumlah = pair.Value,
                        Satuan = produk.Satuan,
                        Expired = produk.TanggalExpired.ToString("dd/MM/yyyy") ?? "-"
                    });
                }
            }

            var owner = _context.Users.FirstOrDefault(u => u.Role == "Owner");
            if (totalProdukDigunakan > 0)
            {
                var pesan = $"Gudang menggunakan {totalProdukDigunakan} produk";
                var detailJson = System.Text.Json.JsonSerializer.Serialize(produkDetailList);

                var notif = new Notifikasi
                {
                    Tanggal = DateTime.Now,
                    Role = "Owner",
                    Pesan = pesan,
                    Kategori = "GudangMemakaiProduk",
                    UserId = owner.Id,
                    DetailJson = detailJson // tambahkan kolom baru DetailJson di tabel Notifikasi
                };

                _context.Notifikasi.Add(notif);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
