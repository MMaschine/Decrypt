namespace Decrypt.Api.Dtos
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Status { get; set; } = null!;

        public ProjectOrganizationDto Organization { get; set; } = null!;

        public DateOnly? StartDate { get; set;  }

        public DateOnly? EndDate { get; set; }

        public int BudgetHours { get; set; }

        public int TotalHoursLogged { get; set; }
    }
}
