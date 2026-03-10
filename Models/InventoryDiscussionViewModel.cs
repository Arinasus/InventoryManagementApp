namespace InventoryManagementApp.Models
{
    public class InventoryDiscussionViewModel
    {
        public int InventoryId { get; set; }
        public List<DiscussionPost> Posts { get; set; }
        public string CurrentUserId { get; set; }
    }
}
