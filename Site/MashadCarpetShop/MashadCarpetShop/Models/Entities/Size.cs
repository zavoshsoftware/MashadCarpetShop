using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Size:BaseEntity
    {
        public Size()
        {
            ProductSizes=new List<ProductSize>();
        }
        [Display(Name="اندازه")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(30, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }
         
        public virtual ICollection<ProductSize> ProductSizes { get; set; }


    }
}