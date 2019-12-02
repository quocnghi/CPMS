using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class LogHistoryController : Controller
    {
		// GET: LogHistory
		fit_misDBEntities db = new fit_misDBEntities();
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult NhatkyHoatdong()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            return View();
        }
    }
}