using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone.Areas.PMS.Controllers
{
    public class ErrorController : Controller
    {
        //Get 400 Error Page
        public ActionResult BadRequest()
        {
            return View();
        }

        //Get 404 Error Page
        public ActionResult NotFound()
        {
            return View();
        }

        //Get 403 Error Page
        public ActionResult Forbidden()
        {
            return View();
        }

        //Get 500 Error Page
        public ActionResult InternalServerError()
        {
            return View();
        }

        
    }
}