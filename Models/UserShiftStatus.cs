using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class UserShiftStatus
    {
        public int UserId { get; set; }
        public int ShiftStatusId { get; set; }
        public DateTime Time { get; set; }

        // Relationships
        public virtual User User { get; set; }
        public virtual ShiftStatus ShiftStatus { get; set; }
    }
}
