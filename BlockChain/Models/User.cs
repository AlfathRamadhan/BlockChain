using System.ComponentModel.DataAnnotations;

namespace BlockChain.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string? NamaToko { get; set; }


        public string? Email { get; set; }

        public string? NoHp { get; set; }

        [Required]
        public string KataSandi { get; set; }

        [Required]
        public string Role { get; set; }

        public string? Kategori { get; set; }
        public string? LogoPath { get; set; }
    }
}
