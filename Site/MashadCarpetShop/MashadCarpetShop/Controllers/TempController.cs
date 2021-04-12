using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace MashadCarpetShop.Controllers
{
    public class TempController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public string UpdatePriceInSize()
        {
            //  var paymentUniqeCodes = db.PaymentUniqeCodes.OrderByDescending(c=>c.Id).ToList();
            var productSizes = db.ProductSizes.Where(c => c.IsDeleted == false).ToList();

            foreach (var product in productSizes)
            {
                decimal addingAmount = (5m / 100m) * (product.Amount);
                product.Amount = product.Amount + addingAmount;
                db.SaveChanges();
            }

            return "";
        }
        public string UpdatePrice()
        {
            //  var paymentUniqeCodes = db.PaymentUniqeCodes.OrderByDescending(c=>c.Id).ToList();
            var products = db.Products.Where(c => c.IsDeleted == false).ToList();

            foreach (var product in products)
            {
                decimal addingAmount = (5m / 100m) * (product.Amount);
                product.Amount = product.Amount + addingAmount;
                db.SaveChanges();
            }

            return "";
        }
        public string InsertToPaymnetCode()
        {
            //  var paymentUniqeCodes = db.PaymentUniqeCodes.OrderByDescending(c=>c.Id).ToList();

            for (int i = 0; i < 10000; i++)
            {
                PaymentUniqeCode paymentUniqeCode = new PaymentUniqeCode()
                {
                    OrderId = new Guid("E30E7D50-27F0-483B-87AC-22A6AF67AF5D"),

                };

                db.PaymentUniqeCodes.Add(paymentUniqeCode);
            }
            db.SaveChanges();

            return "";
        }
        public string ColorConvertor()
        {
            var tempcolors = db.TempColors.ToList();

            foreach (var tempcolor in tempcolors)
            {
                Color color = new Color()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    HexCode = tempcolor.ColorNo,
                    IsActive = true,
                    IsDeleted = tempcolor.IsDelete,
                    Title = tempcolor.ColorTitle,
                    TitleEn = tempcolor.ColorEN_Title
                };

                db.Colors.Add(color);
                db.SaveChanges();

            }
            return "";
        }
        public string SizeConvertor()
        {
            var tempSizes = db.TempSizes;

            foreach (var tempSize in tempSizes)
            {
                Size size = new Size()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = tempSize.IsDelete,
                    Title = tempSize.SizeTitle,
                };

                db.Sizes.Add(size);
            }
            db.SaveChanges();
            return "";
        }
        public string BranchConvertor()
        {
            var tempStores = db.TempStore;

            foreach (var tempStore in tempStores)
            {
                Branch branch = new Branch()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = tempStore.IsDelete,
                    Title = tempStore.StoreName,
                    City = tempStore.StoreCity,
                    Phone = tempStore.StorePhone,
                    Address = tempStore.StoreAddress,
                };

                db.Branches.Add(branch);
            }
            db.SaveChanges();
            return "";
        }
    }
}