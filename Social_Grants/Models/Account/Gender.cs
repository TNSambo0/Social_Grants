using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Account
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string GenderName { get; set; }
    }
}
