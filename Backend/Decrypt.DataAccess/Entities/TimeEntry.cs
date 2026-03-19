using System.ComponentModel.DataAnnotations;

namespace Decrypt.DataAccess.Entities
{
    public sealed class TimeEntry : LegacyEntity
    {
        public int UserId { get; set; }

        public int ProjectId { get; set; }

        public DateOnly Date { get; set; }

        public int Hours { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        #region Navigation

        public User User { get; set; }

        public Project Project { get; set; }

        #endregion
    }
}
