using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class DisabilityGrant
    {
        [Display(Name = "Dependent")]
        public int DependentId { get; set; }
        public bool? DependentsCount { get; set; }
        public IEnumerable<SelectListItem>? ListOfDependers { get; set; }
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
