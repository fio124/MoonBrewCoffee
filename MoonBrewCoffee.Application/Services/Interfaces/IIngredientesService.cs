using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IIngredienteService
    {
        Task<List<IngredienteDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<IngredienteDTO?> GetByIdAsync(int id);

        Task AddAsync(IngredienteDTO ingrediente);

        Task UpdateAsync(IngredienteDTO ingrediente);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
