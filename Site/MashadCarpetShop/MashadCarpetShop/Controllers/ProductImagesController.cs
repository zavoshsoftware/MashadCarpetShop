using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace MashadCarpetShop.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductImagesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index(Guid id)
        {
            var productImages = db.ProductImages.Include(p => p.Product)
                .Where(p => p.ProductId == id && p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(productImages.ToList());
        }

        public ActionResult Create(Guid id)
        {
            ViewBag.ProductId = id;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductImage productImage, HttpPostedFileBase fileupload, Guid id)
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
                    productImage.ImageUrl = newFilenameUrl;
                }
                #endregion
                productImage.IsDeleted = false;
                productImage.CreationDate = DateTime.Now;
                productImage.ProductId = id;
                productImage.Id = Guid.NewGuid();
                db.ProductImages.Add(productImage);
                db.SaveChanges();
                return RedirectToAction("Index",new{id = id});
            }

            ViewBag.ProductId = id;
            return View(productImage);
        }

        // GET: ProductImages/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productImage.ProductId;
            return View(productImage);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductImage productImage, HttpPostedFileBase fileupload)
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
                    productImage.ImageUrl = newFilenameUrl;
                }
                #endregion
                productImage.IsDeleted = false;
                productImage.LastModifiedDate = DateTime.Now;
                db.Entry(productImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new{id= productImage.ProductId });
            }
            ViewBag.ProductId = productImage.ProductId;
            return View(productImage);
        }
         
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productImage.ProductId;

            return View(productImage);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            productImage.IsDeleted = true;
            productImage.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index",new{id= productImage.ProductId });
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
