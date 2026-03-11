using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryCustomIdPart
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public int Order { get; set; }

        public CustomIdPartType Type { get; set; }

        public string? Value { get; set; }
    }
}
