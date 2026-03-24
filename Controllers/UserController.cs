using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using InventoryManagementApp.Services;
using InventoryManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public UserController(IConfiguration config, AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<UserController> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _loggerFactory = loggerFactory;
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
            _logger.LogInformation("=== UserController.Salesforce START ===");
            _logger.LogInformation($"Model: {model.CompanyName}, {model.Phone}");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                _logger.LogInformation($"User found: {user?.UserName}, Email: {user?.Email}");

                _logger.LogInformation("Creating SalesforceService...");
                
                var sfLogger = _loggerFactory.CreateLogger<SalesforceService>();
                var sf = new SalesforceService(_config, sfLogger); 
                _logger.LogInformation("SalesforceService created successfully");

                _logger.LogInformation("Creating Account...");
                var accountId = await sf.CreateAccount(
                    model.CompanyName,
                    model.Phone,
                    model.Website,
                    model.Industry,
                    model.Description
                );
                _logger.LogInformation($"Account created with ID: {accountId}");

                _logger.LogInformation("Creating Contact...");
                await sf.CreateContact(user.Email, user.UserName, accountId);
                _logger.LogInformation("Contact created successfully");

                TempData["Success"] = "Salesforce integration completed!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR in Salesforce action");
                TempData["Error"] = $"Salesforce error: {ex.Message}";
            }

            _logger.LogInformation("=== UserController.Salesforce END ===");
            return RedirectToAction("Profile");
        }
    }
}
