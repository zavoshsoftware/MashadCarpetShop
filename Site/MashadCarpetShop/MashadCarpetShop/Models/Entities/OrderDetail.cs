using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderDetail : BaseEntity
    {
        [Display(Name = "OrderId", ResourceType = typeof(Resources.Models.OrderDetail))]
        public Guid OrderId { get; set; }

        public Guid ProductSizeId { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resources.Models.OrderDetail))]
        public int Quantity { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.Models.OrderDetail))]
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.OrderDetail))]
        [Column(TypeName = "Money")]
        public decimal RowAmount { get; set; }


        public virtual Order Order { get; set; }
        public virtual ProductSize ProductSize { get; set; }


        internal class Configuration : EntityTypeConfiguration<OrderDetail>
        {
            public Configuration()
            {
                HasRequired(p => p.Order)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.OrderId);

                HasRequired(p => p.ProductSize)
                    .WithMany(j => j.OrderDetails)
                    .HasForeignKey(p => p.ProductSizeId);
            }
        }



        [Display(Name = "Price", ResourceType = typeof(Resources.Models.OrderDetail))]
        [NotMapped]
        public string AmountStr { get { return Amount.ToString("N0"); } }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.OrderDetail))]
        [NotMapped]

        public string RowAmountStr { get { return RowAmount.ToString("N0"); } }

    }
}
