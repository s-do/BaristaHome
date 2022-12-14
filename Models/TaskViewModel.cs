using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class TaskViewModel
    {
        public int CategoryId { get; set; }
        public int StoreTaskId { get; set; }
        public string TaskName { get; set; }
        public bool IsFinished { get; set; }
    }
}
