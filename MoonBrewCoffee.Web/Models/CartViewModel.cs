namespace MoonBrewCoffee.Web.Models
{
    public class CartViewModel
    {
        public const decimal TaxRate = 0.13m;

        public List<CartLineViewModel> Lines { get; init; } = new();
        public int ItemCount => Lines.Sum(line => line.Quantity);
        public decimal Subtotal => Lines.Sum(line => line.Subtotal);
        public decimal Tax => Lines.Sum(line => line.Tax);
        public decimal Total => Subtotal + Tax;
        public bool IsEmpty => Lines.Count == 0;
    }

    public class CartLineViewModel
    {
        public string Key { get; init; } = string.Empty;
        public string ItemType { get; init; } = string.Empty;
        public int ItemId { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public string? Notes { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public decimal Subtotal => UnitPrice * Quantity;
        public decimal Tax => Math.Round(Subtotal * CartViewModel.TaxRate, 2, MidpointRounding.AwayFromZero);
        public decimal Total => Subtotal + Tax;
    }

    public class AddCartItemRequest
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int? Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
