namespace Decrypt.Logic.Dtos
{
    public sealed class DashboardDto
    {
        public int TotalOrganizations { get; set; }

        public int TotalUsers { get; set; }

        public int TotalProjects { get; set; }

        public int ActiveProjects { get; set; }

        public int TotalTimeEntries { get; set; }

        public decimal TotalInvoiced { get; set; }
    }
}
