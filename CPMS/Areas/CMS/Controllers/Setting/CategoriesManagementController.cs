using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;
using Capstone.Areas.CMS.Models;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class CategoriesManagementController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: CategoriesManagement
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult QuanlyHeNganh()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadInfo()
        {
            var lsthe = db.sc_HeNganh.Where(s => s.MaQH == null).Select(s => new { s.MaHN, s.Mota, s.MaDK, KHOA = s.sc_Khoa.TenKhoa, s.Tenrutgon });
            var lstnganh = db.sc_HeNganh.Select(s => new { s.MaHN, s.Mota, s.MaDK, KHOA = s.sc_Khoa.TenKhoa, s.Tenrutgon, TenHe = s.sc_HeNganh2.Mota, s.MaQH });
            var lstkhoa = db.sc_Khoa.Select(s => new { s.MaKhoa, s.TenKhoa });
            return Json(new { list = lsthe, listkhoa = lstkhoa, listhnganh = lstnganh });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addHenganh(HenganhDT hnganh)
        {
            try
            {
                var he = new sc_HeNganh();
                he.MaKhoa = hnganh.Henganh.MaKhoa;
                he.Mota = hnganh.TenHe;
                db.sc_HeNganh.Add(he);
                db.SaveChanges();
                var lhe = db.sc_HeNganh.AsEnumerable().LastOrDefault();
                var nganh = new sc_HeNganh();
                nganh.MaKhoa = hnganh.Henganh.MaKhoa;
                nganh.Mota = hnganh.Henganh.Mota;
                nganh.MaQH = lhe.MaHN;
                db.sc_HeNganh.Add(nganh);
                db.SaveChanges();
                var lstnganh = db.sc_HeNganh.Select(s => new { s.MaHN, s.Mota, s.MaDK, KHOA = s.sc_Khoa.TenKhoa, s.Tenrutgon, TenHe = s.sc_HeNganh2.Mota });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themhenganh, heNganh = lstnganh });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addHe(sc_HeNganh hnganh)
        {
            try
            {
                db.sc_HeNganh.Add(hnganh);
                db.SaveChanges();
                var lstnganh = db.sc_HeNganh.Select(s => new { s.MaHN, s.Mota, s.MaDK, KHOA = s.sc_Khoa.TenKhoa, s.Tenrutgon, TenHe = s.sc_HeNganh2.Mota });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themhenganh, he = lstnganh });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult gethenganh(sc_HeNganh He)
        {
            var lstnganh = db.sc_HeNganh.Where(s => s.MaHN == He.MaHN).Select(s => new { s.MaHN,s.MaQH, s.Mota, s.MaKhoa, s.MaDK, KHOA = s.sc_Khoa.TenKhoa, s.Tenrutgon, TenHe = s.sc_HeNganh2.Mota }).FirstOrDefault();
            var lstkhoa = db.sc_Khoa.Select(s => new { s.MaKhoa, s.TenKhoa });
            return Json(new { He = lstnganh, listkhoa = lstkhoa });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult edithenganh(sc_HeNganh He)
        {
            try
            {
                var Ng = db.sc_HeNganh.Find(He.MaHN);
                Ng.Mota = He.Mota;
                Ng.MaDK = He.MaDK;
                Ng.MaKhoa = He.MaKhoa;
                Ng.Tenrutgon = He.Tenrutgon;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_HeNganh.Select(s => new { s.MaHN, s.MaQH, s.Mota, s.MaDK, s.Tenrutgon, s.sc_Khoa.MaKhoa, KHOA = s.sc_Khoa.TenKhoa, TenHe = s.sc_HeNganh2.Mota });

                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Suahenganh, HNganh = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }
        }

        //[HttpPost]
        //[Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        //public JsonResult deletehenganh(int id)
        //{
        //    try
        //    {
        //        var rs = db.sc_HeNganh.Find(id);
        //        db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
        //        db.SaveChanges();
        //        return Json(new { msg = "Xóa thành công hệ ngành" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { msg = "Có lỗi xảy ra. Vui lòng thử lại" });
        //    }
        //}


        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult QuanlyKhoa()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            return View();
        }
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadKhoa()
        {
            var lstkhoa = db.sc_Khoa.Select(s => new { s.MaKhoa, s.TenKhoa});
            return Json(new { list = lstkhoa });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addKhoa(sc_Khoa khoa)
        {
            try
            {
                db.sc_Khoa.Add(khoa);
                db.SaveChanges();
                var ng = db.sc_Khoa.Select(s => new { s.MaKhoa, s.TenKhoa });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themkhoa, KHoa = ng });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getKhoa(sc_Khoa lop)
        {
            var rs = db.sc_Khoa.Where(s => s.MaKhoa == lop.MaKhoa).Select(s => new { s.MaKhoa, s.TenKhoa }).FirstOrDefault();
            return Json(new { KhOa = rs });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editKhoa(sc_Khoa Hn)
        {
            try
            {
                var Ng = db.sc_Khoa.Find(Hn.MaKhoa);
                Ng.TenKhoa = Hn.TenKhoa;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_Khoa.Select(s => new { s.MaKhoa, s.TenKhoa });

                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Suakhoa, khoA = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }
        }

        //[HttpPost]
        //[Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        //public JsonResult deleteKhoa(int id)
        //{
        //    try
        //    {
        //        var rs = db.sc_Khoa.Find(id);
        //        db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
        //        db.SaveChanges();
        //        return Json(new { msg = "Xóa thành công khoa" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { msg = "Có lỗi xảy ra. Vui lòng thử lại" });
        //    }
        //}

        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult QuanlyKhoiLop()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadList()
        {
            var lstkhoi = db.sc_Khoilop.Select(s => new { s.MaKhoi, s.TenKhoi, s.NamBD, s.NamKT });
            return Json(new { list = lstkhoi });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addKhoilop(sc_Khoilop lop)
        {
            try
            {
                db.sc_Khoilop.Add(lop);
                db.SaveChanges();
                var ng = db.sc_Khoilop.Select(s => new { s.MaKhoi, s.TenKhoi, s.NamBD, s.NamKT });
                return Json(new { mg = NotificationManagement.SuccessMessage.DM_Themkhoilop, LOP = ng });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getKhoilop(sc_Khoilop lop)
        {
            var rs = db.sc_Khoilop.Where(s => s.MaKhoi == lop.MaKhoi).Select(s => new { s.MaKhoi, s.TenKhoi, s.NamBD, s.NamKT }).FirstOrDefault();
            return Json(new { klop = rs });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editKhoilop(sc_Khoilop Hn)
        {
            try
            {
                var Ng = db.sc_Khoilop.Find(Hn.MaKhoi);
                Ng.TenKhoi = Hn.TenKhoi;
                Ng.NamBD = Hn.NamBD;
                Ng.NamKT = Hn.NamKT;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_Khoilop.Select(s => new { s.MaKhoi, s.TenKhoi, s.NamKT, s.NamBD });

                return Json(new { mg = NotificationManagement.SuccessMessage.DM_Suakhoilop, khoi = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }
        }

        //[HttpPost]
        //[Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        //public JsonResult deleteKhoilop(int id)
        //{
        //    try
        //    {
        //        var rs = db.sc_Khoilop.Find(id);
        //        db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
        //        db.SaveChanges();
        //        return Json(new { mg = "Xóa thành công khối lớp" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { msg = "Có lỗi xảy ra. Vui lòng thử lại" });
        //    }
        //}

        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult QuanlyKhoiKT()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            return View();
        }
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadKhoiKT()
        {
            var lstkhoi = db.sc_Khoikienthuc.Where(s => s.MaQH == null).Select(s => new { s.MaKhoiKT, s.Mota, s.MaQH});
            var lst = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota, s.MaQH, TenLoai = s.sc_Khoikienthuc2.Mota });
            return Json(new { list = lstkhoi, ls = lst });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addKhoiKT(KhoiKienThuc khoi)
        {
            try
            {
                var kt = new sc_Khoikienthuc();
                kt.Mota = khoi.TenLoai;
                db.sc_Khoikienthuc.Add(kt);
                db.SaveChanges();
                var loai = db.sc_Khoikienthuc.AsEnumerable().LastOrDefault();
                var tenkt = new sc_Khoikienthuc();
                tenkt.Mota = khoi.khoikienthuc.Mota;
                tenkt.MaQH = loai.MaKhoiKT;
                db.sc_Khoikienthuc.Add(tenkt);
                db.SaveChanges();
                var lstKienthuc = db.sc_Khoikienthuc.Select(s => new {s.MaKhoiKT,s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota,s.MaQH });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themkhoikienthuc, KHoi = lstKienthuc });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addKKT(sc_Khoikienthuc khoi)
        {
            try
            {
                db.sc_Khoikienthuc.Add(khoi);
                db.SaveChanges();
                var lstKienthuc = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themkhoikienthuc, kienthuc = lstKienthuc });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addLoai(sc_Khoikienthuc khoi)
        {
            try
            {
                db.sc_Khoikienthuc.Add(khoi);
                db.SaveChanges();
                var lstKienthuc = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota, s.MaQH });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themkhoikienthuc, loaikt = lstKienthuc });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getKhoiKT(sc_Khoikienthuc kt)
        {
            var rs = db.sc_Khoikienthuc.Where(s => s.MaKhoiKT == kt.MaKhoiKT).Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota,s.MaQH }).FirstOrDefault();
            return Json(new { KhOi = rs });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editKhoiKT(sc_Khoikienthuc KT)
        {
            try
            {
                var Ng = db.sc_Khoikienthuc.Find(KT.MaKhoiKT);
                Ng.Mota = KT.Mota;
                Ng.MaKhoiKT = KT.MaKhoiKT;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota,TenLoai = s.sc_Khoikienthuc2.Mota, s.MaQH });

                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Suakhoikienthuc, kthuc = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }
        }
    }
}