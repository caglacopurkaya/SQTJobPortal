using SQTJobPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize(Roles="Company")]
        public ActionResult Index()
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
            var dataItem = db.User.Where(x => x.Username == model.Username && x.Password == model.Password).First();
            if (dataItem != null)
            {
                FormsAuthentication.SetAuthCookie(dataItem.Username, false);
                if(Url.IsLocalUrl(returnUrl)&&returnUrl.Length>1&&returnUrl.StartsWith("/")&&!returnUrl.StartsWith("//")&& !returnUrl.StartsWith("/\\"))
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