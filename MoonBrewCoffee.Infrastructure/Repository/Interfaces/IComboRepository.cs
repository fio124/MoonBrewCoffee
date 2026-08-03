using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IComboRepository
    {
        Task<List<Combo>> GetAllAsync(bool incluirInactivos = true);

        Task<List<Combo>> GetSummaryAsync(bool incluirInactivos = true);

        Task<List<Combo>> GetOptionsAsync(bool incluirInactivos = false);

        Task<Combo?> GetSummaryByIdAsync(int id);

        Task<string?> GetImageAsync(int id);

        Task<int> CountActiveAsync();

        Task<Combo?> GetByIdAsync(int id);

        Task<int> AddAsync(Combo combo);

        Task UpdateAsync(Combo combo);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
