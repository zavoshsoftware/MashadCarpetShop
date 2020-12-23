using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel:_BaseViewModel
    {
        public string ProductGroupTitle { get; set; }
        public string ProductGroupUrlParam { get; set; }
        public List<ProductCardViewModel> Products { get; set; }
        public List<_PageItem> PageItems { get; set; }

    }

    public class ProductCardViewModel
    {
        public Product Product { get; set; }
        public List<string> Sizes { get; set; }
    }
}