using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendyolClone.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilFotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilFotoUrl",
                table: "Kullanicilar",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilFotoUrl",
                table: "Kullanicilar");
        }
    }
}
