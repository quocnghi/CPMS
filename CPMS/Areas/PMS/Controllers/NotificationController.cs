using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Areas.PMS.Controllers;
using Capstone.Areas.PMS.Models;

namespace Capstone.Areas.PMS.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public JsonResult GetMessages(bool checkBol, string emailUser)
        {
            NotificationsRepository noRes = new NotificationsRepository();
            List<object> listNoti = noRes.getListNotiForHub(checkBol,emailUser);

            return Json(listNoti,JsonRequestBehavior.AllowGet);
        }
    }
}