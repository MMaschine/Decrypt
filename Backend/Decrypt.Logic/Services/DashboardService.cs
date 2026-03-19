using Decrypt.DataAccess;
using Decrypt.Logic.Abstractions;
using Decrypt.Logic.Dtos;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Logic.Services
{
    /// <remarks>I'm not a fan of direct context access but here it is better, then the mess of dependencies</remarks>
    public sealed class DashboardService(DecryptContext context) : IDashboardService
    {
        /// <inheritdoc />
        public async Task<Result<DashboardDto>> GetDashboardAsync()
        {
            try
            {
                var totalOrganizations = await context.Organizations.CountAsync();
                var totalUsers = await context.Users.CountAsync();
                var totalProjects = await context.Projects.CountAsync();
                var activeProjects = await context.Projects.CountAsync(x => x.Status == "active");
                var totalTimeEntries = await context.TimeEntries.CountAsync();
                var totalInvoiced = await context.Invoices.SumAsync(x => (decimal?)x.Amount) ?? 0m;

                return Result.Ok(new DashboardDto
                {
                    TotalOrganizations = totalOrganizations,
                    TotalUsers = totalUsers,
                    TotalProjects = totalProjects,
                    ActiveProjects = activeProjects,
                    TotalTimeEntries = totalTimeEntries,
                    TotalInvoiced = totalInvoiced
                });
            }
            catch (Exception e)
            {
                //TODO: log real reason here 
                return Result.Fail("Failed to build");
            }

           
        }
    }
}
