using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class EstacionCocinaRepository
        : IEstacionCocinaRepository
    {
        private readonly MoonBrewContext _context;

        public EstacionCocinaRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<EstacionCocina>> GetAllAsync(bool incluirInactivos = true)
        {
            var query = _context.EstacionesCocina.AsNoTracking();

            if (!incluirInactivos)
                query = query.Where(e => e.Activo);

            return await query
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<EstacionCocina?> GetByIdAsync(int id)
        {
            return await _context.EstacionesCocina
                .FirstOrDefaultAsync(e => e.IdEstacion == id);
        }
    }
}
