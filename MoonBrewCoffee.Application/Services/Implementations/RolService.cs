using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public RolService(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<List<RolDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var roles = await _rolRepository.GetAllAsync(incluirInactivos);

            return _mapper.Map<List<RolDTO>>(roles);
        }

        public async Task<RolDTO?> GetByIdAsync(int id)
        {
            var rol = await _rolRepository.GetByIdAsync(id);

            if (rol == null)
                return null;

            return _mapper.Map<RolDTO>(rol);
        }
    }
}