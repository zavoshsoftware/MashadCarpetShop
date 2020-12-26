﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Helpers;
using Models;

using ViewModels;

namespace MashadCarpetShop.Controllers
{
    public class BasketController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseViewModelHelper baseViewModel = new BaseViewModelHelper();
     //   ZarinPalHelper zp = new ZarinPalHelper();

        [Route("cart")]
        [HttpPost]
        public ActionResult AddToCart(string productSizeId, string qty)
        { 
            SetCookie(productSizeId, qty);
            return Json("true", JsonRequestBehavior.AllowGet);
        }



        [Route("Basket")]
        public ActionResult Basket()
        {
            CartViewModel cart = new CartViewModel();

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            decimal total = subTotal - discountAmount;

            decimal shipmentAmount = GetShipmentAmountByTotal(total);

            cart.ShipmentAmount = shipmentAmount.ToString("N0") +" تومان";

            cart.Total = (total+shipmentAmount).ToString("n0") + " تومان";

            cart.Policy = db.TextItems.FirstOrDefault(c => c.Name == "policy");
            return View(cart);
        }



        public decimal GetShipmentAmountByTotal(decimal totalAmount)
        {
          decimal shipmentAmount =Convert.ToDecimal(WebConfigurationManager.AppSettings["shipmentAmount"]);
          decimal freeShipmentLimitAmount = Convert.ToDecimal(WebConfigurationManager.AppSettings["freeShipmentLimitAmount"]);

            if (totalAmount >= freeShipmentLimitAmount)
                return 0;

            return shipmentAmount;
        }

