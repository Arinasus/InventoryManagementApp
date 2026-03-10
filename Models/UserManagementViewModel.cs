namespace InventoryManagementApp.Models
{
    public class UserManagementViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int InventoriesCount { get; set; }
        public int PostsCount { get; set; }
    }
}
