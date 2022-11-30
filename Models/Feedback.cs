using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        [StringLength(64)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        // Relationships
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
