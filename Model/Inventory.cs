namespace InventoryManagementApp.Model
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsPublic { get; set; } 
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt  { get; set; }
        public DateTime UpdatedAt  { get; set; }
        public int Version { get; set; }
        public List<InventoryItem> Items { get; set; }
    }
}
