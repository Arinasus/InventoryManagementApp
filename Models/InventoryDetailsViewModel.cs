using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryDetailsViewModel
    {
        public Inventory Inventory { get; set; }
        public InventoryItemsViewModel Items { get; set; }
        public InventoryDiscussionViewModel Discussion { get; set; }
        public InventorySettingsViewModel Settings { get; set; }
        public InventoryCustomIdViewModel CustomId { get; set; }
        public InventoryAccessViewModel Access { get; set; }
        public InventoryFieldsViewModel Fields { get; set; }
        public InventoryStatsViewModel Stats { get; set; }
    }
}
