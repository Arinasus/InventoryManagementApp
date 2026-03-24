using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementApp.Data;
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
                .FirstOrDefaultAsync(i => i.ApiToken == token);

            if (inv == null)
                return Unauthorized(new { error = "Invalid API token" });

            return Ok(new
            {
                id = inv.Id,
                title = inv.Title,
                description = inv.Description,
                fields = inv.Fields.Select(f => new
                {
                    f.Id,
                    f.Title,
                    f.Type
                }),
                itemsCount = inv.Items.Count
            });
        }
    }
}
