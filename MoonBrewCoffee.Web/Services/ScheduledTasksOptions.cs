namespace MoonBrewCoffee.Web.Services
{
    public sealed class ScheduledTasksOptions
    {
        public const string SectionName = "ScheduledTasks";

        public bool Enabled { get; set; } = true;

        public bool PriceChangesEnabled { get; set; }

        public string TardeandoMenuName { get; set; } = "Tardeando";

        public int TardeandoIntervalSeconds { get; set; } = 900;

        public int GeneralComboIntervalSeconds { get; set; } = 1200;

        public decimal TardeandoDiscountPercent { get; set; } = 5m;

        public decimal MinimumComboPrice { get; set; } = 1500m;

        public decimal GeneralComboIncrease { get; set; } = 10m;

        public int MaxPriceExecutionsPerRun { get; set; } = 4;

        public bool RunImmediately { get; set; } = true;
    }
}
