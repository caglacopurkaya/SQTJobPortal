using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SQTJobPortal.Models;

namespace SQTJobPortal.Controllers
{
    public class UserController : Controller
    {
        // GET: User
     
        private SQTJobPortalEntities1 db = new SQTJobPortalEntities1();

        [Authorize(Roles = "Company,JobSeeker")]
        public ActionResult Index()
        {
            var jobs = db.Job.Include(e => e.Category);
            return View(jobs.ToList());
        }

        public PartialViewResult _CategoryList()
        {
            return PartialView(db.Category.ToList());
        }

        [Authorize(Roles = "Company")]
        public ActionResult CompanyProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.FirstOrDefault(x => x.SeekerId == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.Category, "CountryId", "CountryName", user.CountryId);
            ViewBag.TypeOfProfessionId = new SelectList(db.Professions, "TypeOfProfessionId", "TypeOfProfessionName", user.TypeofProfessionId);

            return View(user);
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyProfile([Bind(Include = "SeekerId,Name,Website,Phone,ContactMail,Address,CountryId,TypeOfProfessionId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Category, "CountryId", "CountryName", user.CountryId);
            ViewBag.TypeOfProfessionId = new SelectList(db.Professions, "TypeOfProfessionId", "TypeOfProfessionName", user.TypeofProfessionId);

            return View(user);
        }


        [Authorize(Roles = "Company,JobSeeker")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Job.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        [Authorize(Roles = "Company")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName");
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName");
            return View();
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JobId,Title,CategoryId,ProfessionId,Description,Experience,PostedDate,DueToApply,Salary,UserId")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Job.Add(job);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName", job.CategoryId);
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName", job.ProfessionId);
          
            return View(job);
        }

        [Authorize(Roles = "Company")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Job.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName", job.CategoryId);
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName", job.ProfessionId);

            return View(job);
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JobId,Title,CategoryId,ProfessionId,Description,Experience,PostedDate,DueToApply,Salary,UserId")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName", job.CategoryId);
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName", job.ProfessionId);

            return View(job);
        }

        [Authorize(Roles = "Company")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Job.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        [Authorize(Roles = "Company")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Job.Find(id);
            db.Job.Remove(job);
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