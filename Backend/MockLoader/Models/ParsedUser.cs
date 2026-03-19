using Decrypt.DataAccess.Entities;

namespace MockLoader.Models
{
    public class ParsedUser
    {
        public int SourceIndex { get; set; }

        public User Entity { get; set; } = null!;

        public string OrganizationLegacyId { get; set; } = null!;
    }
}
