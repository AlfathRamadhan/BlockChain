namespace BlockChain.Models
{
    public class NotifikasiAnonim
    {
        public DateTime Tanggal { get; set; }
        public string Role { get; set; }
        public string Pesan { get; set; }
        public bool IsPembelian { get; set; }
        public int? TransaksiKeuanganId { get; set; }
        public List<string> NamaProduk { get; set; }
    }

}
