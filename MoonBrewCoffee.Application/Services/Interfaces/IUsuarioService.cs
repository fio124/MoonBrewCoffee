using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> GetAllAsync();
        Task<int> CountActiveAsync();
        Task<UsuarioDTO?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(UsuarioDTO usuario);
        Task UpdateAsync(UsuarioDTO usuario);
        Task DeleteAsync(int id);
    }
}
