using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helpers;
using Models;

namespace ViewModels
{

    public class _BaseViewModel
    {
        private BaseViewModelHelper baseviewmodel = new BaseViewModelHelper();

        public List<ProductGroupMenuItems> MenuItems { get { return baseviewmodel.GetMenuProductGroup(); } }
        public bool IsAuthenticate { get { return baseviewmodel.GetAuthenticationStatus(); } }

        public string UserFullName { get { return baseviewmodel.GetAuthenticateUserName(); } }
        public string UserRole { get { return baseviewmodel.GetRoleTitle(); } }
    }

    public class ProductGroupMenuItems
    {
        public string Title { get; set; }
        public string UrlParam { get; set; }

    }
     
}