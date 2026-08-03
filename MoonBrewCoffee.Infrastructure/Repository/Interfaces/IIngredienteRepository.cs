using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IIngredienteRepository
    {
        Task<List<Ingrediente>> GetAllAsync(bool incluirInactivos = true);

        Task<Ingrediente?> GetByIdAsync(int id);

        Task AddAsync(Ingrediente ingrediente);

        Task UpdateAsync(Ingrediente ingrediente);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}