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

namespace MashadCarpetShop.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Where(p => p.ParentId != null && p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(Guid? id)
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.ColorId = new SelectList(db.Colors, "Id", "Title");
            ViewBag.ParentId = new SelectList(db.ProductGroups, "Id", "Code");
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Code");
            return View();
        }
        CodeGenerator codeGenerator = new CodeGenerator();

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

        // GET: Products/Delete/5
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

        // POST: Products/Delete/5
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
    }
}
