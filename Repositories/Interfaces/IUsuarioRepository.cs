using MoonBrewCoffee.Models.Entidades;

namespace MoonBrewCoffee.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();

        Task<Usuario?> GetByIdAsync(int id);

        Task AddAsync(Usuario usuario);

        Task UpdateAsync(Usuario usuario);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}