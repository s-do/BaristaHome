using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public TimeSpan StartShift { get; set; }
        public TimeSpan EndShift { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ShiftDate { get; set; }

        // Relationships
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
    }
}
