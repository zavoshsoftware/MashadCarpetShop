using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
    public class Product : BaseEntity
    {
        public Product()
        {
            ProductSizes = new List<ProductSize>();
            ProductComments = new List<ProductComment>();
            Products = new List<Product>();
            ProductImages = new List<ProductImage>();
        }
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Resources.Models.Product))]
        public int Code { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }


        [Display(Name = "عنوان انگلیسی")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string TitleEn { get; set; }

        [Display(Name = "گروه محصول")]
        public Guid ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }

        [Display(Name = "رنگ محصول")]
        public Guid ColorId { get; set; }
        public virtual Color Color { get; set; }

        public Guid? ParentId { get; set; }
        public virtual Product Parent { get; set; }

        [Display(Name = "کد نقشه")]
        public int DesignNo { get; set; }

        [Display(Name = "شانه")]
        public string Reeds { get; set; }

        [Display(Name = "تراکم")]
        public string Shots { get; set; }

        [Display(Name = "تعداد رنگ")]
        public int Frame { get; set; }

        [Display(Name = "PageTitle", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitle { get; set; }

        [Display(Name = "تگ عنوان انگلیسی")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitleEn { get; set; }

        [Display(Name = "PageDescription", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescription { get; set; }

        [Display(Name = "تگ توضیحات انگلیسی")]
        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescriptionEn { get; set; }

        [Display(Name = "ImageUrl", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name = "Body", ResourceType = typeof(Resources.Models.Product))]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }


        [Display(Name = "توضیحات انگلیسی")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string BodyEn { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }


        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Product))]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "IsInPromotion", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "IsInHome", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInHome { get; set; }
        
        [Display(Name = "بازدید")]
        public int Visit { get; set; }

        [Display(Name = "موجود است؟")]
        public bool IsAvailable { get; set; }
        
        [Display(Name = "کد واقعیت افزوده")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string ArCode { get; set; }

        public virtual ICollection<ProductComment> ProductComments { get; set; }

        public virtual ICollection<ProductSize> ProductSizes { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; }

        internal class configuration : EntityTypeConfiguration<Product>
        {
            public configuration()
            {
                HasRequired(p => p.ProductGroup).WithMany(t => t.Products).HasForeignKey(p => p.ProductGroupId);
                HasRequired(p => p.Color).WithMany(t => t.Products).HasForeignKey(p => p.ColorId);
            }
        }

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