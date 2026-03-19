using System.ComponentModel.DataAnnotations;


namespace Decrypt.DataAccess.Entities
{
    public sealed class User : LegacyEntity
    {
        public int OrganizationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(320)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string UserRole { get; set; } = null!;

        public bool IsActive { get; set; }

        [MaxLength(4000)]
        public string? Bio { get; set; }

        #region Navigation

        public Organization Organization { get; set; }

        public List<TimeEntry> TimeEntries { get; set; } = [];

        #endregion
    }
}
