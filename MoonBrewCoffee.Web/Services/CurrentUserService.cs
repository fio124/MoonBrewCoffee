using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using MoonBrewCoffee.Application.DTOs;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private const string UserIdKey = "MoonBrew.CurrentUser.Id";
        private const string FirstNameKey = "MoonBrew.CurrentUser.FirstName";
        private const string LastNameKey = "MoonBrew.CurrentUser.LastName";
        private const string EmailKey = "MoonBrew.CurrentUser.Email";
        private const string PhoneKey = "MoonBrew.CurrentUser.Phone";
        private const string RoleKey = "MoonBrew.CurrentUser.Role";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioService _usuarioService;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IUsuarioService usuarioService)
        {
            _httpContextAccessor = httpContextAccessor;
            _usuarioService = usuarioService;
        }

        private ISession Session =>
            _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("No existe una sesión HTTP activa.");

        public CurrentUserViewModel? GetCurrent()
        {
            var role = Session.GetString(RoleKey);
            if (string.IsNullOrWhiteSpace(role))
                return null;

            return new CurrentUserViewModel
            {
                IdUsuario = Session.GetInt32(UserIdKey),
                Nombre = Session.GetString(FirstNameKey) ?? string.Empty,
                Apellido = Session.GetString(LastNameKey) ?? string.Empty,
                Correo = Session.GetString(EmailKey) ?? string.Empty,
                Telefono = Session.GetString(PhoneKey) ?? string.Empty,
                Rol = role
            };
        }

        public async Task<CurrentUserViewModel?> SignInAsync(string email, string password)
        {
            var users = await _usuarioService.GetAllAsync();
            var selected = users
                .FirstOrDefault(user =>
                    user.Activo &&
                    string.Equals(user.Correo.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (selected is null || !VerifyPassword(selected, password))
                return null;

            var current = FromDto(selected);
            Store(current);
            return current;
        }

        public void SignOut()
        {
            Session.Remove(UserIdKey);
            Session.Remove(FirstNameKey);
            Session.Remove(LastNameKey);
            Session.Remove(EmailKey);
            Session.Remove(PhoneKey);
            Session.Remove(RoleKey);
        }

        private void Store(CurrentUserViewModel user)
        {
            SignOut();
            if (user.IdUsuario.HasValue)
                Session.SetInt32(UserIdKey, user.IdUsuario.Value);

            Session.SetString(FirstNameKey, user.Nombre);
            Session.SetString(LastNameKey, user.Apellido);
            Session.SetString(EmailKey, user.Correo);
            Session.SetString(PhoneKey, user.Telefono);
            Session.SetString(RoleKey, user.Rol);
        }

        private static CurrentUserViewModel FromDto(UsuarioDTO user) => new()
        {
            IdUsuario = user.IdUsuario,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Correo = user.Correo,
            Telefono = user.Telefono,
            Rol = user.NombreRol
        };

        private static bool VerifyPassword(UsuarioDTO user, string password)
        {
            if (string.IsNullOrEmpty(user.PasswordHash) || string.IsNullOrEmpty(password))
                return false;

            if (user.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal))
            {
                var hasher = new PasswordHasher<UsuarioDTO>();
                return hasher.VerifyHashedPassword(user, user.PasswordHash, password)
                    != PasswordVerificationResult.Failed;
            }

            var storedBytes = Encoding.UTF8.GetBytes(user.PasswordHash);
            var suppliedBytes = Encoding.UTF8.GetBytes(password);
            return storedBytes.Length == suppliedBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(storedBytes, suppliedBytes);
        }
    }
}
