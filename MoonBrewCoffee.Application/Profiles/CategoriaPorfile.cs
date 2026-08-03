using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<CategoriaDTO, Categoria>().ReverseMap();

            CreateMap<CategoriaDTO, Categoria>()
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.IdCategoria))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo));
        }
    }
}
