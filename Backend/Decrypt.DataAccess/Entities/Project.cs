using System.ComponentModel.DataAnnotations;

namespace Decrypt.DataAccess.Entities
{
    public sealed class Project : LegacyEntity
    {
        public int OrganizationId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [Required]
        public string Status { get; set; } = null!;

        public int BudgetHours { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        #region Navigation

        public Organization Organization { get; set; }

        public List<TimeEntry> TimeEntries { get; set; } = [];

        public List<Invoice> Invoices { get; set; } = [];

        #endregion
    }
}
