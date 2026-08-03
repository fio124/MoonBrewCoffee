using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Controllers;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class CombosController : Controller
    {
        private readonly IComboService _comboService;
        private readonly IProductoService _productoService;
        private readonly IComboProductoService _comboProductoService;

        public CombosController(
            IComboService comboService,
            IProductoService productoService,
            IComboProductoService comboProductoService)
        {
            _comboService = comboService;
            _productoService = productoService;
            _comboProductoService = comboProductoService;
        }

        //GET: Productos
        private async Task CargarProductos()
        {
            ViewBag.Productos = await _productoService.GetSummaryAsync(false);
        }

        // GET: Combos
        public async Task<IActionResult> Index()
        {
            var combos = await _comboService.GetSummaryAsync();
            return View(combos);
        }

        [HttpGet]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Image(int id)
        {
            return StoredImageResult.Create(
                this,
                await _comboService.GetImageAsync(id));
        }

        // GET: Combos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var combo = await _comboService.GetSummaryByIdAsync(id.Value);

            if (combo == null)
                return NotFound();

            combo.Productos = await _comboProductoService
                .GetProductsByComboAsync(combo.IdCombo);

            return View(combo);
        }

        // GET: Combos/Create
        public async Task<IActionResult> Create()
        {
            await CargarProductos();

            return View(new ComboDTO());
        }

        // POST: Combos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboDTO combo)
        {
            if (ModelState.IsValid)
            {
                await _comboService.AddAsync(combo);
                TempData["SuccessMessage"] = "El combo se creó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            await CargarProductos();

            return View(combo);
        }

        // GET: Combos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var combo = await _comboService.GetByIdAsync(id.Value);

            if (combo == null)
                return NotFound();

            combo.ProductosSeleccionados =
                await _comboProductoService.GetProductosSeleccionadosAsync(id.Value);

            await CargarProductos();

            return View(combo);
        }

        // POST: Combos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComboDTO combo)
        {
            if (id != combo.IdCombo)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _comboService.UpdateAsync(combo);
                TempData["SuccessMessage"] = "El combo se actualizó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            await CargarProductos();

            return View(combo);
        }

        // GET: Combos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var combo = await _comboService.GetByIdAsync(id.Value);

            if (combo == null)
                return NotFound();

            return View(combo);
        }

        // POST: Combos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _comboService.DeleteAsync(id);
            TempData["SuccessMessage"] = "El combo se desactivó correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
