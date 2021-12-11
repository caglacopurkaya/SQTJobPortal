using SQTJobPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SQTJobPortal.Controllers
{
    public class SeekerController : Controller
    {
        // GET: Seeker
        //public ActionResult Index()
        //{
        //    return View();
        //}
        private SQTJobPortalEntities1 db = new SQTJobPortalEntities1();
  

        [Authorize(Roles = "JobSeeker")]
        public ActionResult SeekerProfile(int? id)
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
            

            return View(user);
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SeekerProfile([Bind(Include = "SeekerId,Name,Username,Password,Email,AccountType,GraduationGrade,Phone,Address,Cv,CountryId")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CompanyProfile");
            }
            ViewBag.CountryId = new SelectList(db.Category, "CountryId", "CountryName", user.CountryId);


            return View(user);
        }




        [Authorize(Roles = "JobSeeker")]

        public ActionResult JobDetails(Job job, int id)
        {

            return View(db.Job.Where(i => i.JobId == id).FirstOrDefault());
        }
        //[Authorize(Roles = "JobSeeker")]
        //public ActionResult JobDetails()
        //{
        //    var jointest = from c in db.User
        //                   join jc in db.Job on c.SeekerId equals jc.UserId

        //                   join req in db.JobRequest on jc.JobId equals req.JobId


        //                   select new ViewModel
        //                   {
        //                       companies = c,
        //                       jobs = jc,
        //                       request = req,



        //                   };

        //    ViewData["Jointable"] = jointest;

        //    User user = (User)User.Identity;


        //    return View(user);
        //}



        [Authorize(Roles = "JobSeeker")]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateRequest(int? id)
        {
            var model = new ViewModel2();

            //model.users = db.User.FirstOrDefault(x => x.SeekerId == id);
            model.users = (User)User.Identity;
            if (id== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //model.jobs = db.Job.FirstOrDefault(x => x.JobId == id);
            if (model.users == null)
            {
                return HttpNotFound();
            }

            CompanyHelper.id = model.users.SeekerId;
            //JobHelper.id = model.jobs.JobId;
          
            return View(model);
        }

        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateRequest(ViewModel4 viewModel, HttpPostedFileBase postedFile)
        {

            var findJobId = db.Job.OrderByDescending(x => x.JobId).FirstOrDefault(x => x.JobId== viewModel.JobId && x.JobId == JobHelper.id);
            var findFileId = db.FileDetails.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Id == viewModel.CvId && x.UserId== CompanyHelper.id);
            JobRequest req = new JobRequest();
            req.ApplyDate = viewModel.ApplyDate;
            req.Education = viewModel.Education;
            req.Email = viewModel.Email;
            req.JobId = findJobId.JobId;
            req.JobSeekerId = CompanyHelper.id;
            req.Phone = viewModel.Phone;
            req.Profession = viewModel.Profession;
            req.MotivationLetter = viewModel.MotivationLetter;
            req.CvId = findFileId.Id;


            db.JobRequest.Add(req);
            db.SaveChanges();
            TempData["success"] = "Your request is succesfully created!";

            return RedirectToAction("PastRequests", "Seeker", viewModel);





        }

        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        public ActionResult Cv(int id)
        {
            var model = new ViewModel2();

            model.users = db.User.FirstOrDefault(x => x.SeekerId == id);
            CompanyHelper.id = model.users.SeekerId;
            int files = CompanyHelper.id;
            model.files = db.FileDetails.OrderByDescending(x => x.Id).FirstOrDefault(x => x.User.SeekerId== files);
            return View(model);
        }
        [Authorize(Roles = "JobSeeker")]
        [HttpPost]
        public ActionResult Cv(HttpPostedFileBase postedFile)
        {
            FileDetails file = new FileDetails();
            string filename = System.IO.Path.GetFileNameWithoutExtension(postedFile.FileName);
            string extension = System.IO.Path.GetExtension(postedFile.FileName);
            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
            file.FileName = filename;
            file.FilePath = "~/Uploads/" + filename;
            file.UserId = CompanyHelper.id;
            filename = System.IO.Path.Combine(Server.MapPath("~/Uploads"), filename);
            postedFile.SaveAs(filename);
            using (SQTJobPortalEntities1 db = new SQTJobPortalEntities1())
            {
                db.FileDetails.Add(file);
                db.SaveChanges();

            }
            return RedirectToAction("Cv");
        }

    


        [Authorize(Roles = "JobSeeker")]
        public ActionResult Download(int id)
        {
            SQTJobPortalEntities1 db = new SQTJobPortalEntities1();
            FileDetails file = db.FileDetails.Where(x => x.Id == id).FirstOrDefault();
            if (System.IO.File.Exists(Server.MapPath(file.FilePath)))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath(file.FilePath));
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
            User user = db.User.FirstOrDefault(x => x.SeekerId == id);
            return RedirectToAction("Cv", "Seeker", user);
        }

        public ActionResult PastRequests(ViewModel viewModel,int id)
        {
            var testTable = from req in db.JobRequest
                            join file in db.FileDetails on req.CvId equals file.Id
                            join j in db.Job on req.JobId equals j.JobId
                            join comp in db.User on j.UserId equals comp.SeekerId

                            select new ViewModel
                            {
                                request = req,
                                files = file,
                                jobs = j,
                                companies=comp,
                               

                            };
            ViewData["TestTable"] = testTable;
       
            viewModel.user = db.User.FirstOrDefault(x => x.SeekerId == id);
            int files = CompanyHelper.id;
            viewModel.files = db.FileDetails.OrderByDescending(x => x.Id).FirstOrDefault(x => x.User.SeekerId == files);
   
            return View(viewModel);
        }
    }
}   

