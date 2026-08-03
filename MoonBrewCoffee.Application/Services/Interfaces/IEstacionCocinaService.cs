using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IEstacionCocinaService
    {
        Task<List<EstacionCocinaDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<EstacionCocinaDTO?> GetByIdAsync(int id);
    }
}