using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class ProcesoPreparacionProfile : Profile
    {
        public ProcesoPreparacionProfile()
        {
            CreateMap<ProcesoPreparacion, ProcesoPreparacionDTO>()
                .ForMember(
                    destino => destino.NombreProducto,
                    opciones => opciones.MapFrom(origen =>
                        origen.Producto != null
                            ? origen.Producto.Nombre
                            : "Sin producto"))
                .ForMember(
                    destino => destino.NombreEstacion,
                    opciones => opciones.MapFrom(origen =>
                        origen.EstacionCocina != null
                            ? origen.EstacionCocina.Nombre
                            : "Sin estación"));

            CreateMap<ProcesoPreparacionDTO, ProcesoPreparacion>()
                .ForMember(
                    destino => destino.Producto,
                    opciones => opciones.Ignore())
                .ForMember(
                    destino => destino.EstacionCocina,
                    opciones => opciones.Ignore());
        }
    }
}