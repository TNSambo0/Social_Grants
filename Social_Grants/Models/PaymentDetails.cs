using Social_Grants.Models.Grant;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class PaymentDetails
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Amount { get; set; }
        [StringLength(50)]
        public string PaymentMethod { get; set; }
        [StringLength(50)]
        public string PaymentDate { get; set; }
        [NotMapped]
        [Display(Name = "Grant")]
        public string GrantCode { get; set; }
        [ForeignKey("GrantApplications")]
        public int GrantApplicationsId { get; set; }
        public GrantApplications GrantApplications { get; set; }
    }
}
