using System.ComponentModel.DataAnnotations;
namespace Social_Grants.Models
{
    public class ApplyForWho
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Answer { get; set; }
    }
}
