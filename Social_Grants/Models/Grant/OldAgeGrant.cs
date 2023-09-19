using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class OldAgeGrant
    {
        [Display(Name = "Recieve other grant ?")]
        public string OlderPersonGrant { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
    }
}
