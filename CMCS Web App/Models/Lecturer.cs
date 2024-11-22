using System.ComponentModel.DataAnnotations;

namespace CMCS_Web_App.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
        public string Password { get; set; }

        public ICollection<UserClaim> Claims { get; set; }
    }
}
