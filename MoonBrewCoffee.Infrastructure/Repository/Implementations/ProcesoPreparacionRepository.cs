using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Infrastructure.Implementations
{
    public class ProcesoPreparacionRepository
        : IProcesoPreparacionRepository
    {
        private readonly MoonBrewContext _context;

        public ProcesoPreparacionRepository(MoonBrewContext context)
        {
            _context = context;
        }

        public async Task<List<ProcesoPreparacion>> GetAllAsync()
        {
            return await _context.ProcesosPreparacion
                .AsNoTracking()
                .OrderBy(p => p.Producto!.Nombre)
                .ThenBy(p => p.Orden)
                .Select(p => new ProcesoPreparacion
                {
                    IdProceso = p.IdProceso,
                    IdProducto = p.IdProducto,
                    IdEstacion = p.IdEstacion,
                    TiempoPreparacionMin = p.TiempoPreparacionMin,
                    Orden = p.Orden,
                    Producto = p.Producto == null
                        ? null
                        : new Producto
                        {
                            IdProducto = p.Producto.IdProducto,
                            Nombre = p.Producto.Nombre
                        },
                    EstacionCocina = p.EstacionCocina == null
                        ? null
                        : new EstacionCocina
                        {
                            IdEstacion = p.EstacionCocina.IdEstacion,
                            Nombre = p.EstacionCocina.Nombre
                        }
                })
                .ToListAsync();
        }

        public async Task<List<ProcesoPreparacion>> GetByProductoAsync(
            int idProducto)
        {
            return await _context.ProcesosPreparacion
                .AsNoTracking()
                .Where(p => p.IdProducto == idProducto)
                .OrderBy(p => p.Orden)
                .Select(p => new ProcesoPreparacion
                {
                    IdProceso = p.IdProceso,
                    IdProducto = p.IdProducto,
                    IdEstacion = p.IdEstacion,
                    TiempoPreparacionMin = p.TiempoPreparacionMin,
                    Orden = p.Orden,
                    Producto = p.Producto == null
                        ? null
                        : new Producto
                        {
                            IdProducto = p.Producto.IdProducto,
                            Nombre = p.Producto.Nombre
                        },
                    EstacionCocina = p.EstacionCocina == null
                        ? null
                        : new EstacionCocina
                        {
                            IdEstacion = p.EstacionCocina.IdEstacion,
                            Nombre = p.EstacionCocina.Nombre
                        }
                })
                .ToListAsync();
        }

        public async Task<ProcesoPreparacion?> GetByIdAsync(int id)
        {
            return await _context.ProcesosPreparacion
                .AsNoTracking()
                .Where(p => p.IdProceso == id)
                .Select(p => new ProcesoPreparacion
                {
                    IdProceso = p.IdProceso,
                    IdProducto = p.IdProducto,
                    IdEstacion = p.IdEstacion,
                    TiempoPreparacionMin = p.TiempoPreparacionMin,
                    Orden = p.Orden,
                    Producto = p.Producto == null
                        ? null
                        : new Producto
                        {
                            IdProducto = p.Producto.IdProducto,
                            Nombre = p.Producto.Nombre
                        },
                    EstacionCocina = p.EstacionCocina == null
                        ? null
                        : new EstacionCocina
                        {
                            IdEstacion = p.EstacionCocina.IdEstacion,
                            Nombre = p.EstacionCocina.Nombre
                        }
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(ProcesoPreparacion proceso)
        {
            _context.ProcesosPreparacion.Add(proceso);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProcesoPreparacion proceso)
        {
            var existente = await _context.ProcesosPreparacion
                .FindAsync(proceso.IdProceso);

            if (existente == null)
                return;

            existente.IdProducto = proceso.IdProducto;
            existente.IdEstacion = proceso.IdEstacion;
            existente.TiempoPreparacionMin =
                proceso.TiempoPreparacionMin;
            existente.Orden = proceso.Orden;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var proceso = await _context.ProcesosPreparacion.FindAsync(id);

            if (proceso == null)
                return;

            _context.ProcesosPreparacion.Remove(proceso);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ProcesosPreparacion
                .AnyAsync(p => p.IdProceso == id);
        }

        public async Task<bool> OrdenExisteAsync(
            int idProducto,
            int orden,
            int? excluirIdProceso = null)
        {
            return await _context.ProcesosPreparacion.AnyAsync(p =>
                p.IdProducto == idProducto &&
                p.Orden == orden &&
                (!excluirIdProceso.HasValue ||
                 p.IdProceso != excluirIdProceso.Value));
        }
    }

}
