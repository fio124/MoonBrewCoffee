using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class CuentaController : Controller
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;

        public CuentaController(
            ICurrentUserService currentUserService,
            IUsuarioService usuarioService,
            IRolService rolService)
        {
            _currentUserService = currentUserService;
            _usuarioService = usuarioService;
            _rolService = rolService;
        }

        [HttpGet]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            var currentUser = _currentUserService.GetCurrent();
            if (currentUser is not null)
                return RedirectForRole(currentUser);

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _currentUserService.SignInAsync(model.Email, model.Password);
            if (currentUser is null)
            {
                ModelState.AddModelError(string.Empty, "El correo o la contraseña no son correctos.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectForRole(currentUser);
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            if (_currentUserService.GetCurrent() is not null)
                return RedirectToAction("Index", "Cliente");

            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(RegistroViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Correo) &&
                await _usuarioService.EmailExistsAsync(model.Correo))
            {
                ModelState.AddModelError(nameof(model.Correo),
                    "Ya existe una cuenta registrada con este correo.");
            }

            var clientRole = (await _rolService.GetAllAsync(false))
                .FirstOrDefault(role => role.Nombre.Contains("cliente", StringComparison.OrdinalIgnoreCase));

            if (clientRole is null)
                ModelState.AddModelError(string.Empty,
                    "No se encontró el rol Cliente. Comunícate con la administración.");

            if (!ModelState.IsValid)
                return View(model);

            var user = new UsuarioDTO
            {
                IdRol = clientRole!.IdRol,
                Nombre = model.Nombre.Trim(),
                Apellido = model.Apellido.Trim(),
                Correo = model.Correo.Trim().ToLowerInvariant(),
                Telefono = model.Telefono.Trim(),
                FechaRegistro = DateTime.Now,
                Activo = true
            };
            user.PasswordHash = new PasswordHasher<UsuarioDTO>().HashPassword(user, model.Password);

            await _usuarioService.AddAsync(user);
            var currentUser = await _currentUserService.SignInAsync(user.Correo, model.Password);

            TempData["SuccessMessage"] = "Tu cuenta fue creada correctamente. ¡Bienvenido a MoonBrew!";
            return currentUser is null
                ? RedirectToAction(nameof(IniciarSesion))
                : RedirectToAction("Index", "Cliente");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarSesion()
        {
            _currentUserService.SignOut();
            return RedirectToAction("Index", "Cliente");
        }

        private IActionResult RedirectForRole(CurrentUserViewModel currentUser)
        {
            return currentUser.EsAdministrador
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Index", "Cliente");
        }
    }
}
