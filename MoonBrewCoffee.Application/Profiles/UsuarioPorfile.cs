using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            // Entidad -> DTO
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.NombreRol,
                    opt => opt.MapFrom(src =>
                        src.Rol != null ? src.Rol.Nombre : ""));

            // DTO -> Entidad
            CreateMap<UsuarioDTO, Usuario>();
        }
    }
}