using System.ComponentModel.DataAnnotations;

namespace BlockChain.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
