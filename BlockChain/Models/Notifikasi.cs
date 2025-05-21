using System;

namespace BlockChain.Models
{
    public class Notifikasi
    {
        public int Id { get; set; }

        // Isi pesan notifikasi
        public string Pesan { get; set; }

        // Tanggal notifikasi dibuat
        public DateTime Tanggal { get; set; }

        // Kategori (misalnya: "Pelacakan", "Pendaftaran", dll)
        public string Kategori { get; set; }

        // Penanda apakah notifikasi sudah dibaca
        public bool SudahDibaca { get; set; }

        // Optional: URL untuk aksi (misalnya menuju detail status)
        public string UrlAksi { get; set; }

        // Untuk tampilan tanggal singkat, seperti "04"
        public string TanggalSingkat => Tanggal.ToString("dd");

        // Untuk tampilan hari singkat, seperti "Sen"
        public string HariSingkat => Tanggal.ToString("ddd");
    }
}
