using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace MashadCarpetShop.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            HomeViewModel result=new HomeViewModel()
            {
                TopProducts = GetProductCardByProducts(db.Products.Where(c=>c.IsInHome&&c.IsDeleted==false&&c.IsActive&&c.ParentId!=null).Take(8).ToList()),
                NewestProducts = GetProductCardByProducts(db.Products.Where(c=>c.IsInHome&&c.IsDeleted==false&&c.IsActive && c.ParentId != null).OrderByDescending(c=>c.CreationDate).Take(8).ToList()),
                ProductGroups = db.ProductGroups.Where(c=>c.IsDeleted==false&&c.IsActive).OrderBy(c=>c.Order).ToList(),
                Blogs = db.Blogs.Where(c=>c.IsDeleted==false&&c.IsActive).OrderByDescending(c=>c.CreationDate).Take(3).ToList()
            };
            return View(result);
        }
        public List<ProductCardViewModel> GetProductCardByProducts(List<Product> products)
        {
            List<ProductCardViewModel> productCard = new List<ProductCardViewModel>();

            foreach (Product product in products)
            {
                var sizes = db.ProductSizes.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive)
                    .ToList();

                if (sizes.Any())
                {
                    List<string> productSizes = new List<string>();

                    foreach (ProductSize productSize in sizes)
                    {
                        productSizes.Add(productSize.Size.Title);
                    }

                    productCard.Add(new ProductCardViewModel()
                    {
                        Product = product,
                        Sizes = productSizes
                    });
                }
            }

            return productCard;
        }
        [Route("contact")]
        public ActionResult Contact()
        {
            ContactViewModel result=new ContactViewModel();
            return View(result);
        }

        [Route("faq")]
        public ActionResult Faq()
        {
            FaqViewModel result =new FaqViewModel();
            return View(result);
        }

        [Route("about")]
        public ActionResult About()
        {
            AboutViewModel result=new AboutViewModel();
            return View(result);
        }
        [Route("about/value")]
        public ActionResult AboutValue()
        {
            AboutViewModel result=new AboutViewModel();
            return View(result);
        }
        [Route("about/honors")]
        public ActionResult AboutHonor()
        {
            AboutViewModel result=new AboutViewModel();
            return View(result);
        }
        [Route("about/international")]
        public ActionResult AboutInternational()
        {
            AboutViewModel result=new AboutViewModel();
            return View(result);
        }
        [Route("corporate-social-responsibility")]
        public ActionResult SocialResponsibility()
        {
            AboutViewModel result=new AboutViewModel();
            return View(result);
        }
    
    }
}