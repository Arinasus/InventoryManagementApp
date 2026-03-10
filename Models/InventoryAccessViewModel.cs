using InventoryManagementApp.Model;

namespace InventoryManagementApp.Models
{
    public class InventoryAccessViewModel
    {
        public int InventoryId { get; set; }
        public List<InventoryAccess> AccessList { get; set; }
    }
}
