using MoonBrewCoffee.Models.Entidades;

namespace MoonBrewCoffee.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        // Obtener todos los productos
        Task<List<Producto>> GetAllAsync(bool incluirInactivos = true);

        // Obtener un producto por Id
        Task<Producto?> GetByIdAsync(int id);

        // Crear
        Task AddAsync(Producto producto);

        // Editar
        Task UpdateAsync(Producto producto);

        // Desactivar
        Task DeleteAsync(int id);

        // Reactivar
        Task ActivarAsync(int id);

        // Validar existencia
        Task<bool> ExistsAsync(int id);
    }
}