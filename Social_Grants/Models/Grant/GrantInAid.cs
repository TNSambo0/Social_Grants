using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Grant
{
    public class GrantInAid
    {
        [Display(Name = "Do you recieve other such as older person grant, disability grant or war veteran grant ?")]
        public string OtherGrant { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
        [Display(Name = "Do you require full-time attendance by another person ?")]

        public string Assistance { get; set; }
    }
}
