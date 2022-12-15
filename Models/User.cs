using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(32), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, StringLength(32), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, EmailAddress, StringLength(64)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "Password must be between 8 and 24 characters and contain " +
            "one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        [NotMapped, DataType(DataType.Password), Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(7)]
        public string? Color { get; set; }

        [StringLength(5), Display(Name = "Store Invitation Code")]
        public string? InviteCode { get; set; }

        [StringLength(64)]
        public string? UserImage { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public byte[]? UserImageData { get; set; }

        public decimal? Wage { get; set; }

        [StringLength(256)]
        public string? UserDescription { get; set; }

        // Relationships
        public int? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        public int? StoreId { get; set; }
        public virtual Store? Store { get; set; }

        public ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<Payroll>? Payrolls { get; set; }
        public virtual ICollection<Shift>? Shifts { get; set; }
        public virtual ICollection<UserShiftStatus>? UserShiftStatuses { get; set; }
    }
}
