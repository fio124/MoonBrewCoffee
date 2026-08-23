using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Repository.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly MoonBrewContext _context;

        public UsuarioRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();
        }

        public Task<int> CountActiveAsync()
        {
            return _context.Usuarios
                .AsNoTracking()
                .CountAsync(usuario => usuario.Activo);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();
            return _context.Usuarios
                .AsNoTracking()
                .AnyAsync(usuario => usuario.Correo.ToLower() == normalizedEmail);
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.IdUsuario == id);
        }
    }
}
