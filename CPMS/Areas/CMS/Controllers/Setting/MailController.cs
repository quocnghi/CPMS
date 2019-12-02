using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class MailController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: Mail
        //giao dien cho truong ban soan thao
        public ActionResult HopThuDen()
        {
            return View();
        }
        //giao dien cho ng duoc phan cong xay dung de cuong
        public ActionResult HopThuDenGV()
        {
            return View();
        }

        [HttpPost]
        public JsonResult loadMail()
        {
            try
            {
                HttpCookie ck = Request.Cookies["Id"];
                var userid = ck.Value;
                var email = db.sf_Notification.Where(s => s.NguoiNhan == userid).Select(s => new { s.MaTB, s.DaXem, s.Thongtin, s.Kieu, s.Ngaytao, s.Chude, Tinhtrang = s.tc_DecuongGV.Trangthai });
                return Json(new { emails = email });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        public JsonResult getSeen(int id)
        {
            try
            {
                var rs = db.sf_Notification.Find(id);
                rs.DaXem = true;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HttpCookie ck = Request.Cookies["Id"];
                var userid = ck.Value;
                var email = db.sf_Notification.Where(s => s.NguoiNhan == userid).Select(s => new { s.MaTB, s.Nguon, s.DaXem, s.Thongtin, s.Kieu, s.Ngaytao, s.Trangthai, s.Chude, Tinhtrang = s.tc_DecuongGV.Trangthai, s.MaDC });
                return Json(new { emails = email });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        public JsonResult Accept(int id)
        {
            try
            {
                var rs = db.tc_DecuongGV.Find(id);
                rs.Trangthai = true;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HttpCookie ck = Request.Cookies["Id"];
                var userid = ck.Value;
                var email = db.sf_Notification.Where(s => s.NguoiNhan == userid).Select(s => new { s.MaTB, s.Nguon, s.DaXem, s.Thongtin, s.Kieu, s.Ngaytao, s.Trangthai, s.Chude, Tinhtrang = s.tc_DecuongGV.Trangthai, s.MaDC });
                return Json(new { emails = email });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        public JsonResult Decline(int id)
        {
            try
            {
                var rs = db.tc_DecuongGV.Find(id);
                rs.Trangthai = false;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HttpCookie ck = Request.Cookies["Id"];
                var userid = ck.Value;
                var email = db.sf_Notification.Where(s => s.NguoiNhan == userid).Select(s => new { s.MaTB, s.Nguon, s.DaXem, s.Thongtin, s.Kieu, s.Ngaytao, s.Trangthai, s.Chude, Tinhtrang = s.tc_DecuongGV.Trangthai, s.MaDC });
                return Json(new { emails = email });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }
    }
}