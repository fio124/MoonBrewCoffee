using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonBrewCoffee.Migrations
{
    /// <inheritdoc />
    public partial class CocinaCarrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carritos",
                columns: table => new
                {
                    IdCarrito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carritos", x => x.IdCarrito);
                    table.ForeignKey(
                        name: "FK_Carritos_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "EstacionesCocina",
                columns: table => new
                {
                    IdEstacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstacionesCocina", x => x.IdEstacion);
                });

            migrationBuilder.CreateTable(
                name: "CarritoDetalles",
                columns: table => new
                {
                    IdCarritoDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCarrito = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    IdCombo = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarritoIdCarrito = table.Column<int>(type: "int", nullable: true),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true),
                    ComboIdCombo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoDetalles", x => x.IdCarritoDetalle);
                    table.ForeignKey(
                        name: "FK_CarritoDetalles_Carritos_CarritoIdCarrito",
                        column: x => x.CarritoIdCarrito,
                        principalTable: "Carritos",
                        principalColumn: "IdCarrito");
                    table.ForeignKey(
                        name: "FK_CarritoDetalles_Combos_ComboIdCombo",
                        column: x => x.ComboIdCombo,
                        principalTable: "Combos",
                        principalColumn: "IdCombo");
                    table.ForeignKey(
                        name: "FK_CarritoDetalles_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "ProcesosPreparacion",
                columns: table => new
                {
                    IdProceso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdEstacion = table.Column<int>(type: "int", nullable: false),
                    TiempoPreparacionMin = table.Column<int>(type: "int", nullable: false),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true),
                    EstacionCocinaIdEstacion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesosPreparacion", x => x.IdProceso);
                    table.ForeignKey(
                        name: "FK_ProcesosPreparacion_EstacionesCocina_EstacionCocinaIdEstacion",
                        column: x => x.EstacionCocinaIdEstacion,
                        principalTable: "EstacionesCocina",
                        principalColumn: "IdEstacion");
                    table.ForeignKey(
                        name: "FK_ProcesosPreparacion_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoDetalles_CarritoIdCarrito",
                table: "CarritoDetalles",
                column: "CarritoIdCarrito");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoDetalles_ComboIdCombo",
                table: "CarritoDetalles",
                column: "ComboIdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoDetalles_ProductoIdProducto",
                table: "CarritoDetalles",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Carritos_UsuarioIdUsuario",
                table: "Carritos",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ProcesosPreparacion_EstacionCocinaIdEstacion",
                table: "ProcesosPreparacion",
                column: "EstacionCocinaIdEstacion");

            migrationBuilder.CreateIndex(
                name: "IX_ProcesosPreparacion_ProductoIdProducto",
                table: "ProcesosPreparacion",
                column: "ProductoIdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarritoDetalles");

            migrationBuilder.DropTable(
                name: "ProcesosPreparacion");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.DropTable(
                name: "EstacionesCocina");
        }
    }
}
