using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Branch:BaseEntity
    {
        [Display(Name ="عنوان")]
        public string  Title { get; set; }
        [Display(Name ="تلفن")]
        public string Phone { get; set; }
        [Display(Name ="شهر")]
        public string City { get; set; }
        [Display(Name ="آدرس")]
        public string Address { get; set; }
    }
}