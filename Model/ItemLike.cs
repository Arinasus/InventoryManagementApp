namespace InventoryManagementApp.Model
{
    public class ItemLike
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public InventoryItem Item { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
