using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class MenuProductoRepository : IMenuProductoRepository
    {
        private readonly MoonBrewContext _context;

        public MenuProductoRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<MenuProducto>> GetByMenuAsync(int idMenu)
        {
            return await _context.MenuProductos
                .Where(x => x.IdMenu == idMenu)
                .ToListAsync();
        }

        public async Task EliminarProductosAsync(int idMenu)
        {
            var relaciones = await _context.MenuProductos
                .Where(x => x.IdMenu == idMenu)
                .ToListAsync();

            if (relaciones.Count == 0)
                return;

            _context.MenuProductos.RemoveRange(relaciones);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarProductosAsync(
            int idMenu,
            List<int> productos)
        {
            productos ??= new List<int>();

            var productosSinDuplicados = productos.Distinct();

            foreach (var idProducto in productosSinDuplicados)
            {
                _context.MenuProductos.Add(
                    new MenuProducto
                    {
                        IdMenu = idMenu,
                        IdProducto = idProducto
                    });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuProducto>>
        GetProductosCompletosByMenuAsync(int idMenu)
        {
            return await _context.MenuProductos
                .AsNoTracking()
                .Where(x => x.IdMenu == idMenu)
                .Select(relacion => new MenuProducto
                {
                    IdMenu = relacion.IdMenu,
                    IdProducto = relacion.IdProducto,
                    Producto = relacion.Producto == null
                        ? null
                        : new Producto
                        {
                            IdProducto = relacion.Producto.IdProducto,
                            IdCategoria = relacion.Producto.IdCategoria,
                            Nombre = relacion.Producto.Nombre,
                            Descripcion = relacion.Producto.Descripcion,
                            Precio = relacion.Producto.Precio,
                            TiempoPreparacion = relacion.Producto.TiempoPreparacion,
                            Activo = relacion.Producto.Activo,
                            Image64 = string.IsNullOrEmpty(relacion.Producto.Image64)
                                ? null
                                : "available",
                            Categoria = relacion.Producto.Categoria == null
                                ? null
                                : new Categoria
                                {
                                    IdCategoria = relacion.Producto.Categoria.IdCategoria,
                                    Nombre = relacion.Producto.Categoria.Nombre
                                }
                        }
                })
                .ToListAsync();
        }
    }
}
