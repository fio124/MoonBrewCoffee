using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class MenuProductoService : IMenuProductoService
    {
        private readonly IMenuProductoRepository _repository;
        private readonly IMapper _mapper;

        public MenuProductoService(
            IMenuProductoRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task GuardarProductosAsync(
            int idMenu,
            List<int> productos)
        {
            await _repository.EliminarProductosAsync(idMenu);

            await _repository.GuardarProductosAsync(
                idMenu,
                productos ?? new List<int>());
        }

        public async Task<List<int>>
            GetProductosSeleccionadosAsync(int idMenu)
        {
            var relaciones =
                await _repository.GetByMenuAsync(idMenu);

            return relaciones
                .Select(x => x.IdProducto)
                .ToList();
        }

        public async Task<List<ProductoDTO>>
            GetProductosCompletosAsync(int idMenu)
        {
            var relaciones =
                await _repository
                    .GetProductosCompletosByMenuAsync(idMenu);

            var productos = relaciones
                .Where(x => x.Producto != null)
                .Select(x => x.Producto!)
                .ToList();

            return _mapper.Map<List<ProductoDTO>>(productos);
        }
    }
}