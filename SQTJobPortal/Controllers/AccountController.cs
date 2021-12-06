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

                           join req in db.JobRequest on jc.JobId equals req.JobId


                           select new ViewModel
                           {
                               companies = c,
                               jobs = jc,
                               request = req,



                           };

            ViewData["Jointable"] = jointest;

            User company = new User();
            company.SeekerId = CompanyHelper.id;

            return View(company);
        }


        public ActionResult ErrorPage()
        {
            return View();
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
                else if (dataItem.AccountType == "Company")
                {
                    return RedirectToAction("Index");
                }
                else if (dataItem.AccountType == "JobSeeker")
                {
                    return RedirectToAction("JobSeekerIndex");
                }
                else
                {
                    return RedirectToAction("ErrorPage");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user/pass");
                return View();
            }

    
        }


        public ActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (!ModelState.IsValid)
            {

                return View("Register");

            }
            if (ModelState.IsValid)
            {
                User user = new User();
                var SeekerControl = db.User.FirstOrDefault(x => x.Username == model.Username || x.Email == model.Email);
                if (SeekerControl != null)
                {
                    ViewBag.Message2 = "There is an account with this email or Username please log in your account";
                    return View("Login");
                }
                else
                {


                    user.Name = model.Name;
                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.Password = model.Password;
                    user.AccountType = model.AccountType;


                    db.User.Add(user);
                    db.SaveChanges();
                    TempData["signup"] = "Your account succesfully created. Please Log in!";
                    return RedirectToAction("Login", "Account");
                }
            }

            else
            { ModelState.AddModelError("RegisterUserError", "Register User Error!"); }
            return View(model);
        }

        [Authorize]
        
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        
        //public ActionResult SignUp()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SignUp(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.User.Add(user);
        //        db.SaveChanges();
        //        return RedirectToAction("Login");
        //    }
        //    return View();
        //}

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}