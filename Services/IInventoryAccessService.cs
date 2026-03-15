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
}
