using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class ComboProfile : Profile
    {
        public ComboProfile()
        {
            CreateMap<ComboDTO, Combo>().ReverseMap();

            CreateMap<ComboDTO, Combo>()
                .ForMember(dest => dest.IdCombo, opt => opt.MapFrom(src => src.IdCombo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.PrecioCombo, opt => opt.MapFrom(src => src.PrecioCombo))
                .ForMember(dest => dest.ImagenURL, opt => opt.MapFrom(src => src.ImagenURL))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo));
        }
    }
}
