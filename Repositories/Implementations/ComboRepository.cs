using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Data;
using MoonBrewCoffee.Models.Entidades;
using MoonBrewCoffee.Repositories.Interfaces;

namespace MoonBrewCoffee.Repositories.Implementations
{
    public class ComboRepository : IComboRepository
    {
        private readonly MoonBrewContext _context;

        public ComboRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Combo>> GetAllAsync()
        {
            return await _context.Combos.ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _context.Combos
                .FirstOrDefaultAsync(c => c.IdCombo == id);
        }

        public async Task AddAsync(Combo combo)
        {
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Combo combo)
        {
            _context.Combos.Update(combo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);

            if (combo != null)
            {
                _context.Combos.Remove(combo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Combos
                .AnyAsync(c => c.IdCombo == id);
        }
    }
}