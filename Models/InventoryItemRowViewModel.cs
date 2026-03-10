namespace InventoryManagementApp.Models
{
    public class InventoryItemRowViewModel
    {
        public int Id { get; set; }
        public string CustomId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }

        public Dictionary<string, string> FieldValues { get; set; } = new(); // Title -> Value
    }
}
