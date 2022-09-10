using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        [StringLength(32)]
        public string RoleName { get; set; }

        // Relationships
        public virtual ICollection<User> Users { get; set; }
    }
}
