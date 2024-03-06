using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Grant
{
    public class SocialReliefDistressGrant
    {
        [Display(Name = "Employment status")]
        public string EmploymentStatus { get; set; }
        [Display(Name = "Recieve older person grant ?")]
        public string OlderPersonGrant { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
    }
}
