using Microsoft.AspNetCore.Mvc;
using MoonBrewCoffee.Web.Models;
using MoonBrewCoffee.Web.Services;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class CuentaController : Controller
    {
        private readonly ICurrentUserService _currentUserService;

        public CuentaController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
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
