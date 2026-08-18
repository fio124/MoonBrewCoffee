using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using AutoMapper;
using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class ComboProductoService : IComboProductoService
    {
        private readonly IComboProductoRepository _repository;
        private readonly IMapper _mapper;

        public ComboProductoService(
            IComboProductoRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task GuardarProductosAsync(int idCombo, List<int> productos)
        {
            await _repository.EliminarProductosAsync(idCombo);

            await _repository.GuardarProductosAsync(idCombo, productos);
        }

        public async Task<List<int>> GetProductosSeleccionadosAsync(int idCombo)
        {
            var lista = await _repository.GetByComboAsync(idCombo);

            return lista.Select(x => x.IdProducto).ToList();
        }

        public async Task<List<ProductoDTO>> GetProductsByComboAsync(int idCombo)
        {
            var productos = await _repository.GetProductsByComboAsync(idCombo);
            return _mapper.Map<List<ProductoDTO>>(productos);
        }
    }
}
