using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? Role { get; set; } // e.g., "AcademicManager", "HR", "ProgrammeCoordinator"
    }
}