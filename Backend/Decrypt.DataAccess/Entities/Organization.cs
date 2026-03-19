using System.ComponentModel.DataAnnotations;
using Decrypt.DataAccess.Entities.References;


namespace Decrypt.DataAccess.Entities
{
    public sealed class Organization :  LegacyEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string Slug { get; set; } = null!;

        [Required]
        [MaxLength(320)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Industry { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Tier { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        public OrganizationMetadata Metadata { get; set; } = new();

        //Settings 
        public int CurrencyId { get; set; }

        public string DefaultLocale { get; set; }

        public string Timezone { get; set; } = null!;

        public bool AllowOvertime { get; set; }

        #region Navigation

        public Currency Currency { get; set; } = null!;

        public List<User> Users { get; set; } = [];

        public List<Project> Projects { get; set; } = [];

        public List<Invoice> Invoices { get; set; } = [];

        #endregion
    }
}
