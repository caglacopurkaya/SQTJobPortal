using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SQTJobPortal.Models;
using System.Net.Mail;

namespace SQTJobPortal.Controllers
{
    public class HomeController : Controller
    {

        private readonly SQTJobPortalEntities1 db = new SQTJobPortalEntities1();
    
        public ActionResult Index()
        {
            return View();
        }

   
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(Contact p1)
        {
            ViewBag.Message = "Your contact page.";
            db.Contact.Add(p1);
            db.SaveChanges();


            TempData["success"] = "Your Mail Succesfully Sended!";

            if (p1.Email.Contains("hotmail"))
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("mediagnosis@hotmail.com", "caglacopurkaya1999");
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                message.To.Add(p1.Email);
                message.From = new MailAddress("mediagnosis@hotmail.com");
                message.Subject = "Contact Process";
                message.Body = "Dear" + " " + p1.Name + " " + "your mail has been succesfully sended." + "We will get back to you shortly.";


                client.Send(message);
                return RedirectToAction("Index");
            }

            else
            {

                TempData["failed"] = "Please fill all blanks!";
                return RedirectToAction("Contact");
            }


        }
     
    }
}