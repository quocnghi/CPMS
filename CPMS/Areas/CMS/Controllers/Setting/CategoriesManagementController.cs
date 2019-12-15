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
            DataInfoModel model = loadDataInfo();

            return View(model);
        }

        /// <summary>
        ///  Lấy data từ databse bao gồm danh sách hệ, danh sách ngành, danh sách khoa
        /// </summary>
        /// <returns>model chứa các thông tin trên</returns>
        public DataInfoModel loadDataInfo()
        {
            DataInfoModel dataInfo = new DataInfoModel();
            List<HeModel> lsthe = db.sc_HeNganh.Where(s => s.MaQH == null).ToList().
                Select(m => new HeModel { MaHN = m.MaHN, MaDK = m.MaDK, Mota = m.Mota, KHOA = m.sc_Khoa.TenKhoa, Tenrutgon = m.Tenrutgon }).ToList();

            List<NganhModel> lstnganh = db.sc_HeNganh.
                Select(m => new NganhModel { MaHN = m.MaHN, MaDK = m.MaDK, Mota = m.Mota, KHOA = m.sc_Khoa.TenKhoa, Tenrutgon = m.Tenrutgon, TenHe = m.sc_HeNganh2.Mota, MaQH = m.MaQH }).ToList();

            List<KhoaModel> lstkhoa = db.sc_Khoa.Select(s => new KhoaModel { MaKhoa = s.MaKhoa, TenKhoa = s.TenKhoa }).ToList();

            dataInfo.lstkhoa = lstkhoa;
            dataInfo.lstnganh = lstnganh;
            dataInfo.lsthe = lsthe;

            return dataInfo;
        }

        /// <summary>
        /// Load thông tin cần chỉnh sửa
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public PartialViewResult EditHeNganhAction(int index)
        {
            DataInfoModel dataInfo = new DataInfoModel();
            dataInfo.lstnganh = new List<NganhModel>();

            List<NganhModel> lstnganh = db.sc_HeNganh.
                Select(s => new NganhModel { MaHN = s.MaHN, MaQH = s.MaQH, Mota = s.Mota, MaKhoa = s.MaKhoa, MaDK = s.MaDK, KHOA = s.sc_Khoa.TenKhoa, Tenrutgon = s.Tenrutgon, TenHe = s.sc_HeNganh2.Mota }).ToList();

            NganhModel item = lstnganh[index];

            List<KhoaModel> lstkhoa = db.sc_Khoa.Select(s => new KhoaModel { MaKhoa = s.MaKhoa, TenKhoa = s.TenKhoa }).ToList();

            dataInfo.lstnganh.Add(item);
            dataInfo.lstkhoa = lstkhoa;

            return PartialView(dataInfo);
        }

        /// <summary>
        /// lưu thông được chỉnh sửa
        /// </summary>
        /// <param name="He">thông tin hệ</param>
        /// <returns>true: nếu thành công false nếu thất bại</returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult SaveEditHN(string MaHN, string MaQH, string Mota, string Tenrutgon, string MaDK)
        {
            try
            {
                var Ng = db.sc_HeNganh.Find(int.Parse(MaHN));
                Ng.MaQH = int.Parse(MaQH);
                Ng.Mota = Mota;
                Ng.MaDK = MaDK;
                Ng.Tenrutgon = Tenrutgon;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
            }
            return RedirectToAction("QuanlyHeNganh");

        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult SaveEditHe(string MaHN, string MaKhoa, string HnMota)
        {
            try
            {
                var Ng = db.sc_HeNganh.Find(int.Parse(MaHN));
                Ng.Mota = HnMota;
                Ng.MaKhoa = int.Parse(MaKhoa);
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
            }
            return RedirectToAction("QuanlyHeNganh");
        }

        /// <summary>
        /// thêm mới hệ ngành
        /// </summary>
        /// <param name="HnKhoa"> mã khoa</param>
        /// <param name="HnTenhe">tên hệ</param>
        /// <param name="HnMota"> mô tả</param>
        /// <param name="HnTenrutgon">tên rút ngọn</param>
        /// <param name="HnMaDK"> mã đăng ký</param>
        /// <returns></returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult AddHeNganh(string HnKhoa, string HnTenhe, string HnMota, string HnTenrutgon, string HnMaDK)
        {
            try
            {
                sc_HeNganh he = new sc_HeNganh();
                he.MaKhoa = int.Parse(HnKhoa);
                he.Mota = HnTenhe;
                db.sc_HeNganh.Add(he);
                db.SaveChanges();

                var lhe = db.sc_HeNganh.AsEnumerable().LastOrDefault();
                var nganh = new sc_HeNganh();
                nganh.MaKhoa = int.Parse(HnKhoa);
                nganh.Mota = HnMota;
                nganh.MaQH = lhe.MaHN;
                nganh.MaDK = HnMaDK;
                nganh.Tenrutgon = HnTenrutgon;
                db.sc_HeNganh.Add(nganh);
                db.SaveChanges();

                return RedirectToAction("QuanlyHeNganh");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyHeNganh");
        }
        /// <summary>
        /// thêm mới hệ
        /// </summary>
        /// <param name="HnKhoa"> mã khoa</param>
        /// <param name="HnMota">mô tả</param>
        /// <returns></returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult AddHe(string HnKhoa, string HnMota)
        {
            sc_HeNganh hnganh = new sc_HeNganh();
            hnganh.MaKhoa = int.Parse(HnKhoa);
            hnganh.Mota = HnMota;

            try
            {
                db.sc_HeNganh.Add(hnganh);
                db.SaveChanges();
                return RedirectToAction("QuanlyHeNganh");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyHeNganh");
        }

        /// <summary>
        /// thêm mới ngành
        /// </summary>
        /// <param name="MaQH"></param>
        /// <param name="HnMota">mô tả</param>
        /// <param name="HnTenrutgon">tên rút ngọn</param>
        /// <param name="HnMaDK">mã đang ký</param>
        /// <returns></returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult AddNganh(string MaQH, string HnMota, string HnTenrutgon, string HnMaDK)
        {
            sc_HeNganh nganh = new sc_HeNganh();
            nganh.Mota = HnMota;
            nganh.MaQH = int.Parse(MaQH);
            nganh.MaDK = HnMaDK;
            nganh.Tenrutgon = HnTenrutgon;

            try
            {
                db.sc_HeNganh.Add(nganh);
                db.SaveChanges();
                return RedirectToAction("QuanlyHeNganh");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyHeNganh");
        }


        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult QuanlyKhoa()
        {
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;

            List<sc_Khoa> DataInfo = loadKhoa();

            return View(DataInfo);
        }
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public List<sc_Khoa> loadKhoa()
        {
            List<sc_Khoa> lstkhoa = new List<sc_Khoa>();
            lstkhoa = db.sc_Khoa.ToList();
            return lstkhoa;
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult AddKhoa(string TenKhoa)
        {
            sc_Khoa khoa = new sc_Khoa();
            khoa.TenKhoa = TenKhoa;
            try
            {
                db.sc_Khoa.Add(khoa);
                db.SaveChanges();
                return RedirectToAction("QuanlyKhoa");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoa");
        }

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

        public PartialViewResult EditKhoaAction(int index)
        {
            sc_Khoa dataInfo = new sc_Khoa();

            dataInfo = db.sc_Khoa.Where(s => s.MaKhoa == index).FirstOrDefault();
            return PartialView(dataInfo);
        }

        public JsonResult getKhoa(sc_Khoa lop)
        {
            var rs = db.sc_Khoa.Where(s => s.MaKhoa == lop.MaKhoa).Select(s => new { s.MaKhoa, s.TenKhoa }).FirstOrDefault();
            return Json(new { KhOa = rs });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult SaveEditKhoa(int maKH, string TenKhoa)
        {

            try
            {
                var Ng = db.sc_Khoa.Find(maKH);
                Ng.TenKhoa = TenKhoa;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("QuanlyKhoa");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoa");
        }
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

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public bool DeleteKhoa(int maKH)
        {
            try
            {
                var rs = db.sc_Khoa.Find(maKH);
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
            }
            return false;
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
            List<sc_Khoilop> data = loadList();
            return View(data);
        }

        private List<sc_Khoilop> loadList()
        {
            List<sc_Khoilop> lstkhoi = db.sc_Khoilop.ToList();
            //var lstkhoi = db.sc_Khoilop.Select(s => new { s.MaKhoi, s.TenKhoi, s.NamBD, s.NamKT });
            return lstkhoi;
        }

        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult AddKhoi(string TenKhoi, string NamBD, string NamKT)
        {
            sc_Khoilop lop = new sc_Khoilop();
            lop.TenKhoi = TenKhoi;
            lop.NamBD = NamBD;
            lop.NamKT = NamKT;

            db.Entry(lop).State = System.Data.Entity.EntityState.Added;
            try
            {
                db.sc_Khoilop.Add(lop);
                db.SaveChanges();
                return RedirectToAction("QuanlyKhoiLop");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiLop");
        }

        public PartialViewResult EditKhoiLopAction(int index)
        {
            sc_Khoilop dataInfo = new sc_Khoilop();

            dataInfo = db.sc_Khoilop.Where(s => s.MaKhoi == index).FirstOrDefault();
            return PartialView(dataInfo);
        }

        public ActionResult SaveEditKhoi(int maKH, string TenKhoi, string NamBD, string NamKT)
        {
            try
            {
                var Ng = db.sc_Khoilop.Find(maKH);
                Ng.TenKhoi = TenKhoi;
                Ng.NamBD = NamBD;
                Ng.NamKT = NamKT;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiLop");
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
            DataInfoModel data = new DataInfoModel();
            data = loadKhoiKT();
            return View(data);
        }
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public DataInfoModel loadKhoiKT()
        {
            DataInfoModel data = new DataInfoModel();
            data.lstkhoi = data.lst = new List<sc_Khoikienthuc>();
            //{ s.MaKhoiKT, s.Mota, s.MaQH }
            data.lstkhoi = db.sc_Khoikienthuc.Where(s => s.MaQH == null).ToList();
            // s.MaKhoiKT, s.Mota, s.MaQH, TenLoai = s.sc_Khoikienthuc2.Mota
            data.lst = db.sc_Khoikienthuc.ToList();
            return data;
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]

        public ActionResult AddKhoiKT(string Tenloai, string Mota)
        {
            try
            {
                var kt = new sc_Khoikienthuc();
                kt.Mota = Tenloai;
                db.sc_Khoikienthuc.Add(kt);
                db.SaveChanges();
                var loai = db.sc_Khoikienthuc.AsEnumerable().LastOrDefault();
                var tenkt = new sc_Khoikienthuc();
                tenkt.Mota = Mota;
                tenkt.MaQH = loai.MaKhoiKT;
                db.sc_Khoikienthuc.Add(tenkt);
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiKT");
        }

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
                var lstKienthuc = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota, s.MaQH });
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Themkhoikienthuc, KHoi = lstKienthuc });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]

        public ActionResult AddKKT(string MaQH, string HnMota)
        {
            sc_Khoikienthuc khoi = new sc_Khoikienthuc();
            khoi.MaQH = int.Parse(MaQH);
            khoi.Mota = HnMota;

            try
            {
                db.sc_Khoikienthuc.Add(khoi);
                db.SaveChanges();
                return RedirectToAction("QuanlyKhoiKT");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiKT");
        }

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
        public ActionResult AddLoai(string tenktMota)
        {
            sc_Khoikienthuc khoi = new sc_Khoikienthuc();
            khoi.Mota = tenktMota;

            try
            {
                db.sc_Khoikienthuc.Add(khoi);
                db.SaveChanges();
                return RedirectToAction("QuanlyKhoiKT");
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiKT");
        }

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
            var rs = db.sc_Khoikienthuc.Where(s => s.MaKhoiKT == kt.MaKhoiKT).Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota, s.MaQH }).FirstOrDefault();
            return Json(new { KhOi = rs });
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public PartialViewResult EditKhoiKT(int id)
        {
            sc_Khoikienthuc Ng = db.sc_Khoikienthuc.Where(s => s.MaKhoiKT == id).FirstOrDefault();
            DataInfoModel data = new DataInfoModel();
            data.lst = new List<sc_Khoikienthuc>();
            data.lst.Add(Ng);
            data.lstkhoi = db.sc_Khoikienthuc.Where(s => s.MaQH == null).ToList();

            return PartialView(data);
        }

        public ActionResult SavEditKhoi(int MaKhoiKT, string HnMota)
        {
            try
            {
                var Ng = db.sc_Khoikienthuc.Find(MaKhoiKT);
                Ng.Mota = HnMota;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiKT");
        }

        public ActionResult SavEditKhoiKT(int MaKhoiKT, string MaQH, string HnMota)
        {
            try
            {
                var Ng = db.sc_Khoikienthuc.Find(MaKhoiKT);
                Ng.Mota = HnMota;
                Ng.MaQH = int.Parse(MaQH);
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("QuanlyKhoiKT");
        }
        public JsonResult editKhoiKT(sc_Khoikienthuc KT)
        {
            try
            {
                var Ng = db.sc_Khoikienthuc.Find(KT.MaKhoiKT);
                Ng.Mota = KT.Mota;
                Ng.MaKhoiKT = KT.MaKhoiKT;
                db.Entry(Ng).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var rs = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota, TenLoai = s.sc_Khoikienthuc2.Mota, s.MaQH });

                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Suakhoikienthuc, kthuc = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DM_Chinhsuadulieu });
            }
        }
    }
}