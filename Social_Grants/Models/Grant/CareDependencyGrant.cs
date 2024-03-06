using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class CareDependencyGrant
    {
        [Display(Name = "Dependent")]
        public int DependentId { get; set; }
        public IEnumerable<SelectListItem>? ListOfDependers { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
        public bool? DependentsCount { get; set; }
        [Display(Name = "Medical assessment report")]
        public IFormFile MedicalReport { get; set; }
        [Display(Name = "Application form")]
        public IFormFile ApplicationForm { get; set; }
    }
}
