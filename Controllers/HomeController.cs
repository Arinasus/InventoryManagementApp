using InventoryManagementApp.Data;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InventoryManagementApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Inventories
                .Include(i => i.CreatedByUser)
                .Include(i => i.Items)
                .Include(i => i.Tags)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i =>
                    i.Title.Contains(search) ||
                    i.Description.Contains(search) ||
                    i.Tags.Any(t => t.Name.Contains(search)));
            }

            var latest = await query
                .OrderByDescending(i => i.CreatedAt)
                .Take(10)
                .Select(i => new InventoryCardViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    CreatorName = i.CreatedByUser.UserName,
                    ItemsCount = i.Items.Count,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            var popular = await query
                .OrderByDescending(i => i.Items.Count)
                .Take(5)
                .Select(i => new InventoryCardViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    CreatorName = i.CreatedByUser.UserName,
                    ItemsCount = i.Items.Count,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            var tags = await _context.InventoryTags
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    Count = t.Inventories.Count
                })
                .ToListAsync();

            var model = new HomePageViewModel
            {
                LatestInventories = latest,
                PopularInventories = popular,
                Tags = tags,
                SearchQuery = search
            };

            return View(model);
        }
    }
}
