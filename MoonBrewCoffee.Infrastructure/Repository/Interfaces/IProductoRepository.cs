using MoonBrewCoffee.Infrastructure.Models.Entidades;


namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IProductoRepository
    {
        // Obtener todos los productos
        Task<List<Producto>> GetAllAsync(bool incluirInactivos = true);

        Task<List<Producto>> GetSummaryAsync(bool incluirInactivos = true);

        Task<List<Producto>> GetOptionsAsync(bool incluirInactivos = false);

        Task<Producto?> GetSummaryByIdAsync(int id);

        Task<string?> GetImageAsync(int id);

        Task<int> CountActiveAsync();

        // Obtener un producto por Id
        Task<Producto?> GetByIdAsync(int id);

        // Crear
        Task<int> AddAsync(Producto producto);

        // Editar
        Task UpdateAsync(Producto producto);

        // Desactivar
        Task DeleteAsync(int id);

        // Reactivar
        Task ActivarAsync(int id);

        // Validar existencia
        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsByNameAsync(string nombre, int? excluirId = null);
    }
}
