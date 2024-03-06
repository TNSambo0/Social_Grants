using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class GrantAnswers
    {
        [Key]
        public int Id { get; set; }
        public string Answer { get; set; }
        public string Reason { get; set; }
        [ForeignKey("Grants")]
        public int GrantId { get; set; }
        public Grants Grants { get; set; }
        [ForeignKey("GrantQuestions")]
        public int QuestionId { get; set; }
        public GrantQuestions GrantQuestions { get; set; }
    }
}
