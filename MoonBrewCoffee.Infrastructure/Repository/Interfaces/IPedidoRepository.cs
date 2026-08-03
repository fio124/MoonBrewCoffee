using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Repository.Interfaces
{
    public interface IPedidoRepository
    {
        Task<int> AddAsync(Pedido pedido, Pago pago);
        Task<Pedido?> GetByIdAsync(int id);
        Task<List<Pedido>> GetByClientAsync(int clientId);
        Task<List<Pedido>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? statusId = null);
        Task<List<EstadoPedido>> GetStatusesAsync();
        Task<EstadoPedido?> GetStatusByNameAsync(string name);
    }
}
