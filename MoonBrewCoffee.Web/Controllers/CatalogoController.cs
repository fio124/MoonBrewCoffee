using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
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
            var productos = await _productoRepository.GetSummaryAsync(false);
            return View(productos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoRepository.GetSummaryByIdAsync(id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }
    }
}
