using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Web.Services
{
    public interface ICurrentUserService
    {
        CurrentUserViewModel? GetCurrent();
        Task<CurrentUserViewModel?> SignInAsync(string email, string password);
        void SignOut();
    }
}
