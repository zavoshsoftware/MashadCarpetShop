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
                User oUser = db.Users.Include(u => u.Role)
                    .FirstOrDefault(a => a.CellNum == model.Username && a.IsDeleted == false);

                if (oUser == null)
                {
                    User user = new User()
                    {
                        Id=Guid.NewGuid(),
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
    }
}