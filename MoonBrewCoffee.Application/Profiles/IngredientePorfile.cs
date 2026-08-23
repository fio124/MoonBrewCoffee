using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class IngredienteProfile : Profile
    {
        public IngredienteProfile()
        {
            CreateMap<IngredienteDTO, Ingrediente>().ReverseMap();

            CreateMap<IngredienteDTO, Ingrediente>()
                .ForMember(dest => dest.IdIngrediente, opt => opt.MapFrom(src => src.IdIngrediente))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo));
        }
    }
}
