using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Grant
{
    public class Grant
    {
        [Key]
        public int Id { get; set; }
        public string GrantName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Status { get; set; }
    }
}
