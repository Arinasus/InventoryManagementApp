namespace InventoryManagementApp.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public List<InventoryCardViewModel> OwnedInventories { get; set; } = new();
        public List<InventoryCardViewModel> AccessibleInventories { get; set; } = new();

        public string SearchQuery { get; set; } = string.Empty;
        public string SortOrder { get; set; } = string.Empty;
    }
}
