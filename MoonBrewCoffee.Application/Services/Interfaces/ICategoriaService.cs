using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<CategoriaDTO?> GetByIdAsync(int id);

        Task AddAsync(CategoriaDTO categoria);

        Task UpdateAsync(CategoriaDTO categoria);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
