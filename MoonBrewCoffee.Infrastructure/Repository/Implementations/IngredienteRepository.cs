using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class IngredienteRepository : IIngredienteRepository
    {
        private readonly MoonBrewContext _context;

        public IngredienteRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Ingrediente>> GetAllAsync(bool incluirInactivos = true)
        {
            var query = _context.Ingredientes.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(i => i.Activo);
            }

            return await query.ToListAsync();
        }

        public async Task ActivarAsync(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente != null)
            {
                ingrediente.Activo = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Ingrediente>> GetAllAsync()
        {
            return await _context.Ingredientes.ToListAsync();
        }

        public async Task<Ingrediente?> GetByIdAsync(int id)
        {
            return await _context.Ingredientes
                .FirstOrDefaultAsync(i => i.IdIngrediente == id);
        }

        public async Task AddAsync(Ingrediente ingrediente)
        {
            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ingrediente ingrediente)
        {
            _context.Ingredientes.Update(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);

            if (ingrediente != null)
            {
                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Ingredientes
                .AnyAsync(i => i.IdIngrediente == id);
        }
    }
}