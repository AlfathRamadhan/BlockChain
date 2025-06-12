namespace BlockChain.Models
{
    public class PesananViewModel
    {
        public TransaksiKeuangan? Transaksi { get; set; }
        public List<NotifikasiPembelianDetail> NotifikasiDetail { get; set; } = new();
        public string? NamaToko { get; set; }

        // Properti tampilan
        public string NomorPesanan => Transaksi?.NomorPembayaran ?? "N/A";
        public DateTime TanggalPemesanan => Transaksi?.Tanggal ?? DateTime.MinValue;
        public decimal Total => Transaksi?.TotalHarga ?? 0;
        public string StatusPembayaran => Transaksi?.Status ?? "Belum Dibayar";
    }
}
