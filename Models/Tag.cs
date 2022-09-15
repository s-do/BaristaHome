using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        [StringLength(32)]
        public string TagName { get; set; }

        // Relationships
        public virtual ICollection<DrinkTag>? DrinkTags { get; set; }
    }
}
