using InventoryManagementApp.Data;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;


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

            NpgsqlTsQuery? tsQuery = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalized = string.Join(" & ",
                    search.Split(' ', StringSplitOptions.RemoveEmptyEntries));

                tsQuery = EF.Functions.ToTsQuery(normalized);

                query = query
                    .Where(i => i.SearchVector != null &&
                                i.SearchVector.Matches(tsQuery))
                    .OrderByDescending(i => i.SearchVector.Rank(tsQuery));
            }
            else
            {
                query = query.OrderByDescending(i => i.CreatedAt);
            }

            var latest = await query
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

            var popular = await _context.Inventories
                .Include(i => i.CreatedByUser)
                .Include(i => i.Items)
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
