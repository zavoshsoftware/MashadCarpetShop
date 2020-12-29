using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace MashadCarpetShop.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        CodeGenerator codeGenerator = new CodeGenerator();

        #region CRUD

        public ActionResult Index()
        {
            var products = db.Products.Where(p => p.ParentId != null && p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.ColorId = new SelectList(db.Colors, "Id", "Title");
            ViewBag.ParentId = new SelectList(db.ProductGroups, "Id", "Code");
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Code");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion
                Guid? parentId = GetParentIdByDesignNoAndColor(product.DesignNo);
                string colorTitle = db.Colors.Find(product.ColorId).Title;

                product.Title = "طرح " + product.DesignNo + " رنگ " + colorTitle;
                product.IsDeleted = false;
                product.CreationDate = DateTime.Now;
                product.Code = codeGenerator.ReturnProductCode();
                product.ParentId = parentId;
                product.Amount = 0;
                product.Visit = 0;
                product.IsAvailable = false;
                product.Id = Guid.NewGuid();
                db.Products.Add(product);
                db.SaveChanges();

                if (parentId == null)
                    CreateChildProduct(product);

                return RedirectToAction("Index");
            }

            ViewBag.ColorId = new SelectList(db.Colors, "Id", "Title", product.ColorId);
            ViewBag.ParentId = new SelectList(db.ProductGroups, "Id", "Code", product.ParentId);
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Code", product.ProductGroupId);
            return View(product);
        }

        public void CreateChildProduct(Product product)
        {

            Product childProduct = new Product()
            {
                Id = Guid.NewGuid(),
                ParentId = product.Id,
                ColorId = product.ColorId,
                Amount = 0,
                Body = product.Body,
                BodyEn = product.BodyEn,
                CreationDate = DateTime.Now,
                Code = codeGenerator.ReturnProductCode(),
                Description = product.Description,
                DesignNo = product.DesignNo,
                Frame = product.Frame,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                IsAvailable = product.IsAvailable,
                IsDeleted = false,
                IsInHome = product.IsInHome,
                IsInPromotion = product.IsInPromotion,
                Order = product.Order,
                PageDescription = product.PageDescription,
                PageDescriptionEn = product.PageDescriptionEn,
                PageTitle = product.PageTitle,
                PageTitleEn = product.PageTitleEn,
                ProductGroupId = product.ProductGroupId,
                Reeds = product.Reeds,
                Shots = product.Shots,
                Visit = 0,
                Title = product.Title,
                TitleEn = product.TitleEn,

            };

            db.Products.Add(childProduct);
            db.SaveChanges();
        }

        public Guid? GetParentIdByDesignNoAndColor(int designNo)
        {
            var product =
                db.Products.FirstOrDefault(c => c.DesignNo == designNo && c.IsDeleted == false && c.ParentId == null);

            return product?.Id;
        }


        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColorId = new SelectList(db.Colors, "Id", "Title", product.ColorId);
            ViewBag.ParentId = new SelectList(db.ProductGroups, "Id", "Code", product.ParentId);
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Code", product.ProductGroupId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion
                product.IsDeleted = false;
                product.LastModifiedDate = DateTime.Now;
                Guid? parentId = GetParentIdByDesignNoAndColor(product.DesignNo);

                if (parentId != product.ParentId)
                {
                    product.ParentId = parentId;
                    if (parentId == null)
                        CreateChildProduct(product);
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorId = new SelectList(db.Colors, "Id", "Title", product.ColorId);
            ViewBag.ParentId = new SelectList(db.ProductGroups, "Id", "Code", product.ParentId);
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Code", product.ProductGroupId);
            return View(product);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        [Route("carpet-online-shopping/{urlParam}")]
        public ActionResult List(string urlParam)
        {
            var productGroup = db.ProductGroups.Where(c => c.UrlParam == urlParam).Select(c => new
            {
                c.Title,
                c.Id,
                c.UrlParam
            }).FirstOrDefault();

            if (productGroup == null)
                return Redirect("/carpet-online-shopping");

            List<Product> products = db.Products.Where(c =>
                   /* c.ProductGroupId == productGroup.Id &&*/ c.ParentId != null && c.IsDeleted == false && c.IsActive)
                .ToList();

            ProductListViewModel result = new ProductListViewModel()
            {
                ProductGroupTitle = productGroup.Title,

                ProductGroupUrlParam = productGroup.UrlParam,

                Products = GetProductCardByProducts(products)
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

        [Route("carpet-online-shopping/{groupUrlParam}/{code:int}")]
        public ActionResult Details(string groupUrlParam, int code, Guid? productSizeId)
        {

            ProductSize productSize = new ProductSize();
            Product product = db.Products.FirstOrDefault(c => c.Code == code && c.IsDeleted == false);

            if (product == null)
                return Redirect("/carpet-online-shopping");


            if (productSizeId == null)
            {
                productSize =
                    db.ProductSizes.FirstOrDefault(c => c.ProductId == product.Id && c.Amount == product.Amount);
            }
            else
            {
                productSize =
                    db.ProductSizes.FirstOrDefault(c => c.Id==productSizeId.Value);
            }
          
            ProductDetailViewModel result = new ProductDetailViewModel()
            {
                ProductSize = productSize,

                ProductComments =
                    db.ProductComments.Where(c => c.ProductId == product.Id && c.IsActive && c.IsDeleted == false)
                        .ToList(),

                RelatedProducts = GetProductCardByProducts(db.Products.Where(c =>
                    c.ProductGroupId == product.ProductGroupId && c.ParentId != product.ParentId &&
                    c.IsDeleted == false && c.IsActive).Take(7).ToList()),

                ProductSizes = GetProductSizes(product, productSizeId),

                ProductColors = GetProductColors(product),

                ProductImages = db.ProductImages.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList()
            };

            if(productSize!=null)
                ViewBag.Title = productSize.Product.Title;
            return View(result);
        }

        public List<ProductSizeViewModel> GetProductSizes(Product product, Guid? productSizeId)
        {
            var productSizes = db.ProductSizes
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive && c.Stock > 0).Select(c =>
                    new
                    {
                        c.Id,
                        c.Size.Title,
                        c.Amount
                    });

            List<ProductSizeViewModel> sizes = new List<ProductSizeViewModel>();

            foreach (var productSize in productSizes)
            {
                bool isActive = false;

                if (productSizeId == null)
                    isActive = productSize.Amount == product.Amount;
                else
                    isActive = productSize.Id == productSizeId;

                sizes.Add(new ProductSizeViewModel()
                {
                    Id = productSize.Id,
                    Title = productSize.Title,
                    IsActive = isActive
                });

                if (isActive)
                    ViewBag.productSizeId = productSize.Id;
            }

            return sizes;
        }

        public List<ProductColorViewModel> GetProductColors(Product product)
        {
            var products = db.Products
                .Where(c => c.ParentId == product.ParentId && c.IsDeleted == false && c.IsActive).Select(c =>
                    new
                    {
                        c.Id,
                        c.Code,
                        c.Color.HexCode,
                        c.Color.Title
                    });

            List<ProductColorViewModel> colors = new List<ProductColorViewModel>();

            foreach (var oProduct in products)
            {
                if (db.ProductSizes.Any(c => c.ProductId == oProduct.Id))
                {
                    bool isActive = oProduct.Id == product.Id;

                    colors.Add(new ProductColorViewModel()
                    {
                        IsActive = isActive,
                        Title = oProduct.Title,
                        Code = oProduct.Code,
                        HexCode = oProduct.HexCode
                    });
                }
            }

            return colors;
        }
    }
}

