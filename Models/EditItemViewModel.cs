using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class EditItemViewModel
    {
        public int ItemId { get; set; }
        public int InventoryId { get; set; }
        public int Version { get; set; }
        public string CustomId { get; set; } = string.Empty;
        public List<InventoryField> Fields { get; set; }
        public Dictionary<int, string> Values { get; set; } = new();
    }
}
