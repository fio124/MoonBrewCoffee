using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class IngredientesController : Controller
    {
        private readonly IIngredienteService _ingredienteService;

        public IngredientesController(IIngredienteService ingredienteService)
        {
            _ingredienteService = ingredienteService;
        }

        // GET: Ingredientes
        public async Task<IActionResult> Index()
        {
            var ingredientes = await _ingredienteService.GetAllAsync();
            return View(ingredientes);
        }

        // GET: Ingredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ingrediente = await _ingredienteService.GetByIdAsync(id.Value);

            if (ingrediente == null)
                return NotFound();

            return View(ingrediente);
        }

        // GET: Ingredientes/Create
        public IActionResult Create()
        {
            return View(new IngredienteDTO());
        }

        // POST: Ingredientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IngredienteDTO ingrediente)
        {
            if (ModelState.IsValid)
            {
                await _ingredienteService.AddAsync(ingrediente);
                return RedirectToAction(nameof(Index));
            }

            return View(ingrediente);
        }

        // GET: Ingredientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ingrediente = await _ingredienteService.GetByIdAsync(id.Value);

            if (ingrediente == null)
                return NotFound();

            return View(ingrediente);
        }

        // POST: Ingredientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IngredienteDTO ingrediente)
        {
            if (id != ingrediente.IdIngrediente)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _ingredienteService.UpdateAsync(ingrediente);
                return RedirectToAction(nameof(Index));
            }

            return View(ingrediente);
        }

        // GET: Ingredientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ingrediente = await _ingredienteService.GetByIdAsync(id.Value);

            if (ingrediente == null)
                return NotFound();

            return View(ingrediente);
        }

        // POST: Ingredientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ingredienteService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}