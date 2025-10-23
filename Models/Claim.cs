using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMCS.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required]
        public string LecturerId { get; set; } // or use int if you have users table

        [Required]
        [Range(0.1, 1000)]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0.1, 10000)]
        public decimal HourlyRate { get; set; }

        public decimal Total => HoursWorked * HourlyRate;

        public string Notes { get; set; }

        public string UploadedFileName { get; set; } // stored filename
        public string OriginalFileName { get; set; } // for display

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}