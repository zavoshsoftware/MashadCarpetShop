using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class CartViewModel : _BaseViewModel
    {
        public List<ProductInCart> Products { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string DiscountAmount { get; set; }
        public TextItem Policy { get; set; }
        public string ShipmentAmount { get; set; }

    }

    public class ProductInCart
    {
        public ProductSize ProductSize { get; set; }
        public int Quantity { get; set; }
        public string RowAmount
        {
            get
            {
                if (ProductSize.IsInPromotion && ProductSize.DiscountAmount != null)
                    return (ProductSize.DiscountAmount.Value * Quantity).ToString("n0") + " تومان";

                return (ProductSize.Amount * Quantity).ToString("n0") + " تومان";
            }
        }

   public string  Amount
        {
            get
            {
                if (ProductSize.IsInPromotion && ProductSize.DiscountAmount != null)
                    return (ProductSize.DiscountAmount.Value).ToString("n0") + " تومان";

                return (ProductSize.Amount).ToString("n0") + " تومان";
            }
        }


    }
}