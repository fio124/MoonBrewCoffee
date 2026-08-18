using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class MenusController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProductoService _productoService;
        private readonly IComboService _comboService;
        private readonly IMenuProductoService _menuProductoService;
        private readonly IMenuComboService _menuComboService;

        public MenusController(
            IMenuService menuService,
            IProductoService productoService,
            IComboService comboService,
            IMenuProductoService menuProductoService,
            IMenuComboService menuComboService)
        {
            _menuService = menuService;
            _productoService = productoService;
            _comboService = comboService;
            _menuProductoService = menuProductoService;
            _menuComboService = menuComboService;
        }

        // GET: Menus
        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var menu = await _menuService.GetSummaryByIdAsync(id.Value);

            if (menu == null)
                return NotFound();

            menu.Productos = await _menuProductoService
                .GetProductosCompletosAsync(menu.IdMenu);
            menu.Combos = await _menuComboService
                .GetCombosCompletosAsync(menu.IdMenu);

            return View(menu);
        }

        // GET: Menus/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();

            return View(new MenuDTO());
        }

        // POST: Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuDTO menu)
        {
            if (ModelState.IsValid)
            {
                await _menuService.AddAsync(menu);
                TempData["SuccessMessage"] = "El menú se creó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            await CargarListasAsync();

            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var menu = await _menuService.GetByIdAsync(id.Value);

            if (menu == null)
                return NotFound();
            await CargarListasAsync();

            return View(menu);
        }

        // POST: Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuDTO menu)
        {
            if (id != menu.IdMenu)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _menuService.UpdateAsync(menu);
                TempData["SuccessMessage"] = "El menú se actualizó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            await CargarListasAsync();

            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var menu = await _menuService.GetByIdAsync(id.Value);

            if (menu == null)
                return NotFound();

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _menuService.DeleteAsync(id);
            TempData["SuccessMessage"] = "El menú se desactivó correctamente.";
            return RedirectToAction(nameof(Index));
        }
        private async Task CargarListasAsync()
        {
            ViewBag.Productos = await _productoService.GetOptionsAsync();
            ViewBag.Combos = await _comboService.GetOptionsAsync();
        }
    }
}
