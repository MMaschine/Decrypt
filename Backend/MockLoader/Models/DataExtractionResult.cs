

namespace MockLoader.Models
{
    internal class DataExtractionResult
    {
        public List<ParsedOrganization> Organizations { get; init; } = [];

        public List<ParsedProject> Projects { get; init; } = [];

        public List<ParsedUser> Users { get; init; } = [];

        public List<ParsedInvoice> Invoices { get; init; } = [];

        public List<ParsedTimeEntry> TimeEntries { get; init; } = [];
    }
}
