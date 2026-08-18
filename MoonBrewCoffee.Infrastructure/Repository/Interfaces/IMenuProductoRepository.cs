using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IMenuProductoRepository
    {
        Task<List<MenuProducto>> GetByMenuAsync(int idMenu);

        Task GuardarProductosAsync(
            int idMenu,
            List<int> productos);

        Task EliminarProductosAsync(int idMenu);
        Task<List<MenuProducto>> GetProductosCompletosByMenuAsync(int idMenu);
    }
}