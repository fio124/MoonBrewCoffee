using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Interfaces
{
    public interface IProcesoPreparacionRepository
    {
        Task<List<ProcesoPreparacion>> GetAllAsync();

        Task<List<ProcesoPreparacion>> GetByProductoAsync(int idProducto);

        Task<ProcesoPreparacion?> GetByIdAsync(int id);

        Task AddAsync(ProcesoPreparacion proceso);

        Task UpdateAsync(ProcesoPreparacion proceso);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> OrdenExisteAsync(
            int idProducto,
            int orden,
            int? excluirIdProceso = null);
    }
}