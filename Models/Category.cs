using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [StringLength(32)]
        public string CategoryName { get; set; }

        // Relationships
        public int ChecklistId { get; set; }
        public virtual Checklist Checklist { get; set; }

        public virtual ICollection<CategoryTask> CategoryTasks { get; set; }

    }
}
