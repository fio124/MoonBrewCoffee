using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Controllers;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class CombosController : Controller
    {
        private readonly IComboService _comboService;
        private readonly IProductoService _productoService;
        private readonly IComboProductoService _comboProductoService;
        private readonly IServicioImagenes _servicioImagenes;

        public CombosController(
            IComboService comboService,
            IProductoService productoService,
            IComboProductoService comboProductoService,
            IServicioImagenes servicioImagenes)
        {
            _comboService = comboService;
            _productoService = productoService;
            _comboProductoService = comboProductoService;
            _servicioImagenes = servicioImagenes;
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
        public async Task<IActionResult> Create(
            ComboDTO combo,
            IFormFile? imagenArchivo,
            CancellationToken cancellationToken)
        {
            ModelState.Remove(nameof(combo.ImagenURL));
            if (imagenArchivo is null)
                ModelState.AddModelError(nameof(combo.ImagenURL), "Seleccione una imagen para el combo.");

            if (ModelState.IsValid)
            {
                string? newImagePath = null;
                try
                {
                    newImagePath = await _servicioImagenes.GuardarAsync(
                        imagenArchivo!, "combos", cancellationToken);
                    combo.ImagenURL = newImagePath;
                    await _comboService.AddAsync(combo);
                    TempData["SuccessMessage"] = "El combo se creó correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException exception)
                {
                    _servicioImagenes.EliminarSiEsLocal(newImagePath);
                    ModelState.AddModelError(nameof(combo.ImagenURL), exception.Message);
                }
                catch
                {
                    _servicioImagenes.EliminarSiEsLocal(newImagePath);
                    throw;
                }
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
        public async Task<IActionResult> Edit(
            int id,
            ComboDTO combo,
            IFormFile? imagenArchivo,
            CancellationToken cancellationToken)
        {
            if (id != combo.IdCombo)
                return NotFound();

            var existing = await _comboService.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            combo.ImagenURL = existing.ImagenURL;
            ModelState.Remove(nameof(combo.ImagenURL));

            if (ModelState.IsValid)
            {
                string? newImagePath = null;
                try
                {
                    if (imagenArchivo is not null)
                    {
                        newImagePath = await _servicioImagenes.GuardarAsync(
                            imagenArchivo, "combos", cancellationToken);
                        combo.ImagenURL = newImagePath;
                    }

                    await _comboService.UpdateAsync(combo);
                    if (newImagePath is not null)
                        _servicioImagenes.EliminarSiEsLocal(existing.ImagenURL);

                    TempData["SuccessMessage"] = "El combo se actualizó correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException exception)
                {
                    _servicioImagenes.EliminarSiEsLocal(newImagePath);
                    combo.ImagenURL = existing.ImagenURL;
                    ModelState.AddModelError(nameof(combo.ImagenURL), exception.Message);
                }
                catch
                {
                    _servicioImagenes.EliminarSiEsLocal(newImagePath);
                    throw;
                }
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
