using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;

        public UsuariosController(IUsuarioService usuarioService,IRolService rolService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
        }

        //Cargar rol
        private async Task CargarRoles()
        {
            var roles = await _rolService.GetAllAsync();

            ViewBag.IdRol = new SelectList(
                roles,
                "IdRol",
                "Nombre");
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> CreateAsync()
        {
            await CargarRoles();
            return View(new UsuarioDTO());
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDTO usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioService.AddAsync(usuario);
                return RedirectToAction(nameof(Index));
            }

            await CargarRoles();
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);

            if (usuario == null)
                return NotFound();

            await CargarRoles();
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioDTO usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _usuarioService.UpdateAsync(usuario);
                return RedirectToAction(nameof(Index));
            }

            await CargarRoles();
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _usuarioService.GetByIdAsync(id.Value);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}