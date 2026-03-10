using Microsoft.AspNetCore.Identity;

namespace InventoryManagementApp.Model
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockedByUser { get; set; }
        public string PrefferedLanguage { get; set; } ="en";
        public string PrefferendTheme { get; set; } = "light";
        public List<Inventory> OwnedInventories { get; set; } = new();
        public List<InventoryAccess> InventoryAccesses { get; set; } = new();
        public List<ItemLike> LikedItems { get; set; } = new();
        public List<DiscussionPost> Posts { get; set; } = new();
        public DateTime? LastLogin { get; internal set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
