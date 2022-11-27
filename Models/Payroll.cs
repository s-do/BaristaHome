using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Payroll
    {
        public int PayrollId { get; set; }
        public decimal Hours { get; set; }
        public decimal Amount { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        // Relationships
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
