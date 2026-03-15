using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    public class InventoryItemController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public InventoryItemController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var like = await _context.ItemLikes
                .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == user.Id);

            if (like == null)
            {
                _context.ItemLikes.Add(new ItemLike
                {
                    ItemId = itemId,
                    UserId = user.Id,
                    LikedAt = DateTime.UtcNow
                });
            }
            else
            {
                _context.ItemLikes.Remove(like);
            }

            await _context.SaveChangesAsync();

            int count = await _context.ItemLikes.CountAsync(l => l.ItemId == itemId);

            return Json(new { likes = count });
        }

    }
}
