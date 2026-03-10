using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApp.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public InventoryController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _context = db;
            _userManager = userManager;
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        public async Task<IActionResult> Create(Inventory model)
        {
            var user = await _userManager.GetUserAsync(User);

            model.CreatedByUserId = user.Id;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
            model.Version = 1;

            _context.Inventories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.Id });
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.CreatedByUser)
                .Include(i => i.Tags)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                return NotFound();

            return View(inventory);
        }
        public async Task<IActionResult> Items(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Fields)
                .Include(i => i.Items)
                    .ThenInclude(it => it.FieldValues)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Likes)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                return NotFound();

            var tableFields = inventory.Fields
                .Where(f => f.ShowInTable)
                .OrderBy(f => f.Order)
                .ToList();

            var items = inventory.Items.Select(item => new InventoryItemRowViewModel
            {
                Id = item.Id,
                CustomId = item.CustomId,
                CreatedAt = item.CreatedAt,
                Likes = item.Likes.Count,
                FieldValues = tableFields.ToDictionary(
                    f => f.Title,
                    f => item.FieldValues.FirstOrDefault(v => v.FieldId == f.Id)?.Value ?? ""
                )
            }).ToList();

            var model = new InventoryItemsViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                Items = items,
                TableFields = tableFields
            };

            return PartialView("_TabItems", model);
        }
        public IActionResult AddItem(int id)
        {
            var model = new AddItemViewModel { InventoryId = id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(AddItemViewModel model)
        {
            var item = new InventoryItem
            {
                InventoryId = model.InventoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1,
                CustomId = "TEMP" // позже заменим генерацией
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Item", new { id = item.Id });
        }
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = item.InventoryId });
        }

    }
}
