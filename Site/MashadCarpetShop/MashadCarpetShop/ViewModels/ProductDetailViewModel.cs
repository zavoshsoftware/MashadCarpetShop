using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductDetailViewModel : _BaseViewModel
    {
        public ProductSize ProductSize { get; set; }
        public List<ProductColorViewModel> ProductColors { get; set; }
        public List<ProductSizeViewModel> ProductSizes { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductComment> ProductComments { get; set; }
        public List<ProductCardViewModel> RelatedProducts { get; set; }

    }

    public class ProductSizeViewModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
    }
     
    public class ProductColorViewModel
    {
        public Guid ColorId { get; set; }
        public string HexCode { get; set; }
        public int Code { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
    }
     
}