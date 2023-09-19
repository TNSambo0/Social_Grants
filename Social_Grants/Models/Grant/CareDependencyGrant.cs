using Microsoft.AspNetCore.Mvc.Rendering;
using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class CareDependencyGrant
    {
        public Dependent Dependent { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
        [Display(Name = "Medical assessment report")]
        public IFormFile MedicalReport { get; set; }
        [Display(Name = "Application form")]
        public IFormFile ApplicationForm { get; set; }
    }
}
