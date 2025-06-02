using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockChain.Migrations
{
    /// <inheritdoc />
    public partial class AddTransaksiKeuangan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alamat",
                table: "Users",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deskripsi",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamaLengkap",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoHp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoRekening",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventaris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamaProduk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stok = table.Column<int>(type: "int", nullable: false),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HargaSatuan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TanggalExpired = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GambarProdukUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventaris", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransaksiKeuangan",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomorPembayaran = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodePembayaran = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaksiKeuangan", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemProduk",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamaProduk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Satuan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JumlahUnit = table.Column<int>(type: "int", nullable: false),
                    HargaSatuan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransaksiKeuanganID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemProduk", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemProduk_TransaksiKeuangan_TransaksiKeuanganID",
                        column: x => x.TransaksiKeuanganID,
                        principalTable: "TransaksiKeuangan",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemProduk_TransaksiKeuanganID",
                table: "ItemProduk",
                column: "TransaksiKeuanganID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventaris");

            migrationBuilder.DropTable(
                name: "ItemProduk");

            migrationBuilder.DropTable(
                name: "TransaksiKeuangan");

            migrationBuilder.DropColumn(
                name: "Alamat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deskripsi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NamaLengkap",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NoHp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NoRekening",
                table: "Users");
        }
    }
}
