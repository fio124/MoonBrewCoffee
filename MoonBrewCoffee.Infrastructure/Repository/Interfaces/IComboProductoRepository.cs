using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IComboProductoRepository
    {
        Task<List<ComboProducto>> GetByComboAsync(int idCombo);

        Task<List<Producto>> GetProductsByComboAsync(int idCombo);

        Task GuardarProductosAsync(int idCombo, List<int> productos);

        Task EliminarProductosAsync(int idCombo);
    }
}
