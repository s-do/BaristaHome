using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }

        [StringLength(512)]
        public string AnnouncementText { get; set; }

        // Relationships 
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
    }
}
