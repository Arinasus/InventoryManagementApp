namespace InventoryManagementApp.Models
{
    public class InventorySettingsViewModel
    {
        public int InventoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsPublic { get; set; }
    }
}
