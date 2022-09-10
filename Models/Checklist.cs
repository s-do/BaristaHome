using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Checklist
    {
        public int ChecklistId { get; set; }

        [StringLength(32)]
        public string ChecklistTitle { get; set; }

        // Relationships 
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
    }
}
