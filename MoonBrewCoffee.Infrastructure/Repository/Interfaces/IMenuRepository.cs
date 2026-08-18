using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<Menu>> GetAllAsync(
            bool incluirInactivos = true);

        Task<int> CountActiveAsync();

        Task<Menu?> GetAvailableNowAsync(DateTime date, TimeSpan time);

        Task<List<Menu>> GetAvailableAsync(DateTime date, TimeSpan time);

        Task<Menu?> GetByIdAsync(int id);

        Task<Menu?> GetSummaryByIdAsync(int id);

        Task<int> AddAsync(Menu menu);

        Task UpdateAsync(Menu menu);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
