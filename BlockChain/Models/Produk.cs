using System;
using System.ComponentModel.DataAnnotations;

namespace BlockChain.Models
{
    public class Produk
    {
        public int Id { get; set; }

        public string KodeProduk { get; set; }

        [Required]
        public string Nama { get; set; }

        [Required]
        public int Stok { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TanggalMasuk { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TanggalExpired { get; set; }

        [Required]
        public string Satuan { get; set; } // Tambahan
    }
}
