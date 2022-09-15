using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Unit
    {
        public int UnitId { get; set; }

        [StringLength(16)]
        public string UnitName { get; set; }

        // Relationships
        public ICollection<Item>? Item { get; set; }
    }
}
