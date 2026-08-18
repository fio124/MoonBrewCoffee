using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class IngredienteService : IIngredienteService
    {
        private readonly IIngredienteRepository _ingredienteRepository;
        private readonly IMapper _mapper;

        public IngredienteService(
            IIngredienteRepository ingredienteRepository,
            IMapper mapper)
        {
            _ingredienteRepository = ingredienteRepository;
            _mapper = mapper;
        }

        public async Task<List<IngredienteDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var ingredientes = await _ingredienteRepository.GetAllAsync(incluirInactivos);
            return _mapper.Map<List<IngredienteDTO>>(ingredientes);
        }

        public async Task<IngredienteDTO?> GetByIdAsync(int id)
        {
            var ingrediente = await _ingredienteRepository.GetByIdAsync(id);

            if (ingrediente == null)
                return null;

            return _mapper.Map<IngredienteDTO>(ingrediente);
        }

        public async Task AddAsync(IngredienteDTO ingrediente)
        {
            var entidad = _mapper.Map<Ingrediente>(ingrediente);
            await _ingredienteRepository.AddAsync(entidad);
        }

        public async Task UpdateAsync(IngredienteDTO ingrediente)
        {
            var entidad = _mapper.Map<Ingrediente>(ingrediente);
            await _ingredienteRepository.UpdateAsync(entidad);
        }

        public async Task DeleteAsync(int id)
        {
            await _ingredienteRepository.DeleteAsync(id);
        }

        public async Task ActivarAsync(int id)
        {
            await _ingredienteRepository.ActivarAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _ingredienteRepository.ExistsAsync(id);
        }
    }
}