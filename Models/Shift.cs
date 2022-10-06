using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public DateTime StartShift { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:h:mm tt}")]
        public DateTime EndShift { get; set; }
        
        // Relationships
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
    }
}
