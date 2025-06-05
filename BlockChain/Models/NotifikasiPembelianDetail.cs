using System.ComponentModel.DataAnnotations.Schema;

namespace BlockChain.Models
{
    public class NotifikasiPembelianDetail
    {
        public int Id { get; set; }
        public int NotifikasiId { get; set; }
        public string NamaProduk { get; set; }
        public int Stok { get; set; }

        public int JumlahDibeli { get; set; }
        public string Satuan { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal TotalHarga { get; set; }
        public DateTime? TanggalExpired { get; set; }
        [NotMapped]
        public int? TransaksiKeuanganId { get; set; }

        public int DistributorId { get; set; }

        // Relasi ke User (Distributor)
        public User Distributor { get; set; }


        public Notifikasi Notifikasi { get; set; }
    }

}
