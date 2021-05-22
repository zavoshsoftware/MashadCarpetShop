using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class HomeViewModel:_BaseViewModel
    {
        public List<ProductCardViewModel> TopProducts { get; set; }
        public List<ProductCardViewModel> NewestProducts { get; set; }
        public List<ProductGroup> ProductGroups { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Slider> Sliders { get; set; }
    }

}