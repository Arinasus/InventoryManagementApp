using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApp.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public InventoryController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var inventories = await _db.Inventories
                .Include(i => i.InventoryAccesses)
                .Where(i => i.CreatedByUserId == user.Id
                         || i.InventoryAccesses != null && i.InventoryAccesses.Any(a => a.UserId == user.Id))
                .ToListAsync();

            return View(inventories);
        }
    }
}
