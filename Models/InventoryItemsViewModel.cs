using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryItemsViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;

        public List<InventoryItemRowViewModel> Items { get; set; } = new();
        public List<InventoryField> TableFields { get; set; } = new(); // поля, которые показываем в таблице
    }
}
