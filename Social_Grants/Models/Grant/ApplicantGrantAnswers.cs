using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models.Grant
{
    public class ApplicantGrantAnswers
    {
        [Key]
        public int Id { get; set; }
        public string? Answer { get; set; }
        [ForeignKey("GrantQuestions")]
        public int QuestionId { get; set; }
        public GrantQuestions GrantQuestions { get; set; }
        [ForeignKey("Grants")]
        public int GrantId { get; set; }
        public Grants Grants { get; set; }
        [ForeignKey("GrantApplications")]
        public int GrantApplicationId { get; set; }
        public GrantApplications GrantApplications { get; set; }
        [ForeignKey("Dependent")]
        public int? DependentId { get; set; }
        public Dependent? Dependent { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
