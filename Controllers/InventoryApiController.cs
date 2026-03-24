using InventoryManagementApp.Data;
using InventoryManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagementApp.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            var inv = await _context.Inventories
                .Include(i => i.Fields)
                .Include(i => i.Items)
                    .ThenInclude(i => i.FieldValues)
                .FirstOrDefaultAsync(i => i.ApiToken == token);

            if (inv == null)
                return Unauthorized(new { error = "Invalid API token" });

            var agg = new AggregationService().Build(inv);

            return Ok(new
            {
                id = inv.Id,
                title = inv.Title,
                description = inv.Description,
                aggregated = agg
            });
        }
    }
}
