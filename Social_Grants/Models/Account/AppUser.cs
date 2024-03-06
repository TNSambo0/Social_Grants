using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models.Account
{
    public class AppUser : IdentityUser
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IDNumber { get; set; }
        [StringLength(50)]
        public string? Gender { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        [StringLength(250)]
        public string? City { get; set; }
        [StringLength(250)]
        public string? Province { get; set; }
        [StringLength(50)]
        public string? PostalCode { get; set; }
        public string? ImageUrl { get; set; }
        //[ForeignKey("AppUser")]
        //public string ChildDetailsId { get; set; }
        //public virtual ICollection<ChildDetails> ChildDetails { get; set; }
    }
}
