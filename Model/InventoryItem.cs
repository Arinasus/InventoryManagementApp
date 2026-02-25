namespace InventoryManagementApp.Model
{
    public class InventoryItem // конкретный объект, элемент
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public string CustomId { get; set; } = string.Empty;

        public string CreatedByUserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int Version { get; set; }

        public List<ItemFieldValue> FieldValues { get; set; } = new(); //каждый элемент хранит значения полей
    }
}
