using Microsoft.EntityFrameworkCore;

namespace BlockChain.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Produk> Produk { get; set; } // Tambahkan ini untuk tabel Produk
    }
}
