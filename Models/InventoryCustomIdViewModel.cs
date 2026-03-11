namespace InventoryManagementApp.Models
{
    public class InventoryCustomIdViewModel
    {
        public int InventoryId { get; set; }
        public List<InventoryCustomIdPart> Parts { get; set; }
        public string Preview { get; set; }
    }
}
