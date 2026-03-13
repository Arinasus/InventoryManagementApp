namespace InventoryManagementApp.DTOs
{
    public class InventoryAutoSaveDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public string RowVersion { get; set; }

    }
}
