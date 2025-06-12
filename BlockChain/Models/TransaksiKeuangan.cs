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
        public decimal TotalHarga { get; set; }



        public ICollection<ItemProduk> Produk { get; set; } // ICollection untuk relasi

        public int DistributorId { get; set; } // FK ke tabel Users
        public User Distributor { get; set; }  // Navigasi opsional


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
