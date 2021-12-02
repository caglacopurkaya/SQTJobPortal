using SQTJobPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SQTJobPortal.Controllers
{
    public class AccountController : Controller
    {

        public SQTJobPortalEntities1 db = new SQTJobPortalEntities1();
        // GET: Account
        [Authorize(Roles = "Company")]
        public ActionResult Index()
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
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model, string returnUrl)
        {
            var dataItem = db.User.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if (dataItem != null)
            {
                FormsAuthentication.SetAuthCookie(dataItem.Username, false);
                var getCompanyId = db.User.SingleOrDefault(x => x.Username == model.Username).SeekerId;
                CompanyHelper.id = getCompanyId;
                if (Url.IsLocalUrl(returnUrl)&&returnUrl.Length>1&&returnUrl.StartsWith("/")&&!returnUrl.StartsWith("//")&& !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user/pass");
                return View();
            }

    
        }

       

        [Authorize]
        
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}