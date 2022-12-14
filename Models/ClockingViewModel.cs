//Selina
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Model for Admin Clocking System View
namespace BaristaHome.Models
{
    public class ClockingViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ShiftStatusId { get; set; }
        public string ShiftStatus { get; set; }
        public DateTime Time { get; set; }
    }
}
