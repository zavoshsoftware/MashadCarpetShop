using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public class DiscountHelper
    {
        private DatabaseContext db = new DatabaseContext();
        public decimal CalculateDiscountAmount(Models.DiscountCode discountCode, decimal totalAmount)
        {
            if (discountCode.IsPercent)
                return totalAmount * discountCode.Amount / 100;
            else
                return discountCode.Amount;
        }

   
      
    }
}