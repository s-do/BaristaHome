using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class StoreTask
    {
        public int StoreTaskId { get; set; }

        [Required(ErrorMessage = "You can't add a blank name."), StringLength(64)]
        public string TaskName { get; set; }

        // Relationships
        public virtual ICollection<CategoryTask> CategoryTasks { get; set; }

    }
}
