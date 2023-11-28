using Microsoft.EntityFrameworkCore.Migrations;

namespace InSupport.O3C.API.Migrations
{
    public partial class o3c_externalhost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalHost",
                table: "Servers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalHost",
                table: "Servers");
        }
    }
}
