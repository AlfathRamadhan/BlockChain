using System.ComponentModel.DataAnnotations;

namespace BlockChain.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password baru wajib diisi.")]
        [RegularExpression(
            @"^[A-Z][A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]*[\d!@#$%^&*()]+.*$",
            ErrorMessage = "Password harus dimulai dengan huruf kapital, mengandung angka dan simbol.")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
        [Compare("NewPassword", ErrorMessage = "Password dan konfirmasi tidak sama.")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
