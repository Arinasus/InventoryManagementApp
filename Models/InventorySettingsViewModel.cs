namespace InventoryManagementApp.Models
{
    public class InventorySettingsViewModel
    {
        public int InventoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = "";
        public string Category { get; set; }

        public bool IsPublic { get; set; }
        public byte[] RowVersion { get; set; }
        public List<string> AvailableCategories { get; set; }
        public List<string> Tags { get; set; } = new();
        public string? ApiToken { get; set; }

    }
}
