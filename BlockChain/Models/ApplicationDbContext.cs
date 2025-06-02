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
        public DbSet<Inventaris> Inventaris { get; set; }
        public DbSet<TransaksiKeuangan> TransaksiKeuangan { get; set; }
        public DbSet<ItemProduk> ItemProduk { get; set; }
        public DbSet<Notifikasi> Notifikasi { get; set; }
        public DbSet<NotifikasiPembelianDetail> NotifikasiPembelianDetail { get; set; }



    }
}
