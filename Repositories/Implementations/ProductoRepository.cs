using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Data;
using MoonBrewCoffee.Models.Entidades;
using MoonBrewCoffee.Repositories.Interfaces;

namespace MoonBrewCoffee.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly MoonBrewContext _context;

        public ProductoRepository(MoonBrewContext context)
        {
            _context = context;
        }

        // ==========================
        // Obtener todos los productos
        // ==========================
        public async Task<List<Producto>> GetAllAsync(bool incluirInactivos = true)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(p => p.Activo);
            }

            return await query
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // ==========================
        // Obtener producto por Id
        // ==========================
        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        // ==========================
        // Agregar producto
        // ==========================
        public async Task AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        // ==========================
        // Actualizar producto
        // ==========================
        public async Task UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        // ==========================
        // Desactivar producto
        // ==========================
        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return;

            producto.Activo = false;

            _context.Productos.Update(producto);

            await _context.SaveChangesAsync();
        }

        // ==========================
        // Reactivar producto
        // ==========================
        public async Task ActivarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return;

            producto.Activo = true;

            _context.Productos.Update(producto);

            await _context.SaveChangesAsync();
        }

        // ==========================
        // Validar existencia
        // ==========================
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Productos
                .AnyAsync(p => p.IdProducto == id);
        }
    }
}