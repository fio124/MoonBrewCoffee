using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class RolProfile : Profile
    {
        public RolProfile()
        {
            CreateMap<Rol, RolDTO>().ReverseMap();
        }
    }
}