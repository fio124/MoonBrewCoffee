using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Data;
using MoonBrewCoffee.Models.Entidades;
using MoonBrewCoffee.Repositories.Interfaces;

namespace MoonBrewCoffee.Repositories.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MoonBrewContext _context;

        public MenuRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> GetAllAsync()
        {
            return await _context.Menus.ToListAsync();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus
                .FirstOrDefaultAsync(m => m.IdMenu == id);
        }

        public async Task AddAsync(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Menus
                .AnyAsync(m => m.IdMenu == id);
        }
    }
}