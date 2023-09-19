using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Social_Grants.Models.Account;

namespace Social_Grants.Models.Grant
{
    public class GrantApplications
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "ID number")]
        public string IDNumber { get; set; }
        [Display(Name = "Application Id")]
        public int ApplicationId { get; set; }
        [NotMapped]
        [Display(Name = "Grant")]
        public string? GrantName { get; set; }
        [Display(Name = "Application status")]
        public string ApplicationStatus { get; set; }
        public string? Reason { get; set; }
        [Display(Name = "Date created")]
        public DateTime? DateCreated { get; set; }
        [Display(Name = "Pay date")]
        public DateTime? PayDate { get; set; }
        [Display(Name = "Payment method")]
        public string? MethodOfPayment { get; set; }
        [ForeignKey("Grants")]
        public int GrantId { get; set; }
        public Grants Grants { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
