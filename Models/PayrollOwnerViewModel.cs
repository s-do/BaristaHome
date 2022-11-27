using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class PayrollOwnerViewModel
    {
/*        public int PayrollId { get; set; }*/
        public decimal Hours { get; set; }
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [DisplayName("Full Name")]
        public string? FullName { get; set; }

        public int? PayrollId { get; set; }

    }





}



/*using System.ComponentModel.DataAnnotations.Schema;

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
}*/