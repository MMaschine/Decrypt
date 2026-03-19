using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Decrypt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController(IGenericDataSource<Invoice> dataSource) : ControllerBase
    {
        private const string NotFound = "Invoice not found";

        [HttpGet]
        public async Task<IActionResult> GetInvoices([FromQuery] int? orgId, [FromQuery] string? status)
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
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await dataSource.GetByIdAsync(id);

            if (invoice is null)
                return NotFound(new { error = NotFound });

            return Ok(invoice);
        }
    }
}
