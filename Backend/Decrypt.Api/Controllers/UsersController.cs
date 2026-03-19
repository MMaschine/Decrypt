using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IGenericDataSource<User> dataSource) : ControllerBase
    {
        private const string UserNotFound = "User not found";

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int? orgId, [FromQuery] string? role, [FromQuery] bool? active)
        {
            var queryable = dataSource.GetQueryableItems();

            if (orgId.HasValue)
            {
                queryable = queryable.Where(x => x.OrganizationId == orgId.Value);
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                queryable = queryable.Where(x => x.UserRole == role);
            }

            if (active.HasValue)
            {
                queryable = queryable.Where(x => x.IsActive == active.Value);
            }

            var result = await queryable.ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await dataSource.GetByIdAsync(id);

            if (user is null)
                return NotFound(new { error = UserNotFound });

            return Ok(user);
        }
    }
}
