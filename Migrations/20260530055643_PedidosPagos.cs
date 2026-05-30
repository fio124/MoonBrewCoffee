using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonBrewCoffee.Migrations
{
    /// <inheritdoc />
    public partial class PedidosPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosPedido",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPedido", x => x.IdEstado);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    FechaPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEntrega = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEntrega = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostoEnvio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    ClienteIdUsuario = table.Column<int>(type: "int", nullable: true),
                    EstadoPedidoIdEstado = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_EstadosPedido_EstadoPedidoIdEstado",
                        column: x => x.EstadoPedidoIdEstado,
                        principalTable: "EstadosPedido",
                        principalColumn: "IdEstado");
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios_ClienteIdUsuario",
                        column: x => x.ClienteIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "DetallePedidos",
                columns: table => new
                {
                    IdDetallePedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    IdCombo = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PedidoIdPedido = table.Column<int>(type: "int", nullable: true),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true),
                    ComboIdCombo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallePedidos", x => x.IdDetallePedido);
                    table.ForeignKey(
                        name: "FK_DetallePedidos_Combos_ComboIdCombo",
                        column: x => x.ComboIdCombo,
                        principalTable: "Combos",
                        principalColumn: "IdCombo");
                    table.ForeignKey(
                        name: "FK_DetallePedidos_Pedidos_PedidoIdPedido",
                        column: x => x.PedidoIdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido");
                    table.ForeignKey(
                        name: "FK_DetallePedidos_Productos_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EstadoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PedidoIdPedido = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pagos_Pedidos_PedidoIdPedido",
                        column: x => x.PedidoIdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallePedidos_ComboIdCombo",
                table: "DetallePedidos",
                column: "ComboIdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePedidos_PedidoIdPedido",
                table: "DetallePedidos",
                column: "PedidoIdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePedidos_ProductoIdProducto",
                table: "DetallePedidos",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PedidoIdPedido",
                table: "Pagos",
                column: "PedidoIdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteIdUsuario",
                table: "Pedidos",
                column: "ClienteIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EstadoPedidoIdEstado",
                table: "Pedidos",
                column: "EstadoPedidoIdEstado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallePedidos");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "EstadosPedido");
        }
    }
}
