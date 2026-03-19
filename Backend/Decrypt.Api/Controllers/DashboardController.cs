using Decrypt.Logic.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Decrypt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class DashboardController(IDashboardService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await service.GetDashboardAsync();

            if (result.IsFailed)
            {
                return Problem(
                    title: "Failed to load dashboard",
                    statusCode: 500);
            }

            return Ok(result.Value);
        }
    }
}