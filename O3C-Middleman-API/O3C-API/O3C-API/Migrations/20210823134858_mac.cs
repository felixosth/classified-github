using Microsoft.EntityFrameworkCore.Migrations;

namespace O3C_API.Migrations
{
    public partial class mac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mac",
                table: "O3CDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mac",
                table: "O3CDevices");
        }
    }
}
