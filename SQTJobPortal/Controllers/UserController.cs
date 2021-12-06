using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        [HttpGet]
        public ActionResult Create(int id)
        {
            var model = new ViewModel2();
          
            model.companies= db.User.FirstOrDefault(x => x.SeekerId == id);
            CompanyHelper.id = model.companies.SeekerId;
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName");
            ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName");

            return View(model);
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewModel3 viewModel)
        {
               
             
                ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "CategoryName", viewModel.CategoryId);
                ViewBag.ProfessionId = new SelectList(db.Professions, "ProfessionId", "ProfessionName", viewModel.ProfessionId);
                //if (findCategoryId!=null && findProfessionId!=null)
                //{
                Job jobs = new Job();
                    jobs.UserId = CompanyHelper.id;
                    jobs.Title = viewModel.Title;
                    jobs.Description = viewModel.Description;
                    jobs.Experience = viewModel.Experience;
                    jobs.PostedDate = viewModel.PostedDate;
                    jobs.DueToApply = viewModel.DueToApply;
                    jobs.Salary = viewModel.Salary;
                    jobs.CategoryId = viewModel.CategoryId;
                    jobs.ProfessionId = viewModel.ProfessionId;
               


                    db.Job.Add(jobs);
                    db.SaveChanges();
                    TempData["success"] = "Your job is succesfully created!";

                    return RedirectToAction("Index","Account", viewModel);
              
                
          
           
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
            var infocomp = db.JobRequest.Where(x => x.RequestId == id).FirstOrDefault();

            return View(infocomp);
        }


      

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestEdit(JobRequest req)
        {
            //var student = studentList.Where(s => s.StudentId == std.StudentId).FirstOrDefault();
            //studentList.Remove(student);
            //studentList.Add(std);

            var reqToUpdate = db.JobRequest.Find(req.RequestId);
            //if (reqToUpdate == null)
            //{
            //    return HttpNotFound();
            //}
            //else
            //{
            //
            reqToUpdate.IsActive = req.IsActive;
            reqToUpdate.Email = req.Email;
            reqToUpdate.User.Name = req.User.Name;
            reqToUpdate.Job.Title = req.Job.Title;

            db.SaveChanges();

          

            if (req.IsActive==true &&req.Email.Contains("hotmail"))
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("dreamjobopportunities@hotmail.com", "caglacopurkaya1999");
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                message.To.Add(req.Email);
                message.From = new MailAddress("dreamjobopportunities@hotmail.com");
                message.Subject = "Request Response from DreamJob";
                message.Body = "Dear" + " " + req.User.Name + " " + "You have successfully passed the first stage of your application for" + " " + req.Job.Title +"."+ " " + " Please wait for the company to contact you. " + " " + " We wish you a good day!";
                client.Send(message);
            }
            else if (req.IsActive == true && req.Email.Contains("gmail"))
            {
                var fromAddress = new MailAddress("eceak1230@gmail.com");
                var toAddress = new MailAddress(req.Email);
                const string fromPassword = "5EF2kRZ5";
                const string subject = "Request Response from DreamJob";
                string body = "Dear" + " " + req.User.Name + " " + "You have successfully passed the first stage of your application for" + " " + req.Job.Title + "." + " " + "Please wait for the company to contact you. " + " " + "We wish you a good day!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }

            else if(req.IsActive == false && req.Email.Contains("hotmail"))
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("dreamjobopportunities@hotmail.com", "caglacopurkaya1999");
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                message.To.Add(req.Email);
                message.From = new MailAddress("dreamjobopportunities@hotmail.com");
                message.Subject = "Request Response from DreamJob";
                message.Body = "Dear" + " " + req.User.Name + " " + "You are not pass the first stage of your application. We are unable to proceed positively for " + " "+req.Job.Title + " " + "with you. But keep searching our site for other alternatives. " + " " + "We wish you a good day!";
                client.Send(message);
            }

            else if (req.IsActive == false && req.Email.Contains("gmail"))
            {
                var fromAddress = new MailAddress("eceak1230@gmail.com");
                var toAddress = new MailAddress(req.Email);
                const string fromPassword = "5EF2kRZ5";
                const string subject = "Request Response from DreamJob";
                string body = "Dear" + " " + req.User.Name + " " + "You are not pass the first stage of your application. We are unable to proceed positively for " + " " + req.Job.Title + " " + "with you. But keep searching our site for other alternatives. " + " " + "We wish you a good day!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            req.Job.UserId = CompanyHelper.id;
            ViewBag.UpdatedMessage = "Status Updated Succesfully!";
            return View("Requests", req);
               
              

            //}
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