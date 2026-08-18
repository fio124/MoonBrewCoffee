using AutoMapper;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Infrastructure.Interfaces;
using MoonBrewCoffee.Infrastructure.Models.Entidades;

namespace MoonBrewCoffee.Application.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuProductoService _menuProductoService;
        private readonly IMenuComboService _menuComboService;
        private readonly IMapper _mapper;

        public MenuService(
            IMenuRepository menuRepository,
            IMenuProductoService menuProductoService,
            IMenuComboService menuComboService,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _menuProductoService = menuProductoService;
            _menuComboService = menuComboService;
            _mapper = mapper;
        }

        public async Task<List<MenuDTO>> GetAllAsync(bool incluirInactivos = true)
        {
            var menus = await _menuRepository
                .GetAllAsync(incluirInactivos);

            var lista = _mapper.Map<List<MenuDTO>>(menus);

            var ahora = DateTime.Now;
            var hoy = ahora.Date;
            var horaActual = ahora.TimeOfDay;

            foreach (var menu in lista)
            {
                menu.VigenteAhora =
                    menu.Activo &&
                    menu.Disponible &&
                    hoy >= menu.FechaInicio.Date &&
                    hoy <= menu.FechaFin.Date &&
                    horaActual >= menu.HoraInicio &&
                    horaActual <= menu.HoraFin;
            }

            return lista;
        }

        public Task<int> CountActiveAsync()
        {
            return _menuRepository.CountActiveAsync();
        }

        public async Task<MenuDTO?> GetByIdAsync(int id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);

            if (menu == null)
                return null;

            var dto = _mapper.Map<MenuDTO>(menu);

            dto.ProductosSeleccionados =
                await _menuProductoService
                    .GetProductosSeleccionadosAsync(id);

            dto.CombosSeleccionados =
                await _menuComboService
                    .GetCombosSeleccionadosAsync(id);

            return dto;
        }

        public async Task<int> AddAsync(MenuDTO menu)
        {
            var entidad = _mapper.Map<Menu>(menu);

            var idMenu =
                await _menuRepository.AddAsync(entidad);

            await _menuProductoService.GuardarProductosAsync(
                idMenu,
                menu.ProductosSeleccionados);

            await _menuComboService.GuardarCombosAsync(
                idMenu,
                menu.CombosSeleccionados);

            return idMenu;
        }

        public async Task UpdateAsync(MenuDTO menu)
        {
            var entidad = _mapper.Map<Menu>(menu);

            await _menuRepository.UpdateAsync(entidad);

            await _menuProductoService.GuardarProductosAsync(
                menu.IdMenu,
                menu.ProductosSeleccionados);

            await _menuComboService.GuardarCombosAsync(
                menu.IdMenu,
                menu.CombosSeleccionados);
        }

        public async Task DeleteAsync(int id)
        {
            await _menuRepository.DeleteAsync(id);
        }

        public async Task ActivarAsync(int id)
        {
            await _menuRepository.ActivarAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _menuRepository.ExistsAsync(id);
        }

        public async Task<MenuDTO?> GetMenuDisponibleActualAsync()
        {
            var ahora = DateTime.Now;
            var hoy = ahora.Date;
            var horaActual = ahora.TimeOfDay;

            var menuActual = await _menuRepository
                .GetAvailableNowAsync(hoy, horaActual);

            if (menuActual == null)
                return null;

            var dto = _mapper.Map<MenuDTO>(menuActual);

            dto.VigenteAhora = true;

            return dto;
        }

        public async Task<List<MenuDTO>> GetMenusDisponiblesActualesAsync()
        {
            var ahora = DateTime.Now;
            var menus = await _menuRepository.GetAvailableAsync(
                ahora.Date,
                ahora.TimeOfDay);

            var resultado = _mapper.Map<List<MenuDTO>>(menus);

            foreach (var menu in resultado)
                menu.VigenteAhora = true;

            return resultado;
        }

        public async Task<MenuDTO?> GetSummaryByIdAsync(int id)
        {
            var menu = await _menuRepository.GetSummaryByIdAsync(id);
            return menu == null ? null : _mapper.Map<MenuDTO>(menu);
        }
    }
}
