using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class EstacionCocinaProfile : Profile
    {
        public EstacionCocinaProfile()
        {
            CreateMap<EstacionCocina, EstacionCocinaDTO>();

            CreateMap<EstacionCocinaDTO, EstacionCocina>();
        }
    }
}