using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class GrantQuestions
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        [ForeignKey("Grants")]
        public int GrantId { get; set; }
        public Grants Grants { get; set; }
    }
}
