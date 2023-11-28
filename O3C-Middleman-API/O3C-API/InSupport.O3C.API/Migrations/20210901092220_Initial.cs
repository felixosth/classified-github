using Microsoft.EntityFrameworkCore.Migrations;

namespace InSupport.O3C.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    MAC = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    O3CServer = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsUp = table.Column<bool>(type: "bit", nullable: false),
                    Product = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firmware = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.MAC);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminPort = table.Column<int>(type: "int", nullable: false),
                    ClientPort = table.Column<int>(type: "int", nullable: false),
                    IsUp = table.Column<bool>(type: "bit", nullable: false),
                    ApplyLoadBalancing = table.Column<bool>(type: "bit", nullable: false),
                    ClusterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MAC",
                table: "Devices",
                column: "MAC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_O3CServer",
                table: "Devices",
                column: "O3CServer");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Name",
                table: "Servers",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Servers");
        }
    }
}
