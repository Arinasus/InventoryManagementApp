namespace InventoryManagementApp.Model
{
    public class Items
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public enum FieldType { }
        public bool IsHidden { get; set; }
    }
}
