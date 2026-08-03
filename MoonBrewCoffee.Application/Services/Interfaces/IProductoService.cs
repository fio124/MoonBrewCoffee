using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<List<ProductoDTO>> GetSummaryAsync(bool incluirInactivos = true);

        Task<List<ProductoDTO>> GetOptionsAsync(bool incluirInactivos = false);

        Task<ProductoDTO?> GetSummaryByIdAsync(int id);

        Task<string?> GetImageAsync(int id);

        Task<int> CountActiveAsync();

        Task<ProductoDTO?> GetByIdAsync(int id);

        Task<int> AddAsync(ProductoDTO producto);

        Task UpdateAsync(ProductoDTO producto);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsByNameAsync(string nombre, int? excluirId = null);
    }
}
