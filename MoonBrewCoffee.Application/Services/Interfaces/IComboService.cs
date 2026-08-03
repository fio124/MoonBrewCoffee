using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IComboService
    {
        Task<List<ComboDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<List<ComboDTO>> GetSummaryAsync(bool incluirInactivos = true);

        Task<List<ComboDTO>> GetOptionsAsync(bool incluirInactivos = false);

        Task<ComboDTO?> GetSummaryByIdAsync(int id);

        Task<string?> GetImageAsync(int id);

        Task<int> CountActiveAsync();

        Task<ComboDTO?> GetByIdAsync(int id);

        Task<int> AddAsync(ComboDTO combo);

        Task UpdateAsync(ComboDTO combo);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
