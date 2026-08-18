using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class ProductoIngredienteRepository : IProductoIngredienteRepository
    {
        private readonly MoonBrewContext _context;

        public ProductoIngredienteRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoIngrediente>> GetByProductoAsync(int idProducto)
        {
            return await _context.ProductoIngredientes
                .Where(x => x.IdProducto == idProducto)
                .ToListAsync();
        }

        public Task<List<Ingrediente>> GetIngredientesByProductoAsync(int idProducto)
        {
            return _context.ProductoIngredientes
                .AsNoTracking()
                .Where(relacion => relacion.IdProducto == idProducto &&
                    relacion.Ingrediente != null)
                .OrderBy(relacion => relacion.Ingrediente!.Nombre)
                .Select(relacion => new Ingrediente
                {
                    IdIngrediente = relacion.Ingrediente!.IdIngrediente,
                    Nombre = relacion.Ingrediente.Nombre,
                    Activo = relacion.Ingrediente.Activo
                })
                .ToListAsync();
        }

        public async Task EliminarIngredientesAsync(int idProducto)
        {
            var lista = await _context.ProductoIngredientes
                .Where(x => x.IdProducto == idProducto)
                .ToListAsync();

            _context.ProductoIngredientes.RemoveRange(lista);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarIngredientesAsync(int idProducto, List<int> ingredientes)
        {
            foreach (var idIngrediente in ingredientes)
            {
                _context.ProductoIngredientes.Add(
                    new ProductoIngrediente
                    {
                        IdProducto = idProducto,
                        IdIngrediente = idIngrediente
                    });
            }

            await _context.SaveChangesAsync();
        }
    }
}
