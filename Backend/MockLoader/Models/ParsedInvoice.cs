using Decrypt.DataAccess.Entities;

namespace MockLoader.Models
{
    public class ParsedInvoice
    {
        public int SourceIndex { get; set; }
        
        public Invoice Entity { get; set; } = null!;

        public string OrganizationLegacyId { get; set; } = null!;
        
        public string ProjectLegacyId { get; set; } = null!;
        
        public string CurrencyCode { get; set; } = null!;
    }
}