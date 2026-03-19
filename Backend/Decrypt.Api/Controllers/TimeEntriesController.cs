using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Controllers
{

    [ApiController]
    [Route("api/time-entries")]
    public class TimeEntriesController(IGenericDataSource<TimeEntry> dataSource) : ControllerBase
    {
        private const string NotFound = "Time entry not found";

        [HttpGet]
        public async Task<IActionResult> GetTimeEntries([FromQuery] int? userId, [FromQuery] int? projectId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var queryable = dataSource.GetQueryableItems();

            if (userId.HasValue)
            {
                queryable = queryable.Where(x => x.UserId == userId.Value);
            }

            if (projectId.HasValue)
            {
                queryable = queryable.Where(x => x.ProjectId == projectId.Value);
            }

            if (from.HasValue)
            {
                queryable = queryable.Where(x => x.Date >= from.Value);
            }

            if (to.HasValue)
            {
                queryable = queryable.Where(x => x.Date <= to.Value);
            }

            var result = await queryable.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeEntryById(int id)
        {
            var timeEntry = await dataSource.GetByIdAsync(id);

            if (timeEntry is null)
                return NotFound(new { error = NotFound });

            return Ok(timeEntry);
        }
    }
}
