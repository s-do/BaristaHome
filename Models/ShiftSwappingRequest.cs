using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaristaHome.Models
{
    [Table("ShiftSwappingRequest")]
    public class ShiftSwappingRequest
    {
        
        [Column("RequestId")]
        [Key]
        public int RequestId { get; set; }

        [ForeignKey("SenderUser")]
        public int SenderUserId { get; set; }

        [ForeignKey("RecipientUser")]
        public int RecipientUserId { get; set; }

        [ForeignKey("SenderShift")]
        public int SenderShiftId { get; set; }

        [ForeignKey("RecipientShift")]
        public int RecipientShiftId { get; set; }
        
        [Column("Response")]
        public bool? Response { get; set; }

        public User SenderUser { get; set; }
        public User RecipientUser { get; set; }

        public Shift SenderShift { get; set; }
        public Shift RecipientShift { get; set; }
    }
}
