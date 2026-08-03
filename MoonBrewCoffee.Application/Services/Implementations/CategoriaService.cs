using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public CategoriaService(
            ICategoriaRepository categoriaRepository,
            IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoriaDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var categorias = await _categoriaRepository.GetAllAsync(incluirInactivos);
            return _mapper.Map<List<CategoriaDTO>>(categorias);
        }

        public async Task<CategoriaDTO?> GetByIdAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);

            if (categoria == null)
                return null;

            return _mapper.Map<CategoriaDTO>(categoria);
        }

        public async Task AddAsync(CategoriaDTO categoria)
        {
            var entidad = _mapper.Map<Categoria>(categoria);
            await _categoriaRepository.AddAsync(entidad);
        }

        public async Task UpdateAsync(CategoriaDTO categoria)
        {
            var entidad = _mapper.Map<Categoria>(categoria);
            await _categoriaRepository.UpdateAsync(entidad);
        }

        public async Task DeleteAsync(int id)
        {
            await _categoriaRepository.DeleteAsync(id);
        }

        public async Task ActivarAsync(int id)
        {
            await _categoriaRepository.ActivarAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _categoriaRepository.ExistsAsync(id);
        }
    }
}