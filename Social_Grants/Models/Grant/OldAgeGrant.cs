using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Grant
{
    public class OldAgeGrant
    {
        [Display(Name = "Recieve other grant ?")]
        public string OtherGrant { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
    }
}
