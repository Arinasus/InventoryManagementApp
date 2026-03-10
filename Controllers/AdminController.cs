using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            // параметры сортировки для ссылок в таблице
            ViewData["EmailSort"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";
            ViewData["RoleSort"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";
            ViewData["StatusSort"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";
            ViewData["CreatedSort"] = sortOrder == "created_asc" ? "created_desc" : "created_asc";
            ViewData["InvSort"] = sortOrder == "inv_asc" ? "inv_desc" : "inv_asc";
            ViewData["PostsSort"] = sortOrder == "posts_asc" ? "posts_desc" : "posts_asc";

            var users = await _userManager.Users
                .Include(u => u.OwnedInventories)
                .Include(u => u.Posts)
                .ToListAsync();

            var model = users.Select(u => new UserManagementViewModel
            {
                Id = u.Id,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                IsBlocked = u.IsBlocked,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                InventoriesCount = u.OwnedInventories.Count,
                PostsCount = u.Posts.Count
            });

            // сортировка
            model = sortOrder switch
            {
                "email_asc" => model.OrderBy(u => u.Email),
                "email_desc" => model.OrderByDescending(u => u.Email),

                "role_asc" => model.OrderBy(u => u.IsAdmin),
                "role_desc" => model.OrderByDescending(u => u.IsAdmin),

                "status_asc" => model.OrderBy(u => u.IsBlocked),
                "status_desc" => model.OrderByDescending(u => u.IsBlocked),

                "created_asc" => model.OrderBy(u => u.CreatedAt),
                "created_desc" => model.OrderByDescending(u => u.CreatedAt),

                "inv_asc" => model.OrderBy(u => u.InventoriesCount),
                "inv_desc" => model.OrderByDescending(u => u.InventoriesCount),

                "posts_asc" => model.OrderBy(u => u.PostsCount),
                "posts_desc" => model.OrderByDescending(u => u.PostsCount),

                _ => model.OrderBy(u => u.Email) // сортировка по умолчанию
            };

            return View(model.ToList());
        }

        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("Index"); 
        }
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsBlocked = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsBlocked = false;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
