using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
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

        public Task<int> CountActiveAsync()
        {
            return _context.Productos
                .AsNoTracking()
                .CountAsync(producto => producto.Activo);
        }

        public async Task<List<Producto>> GetSummaryAsync(
            bool incluirInactivos = true)
        {
            var query = _context.Productos.AsNoTracking();

            if (!incluirInactivos)
                query = query.Where(producto => producto.Activo);

            return await query
                .OrderBy(producto => producto.Nombre)
                .Select(producto => new Producto
                {
                    IdProducto = producto.IdProducto,
                    IdCategoria = producto.IdCategoria,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    TiempoPreparacion = producto.TiempoPreparacion,
                    Activo = producto.Activo,
                    Image64 = string.IsNullOrEmpty(producto.Image64)
                        ? null
                        : "available",
                    Categoria = producto.Categoria == null
                        ? null
                        : new Categoria
                        {
                            IdCategoria = producto.Categoria.IdCategoria,
                            Nombre = producto.Categoria.Nombre
                        }
                })
                .ToListAsync();
        }

        public async Task<List<Producto>> GetOptionsAsync(
            bool incluirInactivos = false)
        {
            var query = _context.Productos.AsNoTracking();

            if (!incluirInactivos)
                query = query.Where(producto => producto.Activo);

            return await query
                .OrderBy(producto => producto.Nombre)
                .Select(producto => new Producto
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Activo = producto.Activo
                })
                .ToListAsync();
        }

        public Task<string?> GetImageAsync(int id)
        {
            return _context.Productos
                .AsNoTracking()
                .Where(producto => producto.IdProducto == id)
                .Select(producto => producto.Image64)
                .FirstOrDefaultAsync();
        }

        public Task<Producto?> GetSummaryByIdAsync(int id)
        {
            return _context.Productos
                .AsNoTracking()
                .Where(producto => producto.IdProducto == id)
                .Select(producto => new Producto
                {
                    IdProducto = producto.IdProducto,
                    IdCategoria = producto.IdCategoria,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    TiempoPreparacion = producto.TiempoPreparacion,
                    Activo = producto.Activo,
                    Image64 = string.IsNullOrEmpty(producto.Image64)
                        ? null
                        : "available",
                    Categoria = producto.Categoria == null
                        ? null
                        : new Categoria
                        {
                            IdCategoria = producto.Categoria.IdCategoria,
                            Nombre = producto.Categoria.Nombre
                        }
                })
                .FirstOrDefaultAsync();
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
        public async Task<int> AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return producto.IdProducto;
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

        public async Task<bool> ExistsByNameAsync(
            string nombre,
            int? excluirId = null)
        {
            var nombreNormalizado = nombre.Trim().ToUpper();

            return await _context.Productos.AnyAsync(producto =>
                producto.Nombre.ToUpper() == nombreNormalizado &&
                (!excluirId.HasValue || producto.IdProducto != excluirId.Value));
        }
    }
}
