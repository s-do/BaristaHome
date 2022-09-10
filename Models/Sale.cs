using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Sale
    {
        // TODO: FIND A WAY TO FUCKING MAKE A CANDIDATE KEY A FOREIGN KEY TO CORRENTLY LINK SALE AND INVENTORYITEM
        public int SaleId { get; set; }
        public decimal UnitsSold { get; set; }
        public decimal Profit { get; set; }
        public DateTime TimeSold { get; set; }

        // Relationships
        [ForeignKey("InventoryItem"), Column(Order = 0)]
        public int ItemId { get; set; }
        [ForeignKey("InventoryItem"), Column(Order = 1)]
        public int StoreId { get; set; }
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
    }
}
