namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class MenuCombo
    {
        public int IdMenu { get; set; }

        public int IdCombo { get; set; }

        public Menu? Menu { get; set; }

        public Combo? Combo { get; set; }
    }
}