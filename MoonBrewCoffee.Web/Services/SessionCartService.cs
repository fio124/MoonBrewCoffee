using System.Text.Json;
using MoonBrewCoffee.Application.Services.Interfaces;
using MoonBrewCoffee.Web.Models;

namespace MoonBrewCoffee.Web.Services
{
    public class SessionCartService : ICartService
    {
        private const string SessionKey = "MoonBrew.Cart.V1";
        private const int MaximumQuantity = 99;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductoService _productoService;
        private readonly IComboService _comboService;

        public SessionCartService(
            IHttpContextAccessor httpContextAccessor,
            IProductoService productoService,
            IComboService comboService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productoService = productoService;
            _comboService = comboService;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("No existe una sesión HTTP activa.");

        public async Task<CartViewModel> GetAsync()
        {
            var storedItems = Read();
            var lines = new List<CartLineViewModel>();

            foreach (var item in storedItems)
            {
                var line = await BuildLineAsync(item);
                if (line is not null)
                    lines.Add(line);
            }

            return new CartViewModel { Lines = lines };
        }

        public async Task<CartViewModel> AddAsync(AddCartItemRequest request)
        {
            var itemType = NormalizeType(request.ItemType);
            if (request.ItemId <= 0)
                throw new ArgumentException("Selecciona un producto o combo válido.");

            var requestedQuantity = ValidateQuantity(request.Quantity);
            await EnsureItemExistsAsync(itemType, request.ItemId);

            var items = Read();
            var existing = items.FirstOrDefault(item =>
                item.ItemType == itemType && item.ItemId == request.ItemId);

            if (existing is null)
            {
                items.Add(new StoredCartItem
                {
                    Key = Guid.NewGuid().ToString("N"),
                    ItemType = itemType,
                    ItemId = request.ItemId,
                    Quantity = requestedQuantity,
                    Notes = NormalizeNotes(request.Notes)
                });
            }
            else
            {
                existing.Quantity = Math.Min(MaximumQuantity, existing.Quantity + requestedQuantity);
                if (!string.IsNullOrWhiteSpace(request.Notes))
                    existing.Notes = NormalizeNotes(request.Notes);
            }

            Write(items);
            return await GetAsync();
        }

        public async Task<CartViewModel> UpdateAsync(string key, UpdateCartItemRequest request)
        {
            var items = Read();
            var item = items.FirstOrDefault(value => value.Key == key)
                ?? throw new KeyNotFoundException("La línea del carrito ya no existe.");

            if (request.Quantity.HasValue)
            {
                if (request.Quantity.Value == 0)
                    items.Remove(item);
                else
                    item.Quantity = ValidateQuantity(request.Quantity.Value);
            }

            if (items.Contains(item) && request.Notes is not null)
                item.Notes = NormalizeNotes(request.Notes);

            Write(items);
            return await GetAsync();
        }

        public async Task<CartViewModel> RemoveAsync(string key)
        {
            var items = Read();
            items.RemoveAll(item => item.Key == key);
            Write(items);
            return await GetAsync();
        }

        public void Clear() => Session.Remove(SessionKey);

        private async Task<CartLineViewModel?> BuildLineAsync(StoredCartItem item)
        {
            if (item.ItemType == "producto")
            {
                var product = await _productoService.GetSummaryByIdAsync(item.ItemId);
                if (product is null || !product.Activo)
                    return null;

                return new CartLineViewModel
                {
                    Key = item.Key,
                    ItemType = item.ItemType,
                    ItemId = item.ItemId,
                    Name = product.Nombre,
                    UnitPrice = product.Precio,
                    Quantity = item.Quantity,
                    Notes = item.Notes,
                    ImageUrl = $"/Productos/Image/{item.ItemId}"
                };
            }

            var combo = await _comboService.GetSummaryByIdAsync(item.ItemId);
            if (combo is null || !combo.Activo)
                return null;

            return new CartLineViewModel
            {
                Key = item.Key,
                ItemType = item.ItemType,
                ItemId = item.ItemId,
                Name = combo.Nombre,
                UnitPrice = combo.PrecioCombo,
                Quantity = item.Quantity,
                Notes = item.Notes,
                ImageUrl = $"/Combos/Image/{item.ItemId}"
            };
        }

        private async Task EnsureItemExistsAsync(string itemType, int itemId)
        {
            if (itemType == "producto")
            {
                var product = await _productoService.GetSummaryByIdAsync(itemId);
                if (product is null || !product.Activo)
                    throw new KeyNotFoundException("El producto seleccionado no está disponible.");
                return;
            }

            var combo = await _comboService.GetSummaryByIdAsync(itemId);
            if (combo is null || !combo.Activo)
                throw new KeyNotFoundException("El combo seleccionado no está disponible.");
        }

        private static string NormalizeType(string itemType)
        {
            var normalized = itemType.Trim().ToLowerInvariant();
            return normalized is "producto" or "combo"
                ? normalized
                : throw new ArgumentException("El tipo de artículo no es válido.");
        }

        private static int ValidateQuantity(int quantity) => quantity is >= 1 and <= MaximumQuantity
            ? quantity
            : throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe estar entre 1 y 99.");

        private static string? NormalizeNotes(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return null;

            var normalized = notes.Trim();
            return normalized.Length <= 250 ? normalized : normalized[..250];
        }

        private List<StoredCartItem> Read()
        {
            var json = Session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(json))
                return new List<StoredCartItem>();

            try
            {
                return JsonSerializer.Deserialize<List<StoredCartItem>>(json) ?? new List<StoredCartItem>();
            }
            catch (JsonException)
            {
                Session.Remove(SessionKey);
                return new List<StoredCartItem>();
            }
        }

        private void Write(List<StoredCartItem> items)
        {
            if (items.Count == 0)
                Session.Remove(SessionKey);
            else
                Session.SetString(SessionKey, JsonSerializer.Serialize(items));
        }

        private sealed class StoredCartItem
        {
            public string Key { get; set; } = string.Empty;
            public string ItemType { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public int Quantity { get; set; }
            public string? Notes { get; set; }
        }
    }
}
