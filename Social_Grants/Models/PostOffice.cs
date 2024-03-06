using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class PostOffice
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
