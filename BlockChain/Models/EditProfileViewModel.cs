namespace BlockChain.Models
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class EditProfileViewModel
    {
        public string? NamaToko { get; set; }
        public string? Email { get; set; }
        public string? NoHp { get; set; }
        public string? Alamat { get; set; }
        public string? Deskripsi { get; set; }
        public string? NoRekening { get; set; }
        [Display(Name = "Bank")]
        public string? Bank { get; set; }

        public string? NamaLengkap { get; set; }
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        public string? PasswordBaru { get; set; }

        public IFormFile? LogoFile { get; set; }

        // Untuk preview logo, tetap string biasa
        public string? LogoPath { get; set; }
    }
}
