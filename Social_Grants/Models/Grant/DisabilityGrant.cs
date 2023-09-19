using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Social_Grants.Models.Grant
{
    public class DisabilityGrant
    {
        public Dependent Dependent { get; set; }
        [Required, Display(Name = "Cared by state ?")]
        public string CaredByState { get; set; }
        [Required, Display(Name = "Recieve another grant")]
        public string OtherGrant { get; set; }
        [Display(Name = "Applying For who? ")]
        public int ForWhoId { get; set; }
        public IEnumerable<SelectListItem>? ForWho { get; set; }
        [Display(Name = "Medical assessment report")]
        public IFormFile MedicalReport { get; set; }
        [Display(Name = "Application form")]
        public IFormFile ApplicationForm { get; set; }
    }
}
