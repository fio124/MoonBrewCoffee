using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IEstacionCocinaRepository
    {
        Task<List<EstacionCocina>> GetAllAsync(bool incluirInactivos = true);

        Task<EstacionCocina?> GetByIdAsync(int id);
    }
}