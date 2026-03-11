namespace InventoryManagementApp.Model
{
    public class DiscussionPost
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ContentMarkdown { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
