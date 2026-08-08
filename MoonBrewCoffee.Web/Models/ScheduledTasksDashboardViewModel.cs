namespace MoonBrewCoffee.Web.Models
{
    public sealed class ScheduledTasksDashboardViewModel
    {
        public bool Enabled { get; init; }
        public bool PriceChangesEnabled { get; init; }
        public string TardeandoMenuName { get; init; } = string.Empty;
        public int TardeandoIntervalSeconds { get; init; }
        public int GeneralIntervalSeconds { get; init; }
        public decimal DiscountPercent { get; init; }
        public decimal MinimumPrice { get; init; }
        public decimal GeneralIncrease { get; init; }
        public int MaxExecutionsPerRun { get; init; }
        public int TardeandoExecutionsThisRun { get; init; }
        public int GeneralExecutionsThisRun { get; init; }
        public IReadOnlyList<ScheduledPriceChangeViewModel> LastTardeandoChanges { get; init; } = [];
        public IReadOnlyList<ScheduledPriceChangeViewModel> LastGeneralChanges { get; init; } = [];
        public bool? TardeandoAvailable { get; init; }
        public string? TardeandoSchedule { get; init; }
        public IReadOnlyList<ScheduledTaskExecutionViewModel> Executions { get; init; } = [];
        public IReadOnlyList<ScheduledComboSnapshotViewModel> Combos { get; init; } = [];
    }

    public sealed class ScheduledTaskExecutionViewModel
    {
        public string TaskName { get; init; } = string.Empty;
        public DateTime StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? Details { get; init; }
    }

    public sealed class ScheduledComboSnapshotViewModel
    {
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public bool IsTardeando { get; init; }
    }

    public sealed record ScheduledPriceChangeViewModel(string Name, decimal Before, decimal After);
}
