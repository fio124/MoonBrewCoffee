using MoonBrewCoffee.Application.DTOs;

namespace MoonBrewCoffee.Application.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoCreadoDTO> CreateAsync(RegistrarPedidoDTO request);
        Task<PedidoDTO?> GetByIdAsync(int id);
        Task<List<PedidoDTO>> GetByClientAsync(int clientId);
        Task<List<PedidoDTO>> GetAllAsync(DateTime? from = null, DateTime? to = null, int? statusId = null);
        Task<List<EstadoPedidoDTO>> GetStatusesAsync();
        Task<List<PedidoProcesoDTO>> GetPreparationBoardAsync();
        Task AdvanceProcessAsync(int processId, bool complete, int? userId);
    }

    public class EstadoPedidoDTO
    {
        public int IdEstado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? ColorHex { get; set; }
    }
}
