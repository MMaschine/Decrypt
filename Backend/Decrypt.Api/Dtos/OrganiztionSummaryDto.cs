using Decrypt.DataAccess.Entities;

namespace Decrypt.Api.Dtos
{
    public class OrganizationSummaryDto
    {
        public Organization Organization { get; set; } = null!;

        public int ProjectCount { get; set; }

        public int UserCount { get; set; }

        public decimal TotalInvoiced { get; set; }

        public string Currency { get; set; } = null!;
    }
}
