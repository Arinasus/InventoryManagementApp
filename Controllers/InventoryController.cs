using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
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
            if (user == null)
            {
                return Content("USER IS NULL");
            }
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

            var itemsModel = await GetItemsModel(id);

            var vm = new InventoryDetailsViewModel
            {
                Inventory = inventory,
                Items =  await GetItemsModel(id),
                Fields = await GetFieldsModel(id)
                /*Discussion = await GetDiscussionModel(id),
                Settings = GetSettingsModel(inventory),
                CustomId = GetCustomIdModel(inventory),
                Access = await GetAccessModel(id),
                ,
                Stats = await GetStatsModel(id)*/
            };

            return View(vm);

        }
        private async Task<InventoryDiscussionViewModel> GetDiscussionModel(int id)
        {
            var posts = await _context.DiscussionPosts
                .Where(p => p.InventoryId == id)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return new InventoryDiscussionViewModel
            {
                InventoryId = id,
                Posts = posts,
                CurrentUserId = _userManager.GetUserId(User)
            };
        }
        private InventorySettingsViewModel GetSettingsModel(Inventory inv)
        {
            return new InventorySettingsViewModel
            {
                InventoryId = inv.Id,
                Title = inv.Title,
                Description = inv.Description,
                Category = inv.Category,
                IsPublic = inv.IsPublic
            };
        }
        private InventoryCustomIdViewModel GetCustomIdModel(Inventory inv)
        {
            return new InventoryCustomIdViewModel
            {
                InventoryId = inv.Id,
                Prefix = inv.CustomIdPrefix,
                NextNumber = inv.NextCustomIdNumber
            };
        }
        private async Task<InventoryAccessViewModel> GetAccessModel(int id)
        {
            var access = await _context.InventoryAccesses
                .Where(a => a.InventoryId == id)
                .Include(a => a.User)
                .ToListAsync();

            return new InventoryAccessViewModel
            {
                InventoryId = id,
                AccessList = access
            };
        }
        private async Task<InventoryFieldsViewModel> GetFieldsModel(int id)
        {
            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == id)
                .OrderBy(f => f.Order)
                .ToListAsync();

            return new InventoryFieldsViewModel
            {
                InventoryId = id,
                Fields = fields
            };
        }
        private async Task<InventoryStatsViewModel> GetStatsModel(int id)
        {
            var items = await _context.InventoryItems
                .Where(i => i.InventoryId == id)
                .Include(i => i.Likes)
                .ToListAsync();

            var posts = await _context.DiscussionPosts
                .Where(p => p.InventoryId == id)
                .CountAsync();

            return new InventoryStatsViewModel
            {
                InventoryId = id,
                TotalItems = items.Count,
                TotalLikes = items.Sum(i => i.Likes.Count),
                TotalPosts = posts
            };
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
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventories.ToListAsync();
            return View(inventories);
        }
        public async Task<IActionResult> Item(int id)
        {
            var item = await _context.InventoryItems
                .Include(i => i.Inventory)
                .Include(i => i.FieldValues)
                    .ThenInclude(f => f.Field)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        public async Task<IActionResult> AddItem(int id)
        {
            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == id)
                .OrderBy(f => f.Order)
                .ToListAsync();

            var model = new AddItemViewModel
            {
                InventoryId = id,
                Fields = fields
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem(AddItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Fields = await _context.InventoryFields
                    .Where(f => f.InventoryId == model.InventoryId)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                return View(model);
            }

            var item = new InventoryItem
            {
                InventoryId = model.InventoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1,
                CustomId = "TEMP"
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            if (model.Values != null && model.Values.Any())
            {
                foreach (var kvp in model.Values)
                {
                    var fieldId = kvp.Key;
                    var value = kvp.Value;

                    var fieldValue = new ItemFieldValue
                    {
                        ItemId = item.Id,
                        FieldId = fieldId,
                        Value = value
                    };

                    _context.ItemFieldValues.Add(fieldValue);
                }

                await _context.SaveChangesAsync();
            }

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
        private async Task<InventoryItemsViewModel> GetItemsModel(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Fields)
                .Include(i => i.Items).ThenInclude(it => it.FieldValues)
                .Include(i => i.Items).ThenInclude(it => it.Likes)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                return null;

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

            return new InventoryItemsViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                Items = items,
                TableFields = tableFields
            };
        }
        public IActionResult AddField(int id)
        {
            return View(new InventoryField { InventoryId = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddField(InventoryField field)
        {
            field.Id = 0;
            field.Order = await _context.InventoryFields
                .Where(f => f.InventoryId == field.InventoryId)
                .CountAsync() + 1;

            _context.InventoryFields.Add(field);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = field.InventoryId });
        }
        public async Task<IActionResult> DeleteField(int id, int inventoryId)
        {
            var field = await _context.InventoryFields.FindAsync(id);
            if (field == null) return NotFound();

            _context.InventoryFields.Remove(field);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        [HttpPost]
        public async Task<IActionResult> ToggleShowInTable(int id, int inventoryId)
        {
            var field = await _context.InventoryFields.FindAsync(id);
            if (field == null) return NotFound();

            field.ShowInTable = !field.ShowInTable;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }

    }
}
