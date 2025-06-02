using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockChain.Models
{
    public class Notifikasi
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Pesan { get; set; }
        public DateTime Tanggal { get; set; }
        public string Kategori { get; set; }

        [NotMapped]
        public bool IsPembelian => Kategori == "Pembelian";

        [NotMapped]
        public List<string> NamaProduk { get; set; }

        [NotMapped]
        public List<int> Stok { get; set; }

        [NotMapped]
        public List<string> Satuan { get; set; }
        [MaxLength(50)]
        public string? Status { get; set; }

        [NotMapped]
        public int? TransaksiKeuanganId { get; set; }

        public ICollection<NotifikasiPembelianDetail> NotifikasiPembelianDetail { get; set; }

    }
}
