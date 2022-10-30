using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int UnitsSold { get; set; }
        public decimal Profit { get; set; }
        public DateTime TimeSold { get; set; }

        // Relationships
        public int? StoreId { get; set; }
        public virtual Store? Store { get; set; }

        public int? DrinkId { get; set; }
        public virtual Drink? Drink { get; set; }
    }
}
