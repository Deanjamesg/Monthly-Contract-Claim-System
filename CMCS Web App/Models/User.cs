using System.ComponentModel.DataAnnotations;

namespace CMCS_Web_App.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string? Faculty { get; set; } // Nullable Attribute, by appending the "?" to the data type.
        public string Password { get; set; }
        public string Role { get; set; } // Role is a string, as it will be used to store the role of the user.

        public ICollection<Claim>? Claims { get; set; }
    }
}
