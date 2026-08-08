using System.ComponentModel.DataAnnotations;

namespace MoonBrewCoffee.Infrastructure.Models.Entidades
{
    public class ScheduledTaskExecution
    {
        [Key]
        public int IdScheduledTaskExecution { get; set; }

        [MaxLength(80)]
        public string TaskName { get; set; } = string.Empty;

        [MaxLength(120)]
        public string ExecutionKey { get; set; } = string.Empty;

        public DateTime StartedAtUtc { get; set; }

        public DateTime? CompletedAtUtc { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Details { get; set; }
    }
}
