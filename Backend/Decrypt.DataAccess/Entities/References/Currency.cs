using System.ComponentModel.DataAnnotations;

namespace Decrypt.DataAccess.Entities.References
{
    public class Currency : BaseEntity
    {
        [MaxLength(3)] //ISO standard
        public string Code { get; set; } = null!;

        [MaxLength(150)]
        public string Name { get; set; } = null!;
    }
}
