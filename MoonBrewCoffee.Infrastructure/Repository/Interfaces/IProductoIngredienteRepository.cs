using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IProductoIngredienteRepository
    {
        Task<List<ProductoIngrediente>> GetByProductoAsync(int idProducto);

        Task<List<Ingrediente>> GetIngredientesByProductoAsync(int idProducto);

        Task GuardarIngredientesAsync(int idProducto, List<int> ingredientes);

        Task EliminarIngredientesAsync(int idProducto);
    }
}
