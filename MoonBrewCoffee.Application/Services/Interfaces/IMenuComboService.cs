using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IMenuComboService
    {
        Task GuardarCombosAsync(
            int idMenu,
            List<int> combos);

        Task<List<int>> GetCombosSeleccionadosAsync(
            int idMenu);

        Task<List<ComboDTO>> GetCombosCompletosAsync(int idMenu);
    }
}