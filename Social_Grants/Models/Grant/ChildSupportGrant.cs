using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class ChildSupportGrant
    {
        public Dependent Dependent { get; set; }
        [Required, Display(Name = "Cared by state ?")]
        public string CaredByState { get; set; }
        [Display(Name = "Application form")]
        public IFormFile ApplicationForm { get; set; }
    }
}
