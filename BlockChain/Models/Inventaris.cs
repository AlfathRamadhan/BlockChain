using BlockChain.Models;

public class Inventaris
{
    public int Id { get; set; }
    public string NamaProduk { get; set; }
    public int Stok { get; set; }
    public string Satuan { get; set; }
    public decimal HargaSatuan { get; set; }
    public DateTime? TanggalExpired { get; set; }
    public string GambarProdukUrl { get; set; }  // Nama kolom sesuai database

    public int UserId { get; set; }  // FK ke User

    public User User { get; set; }   // Properti navigasi ke User
}
