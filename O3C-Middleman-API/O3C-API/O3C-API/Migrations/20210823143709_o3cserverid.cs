using Microsoft.EntityFrameworkCore.Migrations;

namespace O3C_API.Migrations
{
    public partial class o3cserverid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "O3CServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminPort = table.Column<int>(type: "int", nullable: false),
                    ClientPort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_O3CServers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "O3CServers");
        }
    }
}
