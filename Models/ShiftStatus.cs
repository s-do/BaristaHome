using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class ShiftStatus
    {
        public int ShiftStatusId { get; set; }

        [StringLength(32)]
        public string ShiftStatusName { get; set; }

        // Relationships
        public virtual ICollection<UserShiftStatus> UserShiftStatuses { get; set; }
    }
}
