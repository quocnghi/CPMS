using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class EmailTemplateController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: EmailTemplate
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadEmailList()
        {
            var lstemail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung, s.Phanloai, s.Ghichu });
            return Json(new { email = lstemail });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addEmail(sf_EmailTemplate eMail)
        {
            try
            {
                db.sf_EmailTemplate.Add(eMail);
                db.SaveChanges();
                var lstemail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung, s.Phanloai, s.Ghichu });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themmail, email = lstemail });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }

        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editEmail(sf_EmailTemplate eMail)
        {
            try
            {
                var rs = db.sf_EmailTemplate.Find(eMail.MaET);
                rs.Chude = eMail.Chude;
                rs.Noidung = eMail.Noidung;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var lstemail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung, s.Phanloai, s.Ghichu });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Suamail, email = lstemail });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }

        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getDetailEmail(int id)
        {
            var rs = db.sf_EmailTemplate.Find(id);
            return Json(new { email = rs });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult deleteEmail(int id)
        {
            try
            {
                //tìm mã biểu mẫu tương ướng
                var rs = db.sf_EmailTemplate.Find(id);
                //tìm và xóa biểu mẫu trong bảng biểu mẫu Email
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                //lưu lại
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Xoamail });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Xoadulieu });
            }
        }
    }
}