using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.NombreCategoria,
                    opt => opt.MapFrom(src =>
                        src.Categoria != null
                            ? src.Categoria.Nombre
                            : "Sin categoría"));

            CreateMap<ProductoDTO, Producto>()
                .ForMember(dest => dest.IdProducto,
                    opt => opt.MapFrom(src => src.IdProducto))
                .ForMember(dest => dest.IdCategoria,
                    opt => opt.MapFrom(src => src.IdCategoria))
                .ForMember(dest => dest.Nombre,
                    opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Descripcion,
                    opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Precio,
                    opt => opt.MapFrom(src => src.Precio))
                .ForMember(dest => dest.Image64,
                    opt => opt.MapFrom(src => src.Image64))
                .ForMember(dest => dest.TiempoPreparacion,
                    opt => opt.MapFrom(src => src.TiempoPreparacion))
                .ForMember(dest => dest.Activo,
                    opt => opt.MapFrom(src => src.Activo))
                .ForMember(dest => dest.Categoria,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ProductoIngredientes,
                    opt => opt.Ignore());
        }
    }
}