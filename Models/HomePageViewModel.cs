namespace InventoryManagementApp.Models
{
    public class HomePageViewModel
    {
        public List<InventoryCardViewModel> LatestInventories { get; set; } = new();
        public List<InventoryCardViewModel> PopularInventories { get; set; } = new();
        public List<TagViewModel> Tags { get; set; } = new();
        public string SearchQuery { get; set; } = string.Empty;
    }
}
