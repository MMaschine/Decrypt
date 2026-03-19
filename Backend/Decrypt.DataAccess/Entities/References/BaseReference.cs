using System.ComponentModel.DataAnnotations;

namespace Decrypt.DataAccess.Entities.References
{
    public abstract class BaseReference
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Code { get; set; } = null!;

        [MaxLength(200)]
        public string Name { get; set; } = null!;
    }
}
