using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class BankDetails
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Bank name")]
        public string? BankName { get; set; }
        [Display(Name = "Account holder's name")]
        public string? BankAccountHolder { get; set; }
        [StringLength(10)]
        [Display(Name = "Account number")]
        public string? AccountNumber { get; set; }
        [Display(Name = "Branch code")]
        public string? BranchCode { get; set; }
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
