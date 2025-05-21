using System.ComponentModel.DataAnnotations;

namespace BlockChain.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password baru wajib diisi")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password dan konfirmasi tidak sama.")]
        public string ConfirmPassword { get; set; }
    }
}
