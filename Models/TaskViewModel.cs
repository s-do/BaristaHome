using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class TaskViewModel
    {
        public int StoreTaskId { get; set; }
        public string TaskName { get; set; }
        public bool IsFinished { get; set; }
    }
}
