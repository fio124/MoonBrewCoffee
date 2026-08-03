using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IMenuProductoService
    {
        Task GuardarProductosAsync(
            int idMenu,
            List<int> productos);

        Task<List<int>> GetProductosSeleccionadosAsync( int idMenu);
        Task<List<ProductoDTO>> GetProductosCompletosAsync(int idMenu);
    }
}
