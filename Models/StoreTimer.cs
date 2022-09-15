using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class StoreTimer
    {
        public int StoreTimerId { get; set; }

        [StringLength(32)]
        public string TimerName { get; set; }

        // Probably use the Timer class which times based on milliseconds
        public int DurationMin { get; set; }

        // Relationships
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
    }
}
