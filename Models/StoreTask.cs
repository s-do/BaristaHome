using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class StoreTask
    {
        public int StoreTaskId { get; set; }

        [StringLength(32)]
        public string TaskName { get; set; }

        // Relationships
        public virtual ICollection<CategoryTask> CategoryTasks { get; set; }

    }
}
