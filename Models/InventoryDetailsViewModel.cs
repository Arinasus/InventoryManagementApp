using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryDetailsViewModel
    {
        public Inventory Inventory { get; set; }
        public InventoryItemsViewModel Items { get; set; }
    }
}