        [Route("Basket/remove/{code}")]
        public ActionResult RemoveFromBasket(string code)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    string removeArray = coockieItem[0] + "^" + coockieItem[1];
                    coockieItems = coockieItems.Where(current => current != removeArray).ToArray();
                    break;
                }
            }

            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-mashadcarpet")
            {
                Name = "basket-mashadcarpet",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction("basket");
        }

        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount = db.DiscountCodes.FirstOrDefault(current => current.Code == coupon);

            string result = CheckCouponValidation(discount);

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.ProductSize.Amount;

                if (orderDetail.ProductSize.IsInPromotion)
                {
                    if (orderDetail.ProductSize.DiscountAmount != null)
                        amount = orderDetail.ProductSize.DiscountAmount.Value;
                }

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }
        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    Guid productSizeId = new Guid(productItem[0]);
 

                    ProductSize productSize=db.ProductSizes.FirstOrDefault(current =>
                        current.IsDeleted == false && current.Id == productSizeId);

                    productInCarts.Add(new ProductInCart()
                    {
                        ProductSize = productSize,
                        Quantity = Convert.ToInt32(productItem[1]),

                    });
                }
            }

            return productInCarts;
        }
        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            if (Request.Cookies["basket-mashadcarpet"] != null)
            {
                bool changeCurrentItem = false;

                cookievalue = Request.Cookies["basket-mashadcarpet"].Value;

                string[] coockieItems = cookievalue.Split('/');

                for (int i = 0; i < coockieItems.Length - 1; i++)
                {
                    string[] coockieItem = coockieItems[i].Split('^');

                    if (coockieItem[0] == code)
                    {
                        coockieItem[1] = (Convert.ToInt32(coockieItem[1]) + 1).ToString();
                        changeCurrentItem = true;
                        coockieItems[i] = coockieItem[0] + "^" + coockieItem[1];
                        break;
                    }
                }

                if (changeCurrentItem)
                {
                    cookievalue = null;
                    for (int i = 0; i < coockieItems.Length - 1; i++)
                    {
                        cookievalue = cookievalue + coockieItems[i] + "/";
                    }

                }
                else
                    cookievalue = cookievalue + code + "^" + quantity + "/";

            }
            else
                cookievalue = code + "^" + quantity + "/";

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-mashadcarpet")
            {
                Name = "basket-mashadcarpet",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }



        public string[] GetCookie()
        {
            if (Request.Cookies["basket-mashadcarpet"] != null)
            {
                string cookievalue = Request.Cookies["basket-mashadcarpet"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount)
        {
            if (discount == null)
                return "Invald";

            if (!discount.IsMultiUsing)
            {
                if (db.Orders.Any(current => current.DiscountCodeId == discount.Id))
                    return "Used";
            }

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";

            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "/" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }
        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }


        [Route("checkout")]
        [Authorize(Roles = "Customer")]
        public ActionResult CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                  CheckOutViewModel checkOut = new CheckOutViewModel();
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string role = identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                Guid userId = new Guid(id);

                Models.User user = db.Users.Find(userId);

                if (user != null)
                {
                    UserInformation userInformation = new UserInformation()
                    {
                        FullName = user.FullName,
                        CellNumber = user.CellNum
                    };

                    checkOut.UserInformation = userInformation;
                }

                if (role != "Customer")
                {
                    return Redirect("/login?ReturnUrl=checkout");
                }



                List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                checkOut.Products = productInCarts;

                decimal subTotal = GetSubtotal(productInCarts);

                checkOut.SubTotal = subTotal.ToString("n0") + " تومان";


                decimal discountAmount = GetDiscount();

                checkOut.DiscountAmount = discountAmount.ToString("n0") + " تومان";

                decimal total = subTotal - discountAmount;

                decimal shipmentAmount = GetShipmentAmountByTotal(total);

                checkOut.ShipmentAmount = shipmentAmount.ToString("N0") + " تومان";

                checkOut.Total = (total + shipmentAmount).ToString("n0") + " تومان";


                checkOut.Provinces = db.Provinces.OrderBy(current => current.Title).ToList();

              ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");

                return View(checkOut);
            }

            return Redirect("/login?ReturnUrl=checkout");

        }

        public string GetTextByName(string name)
        {
            var textItem = db.TextItems.Where(c => c.Name == name).Select(c => c.Summery).FirstOrDefault();

            if (textItem != null)
                return textItem;
            return string.Empty;
        }
        public ActionResult FillCities(string id)
        {
            Guid provinceId = new Guid(id);
            //   ViewBag.cityId = ReturnCities(provinceId);
            var cities = db.Cities.Where(c => c.ProvinceId == provinceId).OrderBy(current => current.Title).ToList();
            List<CityItemViewModel> cityItems = new List<CityItemViewModel>();
            foreach (Models.City city in cities)
            {
                cityItems.Add(new CityItemViewModel()
                {
                    Text = city.Title,
                    Value = city.Id.ToString()
                });
            }
            return Json(cityItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Finalize(string notes, string cellnumber, string postal,
                                    string address, string city, string fullname)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

                    string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                    Guid userId = new Guid(name);

                    List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                    if (productInCarts.Count == 0)
                    {
                        return Json("emptyBasket", JsonRequestBehavior.AllowGet);

                    }
                    Order order = ConvertCoockieToOrder(productInCarts);

                    if (order != null)
                    {
                        order.UserId = userId;
                        order.DeliverFullName = fullname;
                        order.DeliverCellNumber = cellnumber;
                        order.Address = address;
                        order.PostalCode = postal;
                        order.CustomerDesc = notes;
                        order.CityId = new Guid(city);


                        order.TotalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount, order.ShippingAmount);



                        db.SaveChanges();

                        string res = "";

                        //if (paymentType == "online")
                        //    res = zp.ZarinPalRedirect(order, order.TotalAmount);

                        //else
                        //{
                        //    RemoveCookie();

                        //    res = "notonline";

                        //    User user = db.Users.Find(userId);
                        //   string smsCellnumber = cellnumber;
                        //    if (user != null)
                        //        smsCellnumber = user.CellNum;
                        //    SendSms.SendCommonSms(smsCellnumber, "کاربر گرامی با تشکر از خرید شما. سفارش شما در سایت رنگ و ابزار خوشدست با موفقیت ثبت گردید.");

                        //}
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json("/login?ReturnUrl=checkout", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }


        #region Finalize




        public decimal GetTotalAmount(decimal? subtotal, decimal? discount, decimal? shippment)
        {
            decimal discountAmount = 0;
            if (discount != null)
                discountAmount = (decimal)discount;

            decimal shipmentAmount = 0;
            if (shippment != null)
                shipmentAmount = (decimal)shippment;

            if (subtotal == null)
                subtotal = 0;

            return (decimal)subtotal - discountAmount + shipmentAmount;
        }
        public Order ConvertCoockieToOrder(List<ProductInCart> products)
        {
            try
            {
                CodeGenerator codeCreator = new CodeGenerator();

                Order order = new Order();

                order.Id = Guid.NewGuid();
                order.IsActive = true;
                order.IsDeleted = false;
                order.IsPaid = false;
                order.CreationDate = DateTime.Now;
                order.LastModifiedDate = DateTime.Now;
                order.Code = codeCreator.ReturnOrderCode();
                order.OrderStatusId = db.OrderStatuses.FirstOrDefault(current => current.Code == 1).Id;
                order.SubTotal = GetSubtotal(products);

                order.DiscountAmount = GetDiscount();
                order.DiscountCodeId = GetDiscountId();
                order.ShippingAmount =
                    GetShipmentAmountByTotal(Convert.ToDecimal(order.SubTotal - order.DiscountAmount));

                order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);


                db.Orders.Add(order);

                foreach (ProductInCart product in products)
                {
                    decimal amount = product.ProductSize.Amount;

                    if (product.ProductSize.IsInPromotion)
                    {
                        if (product.ProductSize.DiscountAmount != null)
                            amount = product.ProductSize.DiscountAmount.Value;
                    }

                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductSizeId = product.ProductSize.Id,
                        Quantity = product.Quantity,
                        Amount = amount * product.Quantity,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                    //    Price = amount
                    };

                    db.OrderDetails.Add(orderDetail);
                }

                return order;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Guid? GetDiscountId()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');

                    DiscountCode discountCode =
                        db.DiscountCodes.FirstOrDefault(current => current.Code == basketItems[1]);

                    return discountCode?.Id;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public User CreateUser(string fullName, string cellNumber)
        {
            CodeGenerator codeCreator = new CodeGenerator();

            Random rnd = new Random();

            User oUser =
                db.Users.FirstOrDefault(current => current.CellNum == cellNumber && current.IsDeleted == false);

            if (oUser == null)
            {
                User user = new User()
                {
                    CellNum = cellNumber,
                    FullName = fullName,
                    Password = rnd.Next(1000, 9999).ToString(),
                    Code = 1000,
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.Now,
                    RemainCredit = 0,
                    RoleId = db.Roles.FirstOrDefault(current => current.Name.ToLower() == "customer").Id,

                };

                db.Users.Add(user);
                db.SaveChanges();
                return user;
            }
            return oUser;
        }


        public void RemoveCookie()
        {
            if (Request.Cookies["basket-mashadcarpet"] != null)
            {
                Response.Cookies["basket-mashadcarpet"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        #endregion

        private String MerchantId = WebConfigurationManager.AppSettings["MerchantId"];



        //[Route("callback")]
        //public ActionResult CallBack(string authority, string status)
        //{
        //    String Status = status;
        //    CallBackViewModel callBack = new CallBackViewModel();
        //    ZarinPalHelper zarinPal = new ZarinPalHelper();
        //    if (Status != "OK")
        //    {
        //        callBack.IsSuccess = false;
        //        callBack.RefrenceId = "414";
        //        Order order = zarinPal.GetOrderByAuthority(authority);
        //        if (order != null)
        //        {
        //            callBack.Order = order;
        //            callBack.OrderDetails = db.OrderDetails
        //                        .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product).ToList();
        //        }
        //    }

        //    else
        //    {
        //        try
        //        {
        //            var zarinpal = ZarinPal.ZarinPal.Get();
        //            zarinpal.DisableSandboxMode();
        //            String Authority = authority;
        //            long Amount = zarinPal.GetAmountByAuthority(Authority);

        //            var verificationRequest = new ZarinPal.PaymentVerification(MerchantId, Amount, Authority);
        //            var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
        //            if (verificationResponse.Status == 100 || verificationResponse.Status == 101)
        //            {
        //                Order order = zarinPal.GetOrderByAuthority(authority);
        //                if (order != null)
        //                {

        //                    UpdateOrder(order.Id, verificationResponse.RefID);

        //                    callBack.Order = order;
        //                    callBack.IsSuccess = true;
        //                    callBack.OrderCode = order.Code.ToString();
        //                    callBack.RefrenceId = verificationResponse.RefID;

        //                    callBack.OrderDetails = db.OrderDetails
        //                        .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product).ToList();
        //                    foreach (OrderDetail orderDetail in callBack.OrderDetails)
        //                    {
        //                        Product product = orderDetail.Product;
        //                        product.Stock = orderDetail.Product.Stock - 1;

        //                        if (product.Stock <= 0)
        //                        {
        //                            product.IsAvailable = false;
        //                        }
        //                        db.SaveChanges();
        //                    }
        //                    RemoveCookie();

        //                   SendSms.SendCommonSms(order.User.CellNum,"کاربر گرامی با تشکر از خرید شما. سفارش شما در سایت رنگ و ابزار خوشدست با شماره پیگیری "+ verificationResponse.RefID + " با موفقیت ثبت گردید.");
        //                }
        //                else
        //                {
        //                    callBack.IsSuccess = false;
        //                    callBack.RefrenceId = "سفارش پیدا نشد";
        //                }
        //            }
        //            else
        //            {
        //                callBack.IsSuccess = false;
        //                callBack.RefrenceId = verificationResponse.Status.ToString();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            callBack.IsSuccess = false;
        //            callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
        //        }
        //    }

        //    return View(callBack);
        //}


        //public void ChangeStockAfterPayment(Guid orderId)
        //{
        //    List<OrderDetail> orderDetails = db.OrderDetails.Where(current =>
        //        current.OrderId == orderId && current.IsDeleted == false && current.IsActive == true).ToList();

        //    foreach (OrderDetail orderDetail in orderDetails)
        //    {
        //        Product product = db.Products.FirstOrDefault(current => current.Id == orderDetail.ProductId);

        //        if (product != null)
        //        {
        //            product.Stock = product.Stock
        //                            - orderDetail.Quantity;
        //        }
        //    }
        //}

        //public void UpdateOrder(Guid orderId, string refId)
        //{
        //    Order order = db.Orders.Find(orderId);

        //    order.IsPaid = true;
        //    order.PaymentDate = DateTime.Now;
        //    order.SaleReferenceId = refId;


        //    //OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.Code == 2);
        //    //if (orderStatus != null)
        //    //    order.OrderStatusId = orderStatus.Id;

        //    order.LastModifiedDate = DateTime.Now;


        //    db.SaveChanges();
        //}
    }
}