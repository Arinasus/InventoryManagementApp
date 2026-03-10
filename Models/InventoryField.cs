using InventoryManagementApp.Models;

namespace InventoryManagementApp.Model
{
    public class InventoryField //кастомные поля Каждый инвентарь может иметь до 3 текстовых, 3 числовых, 3 булевых и т.д.
    {
        public int Id { get; set; }
        public int InventoryId { get; set; } // к какому инвентарю относится
        public Inventory Inventory { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FieldType Type { get; set; }
        public bool ShowInTable { get; set; } //показывать ли в таблице
        public int Order {  get; set; }// порядок
    }
}
