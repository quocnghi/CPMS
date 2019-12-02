using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ProgramManagement.Models;


namespace ProgramManagement.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Charts()
        {
            return View();
        }

        public ActionResult Widgets()
        {
            return View();
        }

        public ActionResult Tables()
        {
            return View();
        }

        public ActionResult Grid()
        {
            return View();
        }

        public ActionResult Form_Common()
        {
            return View();
        }

        public ActionResult Form_Validation()
        {
            return View();
        }

        public ActionResult Form_Wizard()
        {
            return View();
        }

        public ActionResult Buttons()
        {
            return View();
        }

        public ActionResult Interface()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult Invoice()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }

        public ActionResult Error403()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Error405()
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }


    }
}