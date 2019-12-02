using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class ConfigManagementController : Controller
    {
        fit_misDBEntities db = new fit_misDBEntities();
        // GET: ConfigManagement
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult Config()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadInfo()
        {
            var lstma = db.sc_Ma.Select(s => new { s.MaCH, s.LoaiMa, s.TenMa, s.Phienban });
            return Json(new { list = lstma });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN)]
        public JsonResult getCode(int id)
        {
            var rs = db.sc_Ma.Where(s => s.MaCH == id).Select(s => new { s.MaCH, s.TenMa, s.LoaiMa, s.Phienban }).FirstOrDefault();
            return Json(new { code = rs });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Edit(sc_Ma ma)
        {
            try
            {
                var m = db.sc_Ma.Find(ma.MaCH);
                m.TenMa = ma.TenMa;
                db.Entry(m).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_Ma.Select(s => new { s.MaCH, s.LoaiMa, s.TenMa, s.Phienban });
                return Json(new { msg = NotificationManagement.SuccessMessage.CH_Suacauhinh, code = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.CH_Suadulieu });
            }
        }
    }
}