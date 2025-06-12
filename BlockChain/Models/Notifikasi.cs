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
        public string? DetailJson { get; set; } // nullable supaya aman dari null


        public int? UserId { get; set; } // foreign key

        [ForeignKey("UserId")]
        public User User { get; set; } // navigational property


        public int? TransaksiKeuanganId { get; set; }

        [ForeignKey("TransaksiKeuanganId")]
        public TransaksiKeuangan TransaksiKeuangan { get; set; }

        public ICollection<NotifikasiPembelianDetail> NotifikasiPembelianDetail { get; set; }

    }
}
