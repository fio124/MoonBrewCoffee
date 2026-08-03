using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CarritoController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(ToResponse(await _cartService.GetAsync()));
        }

        [HttpPost("items")]
        public async Task<IActionResult> Add(AddCartItemRequest request)
        {
            try
            {
                return Ok(ToResponse(await _cartService.AddAsync(request), "Artículo agregado al carrito."));
            }
            catch (Exception exception) when (exception is ArgumentException or KeyNotFoundException)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpPatch("items/{key}")]
        public async Task<IActionResult> Update(string key, UpdateCartItemRequest request)
        {
            try
            {
                return Ok(ToResponse(await _cartService.UpdateAsync(key, request)));
            }
            catch (Exception exception) when (exception is ArgumentException or KeyNotFoundException)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpDelete("items/{key}")]
        public async Task<IActionResult> Remove(string key)
        {
            return Ok(ToResponse(await _cartService.RemoveAsync(key)));
        }

        private static object ToResponse(CartViewModel cart, string? message = null) => new
        {
            message,
            itemCount = cart.ItemCount,
            subtotal = cart.Subtotal,
            tax = cart.Tax,
            total = cart.Total,
            isEmpty = cart.IsEmpty
        };
    }
}
