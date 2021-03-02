using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Helper;
using Helpers;
using MashadCarpetShop.ServiceReference1;
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

            cart.ShipmentAmount = shipmentAmount.ToString("N0") + " تومان";

            cart.Total = (total + shipmentAmount).ToString("n0") + " تومان";

            cart.Policy = db.TextItems.FirstOrDefault(c => c.Name == "policy");
            return View(cart);
        }



        public decimal GetShipmentAmountByTotal(decimal totalAmount)
        {
            decimal shipmentAmount = Convert.ToDecimal(WebConfigurationManager.AppSettings["shipmentAmount"]);
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


                    ProductSize productSize = db.ProductSizes.FirstOrDefault(current =>
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
                //if (User.Identity.IsAuthenticated)
                //{
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

                    decimal totalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount, order.ShippingAmount);
                    order.TotalAmount = totalAmount;



                    db.SaveChanges();

                    string res = "";
                    string uniqueOrderId = GetUniqueOrderId(order.Id);

                    string log = PayRequest(order.Id, uniqueOrderId,
                        totalAmount * 10);

                    if (!log.Contains("false"))
                    {
                        return Json("true-" + log, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("bankerror|" + log.Split('-')[1], JsonRequestBehavior.AllowGet);
                    }

                    //if (paymentType == "online")
                    //    res = zp.ZarinPalRedirect(order, order.TotalAmount);

                    //else
                    //{
                 

                    //    res = "notonline";

                    //    User user = db.Users.Find(userId);
                    //   string smsCellnumber = cellnumber;
                    //    if (user != null)
                    //        smsCellnumber = user.CellNum;
                    //    SendSms.SendCommonSms(smsCellnumber, "کاربر گرامی با تشکر از خرید شما. سفارش شما در سایت رنگ و ابزار خوشدست با موفقیت ثبت گردید.");

                    //}

                }
                //   }

                return Json("false", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                // Get stack trace for the exception with source file information
                var st = new System.Diagnostics.StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                return Json("falsecatch - " + e.Message + " - " + line, JsonRequestBehavior.AllowGet);

            }
        }


        #region Finalize
        public static readonly string PgwSite = ConfigurationManager.AppSettings["PgwSite"];
        public static readonly string CallBackUrl = ConfigurationManager.AppSettings["CallBackUrl"];
        public static readonly string TerminalId = ConfigurationManager.AppSettings["TerminalId"];
        public static readonly string UserName = ConfigurationManager.AppSettings["UserName"];
        public static readonly string UserPassword = ConfigurationManager.AppSettings["UserPassword"];

        public string GetUniqueOrderId(Guid orderId)
        {
            PaymentUniqeCode paymentUniqeCode = new PaymentUniqeCode()
            {
                OrderId = orderId
            };

            db.PaymentUniqeCodes.Add(paymentUniqeCode);
            db.SaveChanges();
            return paymentUniqeCode.Id.ToString();
        }


        public string PayRequest(Guid orderId, string uniqueOrderId, decimal price)
        {
            try
            {
                string payDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                string payTime = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

                string result;

                BankHelper.BypassCertificateError();

                PaymentGatewayClient bpService = new PaymentGatewayClient();

                Int64 uniqueorderIdInt = Int64.Parse(uniqueOrderId);

                result = bpService.bpPayRequest(
                                  Int64.Parse(TerminalId),
                                  UserName,
                                  UserPassword,
                                  uniqueorderIdInt,
                                  Convert.ToInt64(price),
                                  payDate,
                                  payTime,
                                 null,
                               CallBackUrl,
                                 "0");

                if (result != null)
                {
                    // 45zm24554654,0 
                    string[] ResultArray = result.Split(',');

                    if (ResultArray[0].ToString() == "0")
                    {
                        return ResultArray[1];
                    }
                    else
                    {
                        BankHelper.UpdatePayment(orderId, ResultArray[0], 0, null, false);

                        //  UpdatePayment(paymentid, ResultArray[0].ToString(), 0, null, false);
                        return "false-" + BankHelper.MellatResult(ResultArray[0]);
                    }
                }
                return "false-" + BankHelper.MellatResult(result);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


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
                        Amount = amount,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                        RowAmount = amount * product.Quantity,
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





        [Route("callback")]
        public ActionResult CallBack(string authority, string status)
        {
            CallBackViewModel callBack = new CallBackViewModel();

            try
            {
                callBack = MellatReturn();
            }
            catch (Exception e)
            {

                callBack.IsSuccess = false;
                callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
            }


            return View(callBack);
        }


        private CallBackViewModel MellatReturn()
        {
            CallBackViewModel callBack = new CallBackViewModel();


            PaymentGatewayClient bpService = new PaymentGatewayClient();

            BankHelper.BypassCertificateError();
            if (string.IsNullOrEmpty(Request.Params["SaleReferenceId"]))
            {
                //ResCode=StatusPayment
                if (!string.IsNullOrEmpty(Request.Params["ResCode"]))
                {
                    callBack.RefrenceId = "**************";
                    callBack.IsSuccess = false;
                    callBack.Message = BankHelper.MellatResult(Request.Params["ResCode"]);

                }
                else
                {
                    callBack.Message = "رسید قابل قبول نیست";
                    callBack.RefrenceId = "**************";
                    callBack.IsSuccess = false;
                }
            }
            else
            {
                try
                {
                    string terminalId = ConfigurationManager.AppSettings["TerminalId"];
                    string userName = ConfigurationManager.AppSettings["UserName"];
                    string userPassword = ConfigurationManager.AppSettings["UserPassword"];
                    long saleOrderId = 0;  //PaymentID 
                    long saleReferenceId = 0;
                    string refId = null;

                    try
                    {
                        saleOrderId = long.Parse(Request.Params["SaleOrderId"].TrimEnd());
                        saleReferenceId = long.Parse(Request.Params["SaleReferenceId"].TrimEnd());
                        refId = Request.Params["RefId"].TrimEnd();
                    }
                    catch (Exception ex)
                    {
                        callBack.Message = ex + "<br>" + " وضعیت:مشکلی در پرداخت بوجود آمده ، در صورتی که وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد ";
                        callBack.IsSuccess = false;
                        callBack.RefrenceId = "**************";
                        return callBack;
                    }
                    string Result;
                    Result = bpService.bpVerifyRequest(long.Parse(terminalId), userName, userPassword, saleOrderId, saleOrderId, saleReferenceId);

                    if (!string.IsNullOrEmpty(Result))
                    {
                        if (Result == "0")
                        {
                            string qresult;
                            qresult = bpService.bpInquiryRequest(long.Parse(terminalId), userName, userPassword, saleOrderId, saleOrderId, saleReferenceId);
                            if (qresult == "0")
                            {
                                //long paymentID = Convert.ToInt64(saleOrderId);
                                Guid orderId = GetOrginalOrderId(saleOrderId);

                                // پرداخت نهایی
                                string Sresult;
                                // تایید پرداخت
                                Sresult = bpService.bpSettleRequest(long.Parse(terminalId), userName, userPassword, saleOrderId, saleOrderId, saleReferenceId);
                                if (Sresult != null)
                                {
                                    if (Sresult == "0" || Sresult == "45")
                                    {
                                        callBack.IsSuccess = true;
                                        BankHelper.UpdatePayment(orderId, Result, saleReferenceId, refId, true);
                                        callBack.Message = "پرداخت با موفقیت انجام شد.";
                                        callBack.RefrenceId = saleReferenceId.ToString();
                                        //تراکنش تایید و ستل شده است 
                                        Order order = db.Orders.Find(orderId);
                                        if (order != null)
                                        {
                                            //SendMessageToUser(order.User.CellNum, order.Code.ToString());
                                            ViewBag.Code = order.Code;
                                            ViewBag.CellNumber = order.User.CellNum;
                                            //if (order.DiscountCodeId != null)
                                            //{
                                            //    DiscountCode discountCode = db.DiscountCodes.Find(order.DiscountCodeId);

                                            //    if (discountCode != null)
                                            //    {
                                            //        discountCode.IsUsed = true;
                                            //        discountCode.LastModifiedDate = DateTime.Now;

                                            //        db.SaveChanges();
                                            //    }
                                            //}

                                            RemoveCookie();
                                            ChangeProductStock(order);
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        BankHelper.UpdatePayment(orderId, Result, saleReferenceId, refId, false);
                                        callBack.Message = "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
                                        callBack.RefrenceId = "**************";
                                        callBack.IsSuccess = false;
                                        //تراکنش تایید شده ولی ستل نشده است
                                    }
                                }
                            }
                            else
                            {
                                //string Rvresult;
                                ////عملیات برگشت دادن مبلغ
                                //Rvresult = bpService.bpReversalRequest(long.Parse(terminalId), userName, userPassword, saleOrderId, saleOrderId, saleReferenceId);
                                //Guid orderId = GetOrginalOrderId(saleOrderId);
                                //BankHelper.UpdatePayment(orderId, Result, saleReferenceId, refId, false);
                                //ViewBag.Message = "تراکنش بازگشت داده شد";
                                //ViewBag.SaleReferenceId = "**************";
                                //long paymentId = Convert.ToInt64(saleOrderId);
                                //BankHelper.UpdatePayment(GetOrginalOrderId(saleOrderId), Result, saleReferenceId, refId, false);
                            }
                        }
                        else
                        {
                            Guid orderId = GetOrginalOrderId(saleOrderId);
                            BankHelper.UpdatePayment(orderId, Result, saleReferenceId, refId, false);

                            callBack.Message = BankHelper.MellatResult(Result);
                            callBack.IsSuccess = false;
                            long paymentId = Convert.ToInt64(saleOrderId);

                        }
                    }
                    else
                    {
                        Guid orderId = GetOrginalOrderId(saleOrderId);
                        BankHelper.UpdatePayment(orderId, Result, saleReferenceId, refId, false);
                        callBack.Message = "شماره رسید قابل قبول نیست";
                        callBack.IsSuccess = false;
                        ViewBag.SaleReferenceId = "**************";
                    }
                }
                catch (Exception ex)
                {
                    string errors = ex.Message;
                    callBack.IsSuccess = false;
                    callBack.Message = "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
                    ViewBag.SaleReferenceId = "**************";
                }
            }

            return callBack;
        }

        public void ChangeProductStock(Order order)
        {
            List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                ProductSize pro = db.ProductSizes.Find(orderDetail.ProductSizeId);

                if (pro != null)
                {
                    pro.Stock = pro.Stock - orderDetail.Quantity;

                    if (pro.Stock <= 0)
                        pro.IsAvailable = false;

                    pro.LastModifiedDate = DateTime.Now;
                }
            }

        }
        public Guid GetOrginalOrderId(long uniqeOrderId)
        {
            return db.PaymentUniqeCodes.Find((uniqeOrderId)).OrderId;
        }

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