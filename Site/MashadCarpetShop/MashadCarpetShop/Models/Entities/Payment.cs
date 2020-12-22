using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class Payment:BaseEntity
    {
        public string ReferenceNumber { get; set; }
        public Int64 SaleReferenceId { get; set; }
        public string RefId { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsSuccess { get; set; }
        public string Amount { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public int ResCodeRequest { get; set; }
        public int ResCodePayment { get; set; }
        public int ResCodeVerify { get; set; }
        public int ResCodeSettle { get; set; }
        internal class Configuration : EntityTypeConfiguration<Payment>
        {
            public Configuration()
            {
                HasRequired(p => p.Order)
                    .WithMany(j => j.Payments)
                    .HasForeignKey(p => p.OrderId);
            }
        }
    }
}