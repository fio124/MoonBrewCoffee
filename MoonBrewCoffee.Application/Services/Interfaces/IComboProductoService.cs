namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IComboProductoService
    {
        Task GuardarProductosAsync(int idCombo, List<int> productos);

        Task<List<int>> GetProductosSeleccionadosAsync(int idCombo);

        Task<List<MoonBrewCoffee.Application.DTOs.ProductoDTO>> GetProductsByComboAsync(int idCombo);
    }
}
