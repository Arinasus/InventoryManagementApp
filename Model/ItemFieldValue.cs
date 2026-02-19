namespace InventoryManagementApp.Model
{
    public class ItemFieldValue
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public InventoryItem? Item { get; set; }
        public int FieldId { get; set; }
        public InventoryField? Field { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
