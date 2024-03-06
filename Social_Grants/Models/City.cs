﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string CityName { get; set; }
        [ForeignKey("Province")]
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}
