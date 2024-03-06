using System.ComponentModel.DataAnnotations;

namespace Social_Grants.Models
{
    public class Province
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string ProvinceName { get; set; }

    }

}
