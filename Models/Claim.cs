using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMCS.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }
        [Required]
        [Display(Name = "Lecturer ID")]
        public int LecturerID { get; set; }
        [Required]
        [Display(Name = "Date Submitted")]
        [DataType(DataType.Date)]
        public DateTime DateSubmitted { get; set; }
        [Required]
        [Display(Name = "Total Hours")]
        [Range(0.1, 200, ErrorMessage = "Hours must be between 0.1 and 200")]
        public double TotalHours { get; set; }
        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(1, 1000, ErrorMessage = "Hourly rate must be between R1 and R1000")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }
        [Display(Name = "Additional Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
        [Required]
        [Display(Name = "Status")]
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
        [Display(Name = "Total Amount")]
        public decimal TotalAmount => (decimal)TotalHours * HourlyRate;
        public Lecturer? Lecturer { get; set; }
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}