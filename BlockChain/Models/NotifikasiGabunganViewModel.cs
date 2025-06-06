namespace BlockChain.Models
{
    public class NotifikasiGabunganViewModel
    {
        public List<NotifikasiGabunganItemViewModel> NotifikasiGabungan { get; set; }
        public int TotalProdukPerluKonfirmasi { get; set; }
    }

    public class NotifikasiGabunganItemViewModel
    {
        public int Id { get; set; }

        public DateTime Tanggal { get; set; }
        public string Role { get; set; }
        public string Pesan { get; set; }
        public bool IsPembelian { get; set; }
        public int? TransaksiKeuanganId { get; set; }
        public List<string> NamaProduk { get; set; } // hanya ada jika IsPembelian = true
        public List<int> Stok { get; set; }           // Tambahan
        public List<string> Satuan { get; set; }
        public List<decimal> HargaSatuan { get; set; }
        public List<int> JumlahDibeli { get; set; }
        public List<decimal> TotalHarga { get; set; }
        public List<DateTime?> TanggalExpired { get; set; }
        public string Status { get; set; }



    }


}
