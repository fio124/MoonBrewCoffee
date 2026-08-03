using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IRolRepository
    {
        Task<List<Rol>> GetAllAsync(bool incluirInactivos = true);

        Task<Rol?> GetByIdAsync(int id);

        Task AddAsync(Rol rol);

        Task UpdateAsync(Rol rol);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}