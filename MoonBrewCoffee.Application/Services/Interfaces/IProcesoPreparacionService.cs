using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IProcesoPreparacionService
    {
        Task<List<ProcesoPreparacionDTO>> GetAllAsync();
        Task<List<ProcesoPreparacionDTO>> GetProcesoCompletoAsync(int idProducto);

        Task<List<ProcesoPreparacionResumenDTO>> GetResumenAsync();
        Task<List<ProcesoPreparacionDTO>> GetByProductoAsync(int idProducto);

        Task<ProcesoPreparacionDTO?> GetByIdAsync(int id);

        Task AddAsync(ProcesoPreparacionDTO proceso);

        Task UpdateAsync(ProcesoPreparacionDTO proceso);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> OrdenExisteAsync(
            int idProducto,
            int orden,
            int? excluirIdProceso = null);
    }
}