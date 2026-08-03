using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Repository.Implementations
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly MoonBrewContext _context;

        public PedidoRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Pedido pedido, Pago pago)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            pago.IdPedido = pedido.IdPedido;
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return pedido.IdPedido;
        }

        public Task<Pedido?> GetByIdAsync(int id) => Query()
            .FirstOrDefaultAsync(order => order.IdPedido == id);

        public Task<List<Pedido>> GetByClientAsync(int clientId) => Query()
            .Where(order => order.IdCliente == clientId)
            .OrderByDescending(order => order.FechaPedido)
            .ToListAsync();

        public Task<List<Pedido>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? statusId = null)
        {
            var query = Query();
            if (from.HasValue)
                query = query.Where(order => order.FechaPedido >= from.Value.Date);
            if (to.HasValue)
                query = query.Where(order => order.FechaPedido < to.Value.Date.AddDays(1));
            if (statusId.HasValue)
                query = query.Where(order => order.IdEstado == statusId.Value);

            return query.OrderByDescending(order => order.FechaPedido).ToListAsync();
        }

        public Task<List<EstadoPedido>> GetStatusesAsync() => _context.EstadosPedido
            .AsNoTracking()
            .OrderBy(status => status.IdEstado)
            .ToListAsync();

        public Task<EstadoPedido?> GetStatusByNameAsync(string name) => _context.EstadosPedido
            .FirstOrDefaultAsync(status => status.Nombre.ToLower() == name.ToLower());

        private IQueryable<Pedido> Query() => _context.Pedidos
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.Cliente)
            .Include(order => order.Encargado)
            .Include(order => order.EstadoPedido)
            .Include(order => order.Pago)
            .Include(order => order.Detalles).ThenInclude(line => line.Producto)
            .Include(order => order.Detalles).ThenInclude(line => line.Combo);
    }
}
