using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Decrypt.DataAccess.Entities.References;


namespace Decrypt.DataAccess.Entities
{
    public sealed class Invoice : LegacyEntity
    {
        public int OrganizationId { get; set; }

        public int ProjectId { get; set; }

        [Column(TypeName = "decimal(18,2)")] //For finance data 
        public decimal Amount { get; set; }

        public int CurrencyId  { get; set; }

        public string Status { get; set; }

        public DateOnly? DueDate { get; set; }

        public DateOnly? IssuedAt { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }

        #region Navigation

        public Organization Organization { get; set; }

        public Project Project { get; set; }

        public Currency Currency { get; set; }

        #endregion
    }
}
