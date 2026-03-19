using Decrypt.Api.Dtos;
using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationsController(IGenericDataSource<Organization> dataSource) : ControllerBase
    {
        //TODO: to the separate storage of resources in real project 
        private const string OrganizationNotFound = "Organization not found";

        [HttpGet]
        public async Task<IActionResult> GetOrganizations([FromQuery] string? tier, [FromQuery] string? industry)
        {
            var queryable = dataSource.GetQueryableItems();

            if (!string.IsNullOrWhiteSpace(tier))
            {
                queryable = queryable.Where(o => o.Tier == tier);
            }

            if (!string.IsNullOrWhiteSpace(industry))
            {
                queryable = queryable.Where(o => o.Industry == industry);
            }

            var result = await queryable.ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganizationById(int id)
        {
            var organization = await dataSource.GetByIdAsync(id);

            if (organization is null)
                return NotFound(new { error = OrganizationNotFound });

            return Ok(organization);
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetOrganizationSummary(int id)
        {
            var result = await dataSource.GetQueryableItems()
                .Where(x => x.Id == id)
                .Select(x => new OrganizationSummaryDto
                {
                    Organization = x,
                    ProjectCount = x.Projects.Count(),
                    UserCount = x.Users.Count(),
                    TotalInvoiced = x.Invoices.Sum(i => (decimal?)i.Amount) ?? 0m,
                    Currency = x.Currency.Code
                })
                .FirstOrDefaultAsync();

            if (result is null)
                return NotFound(new { error = OrganizationNotFound });

            return Ok(result);
        }
    }
}
