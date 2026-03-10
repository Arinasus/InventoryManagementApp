using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryTag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Inventory> Inventories { get; set; } = new();
    }
}
