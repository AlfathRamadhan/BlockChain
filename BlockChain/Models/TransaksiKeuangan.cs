namespace BlockChain.Models
{
    public class TransaksiKeuangan
    {
        public int ID { get; set; }
        public string NomorPembayaran { get; set; }
        public string Supplier { get; set; }
        public DateTime Tanggal { get; set; }
        public decimal Jumlah { get; set; }
        public string Status { get; set; }
        public string MetodePembayaran { get; set; }

        public ICollection<ItemProduk> Produk { get; set; } // ICollection untuk relasi

        public int UserId { get; set; }  // FK ke User

        public User User { get; set; }   // Properti navigasi ke User

    }

    public class ItemProduk
    {
        public int ID { get; set; }
        public string NamaProduk { get; set; }
        public string Satuan { get; set; }
        public int JumlahUnit { get; set; }
        public decimal HargaSatuan { get; set; }

        // Foreign key ke TransaksiKeuangan
        public int TransaksiKeuanganID { get; set; }
        public TransaksiKeuangan TransaksiKeuangan { get; set; }
    }

}
