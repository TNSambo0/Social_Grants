using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class ChildSupportGrant
    {
        [Display(Name = "Dependent")]
        public int DependentId { get; set; }
        public bool? DependentsCount { get; set; }
        public IEnumerable<SelectListItem>? ListOfDependers { get; set; }
        [Required, Display(Name = "Cared by state ?")]
        public string CaredByState { get; set; }
        [Display(Name = "Application form")]
        public IFormFile ApplicationForm { get; set; }
    }
}
