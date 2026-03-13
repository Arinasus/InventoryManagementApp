using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class AddItemViewModel
    {
        public int InventoryId { get; set; }
        public List<InventoryField> Fields { get; set; }

        public Dictionary<int, string> Values { get; set; } = new();
    }
}
