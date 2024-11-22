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
        // Nullable Attribute, by appending the "?" to the data type.
        public string? Faculty { get; set; }
        public string Password { get; set; }
        // Role is a string, as it will be used to store the role of the user. Might make this an enum.
        public string Role { get; set; }
    }
}
