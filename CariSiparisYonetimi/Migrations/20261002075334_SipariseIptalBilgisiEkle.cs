using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CariSiparisYonetimi.Migrations
{
    /// <inheritdoc />
    public partial class SipariseIptalBilgisiEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IptalMi",
                table: "Siparisler",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IptalMi",
                table: "Siparisler");
        }
    }
}
