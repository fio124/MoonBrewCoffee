using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class MenuComboRepository : IMenuComboRepository
    {
        private readonly MoonBrewContext _context;

        public MenuComboRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<MenuCombo>> GetByMenuAsync(int idMenu)
        {
            return await _context.MenuCombos
                .Where(x => x.IdMenu == idMenu)
                .ToListAsync();
        }

        public async Task EliminarCombosAsync(int idMenu)
        {
            var relaciones = await _context.MenuCombos
                .Where(x => x.IdMenu == idMenu)
                .ToListAsync();

            if (relaciones.Count == 0)
                return;

            _context.MenuCombos.RemoveRange(relaciones);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarCombosAsync(
            int idMenu,
            List<int> combos)
        {
            combos ??= new List<int>();

            var combosSinDuplicados = combos.Distinct();

            foreach (var idCombo in combosSinDuplicados)
            {
                _context.MenuCombos.Add(
                    new MenuCombo
                    {
                        IdMenu = idMenu,
                        IdCombo = idCombo
                    });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuCombo>>GetCombosCompletosByMenuAsync(int idMenu)
        {
            return await _context.MenuCombos
                .AsNoTracking()
                .Where(x => x.IdMenu == idMenu)
                .Select(relacion => new MenuCombo
                {
                    IdMenu = relacion.IdMenu,
                    IdCombo = relacion.IdCombo,
                    Combo = relacion.Combo == null
                        ? null
                        : new Combo
                        {
                            IdCombo = relacion.Combo.IdCombo,
                            Nombre = relacion.Combo.Nombre,
                            Descripcion = relacion.Combo.Descripcion,
                            PrecioCombo = relacion.Combo.PrecioCombo,
                            Activo = relacion.Combo.Activo,
                            ImagenURL = string.IsNullOrEmpty(relacion.Combo.ImagenURL)
                                ? null
                                : "available"
                        }
                })
                .ToListAsync();
        }
    }
}
