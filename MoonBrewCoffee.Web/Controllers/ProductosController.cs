using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Controllers;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IIngredienteService _ingredienteService;
        private readonly IProductoIngredienteService _productoIngredienteService;

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            IIngredienteService ingredienteService,
            IProductoIngredienteService productoIngredienteService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _ingredienteService = ingredienteService;
            _productoIngredienteService = productoIngredienteService;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.GetSummaryAsync();
            return View(productos);
        }

        [HttpGet]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Image(int id)
        {
            return StoredImageResult.Create(
                this,
                await _productoService.GetImageAsync(id));
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetSummaryByIdAsync(id.Value);

            if (producto == null)
                return NotFound();

            ViewBag.IngredientesDetalle = await _productoIngredienteService
                .GetIngredientesCompletosAsync(producto.IdProducto);

            return View(producto);
        }

        // GET: Productos/Create
        public async Task<IActionResult> Create()
        {
            await CargarCategorias();
            await CargarIngredientes();

            return View(new ProductoDTO());
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDTO producto)
        {
            if (!string.IsNullOrWhiteSpace(producto.Nombre) &&
                await _productoService.ExistsByNameAsync(producto.Nombre))
            {
                ModelState.AddModelError(
                    nameof(producto.Nombre),
                    "Ya existe un producto con este nombre.");
            }

            if (ModelState.IsValid)
            {
                await _productoService.AddAsync(producto);
                TempData["SuccessMessage"] = "El producto se creó correctamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarCategorias();
            await CargarIngredientes();
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);

            if (producto == null)
                return NotFound();

            await CargarCategorias();
            await CargarIngredientes();

            producto.IngredientesSeleccionados =
                await _productoIngredienteService.GetIngredientesAsync(producto.IdProducto);

            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoDTO producto)
        {
            if (id != producto.IdProducto)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(producto.Nombre) &&
                await _productoService.ExistsByNameAsync(producto.Nombre, id))
            {
                ModelState.AddModelError(
                    nameof(producto.Nombre),
                    "Ya existe otro producto con este nombre.");
            }

            if (ModelState.IsValid)
            {
                await _productoService.UpdateAsync(producto);
                TempData["SuccessMessage"] = "El producto se actualizó correctamente.";

                return RedirectToAction(nameof(Index));
            }

            await CargarCategorias();
            await CargarIngredientes();

            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productoService.DeleteAsync(id);

            TempData["SuccessMessage"] = "El producto se desactivó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCategorias()
        {
            var categorias = await _categoriaService.GetAllAsync();

            ViewBag.IdCategoria = new SelectList(
                categorias,
                "IdCategoria",
                "Nombre");
        }

        private async Task CargarIngredientes()
        {
            ViewBag.Ingredientes =
                await _ingredienteService.GetAllAsync(false);
        }
    }
}
