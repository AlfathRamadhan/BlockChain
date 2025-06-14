using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BlockChain.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nama Toko wajib diisi")]
        public required string NamaToko { get; set; }

        [Required(ErrorMessage = "Username wajib diisi")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Hanya email gmail.com yang diperbolehkan")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Nomor HP wajib diisi")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Nomor HP harus berupa angka 10-15 digit")]
        public required string NoHp { get; set; }

        [Required(ErrorMessage = "Kategori wajib dipilih")]
        public required string Kategori { get; set; }

        public IFormFile? LogoFile { get; set; }
        public string? LogoPath { get; set; }

        [Required(ErrorMessage = "Kata sandi wajib diisi")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Kata sandi minimal 6 karakter")]
        public required string KataSandi { get; set; }
    }

}
