using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class ProcesosPreparacionController : Controller
    {
        private readonly IProcesoPreparacionService _procesoService;
        private readonly IProductoService _productoService;
        private readonly IEstacionCocinaService _estacionService;

        public ProcesosPreparacionController(
            IProcesoPreparacionService procesoService,
            IProductoService productoService,
            IEstacionCocinaService estacionService)
        {
            _procesoService = procesoService;
            _productoService = productoService;
            _estacionService = estacionService;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _procesoService.GetResumenAsync();

            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var proceso = await _procesoService.GetProcesoCompletoAsync(id);

            if (proceso == null || !proceso.Any())
                return NotFound();

            return View(proceso);
        }

        public async Task<IActionResult> Create(int? idProducto)
        {
            await CargarCombos(idProducto);

            var modelo = new ProcesoPreparacionDTO();

            if (idProducto.HasValue)
            {
                modelo.IdProducto = idProducto.Value;
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProcesoPreparacionDTO proceso)
        {
            if (await _procesoService.OrdenExisteAsync(
                    proceso.IdProducto,
                    proceso.Orden))
            {
                ModelState.AddModelError(
                    "Orden",
                    "Ya existe ese orden para este producto.");
            }

            if (ModelState.IsValid)
            {
                await _procesoService.AddAsync(proceso);

                TempData["SuccessMessage"] = "El paso de preparación se creó correctamente.";

                return RedirectToAction(
                nameof(Details),
                new { id = proceso.IdProducto });
            }

            await CargarCombos(proceso.IdProducto);

            return View(proceso);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var proceso = await _procesoService.GetByIdAsync(id);

            if (proceso == null)
                return NotFound();

            await CargarCombos(proceso.IdProducto);

            return View(proceso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ProcesoPreparacionDTO proceso)
        {
            if (id != proceso.IdProceso)
                return NotFound();

            if (await _procesoService.OrdenExisteAsync(
                    proceso.IdProducto,
                    proceso.Orden,
                    proceso.IdProceso))
            {
                ModelState.AddModelError(
                    "Orden",
                    "Ya existe ese orden para este producto.");
            }

            if (ModelState.IsValid)
            {
                await _procesoService.UpdateAsync(proceso);

                TempData["SuccessMessage"] = "El paso de preparación se actualizó correctamente.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = proceso.IdProducto });
            }

            await CargarCombos(proceso.IdProducto);

            return View(proceso);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var proceso = await _procesoService.GetByIdAsync(id);

            if (proceso == null)
                return NotFound();

            return View(proceso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _procesoService.DeleteAsync(id);

            TempData["SuccessMessage"] = "El paso de preparación se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? idProductoSeleccionado = null)
        {
            ViewBag.Productos = new SelectList(
                await _productoService.GetOptionsAsync(),
                "IdProducto",
                "Nombre",
                idProductoSeleccionado);

            ViewBag.Estaciones = new SelectList(
                await _estacionService.GetAllAsync(false),
                "IdEstacion",
                "Nombre");
        }
    }
}
