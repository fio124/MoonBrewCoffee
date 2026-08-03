using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> GetAllAsync(bool incluirInactivos = true);

        Task<Categoria?> GetByIdAsync(int id);

        Task AddAsync(Categoria categoria);

        Task UpdateAsync(Categoria categoria);

        Task DeleteAsync(int id);

        Task ActivarAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}