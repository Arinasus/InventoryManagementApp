using InventoryManagementApp.Data;
using InventoryManagementApp.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryManagementApp.Services
{
    public interface IInventoryAccessService
    {
        Task<bool> IsOwner(int inventoryId, ClaimsPrincipal user);
        Task<bool> IsAdmin(ClaimsPrincipal user);
        Task<bool> HasWriteAccess(int inventoryId, ClaimsPrincipal user);
        Task<bool> CanEditInventory(int inventoryId, ClaimsPrincipal user);
        Task<bool> CanEditItems(int inventoryId, ClaimsPrincipal user);
    }

    public class InventoryAccessService : IInventoryAccessService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InventoryAccessService(AppDbContext ctx, UserManager<ApplicationUser> um)
        {
            _context = ctx;
            _userManager = um;
        }

        public async Task<bool> IsAdmin(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u != null && await _userManager.IsInRoleAsync(u, "Admin");
        }

        public async Task<bool> IsOwner(int inventoryId, ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            if (u == null) return false;

            var inv = await _context.Inventories.FindAsync(inventoryId);
            return inv != null && inv.CreatedByUserId == u.Id;
        }

        public async Task<bool> HasWriteAccess(int inventoryId, ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            if (u == null) return false;

            if (await IsAdmin(user)) return true;
            if (await IsOwner(inventoryId, user)) return true;

            var inv = await _context.Inventories.FindAsync(inventoryId);
            if (inv == null) return false;

            if (inv.IsPublic) return true;

            return await _context.InventoryAccesses
                .AnyAsync(a => a.InventoryId == inventoryId && a.UserId == u.Id && a.CanWrite);
        }

        public async Task<bool> CanEditInventory(int inventoryId, ClaimsPrincipal user)
        {
            return await IsOwner(inventoryId, user) || await IsAdmin(user);
        }

        public async Task<bool> CanEditItems(int inventoryId, ClaimsPrincipal user)
        {
            return await HasWriteAccess(inventoryId, user);
        }
    }

}
