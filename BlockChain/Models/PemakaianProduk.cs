using BlockChain.Models;

public class PemakaianProduk
{
    public int Id { get; set; }
    public int ProdukId { get; set; }
    public Produk Produk { get; set; }
    public int JumlahDigunakan { get; set; }
    public DateTime TanggalPenggunaan { get; set; }
}
