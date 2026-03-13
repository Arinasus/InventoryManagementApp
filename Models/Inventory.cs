using InventoryManagementApp.Models;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
namespace InventoryManagementApp.Model
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsPublic { get; set; } 
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt  { get; set; }
        public DateTime UpdatedAt  { get; set; }
        public int Version { get; set; } //номер версии для автосохранения
        public List<InventoryField> Fields { get; set; } = new();// список кастомных полей
        public List<InventoryItem> Items { get; set; } = new();//список элементов
        public ApplicationUser? CreatedByUser { get; set; }
        public List<InventoryAccess> InventoryAccesses { get; set; } = new();
        public List<InventoryTag> Tags { get; set; } = new();
        public string CustomIdPrefix { get; set; } = "";
        public int NextCustomIdNumber { get; set; } = 1;
        public List<InventoryCustomIdPart> CustomIdParts { get; set; }
        public NpgsqlTsVector? SearchVector { get; set; } = null!;
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    }
}
