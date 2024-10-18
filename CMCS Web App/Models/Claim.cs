using System.ComponentModel.DataAnnotations;

namespace CMCS_Web_App.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public int LecturerId { get; set; }
        public double ClaimAmount { get; set; }
        public string ClaimStatus { get; set; }
        public double HoursWorked { get; set; }
        public string FileName { get; set; }
    }
}
