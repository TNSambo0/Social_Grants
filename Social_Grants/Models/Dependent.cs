using Social_Grants.Models.Account;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class Dependent
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("ID number")]
        [StringLength(20)]
        public string IDNumber { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }
        [Required]
        [StringLength(250)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string? IdentityDocumentUrl { get; set; }
        [Required]
        [StringLength(100)]
        public string Nationality { get; set; }
        [NotMapped]
        [Display(Name = "Identity document")]
        public IFormFile? IdentityDocument { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
