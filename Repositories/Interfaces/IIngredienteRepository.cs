using MoonBrewCoffee.Models.Entidades;

namespace MoonBrewCoffee.Repositories.Interfaces
{
    public interface IIngredienteRepository
    {
        Task<List<Ingrediente>> GetAllAsync();

        Task<Ingrediente?> GetByIdAsync(int id);

        Task AddAsync(Ingrediente ingrediente);

        Task UpdateAsync(Ingrediente ingrediente);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}