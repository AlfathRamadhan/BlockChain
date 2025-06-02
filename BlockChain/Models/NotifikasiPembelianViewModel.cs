namespace BlockChain.Models
{
    public class NotifikasiPembelianViewModel
    {
        public int TransaksiKeuanganId { get; set; }
        public DateTime Tanggal { get; set; }
        public string Role { get; set; }
        public List<string> NamaProduk { get; set; }
        public List<int> Stok { get; set; }

    }

}
