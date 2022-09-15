using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        [StringLength(32)]
        public string ItemName { get; set; }

        // Relationships
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public virtual ICollection<InventoryItem>? InventoryItems { get; set; }
    }
}
