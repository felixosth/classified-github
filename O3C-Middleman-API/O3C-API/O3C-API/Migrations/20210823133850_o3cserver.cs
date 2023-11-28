using Microsoft.EntityFrameworkCore.Migrations;

namespace O3C_API.Migrations
{
    public partial class o3cserver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "o3c_server",
                table: "O3CDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "o3c_server",
                table: "O3CDevices");
        }
    }
}
