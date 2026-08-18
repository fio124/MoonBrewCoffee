using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<Menu, MenuDTO>();

            CreateMap<MenuDTO, Menu>()
                .ForMember(
                    destino => destino.MenuProductos,
                    opciones => opciones.Ignore())
                .ForMember(
                    destino => destino.MenuCombos,
                    opciones => opciones.Ignore());
        }
    }
}