using MoonBrewCoffee.Models.Entidades;

namespace MoonBrewCoffee.Repositories.Interfaces
{
    public interface IComboRepository
    {
        Task<List<Combo>> GetAllAsync();

        Task<Combo?> GetByIdAsync(int id);

        Task AddAsync(Combo combo);

        Task UpdateAsync(Combo combo);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}