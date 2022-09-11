using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public decimal UnitsSold { get; set; }
        public decimal Profit { get; set; }
        public DateTime TimeSold { get; set; }

        // Relationships
        public virtual InventoryItem InventoryItem { get; set; }
    }
}
