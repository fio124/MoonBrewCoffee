using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Models.Entidades;

namespace MoonBrewCoffee.Data
{
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

        public DbSet<EstacionCocina> EstacionesCocina { get; set; }

        public DbSet<ProcesoPreparacion> ProcesosPreparacion { get; set; }

        public DbSet<Carrito> Carritos { get; set; }

        public DbSet<CarritoDetalle> CarritoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasKey(x => x.IdRol);
            modelBuilder.Entity<Usuario>().HasKey(x => x.IdUsuario);

            modelBuilder.Entity<Categoria>().HasKey(x => x.IdCategoria);
            modelBuilder.Entity<Producto>().HasKey(x => x.IdProducto);
            modelBuilder.Entity<Ingrediente>().HasKey(x => x.IdIngrediente);

            modelBuilder.Entity<Producto>()
                  .Property(p => p.Precio)
                  .HasPrecision(10, 2);


            modelBuilder.Entity<ProductoIngrediente>()
                .HasKey(pi => new
                {
                    pi.IdProducto,
                    pi.IdIngrediente
                });

            modelBuilder.Entity<Combo>()
                 .HasKey(x => x.IdCombo);

            modelBuilder.Entity<Combo>()
                .Property(c => c.PrecioCombo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Menu>()
                .HasKey(x => x.IdMenu);

            modelBuilder.Entity<ComboProducto>()
                .HasKey(cp => new
                {
                    cp.IdCombo,
                    cp.IdProducto
                });

            modelBuilder.Entity<MenuProducto>()
                .HasKey(mp => new
                {
                    mp.IdMenu,
                    mp.IdProducto
                });

            modelBuilder.Entity<Pedido>()
                .Property(p => p.CostoEnvio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Impuesto)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pago>()
                .Property(p => p.TotalPagado)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}