using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;
using ViewModels;

//using ViewModels;

namespace Helpers
{
    public class BaseViewModelHelper
    {
        private DatabaseContext db = new DatabaseContext();

        public List<ProductGroupMenuItems> GetMenuProductGroup()
        {
            List<ProductGroupMenuItems> menuItems = new List<ProductGroupMenuItems>();

           var productGroups = db.ProductGroups
                .Where(c =>c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Select(c=>new
                {
                    c.Title,
                    c.UrlParam
                }).ToList();

            foreach (var productGroup in productGroups)
            {
                menuItems.Add(new ProductGroupMenuItems()
                {
                    Title = productGroup.Title,
                    UrlParam = productGroup.UrlParam
                });
            }
 

            return menuItems;
        }
 

      

        public string GetTextItemByName(string name, string field)
        {
            TextItem textItem = db.TextItems.FirstOrDefault(c => c.Name == name);
            if (textItem != null)
            {
                if (field == "summery")
                    return textItem.Summery;
                return textItem.LinkUrl;
            }
            return string.Empty;
        }

        public bool GetAuthenticationStatus()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }

        public string GetAuthenticateUserName()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;
                string name= identity.FindFirst(System.Security.Claims.ClaimTypes.Surname).Value;
                return name;
            }
            return "ورود";
        }
        public string GetRoleTitle()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;
                string role= identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                return role;
            }
            return "";
        }

    }
}