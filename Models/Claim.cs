                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        public int LecturerID { get; set; }

        [Display(Name = "Total Hours")]
        public double TotalHours { get; set; }

        [Display(Name = "Hourly Rate")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount => (decimal)TotalHours * HourlyRate;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Status is stored as a string to match existing migrations/views
        [Required]
        public string Status { get; set; } = ClaimStatus.Submitted;

        // Optional rejection reason for auditing/feedback
        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Navigation properties
        public Lecturer? Lecturer { get; set; }
        public ICollection<Document>? Documents { get; set; }
    }
}