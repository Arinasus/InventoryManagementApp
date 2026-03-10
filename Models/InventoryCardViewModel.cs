namespace InventoryManagementApp.Models
{
    public class InventoryCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
