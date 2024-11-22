using System.ComponentModel.DataAnnotations;

namespace CMCS_Web_App.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public int UserId { get; set; }
        public double ClaimAmount { get; set; }
        public double HourlyRate { get; set; }
        public string ClaimStatus { get; set; } // Pending, Approved, Rejected, Paid
        public double HoursWorked { get; set; }
        public bool FlaggedClaim { get; set; }
        public string? FileName { get; set; } // Making FileName and FileData nullable.
        public byte[]? FileData { get; set; }

        public User User { get; set; }
    }
}
