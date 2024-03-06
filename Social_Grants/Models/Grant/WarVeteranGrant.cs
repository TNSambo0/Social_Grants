using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Grant
{
    public class WarVeteranGrant
    {
        [Display(Name = "Fought in the Second World War or Korean War ?")]
        public string SecondWar { get; set; }
        [Display(Name = "Recieve other grant ?")]
        public string OlderPersonGrant { get; set; }
        [Display(Name = "Cared by State ?")]
        public string CaredByState { get; set; }
    }
}
