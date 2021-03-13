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
    public class ExcellHistoriesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ExcellHistories
        public ActionResult Index()
        {
            return View(db.ExcellHistories.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: ExcellHistories/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExcellHistory excellHistory = db.ExcellHistories.Find(id);
            if (excellHistory == null)
            {
                return HttpNotFound();
            }
            return View(excellHistory);
        }

        // GET: ExcellHistories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExcellHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FileUrl,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ExcellHistory excellHistory)
        {
            if (ModelState.IsValid)
            {
				excellHistory.IsDeleted=false;
				excellHistory.CreationDate= DateTime.Now; 
					
                excellHistory.Id = Guid.NewGuid();
                db.ExcellHistories.Add(excellHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(excellHistory);
        }

        // GET: ExcellHistories/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExcellHistory excellHistory = db.ExcellHistories.Find(id);
            if (excellHistory == null)
            {
                return HttpNotFound();
            }
            return View(excellHistory);
        }

        // POST: ExcellHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FileUrl,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ExcellHistory excellHistory)
        {
            if (ModelState.IsValid)
            {
				excellHistory.IsDeleted=false;
					excellHistory.LastModifiedDate=DateTime.Now;
                db.Entry(excellHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(excellHistory);
        }

        // GET: ExcellHistories/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExcellHistory excellHistory = db.ExcellHistories.Find(id);
            if (excellHistory == null)
            {
                return HttpNotFound();
            }
            return View(excellHistory);
        }

        // POST: ExcellHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ExcellHistory excellHistory = db.ExcellHistories.Find(id);
			excellHistory.IsDeleted=true;
			excellHistory.DeletionDate=DateTime.Now;
 
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
