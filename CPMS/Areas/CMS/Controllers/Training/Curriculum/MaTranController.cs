using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;
using Capstone.Areas.CMS.Models;

namespace Capstone.Areas.CMS.Controllers.Training.Curriculum
{
    public class MaTranController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: MaTran
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult DSMaTran()
        {
            //lấy danh sách các chương trình đào tạo
            var lst = db.t_CTDaotao.ToList();
            return View(lst);
        }
        public ActionResult NDCDRView(int ID)
        {
            //lấy mã user gán vào cookie
            HttpCookie ck = Request.Cookies["Id"];
            var id = ck.Value;
            //tìm role tương ứng với mã đã nhớ
            var role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault().Name;
            ViewBag.Role = role;
            //lấy mã ctdt gán vào cookie
            HttpCookie ctdt = new HttpCookie("MaTranID");
            ctdt.Value = ID.ToString();
            ctdt.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(ctdt);
            return View();
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult loadKHDTList()
        {
            //lấy mã user gán vào cookie
            HttpCookie ck = Request.Cookies["Id"];
            var userid = ck.Value;
            //lấy mã ctdt gán vào cookie
            HttpCookie ctdt = Request.Cookies["MaTranID"];
            var id = ctdt.Value;
            int ctdtId = int.Parse(id);
            List<tc_KHDaotao> khdt = new List<tc_KHDaotao>();
            var lstkhdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ctdtId).Select(s => new {s.Hocky,s.MaCTDT,MaHP = s.sc_Hocphan.MaHT, s.MaKHDT,s.SoTC,TenHP = s.sc_Hocphan.TenMH});
                return Json(new { khdt = lstkhdt });
           
        }
        public JsonResult LoadHpList(int id)
        {
            var lsthp = db.tc_KHDaotao.Where(s => s.MaCTDT == id).Select(s => new { s.Hocky, s.MaCTDT, s.MaKHDT, s.SoTC, TenHP = s.sc_Hocphan.TenMH, MaCDR = s.t_CDR_CTDT.Select(h => new { h.MaHT }) });
            return Json(new { dsHp = lsthp });
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult loadReviewInfo(int id)
        {
            try
            {
                //lấy dữ liệu từ bảng KHDaotao tương ướng với mã đã có
                var khdt = db.tc_KHDaotao.Where(s => s.MaKHDT == id).Select(s => new { s.Hocky, s.MaCTDT, MaHP = s.sc_Hocphan.MaHT, s.MaKHDT, s.SoTC, TenHP = s.sc_Hocphan.TenMH }).FirstOrDefault();
                //lấy dữ liệu từ bảng chuẩn đầu ra CTDT tương ướng với mã đã có
                var cdrctdt = db.t_CDR_CTDT.Select(s => new { s.MaELO, s.MaHT, s.Mota, s.Phanloai });
                //lấy dữ liệu từ bảng khối kiến thức tương ướng với mã đã có
                var kkt = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, s.Mota });
                //lấy dữ liệu từ bảng chuẩn đầu ra hp tương ướng với mã đã có
                var rs = db.tc_CDR_HP.Where(s => s.MaKHDT == id).Select(s => new { s.MaCELO, s.MaHT, s.Mota, s.Phanloai, MaHT1 = s.tc_MatranCDR.Select(h => new { MaCELO = h.tc_CDR_HP.MaHT, MaELO = h.t_CDR_CTDT.MaHT, h.Mucdo }) });
                //lấy dữ liệu từ bảng tài liệu hp tương ướng với mã đã có
                var tlhp = db.tc_TailieuHP.Where(s => s.MaKHDT == id).Select(s => new { s.MaTL, s.LoaiTL, s.TenTL, s.Tacgia, s.NhaXB, s.NamXB, s.Kieunhap });
                //lấy dữ liệu từ bảng nội dung hp tương ướng với mã đã có
                var nd = db.tc_NoidungHP.Where(s => s.MaKHDT == id).Select(s => new { s.TenHT, s.Phanloai, s.Noidung, s.Ghichu, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaHT }) });
                
                return Json(new { mhoc = khdt, CDR = cdrctdt, KKT = kkt, cdrhp = rs, ndhp = nd, tHp = tlhp });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData});
            }
        }      
    }
}