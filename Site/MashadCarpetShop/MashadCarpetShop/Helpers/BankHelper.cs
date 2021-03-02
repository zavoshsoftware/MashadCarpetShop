using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Models;

namespace Helper
{
    public static class BankHelper
    {
        public static void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }
        //public static String PreparePostForm(string url, NameValueCollection data)
        //{
        //    string formID = "PostForm";
        //    StringBuilder strForm = new StringBuilder();
        //    strForm.Append("< form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\" >");
        //    foreach (string key in data)
        //    {
        //        strForm.Append("< input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\" >");
        //    }
        //    strForm.Append("< /form >");
        //    StringBuilder strScript = new StringBuilder();
        //    strScript.Append("< script language='javascript' >");
        //    strScript.Append("var v" + formID + " = document." + formID + ";");
        //    strScript.Append("v" + formID + ".submit();");
        //    strScript.Append("< /script >");
        //    return strForm.ToString() + strScript.ToString();
        //}

        //public static String PostRequest(string url,string refId)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(url);

        //    var postData = refId;

        //    var data = Encoding.ASCII.GetBytes(postData);

        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = data.Length;

        //    using (var stream = request.GetRequestStream())
        //    {
        //        stream.Write(data, 0, data.Length);
        //    }

        //    var response = (HttpWebResponse)request.GetResponse();

        //    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    return responseString;
        //}

        public static string MellatResult(string ID)
        {
            string result = "";
            switch (ID)
            {
                case "-100":
                    result = "پرداخت لغو شده";
                    break;
                case "0":
                    result = "تراكنش با موفقيت انجام شد";
                    break;
                case "11":
                    result = "شماره كارت نامعتبر است ";
                    break;
                case "12":
                    result = "موجودي كافي نيست ";
                    break;
                case "13":
                    result = "رمز نادرست است ";
                    break;
                case "14":
                    result = "تعداد دفعات وارد كردن رمز بيش از حد مجاز است ";
                    break;
                case "15":
                    result = "كارت نامعتبر است ";
                    break;
                case "16":
                    result = "دفعات برداشت وجه بيش از حد مجاز است ";
                    break;
                case "17":
                    result = "كاربر از انجام تراكنش منصرف شده است ";
                    break;
                case "18":
                    result = "تاريخ انقضاي كارت گذشته است ";
                    break;
                case "19":
                    result = "مبلغ برداشت وجه بيش از حد مجاز است ";
                    break;
                case "111":
                    result = "صادر كننده كارت نامعتبر است ";
                    break;
                case "112":
                    result = "خطاي سوييچ صادر كننده كارت ";
                    break;
                case "113":
                    result = "پاسخي از صادر كننده كارت دريافت نشد ";
                    break;
                case "114":
                    result = "دارنده كارت مجاز به انجام اين تراكنش نيست";
                    break;
                case "21":
                    result = "پذيرنده نامعتبر است ";
                    break;
                case "23":
                    result = "خطاي امنيتي رخ داده است ";
                    break;
                case "24":
                    result = "اطلاعات كاربري پذيرنده نامعتبر است ";
                    break;
                case "25":
                    result = "مبلغ نامعتبر است ";
                    break;
                case "31":
                    result = "پاسخ نامعتبر است ";
                    break;
                case "32":
                    result = "فرمت اطلاعات وارد شده صحيح نمي باشد ";
                    break;
                case "33":
                    result = "حساب نامعتبر است ";
                    break;
                case "34":
                    result = "خطاي سيستمي ";
                    break;
                case "35":
                    result = "تاريخ نامعتبر است ";
                    break;
                case "41":
                    result = "شماره درخواست تكراري است ، دوباره تلاش کنید";
                    break;
                case "42":
                    result = "يافت نشد  Sale تراكنش";
                    break;
                case "43":
                    result = "داده شده است  Verify قبلا درخواست";
                    break;
                case "44":
                    result = "يافت نشد  Verfiy درخواست";
                    break;
                case "45":
                    result = "شده است  Settle تراكنش";
                    break;
                case "46":
                    result = "نشده است  Settle تراكنش";
                    break;
                case "47":
                    result = "يافت نشد  Settle تراكنش";
                    break;
                case "48":
                    result = "شده است  Reverse تراكنش";
                    break;
                case "49":
                    result = "يافت نشد  Refund تراكنش";
                    break;
                case "412":
                    result = "شناسه قبض نادرست است ";
                    break;
                case "413":
                    result = "شناسه پرداخت نادرست است ";
                    break;
                case "414":
                    result = "سازمان صادر كننده قبض نامعتبر است ";
                    break;
                case "415":
                    result = "زمان جلسه كاري به پايان رسيده است ";
                    break;
                case "416":
                    result = "خطا در ثبت اطلاعات ";
                    break;
                case "417":
                    result = "شناسه پرداخت كننده نامعتبر است ";
                    break;
                case "418":
                    result = "اشكال در تعريف اطلاعات مشتري ";
                    break;
                case "419":
                    result = "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است ";
                    break;
                case "421":
                    result = "نامعتبر است  IP";
                    break;
                case "51":
                    result = "تراكنش تكراري است ";
                    break;
                case "54":
                    result = "تراكنش مرجع موجود نيست ";
                    break;
                case "55":
                    result = "تراكنش نامعتبر است ";
                    break;
                case "61":
                    result = "خطا در واريز ";
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }

        public static void UpdatePayment(Guid orderId, string VResult, long SaleRefrenceID, string refId, bool isSuccess)
        {
            DatabaseContext db = new DatabaseContext();

            Payment payment = new Payment()
            {
                OrderId = orderId,
                PaymentStatus = VResult,
                SaleReferenceId = SaleRefrenceID,
                IsSuccess = isSuccess,
                ReferenceNumber = refId,
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now
            };

            db.Payments.Add(payment);
            if (isSuccess)
            {
                UpdateOrderStatus(db, orderId, SaleRefrenceID); 

            }
            db.SaveChanges();
        }

        private static void UpdateOrderStatus(DatabaseContext db, Guid orderId, long? saleRefrenceId)
        {
            Order order = db.Orders.Find(orderId);

            if (order != null)
            {
                OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.IsDeleted == false && current.Code == 2);

                if (orderStatus != null)
                    order.OrderStatusId = orderStatus.Id;

                order.SaleReferenceId = saleRefrenceId.ToString();

               // UpdateUserWallet(db, order);
            }
        }

        //public static void UpdateUserWallet(DatabaseContext db,Order order)
        //{
        //    if (order.WalletAmount > 0)
        //    {
        //        User user = db.Users.Find(order.UserId);
        //        if (user != null)
        //        {
        //            user.Amount -= order.WalletAmount;
        //            user.LastModifiedDate=DateTime.Now;
        //        }
                
        //    }
        //}
   
        public static Guid? GetOrderStatusIdByCode(int statusCode)
        {
            DatabaseContext db = new DatabaseContext();

            OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.IsDeleted == false && current.Code == statusCode);

            return orderStatus?.Id;
        }
    }
}