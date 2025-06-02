using System;

namespace BlockChain.Models
{
    public class InventarisViewModel
    {
        public int Id { get; set; }

        public string NamaProduk { get; set; }
        public string NamaToko { get; set; }

        public int Stok { get; set; }

        public string Satuan { get; set; }

        public decimal HargaSatuan { get; set; }

        public DateTime? TanggalExpired { get; set; }

        // Ini properti tambahan yang tidak ada di entity,
        // berisi URL lengkap gambar yang akan dipakai di view.
        public string GambarProdukUrl { get; set; }
        public int UserId { get; set; }  // FK ke User

        public User User { get; set; }   // Properti navigasi ke User


    }
}
