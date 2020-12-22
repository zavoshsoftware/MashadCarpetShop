using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Color:BaseEntity
    {
        public Color()
        {
            Products=new List<Product>();
        }
        [Display(Name="رنگ")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Title { get; set; }

        [Display(Name="رنگ انگلیسی")]
        public string TitleEn { get; set; }
         
        [Display(Name="کد Hex")]
        [StringLength(10, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string HexCode { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}