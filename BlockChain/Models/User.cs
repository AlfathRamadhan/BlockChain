using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlockChain.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string? NamaToko { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? NoHp { get; set; }

        [Required]
        public string KataSandi { get; set; }

        [Required]
        public string Role { get; set; }

        public string? Kategori { get; set; }
        public string? LogoPath { get; set; }

        // Profil tambahan:

        [MaxLength(100)]
        public string? NamaLengkap { get; set; }

        [MaxLength(250)]
        public string? Alamat { get; set; }

        [MaxLength(500)]
        public string? Deskripsi { get; set; }

        [MaxLength(50)]
        public string? NoRekening { get; set; }

        [NotMapped]
        public IFormFile LogoFile { get; set; } // Tambahan properti untuk upload

        [NotMapped]
        public string PasswordBaru { get; set; }
        public string? Bank { get; set; }
        public bool IsVerified { get; set; } = false; // default: belum diverifikasi


        public ICollection<Notifikasi> NotifikasiList { get; set; }

        public ICollection<Inventaris> InventarisList { get; set; }
        public ICollection<TransaksiKeuangan> TransaksiKeuangan { get; set; }

    }
}
