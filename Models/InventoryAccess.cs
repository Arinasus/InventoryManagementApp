namespace InventoryManagementApp.Model
{
    public class InventoryAccess
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set;  }
        public DateTime GrantedAt { get; set; }
        public bool CanWrite { get; set; } // доступ на запись

    }
}
