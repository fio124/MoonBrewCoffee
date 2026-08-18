using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();

        Task<int> CountActiveAsync();

        Task<Usuario?> GetByIdAsync(int id);

        Task AddAsync(Usuario usuario);

        Task UpdateAsync(Usuario usuario);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
