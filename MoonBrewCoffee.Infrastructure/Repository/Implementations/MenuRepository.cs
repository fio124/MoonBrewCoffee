using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MoonBrewContext _context;

        public MenuRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> GetAllAsync(
            bool incluirInactivos = true)
        {
            var query = _context.Menus.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(m => m.Activo);
            }

            return await query
                .OrderByDescending(m => m.FechaInicio)
                .ThenByDescending(m => m.HoraInicio)
                .ToListAsync();
        }

        public Task<int> CountActiveAsync()
        {
            return _context.Menus
                .AsNoTracking()
                .CountAsync(menu => menu.Activo);
        }

        public Task<Menu?> GetAvailableNowAsync(DateTime date, TimeSpan time)
        {
            return _context.Menus
                .AsNoTracking()
                .Where(menu =>
                    menu.Activo &&
                    menu.Disponible &&
                    date >= menu.FechaInicio &&
                    date <= menu.FechaFin &&
                    time >= menu.HoraInicio &&
                    time <= menu.HoraFin)
                // Puede haber varios menús vigentes al mismo tiempo. En ese
                // caso se publica el configurado más recientemente.
                .OrderByDescending(menu => menu.IdMenu)
                .FirstOrDefaultAsync();
        }

        public Task<List<Menu>> GetAvailableAsync(DateTime date, TimeSpan time)
        {
            return _context.Menus
                .AsNoTracking()
                .Where(menu =>
                    menu.Activo &&
                    menu.Disponible &&
                    date >= menu.FechaInicio &&
                    date <= menu.FechaFin &&
                    time >= menu.HoraInicio &&
                    time <= menu.HoraFin)
                .OrderByDescending(menu => menu.IdMenu)
                .ToListAsync();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus
                .FirstOrDefaultAsync(m => m.IdMenu == id);
        }

        public Task<Menu?> GetSummaryByIdAsync(int id)
        {
            return _context.Menus
                .AsNoTracking()
                .Where(menu => menu.IdMenu == id)
                .Select(menu => new Menu
                {
                    IdMenu = menu.IdMenu,
                    Nombre = menu.Nombre,
                    FechaInicio = menu.FechaInicio,
                    FechaFin = menu.FechaFin,
                    HoraInicio = menu.HoraInicio,
                    HoraFin = menu.HoraFin,
                    Disponible = menu.Disponible,
                    Activo = menu.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(Menu menu)
        {
            _context.Menus.Add(menu);

            await _context.SaveChangesAsync();

            return menu.IdMenu;
        }

        public async Task UpdateAsync(Menu menu)
        {
            var existente = await _context.Menus
                .FirstOrDefaultAsync(
                    m => m.IdMenu == menu.IdMenu);

            if (existente == null)
                return;

            existente.Nombre = menu.Nombre;
            existente.FechaInicio = menu.FechaInicio;
            existente.FechaFin = menu.FechaFin;
            existente.HoraInicio = menu.HoraInicio;
            existente.HoraFin = menu.HoraFin;
            existente.ImagenURL = menu.ImagenURL;
            existente.Disponible = menu.Disponible;
            existente.Activo = menu.Activo;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
                return;

            menu.Activo = false;

            await _context.SaveChangesAsync();
        }

        public async Task ActivarAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
                return;

            menu.Activo = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Menus
                .AnyAsync(m => m.IdMenu == id);
        }
    }
}
