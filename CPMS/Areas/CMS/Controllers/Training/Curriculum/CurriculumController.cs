using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using Capstone.Areas.CMS.Models;

namespace Capstone.Areas.CMS.Controllers.Training.Curriculum
{
    public class CurriculumController : Controller
    {
		fit_misDBEntities db = new fit_misDBEntities();
        // GET: Curriculum
        public ActionResult List(int id)
        {
            //gọi database
			fit_misDBEntities db = new fit_misDBEntities();
            //tạo danh sách nội dung ctdt
            List<NoiDungCtdt> lst = new List<NoiDungCtdt>();
            NoiDungCtdt hp = new NoiDungCtdt();
            //lấy dữ liệu khối kiến thức
            var kkienthuc = db.sc_Khoikienthuc.ToList();
            //lấy dữ liệu kh đào tạo
            var khdaotao = db.tc_KHDaotao.Where(s => s.MaCTDT == id).ToList();
            //lấy dữ liệu chương trình đào tạo
			var ctdtt = db.t_CTDaotao.Find(id);
            var hphan = db.sc_Hocphan.ToList();
            //thêm vào danh sách nội dung ctdt
            hp.khoikienthuc = kkienthuc;
			hp.ctdt = ctdtt;
            hp.hocphan = hphan;
            hp.khdaotao = khdaotao;
            lst.Add(hp);
            HttpCookie ctdt = new HttpCookie("IDList");
            ctdt.Value = id.ToString();
            ctdt.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(ctdt);
            return View(lst);
        }
        public JsonResult LoadHpList()
        {
            HttpCookie ck = Request.Cookies["IDList"];
            var ctdtid = ck.Value;
            int id = int.Parse(ctdtid);
            var lsthp = db.tc_KHDaotao.Where(s => s.MaCTDT == id).Select(s => new { s.Hocky, s.MaCTDT, s.MaKHDT, s.SoTC, TenHP = s.sc_Hocphan.TenMH, MaCDR =s.t_CDR_CTDT.Select(h => new { h.MaHT}) });
            return Json(new { dsHp = lsthp });
        }
        
        public ActionResult Overview(int id)
        {
            //gọi database
			fit_misDBEntities db = new fit_misDBEntities();
            //tạo danh sách ctdt
            List<CTDTData> lst = new List<CTDTData>();
			CTDTData mmap = new CTDTData();
            //lấy dữ liệu chuẩn đầu ra CTDT
            var cd_ctdt = db.t_CDR_CTDT.ToList();
            //lấy dữ liệu chương trình đào tạo
            var ctdaotao = db.t_CTDaotao.Where(s => s.MaCT==id);
            //lấy dữ liệu hệ ngành
            var hnganh = db.sc_HeNganh.ToList();
            //lấy dữ liệu khoa
            var khoa = db.sc_Khoa.ToList();
            //thêm vào danh sách ctdt
            mmap.ctdt = ctdaotao;
            mmap.henganh = hnganh;
            mmap.kh = khoa;
            lst.Add(mmap);
            return View(lst);
        }
    }
}