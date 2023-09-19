using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Social_Grants.Models
{
    public class ChosenPaymentMethod
    {
        [Key]
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
