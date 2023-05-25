using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    public partial class Restricciones2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_restriccionIPs_LlavesApi_LlaveId",
                table: "restriccionIPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_restriccionIPs",
                table: "restriccionIPs");

            migrationBuilder.RenameTable(
                name: "restriccionIPs",
                newName: "RestriccionesIPs");

            migrationBuilder.RenameIndex(
                name: "IX_restriccionIPs_LlaveId",
                table: "RestriccionesIPs",
                newName: "IX_RestriccionesIPs_LlaveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestriccionesIPs",
                table: "RestriccionesIPs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesIPs_LlavesApi_LlaveId",
                table: "RestriccionesIPs",
                column: "LlaveId",
                principalTable: "LlavesApi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesIPs_LlavesApi_LlaveId",
                table: "RestriccionesIPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestriccionesIPs",
                table: "RestriccionesIPs");

            migrationBuilder.RenameTable(
                name: "RestriccionesIPs",
                newName: "restriccionIPs");

            migrationBuilder.RenameIndex(
                name: "IX_RestriccionesIPs_LlaveId",
                table: "restriccionIPs",
                newName: "IX_restriccionIPs_LlaveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_restriccionIPs",
                table: "restriccionIPs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_restriccionIPs_LlavesApi_LlaveId",
                table: "restriccionIPs",
                column: "LlaveId",
                principalTable: "LlavesApi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
