using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class ProductoIngredienteService : IProductoIngredienteService
    {
        private readonly IProductoIngredienteRepository _repository;
        private readonly AutoMapper.IMapper _mapper;

        public ProductoIngredienteService(
            IProductoIngredienteRepository repository,
            AutoMapper.IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<int>> GetIngredientesAsync(int idProducto)
        {
            var lista = await _repository.GetByProductoAsync(idProducto);

            return lista
                .Select(x => x.IdIngrediente)
                .ToList();
        }

        public async Task<List<MoonBrewCoffee.Application.DTOs.IngredienteDTO>>
            GetIngredientesCompletosAsync(int idProducto)
        {
            var ingredientes = await _repository
                .GetIngredientesByProductoAsync(idProducto);

            return _mapper.Map<List<MoonBrewCoffee.Application.DTOs.IngredienteDTO>>(
                ingredientes);
        }

        public async Task GuardarIngredientesAsync(int idProducto, List<int> ingredientes)
        {
            await _repository.EliminarIngredientesAsync(idProducto);

            await _repository.GuardarIngredientesAsync(idProducto, ingredientes);
        }
    }
}
