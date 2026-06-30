using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Data;
using MoonBrewCoffee.Models.Entidades;
using MoonBrewCoffee.Repositories.Interfaces;

namespace MoonBrewCoffee.Repositories.Implementations
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly MoonBrewContext _context;

        public CategoriaRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task AddAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categorias
                .AnyAsync(c => c.IdCategoria == id);
        }
    }
}