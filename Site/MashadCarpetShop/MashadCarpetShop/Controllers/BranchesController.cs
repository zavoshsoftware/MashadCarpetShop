using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace MashadCarpetShop.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BranchesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Branches
        public ActionResult Index()
        {
            return View(db.Branches.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: Branches/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Phone,City,Address,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Branch branch)
        {
            if (ModelState.IsValid)
            {
				branch.IsDeleted=false;
				branch.CreationDate= DateTime.Now; 
					
                branch.Id = Guid.NewGuid();
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Phone,City,Address,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Branch branch)
        {
            if (ModelState.IsValid)
            {
				branch.IsDeleted=false;
					branch.LastModifiedDate=DateTime.Now;
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Branch branch = db.Branches.Find(id);
			branch.IsDeleted=true;
			branch.DeletionDate=DateTime.Now;
 
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

        [Route("Branch")]
        [AllowAnonymous]
        public ActionResult List()
        {
            BranchViewModel result=new BranchViewModel()
            {
                Branches = db.Branches.Where(a => a.IsDeleted == false).OrderBy(a => a.City).ToList()
            };
            return View(result);
        }

    }
}
