using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonBrewCoffee.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOrdenProceso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "ProcesosPreparacion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orden",
                table: "ProcesosPreparacion");
        }
    }
}