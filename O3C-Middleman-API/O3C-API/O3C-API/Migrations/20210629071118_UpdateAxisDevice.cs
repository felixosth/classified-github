using Microsoft.EntityFrameworkCore.Migrations;

namespace O3C_API.Migrations
{
    public partial class UpdateAxisDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "AxisDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AxisDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "AxisDevices");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AxisDevices");
        }
    }
}
