//Selina
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Model for Checklist View
namespace BaristaHome.Models
{
    public class ChecklistViewModel
    {
        public int ChecklistId { get; set; }
        public string ChecklistTitle { get; set; }
        public Dictionary<Category, List<StoreTask>> CategoryTasks { get; set; }

    }
}
