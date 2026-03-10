using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class AddItemViewModel
    {
        public int InventoryId { get; set; }

        // Список полей инвентаря
        public List<InventoryField> Fields { get; set; }

        // Значения, которые придут из формы: key = FieldId, value = строковое значение
        public Dictionary<int, string> Values { get; set; } = new();
    }
}
