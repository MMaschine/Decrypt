using System.ComponentModel.DataAnnotations;


namespace Decrypt.DataAccess.Entities
{
    public class LegacyEntity : BaseEntity
    {
        [MaxLength(50)]
        public string LegacyId { get; set; } = null!;
    }
}
