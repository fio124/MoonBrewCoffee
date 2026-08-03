using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class MenuDisponibleController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IMenuProductoService _menuProductoService;
        private readonly IMenuComboService _menuComboService;

        public MenuDisponibleController(
            IMenuService menuService,
            IMenuProductoService menuProductoService,
            IMenuComboService menuComboService)
        {
            _menuService = menuService;
            _menuProductoService = menuProductoService;
            _menuComboService = menuComboService;
        }

        public async Task<IActionResult> Index(int? menuId)
        {
            var menus = await _menuService.GetMenusDisponiblesActualesAsync();

            if (menus.Count == 0)
                return View(null);

            var menu = menuId.HasValue
                ? menus.FirstOrDefault(item => item.IdMenu == menuId.Value)
                : null;

            menu ??= menus[0];
            ViewBag.MenusDisponibles = menus;

            var productos =
                await _menuProductoService
                    .GetProductosCompletosAsync(menu.IdMenu);

            var combos =
                await _menuComboService
                    .GetCombosCompletosAsync(menu.IdMenu);

            var modelo = new MenuDisponibleDTO
            {
                IdMenu = menu.IdMenu,
                Nombre = menu.Nombre,
                FechaInicio = menu.FechaInicio,
                FechaFin = menu.FechaFin,
                HoraInicio = menu.HoraInicio,
                HoraFin = menu.HoraFin,

                Categorias = productos
                    .Where(p => p.Categoria != null)
                    .GroupBy(p => new
                    {
                        p.Categoria!.IdCategoria,
                        p.Categoria.Nombre
                    })
                    .Select(grupo => new CategoriaMenuDTO
                    {
                        IdCategoria = grupo.Key.IdCategoria,
                        Nombre = grupo.Key.Nombre,
                        Productos = grupo
                            .OrderBy(p => p.Nombre)
                            .ToList()
                    })
                    .OrderBy(c => c.Nombre)
                    .ToList(),

                Combos = combos
                    .OrderBy(c => c.Nombre)
                    .ToList()
            };

            return View(modelo);
        }
    }
}
