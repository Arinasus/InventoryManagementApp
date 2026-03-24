using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using InventoryManagementApp.Services;
using InventoryManagementApp.ViewModels;
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
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IConfiguration config, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        public async Task<IActionResult> Profile(string? sortOrder, string? search)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var ownedQuery = _context.Inventories
                .Where(i => i.CreatedByUserId == user.Id)
                .Select(i => new InventoryCardViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    ItemsCount = i.Items.Count,
                    CreatorName = user.UserName,
                    CreatedAt = i.CreatedAt
                });

            var accessibleQuery = _context.InventoryAccesses
                .Where(a => a.UserId == user.Id)
                .Select(a => new InventoryCardViewModel
                {
                    Id = a.Inventory.Id,
                    Title = a.Inventory.Title,
                    Description = a.Inventory.Description,
                    ItemsCount = a.Inventory.Items.Count,
                    CreatorName = a.Inventory.CreatedByUser.UserName,
                    CreatedAt = a.Inventory.CreatedAt
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                ownedQuery = ownedQuery.Where(i =>
                    i.Title.Contains(search) || i.Description.Contains(search));

                accessibleQuery = accessibleQuery.Where(i =>
                    i.Title.Contains(search) || i.Description.Contains(search));
            }

            sortOrder ??= "";

            ownedQuery = sortOrder switch
            {
                "title_desc" => ownedQuery.OrderByDescending(i => i.Title),
                "items_asc" => ownedQuery.OrderBy(i => i.ItemsCount),
                "items_desc" => ownedQuery.OrderByDescending(i => i.ItemsCount),
                "title_asc" => ownedQuery.OrderBy(i => i.Title),
                _ => ownedQuery.OrderBy(i => i.CreatedAt)
            };

            accessibleQuery = accessibleQuery.OrderBy(i => i.Title);

            var model = new UserProfileViewModel
            {
                UserName = user.UserName,
                SearchQuery = search ?? "",
                SortOrder = sortOrder,
                OwnedInventories = await ownedQuery.ToListAsync(),
                AccessibleInventories = await accessibleQuery.ToListAsync()
            };
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> PublicProfile(string id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View("PublicProfile", user);
        }
        [HttpGet]
        public IActionResult Salesforce()
        {
            return View(new SalesforceViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Salesforce(SalesforceViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var sf = new SalesforceService(_config);

            var accountId = await sf.CreateAccount(model.CompanyName, model.Phone, model.Website, model.Industry, model.Description);

            await sf.CreateContact(user.Email, user.UserName, accountId);

            TempData["Success"] = "Salesforce integration completed!";
            return RedirectToAction("Index");
        }

    }
}
