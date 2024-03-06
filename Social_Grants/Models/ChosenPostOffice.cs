using Microsoft.AspNetCore.Mvc.Rendering;
using Social_Grants.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class ChosenPostOffice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PostOffice")]
        [Display(Name = "Site")]
        public int? PostOfficeId { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? PostOffices { get; set; }
        [Display(Name = "Area")]
        public int? CityId { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Cities { get; set; }
        [Display(Name = "Province")]
        public int? ProvinceId { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Provinces { get; set; }
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public PostOffice? PostOffice { get; set; }
    }
}
