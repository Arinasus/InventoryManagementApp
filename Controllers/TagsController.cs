using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    public class TagsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TagsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> SearchTags(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new string[0]);

            var tags = await _context.InventoryTags
                .Where(t => t.Name.StartsWith(query))
                .Select(t => t.Name)
                .Take(10)
                .ToListAsync();

            return Json(tags);
        }

    }
}
