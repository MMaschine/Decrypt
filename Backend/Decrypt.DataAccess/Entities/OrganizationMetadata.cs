using System.ComponentModel.DataAnnotations;


namespace Decrypt.DataAccess.Entities
{
    public class OrganizationMetadata
    {
        //TODO: possibly can be reference, investigate 
        [MaxLength(100)]
        public string Source { get; set; } = null!;

        public int? LegacyNumericId { get; set; }

        public DateTime? MigratedAt { get; set; }
    }
}
