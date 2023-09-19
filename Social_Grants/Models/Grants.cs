using System.ComponentModel.DataAnnotations;
namespace Social_Grants.Models
{
    public class Grants
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
