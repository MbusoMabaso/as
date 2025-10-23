using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }
        [Required]
        public int ClaimID { get; set; }
        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "File Path")]
        public string FilePath { get; set; } = string.Empty;
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        [Display(Name = "File Size")]
        public long FileSize { get; set; }
        [Display(Name = "File Type")]
        public string FileType { get; set; } = string.Empty;
        public Claim? Claim { get; set; }
    }
}