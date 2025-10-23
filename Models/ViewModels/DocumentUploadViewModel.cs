using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }
}
