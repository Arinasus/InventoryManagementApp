using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<Inventory> Books { get; set; } = new();
        public List<InventoryItem> Items { get; set; } = new();
    }
}
