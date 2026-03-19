using Decrypt.DataAccess.Entities;

namespace MockLoader.Models
{
    public class ParsedProject
    {
        public int SourceIndex { get; set; }

        public Project Entity { get; set; } = null!;

        public string OrganizationLegacyId { get; set; } = null!;
    }
}
