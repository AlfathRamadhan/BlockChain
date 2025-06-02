using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockChain.Migrations
{
    /// <inheritdoc />
    public partial class AddTransaksiDanProduk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemProduk_TransaksiKeuangan_TransaksiKeuanganID",
                table: "ItemProduk");

            migrationBuilder.AlterColumn<int>(
                name: "TransaksiKeuanganID",
                table: "ItemProduk",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemProduk_TransaksiKeuangan_TransaksiKeuanganID",
                table: "ItemProduk",
                column: "TransaksiKeuanganID",
                principalTable: "TransaksiKeuangan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemProduk_TransaksiKeuangan_TransaksiKeuanganID",
                table: "ItemProduk");

            migrationBuilder.AlterColumn<int>(
                name: "TransaksiKeuanganID",
                table: "ItemProduk",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemProduk_TransaksiKeuangan_TransaksiKeuanganID",
                table: "ItemProduk",
                column: "TransaksiKeuanganID",
                principalTable: "TransaksiKeuangan",
                principalColumn: "ID");
        }
    }
}
