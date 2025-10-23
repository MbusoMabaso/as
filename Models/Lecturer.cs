using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}