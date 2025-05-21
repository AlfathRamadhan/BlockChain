using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockChain.Migrations
{
    /// <inheritdoc />
    public partial class AddProdukTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NamaToko",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Produk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeProduk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stok = table.Column<int>(type: "int", nullable: false),
                    TanggalMasuk = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TanggalExpired = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produk", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produk");

            migrationBuilder.AlterColumn<string>(
                name: "NamaToko",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
