using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "You forgot to specify the start time.")]
        public DateTime StartShift { get; set; }

        [Required(ErrorMessage = "Every shift should have a end time! No one works forever...")]
        public DateTime EndShift { get; set; }
        
        // Relationships
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
    }
}
