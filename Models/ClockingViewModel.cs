using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class ClockingViewModel
    {
        public string User { get; set; }
        public string ShiftStatus { get; set; }
        public DateTime Time { get; set; }
    }
}
