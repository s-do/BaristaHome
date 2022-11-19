using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class ChecklistViewModel
    {
        public string ChecklistTitle { get; set; }
        public Category Category { get; set; }
        public StoreTask StoreTask { get; set; }

    }
}
