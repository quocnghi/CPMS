using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;
using Capstone.Areas.CMS.Models;

namespace Capstone.Areas.CMS.Controllers.Management
{
    public class QuanLyUsersController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: QuanLyUsers
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult Thongtin(string id)
        {
            var pro = db.AspNetUsers.Where(st => st.Id == id);
            return View(pro);
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult Thongtin(string id, string tehhienthi, string mail, string sdt)
        {
            var tt = db.AspNetUsers.Find(id);
            tt.UserName = tehhienthi;
            tt.Email = mail;
            tt.PhoneNumber = sdt;
            db.Entry(tt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("thongtin", "quanlyusers");
        }
        [Authorize(Roles = ROLES.ADMIN)]
        public ActionResult DsTaikhoan()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            return View();
        }
        [Authorize(Roles = ROLES.ADMIN)]
        public ActionResult DsVaiTro()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadListNhavien()
        {
            var lstnv = db.m_Nhanvien.Select(s => new { s.MaNV, s.Ten, s.Ho, s.Email, s.Gioitinh, s.LoaiGV, s.Ngaysinh, s.SDT, s.Thongtinhocham, s.Thongtinhocvi, s.MaQL });
            var pb = db.sc_Ma.Where(s => s.LoaiMa == Ma.Nhanvien).Select(s => new { s.TenMa, s.MaCH, s.Phienban }).FirstOrDefault();
            return Json(new { list = lstnv, ma = pb });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadListUser()
        {
            var lstuser = db.AspNetUsers.Select(s => new { s.Id, s.UserName, s.Email, Vaitro = s.AspNetRoles.FirstOrDefault().Id, Tenvaitro = s.AspNetRoles.FirstOrDefault().Name, TenND = s.m_Nhanvien.Ten, HoND = s.m_Nhanvien.Ho, Nhanvien = s.m_Nhanvien.LoaiGV, s.MaNV });
            var lstnv = db.m_Nhanvien.Select(s => new { s.MaNV, s.Ten, s.Ho });
            var lstvt = db.AspNetRoles.Select(s => new { s.Id, s.Name });
            return Json(new { list = lstuser, listnv = lstnv, listvt = lstvt });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addNhanvien(m_Nhanvien nv)
        {
            try
            {
                db.m_Nhanvien.Add(nv);
                db.SaveChanges();
                var pb = db.sc_Ma.Find(db.sc_Ma.Where(s => s.LoaiMa == Ma.Nhanvien).FirstOrDefault().MaCH);
                pb.Phienban = nv.MaQL.Substring(nv.MaQL.Length - 3);
                db.Entry(pb).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var nhanvien = db.m_Nhanvien.Select(s => new { s.MaNV, s.Ten, s.Ho, s.Email, s.Gioitinh, s.LoaiGV, s.Ngaysinh, s.SDT, s.Thongtinhocham, s.Thongtinhocvi, s.MaQL });
                return Json(new { msg = NotificationManagement.SuccessMessage.NV_Themnv, list = nhanvien });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.NV_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getNhanvien(m_Nhanvien nv)
        {
            var rs = db.m_Nhanvien.Where(s => s.MaNV == nv.MaNV).Select(s => new { s.MaNV, s.Ten, s.Ho, s.Email, s.Gioitinh, s.LoaiGV, s.Ngaysinh, s.SDT, s.Thongtinhocham, s.Thongtinhocvi, s.MaQL }).FirstOrDefault();
            return Json(new { nv = rs });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editNhanvien(m_Nhanvien nv)
        {
            try
            {
                db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.m_Nhanvien.Select(s => new { s.MaNV, s.MaQL, s.Ten, s.Ho, s.Email, s.Gioitinh, s.LoaiGV, s.Ngaysinh, s.SDT, s.Thongtinhocham, s.Thongtinhocvi });

                return Json(new { msg = NotificationManagement.SuccessMessage.NV_Suanv, list = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.NV_Chinhsuadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult deleteNhanvien(int id)
        {
            try
            {
                var rs = db.m_Nhanvien.Find(id);
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.NV_Xoanv });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.NV_Xoadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editUser(QuanLyUser user)
        {
            try
            {
                var us = db.AspNetUsers.Find(user.Id);
                us.MaNV = user.MaNV;
                db.Entry(us).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var vt = db.AspNetUsers.Find(user.Id);
                foreach (var item in vt.AspNetRoles.ToList())
                {
                    db.AspNetUsers.Find(user.Id).AspNetRoles.Remove(item);
                    db.SaveChanges();
                }
                var vaitro = db.AspNetRoles.Find(user.Vaitro);
                db.AspNetUsers.Find(user.Id).AspNetRoles.Add(vaitro);
                db.SaveChanges();
                var users = db.AspNetUsers.Select(s => new { s.Id, s.UserName, s.Email, Vaitro = s.AspNetRoles.FirstOrDefault().Id, Tenvaitro = s.AspNetRoles.FirstOrDefault().Name, TenND = s.m_Nhanvien.Ten, HoND = s.m_Nhanvien.Ho, Nhanvien = s.m_Nhanvien.LoaiGV, s.MaNV });
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_SuaND, list = users });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.ND_Suadulieu });
            }
        }
    }
}