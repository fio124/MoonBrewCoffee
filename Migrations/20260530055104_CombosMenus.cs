using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonBrewCoffee.Migrations
{
    /// <inheritdoc />
    public partial class CombosMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    IdCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecioCombo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.IdCombo);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    IdMenu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.IdMenu);
                });

            migrationBuilder.CreateTable(
                name: "ComboProductos",
                columns: table => new
                {
                    IdCombo = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    ComboIdCombo = table.Column<int>(type: "int", nullable: true),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboProductos", x => new { x.IdCombo, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_ComboProductos_Combos_ComboIdCombo",
                        column: x => x.ComboIdCombo,
                        principalTable: "Combos",
                        principalColumn: "IdCombo");
                    table.ForeignKey(
                        name: "FK_ComboProductos_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "MenuProductos",
                columns: table => new
                {
                    IdMenu = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    MenuIdMenu = table.Column<int>(type: "int", nullable: true),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuProductos", x => new { x.IdMenu, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_MenuProductos_Menus_MenuIdMenu",
                        column: x => x.MenuIdMenu,
                        principalTable: "Menus",
                        principalColumn: "IdMenu");
                    table.ForeignKey(
                        name: "FK_MenuProductos_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductos_ComboIdCombo",
                table: "ComboProductos",
                column: "ComboIdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductos_ProductoIdProducto",
                table: "ComboProductos",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_MenuProductos_MenuIdMenu",
                table: "MenuProductos",
                column: "MenuIdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_MenuProductos_ProductoIdProducto",
                table: "MenuProductos",
                column: "ProductoIdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboProductos");

            migrationBuilder.DropTable(
                name: "MenuProductos");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }
    }
}
