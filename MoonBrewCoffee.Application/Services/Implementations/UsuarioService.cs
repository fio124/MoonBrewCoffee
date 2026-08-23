using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return _mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        public Task<int> CountActiveAsync()
        {
            return _usuarioRepository.CountActiveAsync();
        }

        public async Task<UsuarioDTO?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return null;

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _usuarioRepository.EmailExistsAsync(email);
        }

        public async Task AddAsync(UsuarioDTO usuario)
        {
            var entidad = _mapper.Map<Usuario>(usuario);
            await _usuarioRepository.AddAsync(entidad);
        }

        public async Task UpdateAsync(UsuarioDTO usuario)
        {
            var entidad = _mapper.Map<Usuario>(usuario);
            await _usuarioRepository.UpdateAsync(entidad);
        }

        public async Task DeleteAsync(int id)
        {
            await _usuarioRepository.DeleteAsync(id);
        }
    }
}
