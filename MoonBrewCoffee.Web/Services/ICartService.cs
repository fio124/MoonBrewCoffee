using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Web.Services
{
    public interface ICartService
    {
        Task<CartViewModel> GetAsync();
        Task<CartViewModel> AddAsync(AddCartItemRequest request);
        Task<CartViewModel> UpdateAsync(string key, UpdateCartItemRequest request);
        Task<CartViewModel> RemoveAsync(string key);
        void Clear();
    }
}
