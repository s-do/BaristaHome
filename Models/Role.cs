namespace BaristaHome.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // Relationships
        public ICollection<User> Users { get; set; }
    }
}
