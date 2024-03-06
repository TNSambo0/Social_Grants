using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Account
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
