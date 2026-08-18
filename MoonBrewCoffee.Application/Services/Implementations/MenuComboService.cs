using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class MenuComboService : IMenuComboService
    {
        private readonly IMenuComboRepository _repository;
        private readonly IMapper _mapper;

        public MenuComboService(
            IMenuComboRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task GuardarCombosAsync(
            int idMenu,
            List<int> combos)
        {
            await _repository.EliminarCombosAsync(idMenu);

            await _repository.GuardarCombosAsync(
                idMenu,
                combos ?? new List<int>());
        }

        public async Task<List<int>>
            GetCombosSeleccionadosAsync(int idMenu)
        {
            var relaciones =
                await _repository.GetByMenuAsync(idMenu);

            return relaciones
                .Select(x => x.IdCombo)
                .ToList();
        }

        public async Task<List<ComboDTO>>
            GetCombosCompletosAsync(int idMenu)
        {
            var relaciones =
                await _repository
                    .GetCombosCompletosByMenuAsync(idMenu);

            var combos = relaciones
                .Where(x => x.Combo != null)
                .Select(x => x.Combo!)
                .ToList();

            return _mapper.Map<List<ComboDTO>>(combos);
        }
    }
}