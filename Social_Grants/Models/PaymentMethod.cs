using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Social_Grants.Models
{
    public class PaymentMethod
    {
        [Display(Name = "Payment")]
        public int SelectedPaymentMethodId { get; set; }
        public ChosenPostOffice? ChosenPostOffice { get; set; }
        public BankDetails? BankDetails { get; set; }
        public IEnumerable<SelectListItem>? PaymentMethods { get; set; }
    }
}
