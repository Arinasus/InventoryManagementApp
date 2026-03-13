using InventoryManagementApp.Data;
using InventoryManagementApp.DTOs;
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

        private async Task<ApplicationUser?> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(User);
        }

        private async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<bool> IsOwner(int inventoryId, ApplicationUser user)
        {
            var inv = await _context.Inventories.FindAsync(inventoryId);
            return inv != null && inv.CreatedByUserId == user.Id;
        }

        private async Task<bool> HasWriteAccess(int inventoryId)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return false;

            if (await IsAdmin(user))
                return true;

            var inv = await _context.Inventories.FindAsync(inventoryId);
            if (inv == null)
                return false;

            if (inv.CreatedByUserId == user.Id)
                return true;

            if (inv.IsPublic)
                return true;

            return await _context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == inventoryId && a.UserId == user.Id && a.CanWrite);
        }

        private async Task<bool> HasReadAccess(int inventoryId)
        {
            return true;
        }

        private async Task<bool> HasOwnerAccess(int inventoryId)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return false;

            if (await IsAdmin(user))
                return true;

            return await IsOwner(inventoryId, user);
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
                Fields = await GetFieldsModel(id),
                CustomId = await GetCustomIdModel(inventory),
                Discussion = await GetDiscussionModel(id),
                Stats = await GetStatsModel(id),
                Settings = GetSettingsModel(inventory),
                Access = await GetAccessModel(id)
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
                Description = inv.Description ?? "",
                Category = inv.Category,
                IsPublic = inv.IsPublic,
                RowVersion = inv.RowVersion ?? Array.Empty<byte>(),
                AvailableCategories = new List<string> { "Оборудование", "Мебель", "Книга", "Другое" },
                Tags = inv.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }
        private async Task<InventoryCustomIdViewModel> GetCustomIdModel(Inventory inv)
        {
            var parts = await _context.InventoryCustomIdParts
                .Where(p => p.InventoryId == inv.Id)
                .OrderBy(p => p.Order)
                .ToListAsync();

            return new InventoryCustomIdViewModel
            {
                InventoryId = inv.Id,
                Parts = parts,
                Preview = GenerateCustomIdPreview(parts)
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
                .Include(i => i.FieldValues)
                .Include(i => i.Likes)
                .ToListAsync();

            var posts = await _context.DiscussionPosts
                .Where(p => p.InventoryId == id)
                .CountAsync();

            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == id)
                .ToListAsync();

            var numeric = new Dictionary<string, double>();
            var common = new Dictionary<string, string>();

            foreach (var f in fields)
            {
                var values = items
                    .Select(i => i.FieldValues.FirstOrDefault(v => v.FieldId == f.Id)?.Value)
                    .Where(v => v != null)
                    .ToList();

                if (f.Type == FieldType.Number)
                {
                    var nums = values
                        .Select(v => double.TryParse(v, out var n) ? n : (double?)null)
                        .Where(n => n.HasValue)
                        .Select(n => n.Value)
                        .ToList();

                    if (nums.Any())
                        numeric[f.Title] = nums.Average();
                }
                else if (f.Type == FieldType.SingleLineText || f.Type == FieldType.MultiLineText)
                {
                    var most = values
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .GroupBy(v => v)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();

                    if (most != null)
                        common[f.Title] = most.Key;
                }
            }

            return new InventoryStatsViewModel
            {
                InventoryId = id,
                TotalItems = items.Count,
                TotalLikes = items.Sum(i => i.Likes.Count),
                TotalPosts = posts,
                AverageNumericFields = numeric,
                MostCommonStringFields = common
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
                .Include(i => i.FieldValues)
                .Include(i => i.Inventory)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound();

            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == item.InventoryId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            var model = new EditItemViewModel
            {
                ItemId = item.Id,
                InventoryId = item.InventoryId,
                Version = item.Version,
                Fields = fields,
                Values = fields.ToDictionary(
                    f => f.Id,
                    f => item.FieldValues.FirstOrDefault(v => v.FieldId == f.Id)?.Value ?? ""
                )
            };

            return View(model);
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
            if (!await HasWriteAccess(model.InventoryId))
                return Forbid();
            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == model.InventoryId)
                .ToListAsync();

            foreach (var field in fields)
            {
                model.Values.TryGetValue(field.Id, out var rawValue);
                rawValue ??= "";

                switch (field.Type)
                {
                    case FieldType.Number:
                        if (!double.TryParse(rawValue, out _))
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть числом.");
                        break;

                    case FieldType.DocumentLink:
                        if (!Uri.IsWellFormedUriString(rawValue, UriKind.Absolute))
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть ссылкой (URL).");
                        break;

                    case FieldType.Boolean:
                        if (rawValue != "true" && rawValue != "")
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть галочкой.");
                        break;
                }
            }

            if (!ModelState.IsValid)
            {
                model.Fields = fields;
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            var item = new InventoryItem
            {
                InventoryId = model.InventoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByUserId = user.Id,
                Version = 1,
                CustomId = await GenerateCustomId(model.InventoryId)
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            foreach (var kvp in model.Values)
            {
                _context.ItemFieldValues.Add(new ItemFieldValue
                {
                    ItemId = item.Id,
                    FieldId = kvp.Key,
                    Value = kvp.Value ?? ""
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Item", new { id = item.Id });
        }
        private async Task<string> GenerateCustomId(int inventoryId)
        {
            var parts = await _context.InventoryCustomIdParts
                .Where(p => p.InventoryId == inventoryId)
                .OrderBy(p => p.Order)
                .ToListAsync();
            if (!parts.Any())
                return Guid.NewGuid().ToString("N");

            var sb = new System.Text.StringBuilder();

            foreach (var p in parts)
            {
                switch (p.Type)
                {
                    case CustomIdPartType.FixedText:
                        sb.Append(p.Value);
                        break;

                    case CustomIdPartType.Random20Bit:
                        sb.Append(new Random().Next(0, 1 << 20));
                        break;

                    case CustomIdPartType.Random32Bit:
                        sb.Append(new Random().Next());
                        break;

                    case CustomIdPartType.Random6Digits:
                        sb.Append(new Random().Next(0, 999999).ToString("D6"));
                        break;

                    case CustomIdPartType.Random9Digits:
                        sb.Append(new Random().Next(0, 999999999).ToString("D9"));
                        break;

                    case CustomIdPartType.Guid:
                        sb.Append(Guid.NewGuid().ToString("N"));
                        break;

                    case CustomIdPartType.DateTime:
                        sb.Append(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                        break;

                    case CustomIdPartType.Sequence:
                        var max = await _context.InventoryItems
                            .Where(i => i.InventoryId == inventoryId)
                            .MaxAsync(i => (int?)i.SequenceNumber) ?? 0;

                        sb.Append(max + 1);
                        break;
                }
            }

            return sb.ToString();
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(EditItemViewModel model)
        {
            if (!await HasWriteAccess(model.InventoryId))
                return Forbid();

            var item = await _context.InventoryItems
                .Include(i => i.FieldValues)
                .FirstOrDefaultAsync(i => i.Id == model.ItemId);

            if (item == null)
                return NotFound();

            var fields = await _context.InventoryFields
                .Where(f => f.InventoryId == model.InventoryId)
                .OrderBy(f => f.Order)
                .ToListAsync();

            foreach (var field in fields)
            {
                model.Values.TryGetValue(field.Id, out var rawValue);
                rawValue ??= "";

                switch (field.Type)
                {
                    case FieldType.Number:
                        if (!double.TryParse(rawValue, out _))
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть числом.");
                        break;

                    case FieldType.DocumentLink:
                        if (!Uri.IsWellFormedUriString(rawValue, UriKind.Absolute))
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть ссылкой (URL).");
                        break;

                    case FieldType.Boolean:
                        if (rawValue != "true" && rawValue != "")
                            ModelState.AddModelError($"Values[{field.Id}]", $"Поле «{field.Title}» должно быть галочкой.");
                        break;
                }
            }

            if (!ModelState.IsValid)
            {
                model.Fields = fields;
                return View("Item", model);
            }
            
            if (item.Version != model.Version)
            {
                ModelState.AddModelError("", "Этот предмет был изменён другим пользователем. Обновите страницу.");

                model.Fields = fields;
                model.Values = fields.ToDictionary(
                    f => f.Id,
                    f => item.FieldValues.FirstOrDefault(v => v.FieldId == f.Id)?.Value ?? ""
                );

                return View("Item", model);
            }

            foreach (var kvp in model.Values)
            {
                var fieldId = kvp.Key;
                var value = kvp.Value ?? "";

                var existing = item.FieldValues.FirstOrDefault(v => v.FieldId == fieldId);

                if (existing == null)
                {
                    _context.ItemFieldValues.Add(new ItemFieldValue
                    {
                        ItemId = item.Id,
                        FieldId = fieldId,
                        Value = value
                    });
                }
                else
                {
                    existing.Value = value;
                }
            }

            item.Version++;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("Item", new { id = item.Id });
        }
        public async Task<IActionResult> DeleteItem(int id)
        {

            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();
            if (!await HasWriteAccess(item.InventoryId))
                return Forbid();

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
        [HttpPost]
        public async Task<IActionResult> ReorderFields([FromBody] List<int> orderedIds)
        {
            var firstField = await _context.InventoryFields.FindAsync(orderedIds.First());
            if (firstField == null) return NotFound();

            if (!await HasOwnerAccess(firstField.InventoryId))
                return Forbid();

            var fields = await _context.InventoryFields
                .Where(f => orderedIds.Contains(f.Id))
                .ToListAsync();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var field = fields.First(f => f.Id == orderedIds[i]);
                field.Order = i + 1;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        public IActionResult AddField(int id)
        {
            return View(new InventoryField { InventoryId = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddField(InventoryField field)
        {
            if (!await HasOwnerAccess(field.InventoryId))
                return Forbid();

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
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            var field = await _context.InventoryFields.FindAsync(id);
            if (field == null) return NotFound();

            _context.InventoryFields.Remove(field);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        [HttpPost]
        public async Task<IActionResult> ToggleShowInTable(int id, int inventoryId)
        {
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            var field = await _context.InventoryFields.FindAsync(id);
            if (field == null) return NotFound();

            field.ShowInTable = !field.ShowInTable;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        public async Task<IActionResult> CustomId(int id)
        {
            if (!await HasOwnerAccess(id))
                return Forbid();

            var parts = await _context.InventoryCustomIdParts
                .Where(p => p.InventoryId == id)
                .OrderBy(p => p.Order)
                .ToListAsync();

            var preview = GenerateCustomIdPreview(parts);

            var model = new InventoryCustomIdViewModel
            {
                InventoryId = id,
                Parts = parts,
                Preview = preview
            };

            return PartialView("_TabCustomId", model);
        }
        private string GenerateCustomIdPreview(List<InventoryCustomIdPart> parts)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var p in parts)
            {
                switch (p.Type)
                {
                    case CustomIdPartType.FixedText:
                        sb.Append(p.Value);
                        break;

                    case CustomIdPartType.Random20Bit:
                        sb.Append(new Random().Next(0, 1 << 20).ToString());
                        break;

                    case CustomIdPartType.Random32Bit:
                        sb.Append(new Random().Next().ToString());
                        break;

                    case CustomIdPartType.Random6Digits:
                        sb.Append(new Random().Next(0, 999999).ToString("D6"));
                        break;

                    case CustomIdPartType.Random9Digits:
                        sb.Append(new Random().Next(0, 999999999).ToString("D9"));
                        break;

                    case CustomIdPartType.Guid:
                        sb.Append(Guid.NewGuid().ToString("N"));
                        break;

                    case CustomIdPartType.DateTime:
                        sb.Append(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                        break;

                    case CustomIdPartType.Sequence:
                        sb.Append("###");
                        break;
                }
            }

            return sb.ToString();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomIdPart(int inventoryId, CustomIdPartType type)
        {
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            var order = await _context.InventoryCustomIdParts
                .Where(p => p.InventoryId == inventoryId)
                .CountAsync() + 1;

            var part = new InventoryCustomIdPart
            {
                InventoryId = inventoryId,
                Type = type,
                Order = order,
                Value = type == CustomIdPartType.FixedText ? "TEXT" : null
            };

            _context.InventoryCustomIdParts.Add(part);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomIdPart(int id, int inventoryId)
        {
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            var part = await _context.InventoryCustomIdParts.FindAsync(id);
            if (part == null) return NotFound();

            _context.InventoryCustomIdParts.Remove(part);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        [HttpPost]
        public async Task<IActionResult> ReorderCustomIdParts([FromBody] List<int> orderedIds)
        {
            var first = await _context.InventoryCustomIdParts.FindAsync(orderedIds.First());
            if (first == null) return NotFound();

            if (!await HasOwnerAccess(first.InventoryId))
                return Forbid();

            var parts = await _context.InventoryCustomIdParts
                .Where(p => orderedIds.Contains(p.Id))
                .ToListAsync();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var part = parts.First(p => p.Id == orderedIds[i]);
                part.Order = i + 1;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        public async Task<IActionResult> Discussion(int id)
        {
            if (!await HasReadAccess(id))
                return Forbid();

            var model = await GetDiscussionModel(id);
            return PartialView("_TabDiscussion", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddPost(int inventoryId, string content)
        {
            if (!await HasWriteAccess(inventoryId))
                return Forbid();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", new { id = inventoryId });

            var post = new DiscussionPost
            {
                InventoryId = inventoryId,
                UserId = user.Id,
                ContentMarkdown = content,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.DiscussionPosts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = inventoryId });
        }
        public async Task<IActionResult> DiscussionPosts(int id)
        {
            if (!await HasReadAccess(id))
                return Forbid();

            var model = await GetDiscussionModel(id);
            return PartialView("_DiscussionPosts", model);
        }
        [HttpPost]
        public async Task<IActionResult> AutoSave([FromBody] InventoryAutoSaveDto dto)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == dto.Id);
            if (inventory == null)
                return NotFound();
            if (!await HasWriteAccess(dto.Id))
                return Forbid();
            inventory.Title = dto.Title;
            inventory.Description = dto.Description;
            inventory.Category = dto.Category;
            if (dto.Tags != null)
            {
                await _context.Entry(inventory)
                    .Collection(i => i.Tags)
                    .LoadAsync();

                inventory.Tags.Clear();

                foreach (var tagName in dto.Tags)
                {
                    var tag = await _context.InventoryTags
                        .FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new InventoryTag { Name = tagName };
                        _context.InventoryTags.Add(tag);
                    }
                    inventory.Tags.Add(tag);
                }
            }
            _context.Entry(inventory).Property("RowVersion").OriginalValue = dto.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, rowVersion = inventory.RowVersion });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { success = false, message = "Конфликт изменений" });
            }
        }
        public async Task<IActionResult> Settings(int id)
        {
            if (!await HasWriteAccess(id))
                return Forbid();

            var inv = await _context.Inventories.FindAsync(id);

            var model = new InventorySettingsViewModel
            {
                InventoryId = inv.Id,
                Title = inv.Title,
                Description = inv.Description,
                Category = inv.Category,
                AvailableCategories = new List<string> { "Оборудование", "Мебель", "Книга", "Другое" },
                RowVersion = inv.RowVersion
            };

            return PartialView("_TabSettings", model);
        }
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());

            var users = await _context.Users
                .Where(u => u.Email.Contains(query) || u.UserName.Contains(query))
                .Select(u => new { u.Id, u.Email, u.UserName })
                .Take(10)
                .ToListAsync();

            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccess(int inventoryId, string userId)
        {
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            if (!_context.InventoryAccesses
                .Any(a => a.InventoryId == inventoryId && a.UserId == userId))
            {
                _context.InventoryAccesses.Add(new InventoryAccess
                {
                    InventoryId = inventoryId,
                    UserId = userId,
                    CanWrite = true,
                    GrantedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = inventoryId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAccess(int id, int inventoryId)
        {
            if (!await HasOwnerAccess(inventoryId))
                return Forbid();

            var access = await _context.InventoryAccesses.FindAsync(id);
            if (access != null)
            {
                _context.InventoryAccesses.Remove(access);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = inventoryId });
        }

        public async Task<IActionResult> Access(int id)
        {
            if (!await HasOwnerAccess(id))
                return Forbid();

            var model = await GetAccessModel(id);
            return PartialView("_TabAccess", model);
        }

    }
}
