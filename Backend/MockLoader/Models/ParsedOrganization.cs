using Decrypt.DataAccess.Entities;

namespace MockLoader.Models
{
    internal class ParsedOrganization : ParsedEntity
    {
        public required Organization Entity { get; init; }

        public required string CurrencyCode { get; set; }

    }
}
