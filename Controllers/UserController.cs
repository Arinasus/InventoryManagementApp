using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile(string sortOrder, string search)
        {
            var user = await _userManager.GetUserAsync(User);

            var ownedQuery = _context.Inventories
                .Include(i => i.Items)
                .Where(i => i.CreatedByUserId == user.Id);

            var accessQuery = _context.InventoryAccesses
                .Include(a => a.Inventory)
                .ThenInclude(i => i.Items)
                .Where(a => a.UserId == user.Id)
                .Select(a => a.Inventory);

            if (!string.IsNullOrWhiteSpace(search))
            {
                ownedQuery = ownedQuery.Where(i => i.Title.Contains(search));
                accessQuery = accessQuery.Where(i => i.Title.Contains(search));
            }

            ownedQuery = sortOrder switch
            {
                "title_desc" => ownedQuery.OrderByDescending(i => i.Title),
                "items_asc" => ownedQuery.OrderBy(i => i.Items.Count),
                "items_desc" => ownedQuery.OrderByDescending(i => i.Items.Count),
                _ => ownedQuery.OrderBy(i => i.Title)
            };

            accessQuery = sortOrder switch
            {
                "title_desc" => accessQuery.OrderByDescending(i => i.Title),
                "items_asc" => accessQuery.OrderBy(i => i.Items.Count),
                "items_desc" => accessQuery.OrderByDescending(i => i.Items.Count),
                _ => accessQuery.OrderBy(i => i.Title)
            };

            var model = new UserProfileViewModel
            {
                UserName = user.UserName,
                SearchQuery = search,
                SortOrder = sortOrder,

                OwnedInventories = await ownedQuery
                    .Select(i => new InventoryCardViewModel
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        ItemsCount = i.Items.Count,
                        CreatorName = user.UserName,
                        CreatedAt = i.CreatedAt
                    })
                    .ToListAsync(),

                AccessibleInventories = await accessQuery
                    .Select(i => new InventoryCardViewModel
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        ItemsCount = i.Items.Count,
                        CreatorName = i.CreatedByUser.UserName,
                        CreatedAt = i.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
