using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IMenuComboRepository
    {
        Task<List<MenuCombo>> GetByMenuAsync(int idMenu);

        Task GuardarCombosAsync(
            int idMenu,
            List<int> combos);

        Task EliminarCombosAsync(int idMenu);
        Task<List<MenuCombo>> GetCombosCompletosByMenuAsync(int idMenu);
    }
}