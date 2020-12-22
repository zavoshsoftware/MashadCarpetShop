using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductSize:BaseEntity
    {
        public ProductSize()
        {
            OrderDetails=new List<OrderDetail>();

        }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid SizeId { get; set; }
        public virtual Size Size { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }


        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Product))]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "IsInPromotion", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "موجودی")]
        public int Stock { get; set; }

        [Display(Name = "موجودی اولیه")]
        public int SeedStock { get; set; }

        [Display(Name = "تعداد فروش")]
        public int SellNumber { get; set; }

        [Display(Name = "موجود است؟")]
        public bool IsAvailable { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        #region NotMapped

        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        public string AmountStr
        {
            get { return Amount.ToString("n0") + " تومان"; }
        }

        [NotMapped]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount != null)
                    return DiscountAmount.Value.ToString("n0") + " تومان";

                return string.Empty;
            }
        }


        #endregion
    }
}