using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class ComboProductoRepository : IComboProductoRepository
    {
        private readonly MoonBrewContext _context;

        public ComboProductoRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<ComboProducto>> GetByComboAsync(int idCombo)
        {
            return await _context.ComboProductos
                .Where(x => x.IdCombo == idCombo)
                .ToListAsync();
        }

        public Task<List<Producto>> GetProductsByComboAsync(int idCombo)
        {
            return _context.ComboProductos
                .AsNoTracking()
                .Where(relacion => relacion.IdCombo == idCombo &&
                                   relacion.Producto != null)
                .OrderBy(relacion => relacion.Producto!.Nombre)
                .Select(relacion => new Producto
                {
                    IdProducto = relacion.IdProducto,
                    Nombre = relacion.Producto!.Nombre,
                    Precio = relacion.Producto.Precio,
                    Activo = relacion.Producto.Activo
                })
                .ToListAsync();
        }

        public async Task EliminarProductosAsync(int idCombo)
        {
            var lista = await _context.ComboProductos
                .Where(x => x.IdCombo == idCombo)
                .ToListAsync();

            _context.ComboProductos.RemoveRange(lista);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarProductosAsync(int idCombo, List<int> productos)
        {
            foreach (var idProducto in productos)
            {
                _context.ComboProductos.Add(new ComboProducto
                {
                    IdCombo = idCombo,
                    IdProducto = idProducto,
                    Cantidad = 1
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
