using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class ProcesoPreparacionService: IProcesoPreparacionService
    {

        private readonly IProcesoPreparacionRepository _repository;
        private readonly IMapper _mapper;

        public ProcesoPreparacionService(
            IProcesoPreparacionRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProcesoPreparacionDTO>> GetAllAsync()
        {
            var lista = await _repository.GetAllAsync();

            return _mapper.Map<List<ProcesoPreparacionDTO>>(lista);
        }

        public async Task<List<ProcesoPreparacionDTO>> GetByProductoAsync(
            int idProducto)
        {
            var lista = await _repository.GetByProductoAsync(idProducto);

            return _mapper.Map<List<ProcesoPreparacionDTO>>(lista);
        }

        public async Task<ProcesoPreparacionDTO?> GetByIdAsync(int id)
        {
            var proceso = await _repository.GetByIdAsync(id);

            if (proceso == null)
                return null;

            return _mapper.Map<ProcesoPreparacionDTO>(proceso);
        }

        public async Task AddAsync(ProcesoPreparacionDTO proceso)
        {
            var entidad = _mapper.Map<ProcesoPreparacion>(proceso);

            await _repository.AddAsync(entidad);
        }

        public async Task UpdateAsync(ProcesoPreparacionDTO proceso)
        {
            var entidad = _mapper.Map<ProcesoPreparacion>(proceso);

            await _repository.UpdateAsync(entidad);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> OrdenExisteAsync(
            int idProducto,
            int orden,
            int? excluirIdProceso = null)
        {
            return await _repository.OrdenExisteAsync(
                idProducto,
                orden,
                excluirIdProceso);
        }
        public async Task<List<ProcesoPreparacionResumenDTO>> GetResumenAsync()
        {
            var procesos = await _repository.GetAllAsync();

            return procesos
                .GroupBy(p => new
                {
                    p.IdProducto,
                    NombreProducto = p.Producto?.Nombre ?? "Sin producto"
                })
                .Select(grupo => new ProcesoPreparacionResumenDTO
                {
                    IdProducto = grupo.Key.IdProducto,
                    NombreProducto = grupo.Key.NombreProducto,
                    CantidadPasos = grupo.Count(),
                    TiempoTotal = grupo.Sum(p => p.TiempoPreparacionMin)
                })
                .OrderBy(r => r.NombreProducto)
                .ToList();
        }

        public async Task<List<ProcesoPreparacionDTO>> GetProcesoCompletoAsync(int idProducto)
        {
            var lista = await _repository.GetByProductoAsync(idProducto);

            return _mapper.Map<List<ProcesoPreparacionDTO>>(lista);
        }

    }
}