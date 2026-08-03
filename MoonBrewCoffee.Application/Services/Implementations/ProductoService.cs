using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;
        private readonly IProductoIngredienteService _productoIngredienteService;

        public ProductoService(
            IProductoRepository productoRepository,
            IProductoIngredienteService productoIngredienteService,
            IMapper mapper)
        {
            _productoRepository = productoRepository;
            _productoIngredienteService = productoIngredienteService;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var productos = await _productoRepository.GetAllAsync(incluirInactivos);
            return _mapper.Map<List<ProductoDTO>>(productos);
        }

        public Task<int> CountActiveAsync()
        {
            return _productoRepository.CountActiveAsync();
        }

        public async Task<List<ProductoDTO>> GetSummaryAsync(
            bool incluirInactivos = true)
        {
            var productos = await _productoRepository
                .GetSummaryAsync(incluirInactivos);

            return _mapper.Map<List<ProductoDTO>>(productos);
        }

        public async Task<List<ProductoDTO>> GetOptionsAsync(
            bool incluirInactivos = false)
        {
            var productos = await _productoRepository
                .GetOptionsAsync(incluirInactivos);

            return _mapper.Map<List<ProductoDTO>>(productos);
        }

        public Task<string?> GetImageAsync(int id)
        {
            return _productoRepository.GetImageAsync(id);
        }

        public async Task<ProductoDTO?> GetSummaryByIdAsync(int id)
        {
            var producto = await _productoRepository.GetSummaryByIdAsync(id);
            return producto == null ? null : _mapper.Map<ProductoDTO>(producto);
        }

        public async Task<ProductoDTO?> GetByIdAsync(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);

            if (producto == null)
                return null;

            return _mapper.Map<ProductoDTO>(producto);
        }

        public async Task<int> AddAsync(ProductoDTO producto)
        {
            var entidad = _mapper.Map<Producto>(producto);

            int idProducto = await _productoRepository.AddAsync(entidad);

            await _productoIngredienteService.GuardarIngredientesAsync(
                idProducto,
                producto.IngredientesSeleccionados);

            return idProducto;
        }

        public async Task UpdateAsync(ProductoDTO producto)
        {
            var entidad = _mapper.Map<Producto>(producto);

            await _productoRepository.UpdateAsync(entidad);

            await _productoIngredienteService.GuardarIngredientesAsync(
                producto.IdProducto,
                producto.IngredientesSeleccionados);
        }

        public async Task DeleteAsync(int id)
        {
            await _productoRepository.DeleteAsync(id);
        }

        public async Task ActivarAsync(int id)
        {
            await _productoRepository.ActivarAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _productoRepository.ExistsAsync(id);
        }

        public async Task<bool> ExistsByNameAsync(
            string nombre,
            int? excluirId = null)
        {
            return await _productoRepository.ExistsByNameAsync(nombre, excluirId);
        }
    }
}
