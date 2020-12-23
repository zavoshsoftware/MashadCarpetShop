using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductImage : BaseEntity
    { 
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Display(Name="اولویت")]
        public int Order { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }
    }
}