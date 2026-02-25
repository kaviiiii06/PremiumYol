using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendyolClone.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSiparisModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotu",
                table: "Siparisler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaturaAdresi",
                table: "Siparisler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IndirimTutari",
                table: "Siparisler",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "KargoUcreti",
                table: "Siparisler",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SiparisNo",
                table: "Siparisler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Adet",
                table: "SiparisKalemleri",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaAdresi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "IndirimTutari",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KargoUcreti",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "SiparisNo",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "Adet",
                table: "SiparisKalemleri");
        }
    }
}
