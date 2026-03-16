using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string q, string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var booksByTag = await _context.Inventories
                    .Include(i => i.Tags)
                    .Where(i => i.Tags.Any(t => t.Name == tag))
                    .ToListAsync();

                return View(new SearchResultsViewModel
                {
                    Query = tag,
                    Books = booksByTag,
                    Items = new List<InventoryItem>()
                });
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim()
                     .Replace("…", "")  
                     .Replace("...", "") 
                     .Replace(".", "");  

                if (q.StartsWith("Поиск", StringComparison.OrdinalIgnoreCase))
                    q = null;
            }

            if (string.IsNullOrWhiteSpace(q))
                return View(new SearchResultsViewModel());

            var books = await _context.Inventories
                .Where(i => EF.Functions.ToTsVector("russian", i.Title + " " + i.Description)
                    .Matches(EF.Functions.PlainToTsQuery("russian", q)))
                .ToListAsync();
            var items = await _context.InventoryItems
                .Include(i => i.FieldValues)
                .Where(i => EF.Functions.ToTsVector("russian", i.CustomId)
                    .Matches(EF.Functions.PlainToTsQuery("russian", q)))
                .ToListAsync();

            return View(new SearchResultsViewModel
            {
                Query = q,
                Books = books,
                Items = items
            });
        }
    }
}
