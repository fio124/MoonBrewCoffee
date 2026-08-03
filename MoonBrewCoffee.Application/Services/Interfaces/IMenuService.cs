using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDTO>> GetAllAsync(
            bool incluirInactivos = true);

        Task<int> CountActiveAsync();

        Task<MenuDTO?> GetByIdAsync(int id);

        Task<MenuDTO?> GetSummaryByIdAsync(int id);

        Task<int> AddAsync(MenuDTO menu);

        Task UpdateAsync(MenuDTO menu);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
        Task<MenuDTO?> GetMenuDisponibleActualAsync();
        Task<List<MenuDTO>> GetMenusDisponiblesActualesAsync();
    }
}
