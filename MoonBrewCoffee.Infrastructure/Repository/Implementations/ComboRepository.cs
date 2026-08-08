using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class ComboRepository : IComboRepository
    {
        private readonly MoonBrewContext _context;

        public ComboRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Combo>> GetAllAsync(bool incluirInactivos = true)
        {
            var query = _context.Combos.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(c => c.Activo);
            }

            return await query.ToListAsync();
        }

        public Task<int> CountActiveAsync()
        {
            return _context.Combos
                .AsNoTracking()
                .CountAsync(combo => combo.Activo);
        }

        public async Task<List<Combo>> GetSummaryAsync(
            bool incluirInactivos = true)
        {
            var query = _context.Combos.AsNoTracking();

            if (!incluirInactivos)
                query = query.Where(combo => combo.Activo);

            return await query
                .OrderBy(combo => combo.Nombre)
                .Select(combo => new Combo
                {
                    IdCombo = combo.IdCombo,
                    Nombre = combo.Nombre,
                    Descripcion = combo.Descripcion,
                    PrecioCombo = combo.PrecioCombo,
                    Activo = combo.Activo,
                    ImagenURL = string.IsNullOrEmpty(combo.ImagenURL)
                        ? null
                        : "available"
                })
                .ToListAsync();
        }

        public async Task<List<Combo>> GetOptionsAsync(
            bool incluirInactivos = false)
        {
            var query = _context.Combos.AsNoTracking();

            if (!incluirInactivos)
                query = query.Where(combo => combo.Activo);

            return await query
                .OrderBy(combo => combo.Nombre)
                .Select(combo => new Combo
                {
                    IdCombo = combo.IdCombo,
                    Nombre = combo.Nombre,
                    Activo = combo.Activo
                })
                .ToListAsync();
        }

        public Task<Combo?> GetSummaryByIdAsync(int id)
        {
            return _context.Combos
                .AsNoTracking()
                .Where(combo => combo.IdCombo == id)
                .Select(combo => new Combo
                {
                    IdCombo = combo.IdCombo,
                    Nombre = combo.Nombre,
                    Descripcion = combo.Descripcion,
                    PrecioCombo = combo.PrecioCombo,
                    Activo = combo.Activo,
                    ImagenURL = string.IsNullOrEmpty(combo.ImagenURL)
                        ? null
                        : "available"
                })
                .FirstOrDefaultAsync();
        }

        public Task<string?> GetImageAsync(int id)
        {
            return _context.Combos
                .AsNoTracking()
                .Where(combo => combo.IdCombo == id)
                .Select(combo => combo.ImagenURL)
                .FirstOrDefaultAsync();
        }

        public async Task ActivarAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);

            if (combo != null)
            {
                combo.Activo = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _context.Combos
                .FirstOrDefaultAsync(c => c.IdCombo == id);
        }

        public async Task<int> AddAsync(Combo combo)
        {
            _context.Combos.Add(combo);

            await _context.SaveChangesAsync();

            return combo.IdCombo;
        }

        public async Task UpdateAsync(Combo combo)
        {
            _context.Combos.Update(combo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);

            if (combo == null)
                return;

            // Borrado lógico: el combo puede estar relacionado con menús,
            // pedidos y productos que deben conservar su historial.
            combo.Activo = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Combos
                .AnyAsync(c => c.IdCombo == id);
        }
    }
}
