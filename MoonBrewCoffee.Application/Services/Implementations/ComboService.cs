using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class ComboService : IComboService
    {
        private readonly IComboRepository _comboRepository;
        private readonly IMapper _mapper;
        private readonly IComboProductoService _comboProductoService;

        public ComboService(
            IComboRepository comboRepository,
            IComboProductoService comboProductoService,
            IMapper mapper)
        {
            _comboRepository = comboRepository;
            _comboProductoService = comboProductoService;
            _mapper = mapper;
        }

        public async Task<List<ComboDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var combos = await _comboRepository.GetAllAsync(incluirInactivos);
            return _mapper.Map<List<ComboDTO>>(combos);
        }

        public Task<int> CountActiveAsync()
        {
            return _comboRepository.CountActiveAsync();
        }

        public async Task<List<ComboDTO>> GetSummaryAsync(
            bool incluirInactivos = true)
        {
            var combos = await _comboRepository
                .GetSummaryAsync(incluirInactivos);

            return _mapper.Map<List<ComboDTO>>(combos);
        }

        public async Task<List<ComboDTO>> GetOptionsAsync(
            bool incluirInactivos = false)
        {
            var combos = await _comboRepository
                .GetOptionsAsync(incluirInactivos);

            return _mapper.Map<List<ComboDTO>>(combos);
        }

        public async Task<ComboDTO?> GetSummaryByIdAsync(int id)
        {
            var combo = await _comboRepository.GetSummaryByIdAsync(id);
            return combo == null ? null : _mapper.Map<ComboDTO>(combo);
        }

        public Task<string?> GetImageAsync(int id)
        {
            return _comboRepository.GetImageAsync(id);
        }

        public async Task<ComboDTO?> GetByIdAsync(int id)
        {
            var combo = await _comboRepository.GetByIdAsync(id);

            if (combo == null)
                return null;

            return _mapper.Map<ComboDTO>(combo);
        }

        public async Task<int> AddAsync(ComboDTO combo)
        {
            var entidad = _mapper.Map<Combo>(combo);

            int idCombo = await _comboRepository.AddAsync(entidad);

            await _comboProductoService.GuardarProductosAsync(
                idCombo,
                combo.ProductosSeleccionados);

            return idCombo;
        }

        public async Task UpdateAsync(ComboDTO combo)
        {
            var entidad = _mapper.Map<Combo>(combo);

            await _comboRepository.UpdateAsync(entidad);

            await _comboProductoService.GuardarProductosAsync(
                combo.IdCombo,
                combo.ProductosSeleccionados);
        }

        public async Task DeleteAsync(int id)
        {
            await _comboRepository.DeleteAsync(id);
        }

        public async Task ActivarAsync(int id)
        {
            await _comboRepository.ActivarAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _comboRepository.ExistsAsync(id);
        }
    }
}
