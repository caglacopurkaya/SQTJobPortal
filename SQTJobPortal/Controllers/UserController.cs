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
        public ActionResult CompanyProfile([Bind(Include = "SeekerId,Name,Username,Password,Email,AccountType,Website,Phone,ContactMail,Address,CountryId,TypeOfProfessionId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CompanyProfile");
            }
            ViewBag.CountryId = new SelectList(db.Category, "CountryId", "CountryName", user.CountryId);
            ViewBag.TypeOfProfessionId = new SelectList(db.Professions, "TypeOfProfessionId", "TypeOfProfessionName", user.TypeofProfessionId);

            return View(user);
        }


        [Authorize(Roles = "Company")]

        public ActionResult Requests()
        {

           
            var testTable = from c in db.User
                            join jc in db.Job on c.SeekerId equals jc.UserId
                            join req in db.JobRequest on jc.JobId equals req.JobId
                            join user in db.User on req.JobSeekerId equals user.SeekerId
                     
                            select new ViewModel
                            {
                                companies = c,
                                jobs = jc,
                                request = req,
                                user = user,
                           


                            };
            ViewData["TestTable"] = testTable;
         

            User company = new User();
            company.SeekerId = CompanyHelper.id;
            return View(company);
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
        public ActionResult Create(int id)
        {
            var model = new ViewModel();
            model.companies= db.User.FirstOrDefault(x => x.SeekerId == id);
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName");
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName");
            return View(model);
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewModel viewModel)
        {
            var findUserId = db.User.FirstOrDefault(x => x.SeekerId == viewModel.companies.SeekerId);
            if (ModelState.IsValid && viewModel.jobs.PostedDate >= DateTime.Now)
            {
                Job jobs = new Job();
                jobs.UserId = CompanyHelper.id;
                db.Job.Add(jobs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName", viewModel.jobs.CategoryId);
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName", viewModel.jobs.ProfessionId);
          
            return RedirectToAction("Index","Account", findUserId.SeekerId);
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



        [Authorize(Roles = "Company")]

        public ActionResult JobAdversiments()
        {

            var jointest = from c in db.User
                           join jc in db.Job on c.SeekerId equals jc.UserId
                           //join j in db.Job on jc.CategoryId equals j.CategoryId
                           join req in db.JobRequest on jc.JobId equals req.JobId
                           //join seeker in db.JobSeeker on req.SeekerId equals seeker.Id
                           //join reqans in db.ReqAnswer on c.Id equals reqans.CompanyId
                           //join conf in db.ConfirmRequest on reqans.AnswerId equals conf.ReqAnsId

                           select new ViewModel
                           {
                               companies = c,
                               jobs = jc,
                               request = req,

                               //request = req,
                               //seeker = seeker
                               //reqanswer = reqans,
                               //confirm = conf

                           };

            ViewData["Jointable"] = jointest;

            User company = new User();
            company.SeekerId = CompanyHelper.id;

            return View(company);
           
        }
        [Authorize(Roles = "Company")]
        public ActionResult RequestEdit(int id)
        {
            var infocomp = db.JobRequest.FirstOrDefault(x => x.RequestId == id);

            return View(infocomp);
        }


      

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestEdit(JobRequest req)
        {
            var reqToUpdate = db.JobRequest.Find(req.RequestId);
            if (reqToUpdate == null)
            {
                return HttpNotFound();
            }
            else
            {

               
                    reqToUpdate.IsActive = req.IsActive;
                    db.SaveChanges();
                    ViewBag.UpdatedMessage = "Status Updated Succesfully!";
                    return View("Requests", reqToUpdate);
               
              

            }
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