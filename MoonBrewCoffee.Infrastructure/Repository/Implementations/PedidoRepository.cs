using Microsoft.EntityFrameworkCore;
using System.Data;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Repository.Interfaces;
using MoonBrewCoffee.Infrastructure.Workflow;

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
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var existingId = await _context.Pedidos
                .Where(order => order.ClaveOperacion == pedido.ClaveOperacion)
                .Select(order => (int?)order.IdPedido)
                .FirstOrDefaultAsync();
            if (existingId.HasValue)
            {
                await transaction.RollbackAsync();
                return existingId.Value;
            }
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _context.PedidoEstadoHistorial.Add(new PedidoEstadoHistorial
            {
                IdPedido = pedido.IdPedido,
                IdEstado = pedido.IdEstado,
                FechaCambio = pedido.FechaPedido,
                IdUsuario = pedido.IdEncargado
            });

            pago.IdPedido = pedido.IdPedido;
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return pedido.IdPedido;
        }

        public Task<Pedido?> GetByOperationKeyAsync(string operationKey) => _context.Pedidos
            .AsNoTracking()
            .Where(order => order.ClaveOperacion == operationKey)
            .Select(order => new Pedido { IdPedido = order.IdPedido, Total = order.Total })
            .FirstOrDefaultAsync();

        public Task<Pedido?> GetByIdAsync(int id) => DetailQuery()
            .FirstOrDefaultAsync(order => order.IdPedido == id);

        public Task<List<Pedido>> GetByClientAsync(int clientId) => SummaryQuery()
            .Where(order => order.IdCliente == clientId)
            .OrderByDescending(order => order.FechaPedido)
            .ToListAsync();

        public Task<List<Pedido>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? statusId = null)
        {
            var query = ReportQuery();
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

        public Task<List<PedidoProceso>> GetPreparationBoardAsync() => _context.PedidoProcesos
            .AsNoTracking()
            .Include(step => step.Pedido)!.ThenInclude(order => order!.Cliente)
            .Include(step => step.Estacion)
            .Where(step => step.Pedido != null && step.Pedido.Activo && step.Estado != "Completado")
            .OrderBy(step => step.IdPedido)
            .ThenBy(step => step.Orden)
            .ToListAsync();

        public Task<PedidoProceso?> GetProcessByIdAsync(int id) => _context.PedidoProcesos
            .AsNoTracking()
            .FirstOrDefaultAsync(step => step.IdPedidoProceso == id);

        public async Task AdvanceProcessAsync(int processId, bool complete, int? userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var step = await _context.PedidoProcesos
                .Include(item => item.Pedido)
                .FirstOrDefaultAsync(item => item.IdPedidoProceso == processId)
                ?? throw new ArgumentException("La etapa seleccionada ya no existe.");

            var previousPending = await _context.PedidoProcesos.AnyAsync(item =>
                item.IdPedido == step.IdPedido && item.Orden < step.Orden && item.Estado != "Completado");
            if (previousPending)
                throw new InvalidOperationException("Primero debe completarse la etapa anterior del pedido.");

            step.Estado = PedidoWorkflowRules.AdvanceStep(step.Estado, complete);
            if (complete) step.FechaFin = DateTime.Now;
            else step.FechaInicio = DateTime.Now;
            step.IdEncargado = userId;

            var allSteps = await _context.PedidoProcesos.Where(item => item.IdPedido == step.IdPedido).ToListAsync();
            var allCompleted = allSteps.All(item => item.IdPedidoProceso == step.IdPedidoProceso ? complete : item.Estado == "Completado");
            var targetName = PedidoWorkflowRules.ResolveOrderStatus(complete, allCompleted);
            var status = await _context.EstadosPedido.FirstOrDefaultAsync(item => item.Nombre == targetName)
                ?? throw new InvalidOperationException($"No está configurado el estado '{targetName}'.");

            if (step.Pedido!.IdEstado != status.IdEstado)
            {
                step.Pedido.IdEstado = status.IdEstado;
                _context.PedidoEstadoHistorial.Add(new PedidoEstadoHistorial
                {
                    IdPedido = step.IdPedido,
                    IdEstado = status.IdEstado,
                    FechaCambio = DateTime.Now,
                    IdUsuario = userId
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private IQueryable<Pedido> SummaryQuery() => _context.Pedidos
            .AsNoTracking()
            .Include(order => order.Cliente)
            .Include(order => order.Encargado)
            .Include(order => order.EstadoPedido);

        private IQueryable<Pedido> ReportQuery() => SummaryQuery()
            .Include(order => order.Detalles);

        private IQueryable<Pedido> DetailQuery() => _context.Pedidos
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.Cliente)
            .Include(order => order.Encargado)
            .Include(order => order.EstadoPedido)
            .Include(order => order.Pago)
            .Include(order => order.HistorialEstados).ThenInclude(change => change.Estado)
            .Include(order => order.HistorialEstados).ThenInclude(change => change.Usuario)
            .Include(order => order.Detalles).ThenInclude(line => line.Producto)
            .Include(order => order.Detalles).ThenInclude(line => line.Combo);
    }
}
