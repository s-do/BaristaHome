using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class InventoryItem
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }

        // Relationships
        public virtual Sale? Sale { get; set; }

        // Always set virtual properties when referencing navigation properties to utilize lazy loading (loading data when it's actually used)
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }


    }
}
