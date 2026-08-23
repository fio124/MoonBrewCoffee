using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Data;
    public class MoonBrewContext : DbContext
    {
        public MoonBrewContext(DbContextOptions<MoonBrewContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Combo> Combos { get; set; }

        public DbSet<ComboProducto> ComboProductos { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuProducto> MenuProductos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Ingrediente> Ingredientes { get; set; }

        public DbSet<ProductoIngrediente> ProductoIngredientes { get; set; }

        public DbSet<EstadoPedido> EstadosPedido { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<DetallePedido> DetallePedidos { get; set; }

        public DbSet<Pago> Pagos { get; set; }

        public DbSet<PedidoProceso> PedidoProcesos { get; set; }

        public DbSet<PedidoEstadoHistorial> PedidoEstadoHistorial { get; set; }

        public DbSet<EstacionCocina> EstacionesCocina { get; set; }

        public DbSet<ProcesoPreparacion> ProcesosPreparacion { get; set; }

        public DbSet<Carrito> Carritos { get; set; }

        public DbSet<CarritoDetalle> CarritoDetalles { get; set; }
        public DbSet<MenuCombo> MenuCombos { get; set; }

        public DbSet<ScheduledTaskExecution> ScheduledTaskExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasKey(x => x.IdRol);

            modelBuilder.Entity<Usuario>().HasKey(x => x.IdUsuario);

            modelBuilder.Entity<Usuario>().Property(x => x.Correo).HasMaxLength(256);
            modelBuilder.Entity<Usuario>().HasIndex(x => x.Correo).IsUnique();

            modelBuilder.Entity<Usuario>().HasOne(u => u.Rol).WithMany(r => r.Usuarios).HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<Categoria>().HasKey(x => x.IdCategoria);

            modelBuilder.Entity<Producto>().HasKey(x => x.IdProducto);

            modelBuilder.Entity<Ingrediente>().HasKey(x => x.IdIngrediente);

            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(10, 2);

            modelBuilder.Entity<ProductoIngrediente>()
                .HasKey(pi => new
                {
                    pi.IdProducto,
                    pi.IdIngrediente
                });

            modelBuilder.Entity<ProductoIngrediente>().HasOne(pi => pi.Producto).WithMany(p => p.ProductoIngredientes).HasForeignKey(pi => pi.IdProducto);

            modelBuilder.Entity<ProductoIngrediente>().HasOne(pi => pi.Ingrediente).WithMany().HasForeignKey(pi => pi.IdIngrediente);

            modelBuilder.Entity<Combo>().HasKey(x => x.IdCombo);

            modelBuilder.Entity<Combo>().Property(c => c.PrecioCombo).HasPrecision(10, 2);

            modelBuilder.Entity<Menu>().HasKey(x => x.IdMenu);

            modelBuilder.Entity<ComboProducto>()
                    .HasKey(cp => new
                    {
                        cp.IdCombo,
                        cp.IdProducto
                    });

            modelBuilder.Entity<ComboProducto>().HasOne(cp => cp.Combo).WithMany().HasForeignKey(cp => cp.IdCombo);

            modelBuilder.Entity<ComboProducto>().HasOne(cp => cp.Producto).WithMany().HasForeignKey(cp => cp.IdProducto);

            modelBuilder.Entity<MenuProducto>()
                    .HasKey(mp => new
                    {
                        mp.IdMenu,
                        mp.IdProducto
                    });

            modelBuilder.Entity<MenuProducto>().HasOne(mp => mp.Menu).WithMany(m => m.MenuProductos).HasForeignKey(mp => mp.IdMenu);

            modelBuilder.Entity<MenuProducto>().HasOne(mp => mp.Producto).WithMany(p => p.MenuProductos).HasForeignKey(mp => mp.IdProducto);

            modelBuilder.Entity<MenuCombo>()
                .HasKey(mc => new
                {
                    mc.IdMenu,
                    mc.IdCombo
                });

            modelBuilder.Entity<MenuCombo>().HasOne(mc => mc.Menu).WithMany(m => m.MenuCombos).HasForeignKey(mc => mc.IdMenu);

            modelBuilder.Entity<MenuCombo>().HasOne(mc => mc.Combo).WithMany(c => c.MenuCombos).HasForeignKey(mc => mc.IdCombo);

            modelBuilder.Entity<ScheduledTaskExecution>().HasKey(x => x.IdScheduledTaskExecution);
            modelBuilder.Entity<ScheduledTaskExecution>()
                .HasIndex(x => new { x.TaskName, x.ExecutionKey })
                .IsUnique();

            modelBuilder.Entity<Pedido>().Property(p => p.CostoEnvio).HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>().Property(p => p.Subtotal).HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>().Property(p => p.Impuesto).HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>().Property(p => p.Total).HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>().Property(p => p.ClaveOperacion).HasMaxLength(64);
            modelBuilder.Entity<Pedido>().HasIndex(p => p.ClaveOperacion).IsUnique();

            modelBuilder.Entity<Pedido>().HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.IdCliente).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>().HasOne(p => p.Encargado).WithMany().HasForeignKey(p => p.IdEncargado).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>().HasOne(p => p.EstadoPedido).WithMany().HasForeignKey(p => p.IdEstado).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedido>().HasOne(d => d.Pedido).WithMany(p => p.Detalles).HasForeignKey(d => d.IdPedido);

            modelBuilder.Entity<Pago>().HasOne(p => p.Pedido).WithOne(p => p.Pago).HasForeignKey<Pago>(p => p.IdPedido);

            modelBuilder.Entity<DetallePedido>().Property(d => d.PrecioUnitario).HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>().Property(d => d.Subtotal).HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>().Property(d => d.Impuesto).HasPrecision(10, 2);

            modelBuilder.Entity<Pago>().Property(p => p.TotalPagado).HasPrecision(10, 2);

            modelBuilder.Entity<PedidoProceso>().HasOne(p => p.Pedido).WithMany(p => p.Procesos).HasForeignKey(p => p.IdPedido);
            modelBuilder.Entity<PedidoProceso>().HasOne(p => p.Estacion).WithMany().HasForeignKey(p => p.IdEstacion).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PedidoProceso>().HasOne(p => p.Encargado).WithMany().HasForeignKey(p => p.IdEncargado).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PedidoProceso>().HasIndex(p => new { p.IdPedido, p.Orden });

            modelBuilder.Entity<PedidoEstadoHistorial>().HasOne(h => h.Pedido).WithMany(p => p.HistorialEstados).HasForeignKey(h => h.IdPedido);
            modelBuilder.Entity<PedidoEstadoHistorial>().HasOne(h => h.Estado).WithMany().HasForeignKey(h => h.IdEstado).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PedidoEstadoHistorial>().HasOne(h => h.Usuario).WithMany().HasForeignKey(h => h.IdUsuario).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProcesoPreparacion>().HasKey(p => p.IdProceso);

            modelBuilder.Entity<ProcesoPreparacion>().HasOne(p => p.Producto).WithMany().HasForeignKey(p => p.IdProducto).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProcesoPreparacion>().HasOne(p => p.EstacionCocina).WithMany().HasForeignKey(p => p.IdEstacion).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProcesoPreparacion>()
                .HasIndex(p => new
                {
                    p.IdProducto,
                    p.Orden
                })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
