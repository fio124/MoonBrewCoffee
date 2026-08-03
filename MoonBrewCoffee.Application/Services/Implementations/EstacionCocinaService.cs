using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class EstacionCocinaService
        : IEstacionCocinaService
    {
        private readonly IEstacionCocinaRepository _repository;
        private readonly IMapper _mapper;

        public EstacionCocinaService(
            IEstacionCocinaRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<EstacionCocinaDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var lista = await _repository.GetAllAsync(incluirInactivos);

            return _mapper.Map<List<EstacionCocinaDTO>>(lista);
        }

        public async Task<EstacionCocinaDTO?> GetByIdAsync(int id)
        {
            var estacion = await _repository.GetByIdAsync(id);

            if (estacion == null)
                return null;

            return _mapper.Map<EstacionCocinaDTO>(estacion);
        }
    }
}