using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IRolService
    {
        Task<List<RolDTO>> GetAllAsync(bool incluirInactivos = true);

        Task<RolDTO?> GetByIdAsync(int id);
    }
}