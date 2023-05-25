using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    public partial class Restricciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestriccionesDominios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesDominios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesDominios_LlavesApi_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "LlavesApi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "restriccionIPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restriccionIPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_restriccionIPs_LlavesApi_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "LlavesApi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominios_LlaveId",
                table: "RestriccionesDominios",
                column: "LlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_restriccionIPs_LlaveId",
                table: "restriccionIPs",
                column: "LlaveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesDominios");

            migrationBuilder.DropTable(
                name: "restriccionIPs");
        }
    }
}
