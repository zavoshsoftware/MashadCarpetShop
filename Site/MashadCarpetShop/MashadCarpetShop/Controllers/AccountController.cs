using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SmsIrRestful;

namespace MashadCarpetShop.Controllers
{
    public class AccountController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        [Route("login")]
        public ActionResult Login(string ReturnUrl = "")
        {
            ViewBag.Message = "";
            ViewBag.ReturnUrl = ReturnUrl;
            LoginViewModel login = new LoginViewModel();
            return View(login);

        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User oUser = db.Users.Include(u => u.Role).Where(a => a.CellNum == model.Username && a.Password == model.Password && a.IsDeleted == false).FirstOrDefault();

                if (oUser != null)
                {
                    var ident = new ClaimsIdentity(
                      new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, oUser.CellNum),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                          new Claim(ClaimTypes.Name,oUser.Id.ToString()),

              // optionally you could add roles if any
                          new Claim(ClaimTypes.Role, oUser.Role.Name),
                          new Claim(ClaimTypes.Surname, oUser.FullName)

                      },
                      DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = true }, ident);
                    return RedirectToLocal(returnUrl, oUser.Role.Name); // auth succeed 
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "نام کاربری و یا کلمه عبور وارد شده صحیح نمی باشد.";
                }
            }


            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl, string role)
        {
            if (role == "Administrator")
                return RedirectToAction("Index", "Blogs");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl == "checkout")
                    return Redirect("/checkout");
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("Register")]
        public ActionResult Register(string ReturnUrl = "")
        {
            ViewBag.Message = "";
            ViewBag.ReturnUrl = ReturnUrl;
            RegisterViewModel reg = new RegisterViewModel();
            return View(reg);

        }


        [Route("Register")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string englishcellNumber = "";
                foreach (char ch in model.Username)
                {
                    englishcellNumber += char.GetNumericValue(ch);
                }
                model.Username = englishcellNumber;
                
                bool isValidMobile = Regex.IsMatch(model.Username, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

                if (!isValidMobile)
                {
                    TempData["wrongCellnumber"] = "شماره موبایل وارد شده صحیح نمی باشد.";
                    return View(model);
                }


                User oUser = db.Users.Include(u => u.Role)
                    .FirstOrDefault(a => a.CellNum == model.Username && a.IsDeleted == false);

                if (oUser == null)
                {
                    User user = new User()
                    {
                        Id = Guid.NewGuid(),
                        FullName = model.FullName,
                        CellNum = model.Username,
                        Password = model.Password,
                        Email = model.Email,
                        CreationDate = DateTime.Now,
                        IsDeleted = false,
                        IsActive = true,
                        RoleId = new Guid("DFBAD6B3-C605-4DD9-8DB1-2659D38D7AEC")
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                    var ident = new ClaimsIdentity(
                        new[] { 
                            // adding following 2 claim just for supporting default antiforgery provider
                            new Claim(ClaimTypes.NameIdentifier, user.CellNum),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                            new Claim(ClaimTypes.Name,user.Id.ToString()),

                            // optionally you could add roles if any
                            new Claim(ClaimTypes.Role, "Customer"),
                            new Claim(ClaimTypes.Surname, user.FullName)

                        },
                        DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties { IsPersistent = true }, ident);
                    return RedirectToLocal(returnUrl, "Customer"); // auth succeed 
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "با این شماره موبایل قبلا در وب سایت کاربری ثبت شده است. و نمی توانید حساب جدید ایجاد کنید.";
                }
            }


            return View(model);
        }


        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            return RedirectToAction("index", "Home");
        }




        [Route("RecoverPassword")]
        public ActionResult RecoverPassword()
        {

            RecoverPasswordViewModel result = new RecoverPasswordViewModel();
            return View(result);

        }

        [Route("RecoverPassword")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid customerRoleId = new Guid("DFBAD6B3-C605-4DD9-8DB1-2659D38D7AEC");
                User oUser = db.Users.Include(u => u.Role)
                    .FirstOrDefault(a =>
                        a.CellNum == model.CellNumber && a.RoleId == customerRoleId && a.IsDeleted == false);
                
                if (oUser != null)
                {
                    string pass = RandomCode();
                    oUser.Password = pass;
                    oUser.LastModifiedDate = DateTime.Now;
                    oUser.Description += "password change in " + DateTime.Now + " to " + pass;

                    db.SaveChanges();

                    SendSms(oUser.CellNum, pass);
                    TempData["success"] = "کلمه عبور برای شما از طریق پیامک ارسال شد.";
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "این شماره موبایل در سایت ثبت نام نکرده است. لطفا در سایت ثبت نام کنید.";
                }
            }


            return View(model);
        }
        public string RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return (r);
        }


        public void SendSms(string cellNumber, string code)
        {
            var token = new Token().GetToken("773e6490afdaeccca1206490", "123qwe!@#QWE");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = Convert.ToInt64(cellNumber),
                TemplateId = 44159,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters()
                    {
                        Parameter = "verifyCode" , ParameterValue = code
                    }
                }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {

            }
            else
            {

            }
        }
    }
}