using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Models.Entidades;
using MoonBrewCoffee.Repositories.Interfaces;

namespace MoonBrewCoffee.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly IProductoRepository _productoRepository;

        public CatalogoController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoRepository.GetAllAsync();
            return View(productos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}