using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace MashadCarpetShop.Controllers
{
    public class ProductSizesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index(Guid id)
        {
            var productSizes = db.ProductSizes.Include(p => p.Product).Where(p => p.ProductId == id && p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(productSizes.ToList());
        }

        public ActionResult Create(Guid id)
        {
            ViewBag.ProductId = id;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductSize productSize, Guid id)
        {
            if (ModelState.IsValid)
            {
                productSize.IsDeleted = false;
                productSize.CreationDate = DateTime.Now;
                productSize.ProductId = id;
                productSize.Id = Guid.NewGuid();
                productSize.SeedStock = productSize.Stock;
                productSize.SellNumber = 0;
                if (productSize.Stock > 0)
                    productSize.IsAvailable = true;

                UpdateParentProductOnCreate(productSize);
                db.ProductSizes.Add(productSize);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = id });
            }

            ViewBag.ProductId = id;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title", productSize.SizeId);
            return View(productSize);
        }

        // GET: ProductSizes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productSize.ProductId;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title", productSize.SizeId);
            return View(productSize);
        }

        public void UpdateParentProductOnCreate(ProductSize productSize)
        {
            Product product = db.Products.Find(productSize.ProductId);

            if (product.Amount == 0 || product.Amount > productSize.Amount)
            {

                product.Amount = productSize.Amount;
                product.DiscountAmount = productSize.DiscountAmount;
                product.IsInPromotion = productSize.IsInPromotion;
                product.LastModifiedDate = DateTime.Now;
            }
        }
        public void UpdateParentProductOnEdit(ProductSize productSize)
        {
            Product product = db.Products.Find(productSize.ProductId);

            var oProductSize = db.ProductSizes.Where(c => c.ProductId == productSize.ProductId&&c.Id!=productSize.Id).OrderBy(c => c.Amount)
                .FirstOrDefault();

            if (oProductSize != null)
            {
                if (oProductSize.Amount < productSize.Amount)
                {
                    product.Amount = oProductSize.Amount;
                    product.DiscountAmount = oProductSize.DiscountAmount;
                    product.IsInPromotion = oProductSize.IsInPromotion;
                }
                else
                {
                    product.Amount = productSize.Amount;
                    product.DiscountAmount = productSize.DiscountAmount;
                    product.IsInPromotion = productSize.IsInPromotion;
                }
                product.LastModifiedDate = DateTime.Now;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                productSize.IsDeleted = false;
                productSize.LastModifiedDate = DateTime.Now;

                UpdateParentProductOnEdit(productSize);

                db.Entry(productSize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = productSize.ProductId });
            }
            ViewBag.ProductId = productSize.ProductId;
            ViewBag.SizeId = new SelectList(db.Sizes, "Id", "Title", productSize.SizeId);
            return View(productSize);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSize productSize = db.ProductSizes.Find(id);
            if (productSize == null)
            {
                return HttpNotFound();
            }
            return View(productSize);
        }

        // POST: ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ProductSize productSize = db.ProductSizes.Find(id);
            productSize.IsDeleted = true;
            productSize.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index", new { id = productSize.ProductId });
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
