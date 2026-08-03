namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IProductoIngredienteService
    {
        Task<List<int>> GetIngredientesAsync(int idProducto);

        Task<List<MoonBrewCoffee.Application.DTOs.IngredienteDTO>>
            GetIngredientesCompletosAsync(int idProducto);

        Task GuardarIngredientesAsync(int idProducto, List<int> ingredientes);
    }
}
