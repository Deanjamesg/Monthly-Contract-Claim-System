using System.ComponentModel.DataAnnotations;

namespace CMCS_Web_App.Models
{
    public class ProgrammeCoordinator
    {
        [Key]
        public int ProgrammeCoordinatorId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
        public string Password { get; set; }
    }
}
