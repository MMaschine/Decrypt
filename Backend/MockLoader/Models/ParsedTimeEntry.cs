using Decrypt.DataAccess.Entities;


namespace MockLoader.Models
{
    public class ParsedTimeEntry
    {
        public int SourceIndex { get; set; }

        public TimeEntry Entity { get; set; } = null!;

        public string UserLegacyId { get; set; } = null!;

        public string ProjectLegacyId { get; set; } = null!;
    }
}
