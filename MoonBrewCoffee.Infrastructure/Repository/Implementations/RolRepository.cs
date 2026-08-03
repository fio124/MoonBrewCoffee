using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class RolRepository : IRolRepository
    {
        private readonly MoonBrewContext _context;

        public RolRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Rol>> GetAllAsync(bool incluirInactivos = true)
        {
            var query = _context.Roles.AsQueryable();

            if (!incluirInactivos)
                query = query.Where(r => r.Activo);

            return await query.ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.IdRol == id);
        }

        public async Task AddAsync(Rol rol)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rol rol)
        {
            _context.Roles.Update(rol);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol != null)
            {
                rol.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivarAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol != null)
            {
                rol.Activo = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Roles
                .AnyAsync(r => r.IdRol == id);
        }
    }
}