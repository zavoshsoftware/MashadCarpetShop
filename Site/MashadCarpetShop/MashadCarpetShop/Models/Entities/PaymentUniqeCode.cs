using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class PaymentUniqeCode
    {
        
        [Key]
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Orders { get; set; }
       
        internal class Configuration : EntityTypeConfiguration<PaymentUniqeCode>
        {
            public Configuration()
            {
                HasRequired(p => p.Orders)
                    .WithMany(j => j.PaymentUniqeCodes)
                    .HasForeignKey(p => p.OrderId);
            }
        }
    }
}