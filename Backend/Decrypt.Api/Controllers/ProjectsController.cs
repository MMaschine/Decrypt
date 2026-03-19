using Decrypt.Api.Dtos;
using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IGenericDataSource<Project> dataSource) : ControllerBase
    {
        private const string NotFoundError = "Project not found";

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] int? orgId, [FromQuery] string? status)
        {
            var queryable = dataSource.GetQueryableItems();

            if (orgId.HasValue)
            {
                queryable = queryable.Where(x => x.OrganizationId == orgId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryable = queryable.Where(x => x.Status == status);
            }

            var result = await queryable.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var result = await dataSource.GetQueryableItems()
                .Include(x=>x.TimeEntries)
                .Include(x=>x.Organization)
                .Where(x => x.Id == id)
                .Select(x => new ProjectDetailsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    Organization = new ProjectOrganizationDto
                    {
                        Id = x.Organization.Id,
                        Name = x.Organization.Name,
                    },
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    BudgetHours = x.BudgetHours,
                    TotalHoursLogged = x.TimeEntries.Sum(te => (int?)te.Hours) ?? 0
                })
                .FirstOrDefaultAsync();

            if (result is null)
                return NotFound(new { error = NotFoundError });

            return Ok(result);
        }
    }
}
