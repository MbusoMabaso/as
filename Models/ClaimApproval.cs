using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ClaimApproval
    {
        [Key]
        public int ApprovalID { get; set; }
        [Required]
        public int ClaimID { get; set; }
        [Required]
        [Display(Name = "Approval Date")]
        public DateTime ApprovalDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }
        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
        [Display(Name = "Approval Level")]
        public string ApprovalLevel { get; set; } = "ProgrammeCoordinator"; // ProgrammeCoordinator or AcademicManager
        public Claim? Claim { get; set; }
    }
}