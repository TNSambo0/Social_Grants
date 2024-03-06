using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Social_Grants.Models.Grant;

namespace Social_Grants.Models
{
    public class AdminDashboard
    {
        [Key]
        public int Id { get; set; }
        public IEnumerable<GrantApplications> Applications { get; set; }
        [Display(Name = "Approved Applications")]
        public int ApprovedApplications { get; set; }
        [Display(Name = "Number Of Applications")]
        public int NumberOfApplications { get; set; }
        [Display(Name = "Pending Applications")]
        public int PendingApplications { get; set; }
        [Display(Name = "Rejected Applications")]
        public int RejectedApplications { get; set; }
    }
}
