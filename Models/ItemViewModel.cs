using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class ItemViewModel
    {
        public string Name { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Only two decimals cuck")] // This ensures the property has at most 2 decimal places
        [Range(0, 99999999999999.99)]
        public decimal Quantity { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Only two decimals cuck")]
        [Range(0, 99999999999999.99)]
        public decimal PricePerUnit { get; set; }

        public string UnitName { get; set; }

    }
}
