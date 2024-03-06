﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social_Grants.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Contact name")]
        [DataType(DataType.Text)]
        public string ContactName { get; set; }
        [StringLength(50)]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(250)]
        [DataType(DataType.Text)]
        public string Subject { get; set; }
        [DataType(DataType.Text)]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
    }
}
