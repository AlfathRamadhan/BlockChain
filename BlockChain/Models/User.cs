using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlockChain.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string KataSandi { get; set; }

        [Required]
        public required string Role { get; set; }

        public string? NamaToko { get; set; }
        public string? Email { get; set; }
        public string? NoHp { get; set; }
        public string? Kategori { get; set; }
        public string? LogoPath { get; set; }

        public string? NamaLengkap { get; set; }
        public string? Alamat { get; set; }
        public string? Deskripsi { get; set; }
        public string? NoRekening { get; set; }

        [NotMapped]
        public IFormFile? LogoFile { get; set; }

        [NotMapped]
        public string? PasswordBaru { get; set; }

        public string? Bank { get; set; }
        public bool IsVerified { get; set; } = false;

        public ICollection<Notifikasi> NotifikasiList { get; set; } = new List<Notifikasi>();
        public ICollection<Inventaris> InventarisList { get; set; } = new List<Inventaris>();
        public ICollection<TransaksiKeuangan> TransaksiKeuangan { get; set; } = new List<TransaksiKeuangan>();

    }
}
