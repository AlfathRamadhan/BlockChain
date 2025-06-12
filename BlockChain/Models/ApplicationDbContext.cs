using Microsoft.EntityFrameworkCore;

namespace BlockChain.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Produk> Produk { get; set; }
        public DbSet<Inventaris> Inventaris { get; set; }
        public DbSet<TransaksiKeuangan> TransaksiKeuangan { get; set; }
        public DbSet<ItemProduk> ItemProduk { get; set; }
        public DbSet<Notifikasi> Notifikasi { get; set; }
        public DbSet<NotifikasiPembelianDetail> NotifikasiPembelianDetail { get; set; }
        public DbSet<PemakaianProduk> PemakaianProduk { get; set; }


        // Tambahkan method ini
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurasi relasi antara TransaksiKeuangan dan User (Distributor)
            modelBuilder.Entity<TransaksiKeuangan>()
                .HasOne(t => t.Distributor)
                .WithMany(u => u.TransaksiKeuangan) // perlu navigasi balik di User juga
                .HasForeignKey(t => t.DistributorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
