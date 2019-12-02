using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonLibrary;
using Capstone.Areas.CMS.Models;

namespace Capstone.Areas.CMS.Controllers.Training.Curriculum
{
    public class HomeController : Controller
    {
        fit_misDBEntities db = new fit_misDBEntities();
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult Index()
        {
            //lấy danh sách các chương trình đào tạo
            var ctdaotao = db.t_CTDaotao.ToList();
            return View(ctdaotao);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult mmap()
        {
            //tạo danh sách Mmapdata
            List<MmapData> lst = new List<MmapData>();
            MmapData mmap = new MmapData();
            //lấy dữ liệu chuẩn đầu ra ctdt
            var cd_ctdt = db.t_CDR_CTDT.ToList();
            //lấy dữ liệu ctdt
            var ctdaotao = db.t_CTDaotao.ToList();
            //lấy dữ liệu hệ ngành
            var hnganh = db.sc_HeNganh.ToList();
            //lấy dữ liệu khoa
            var khoa = db.sc_Khoa.ToList();
            //thêm vào danh sách  mmap
            mmap.cdr_cd = cd_ctdt;
            mmap.ctdt = ctdaotao;
            mmap.henganh = hnganh;
            mmap.kh = khoa;
            lst.Add(mmap);

            List<APIMindMap01> lstm1 = new List<APIMindMap01>();
            List<APIMindMap01> lstm2 = new List<APIMindMap01>();

            string pb1 = "";
            string pb2 = "";

            List<APIMindMap00> lstmm = new List<APIMindMap00>();
            string loaict = "";
            string loaict2 = LoaiHinhDT.CTDTKH;
            if (lst.FirstOrDefault().ctdt.FirstOrDefault().LoaiCT.Equals(LoaiHinhDT.CTDT_KHUNG))
            {
                loaict = LoaiHinhDT.CTDTKHUNG;
            }
            else if (lst.FirstOrDefault().ctdt.FirstOrDefault().LoaiCT.Equals(LoaiHinhDT.CTDT_KH))
            {
                loaict = LoaiHinhDT.CTDTKH;
            }


            List<APIMindMap0> lstm0 = new List<APIMindMap0>();
            List<APIMindMap1> lsthn1 = new List<APIMindMap1>();


            foreach (var item in hnganh)
            {
                lstm0 = new List<APIMindMap0>();
                if (item.MaQH == null)
                {

                    foreach (var i in ctdaotao)
                    {
                        if (i.Tinhtrang == TinhTrang.HieuLuc && i.sc_HeNganh.MaQH == item.MaHN)
                        {
                            lstm1 = new List<APIMindMap01>();
                            lstm2 = new List<APIMindMap01>();
                            var mm1 = new APIMindMap00() { name = loaict, color = Caymatran.Green, manganh = i.MaCT, children = lstm1 };
                            var mm2 = new APIMindMap00() { name = loaict2, color = Caymatran.Green, manganh = i.MaCT, children = lstm2 };
                            var m1 = new APIMindMap0() { name = i.TenCT, color = Caymatran.Green, children = new List<APIMindMap00>() { mm1, mm2 } };
                            lstm0.Add(m1);
                            if (i.LoaiCT == LoaiHinhDT.CTDT_KHUNG)
                            {
                                pb1 = i.Phienban;
                                var phienBan = Caymatran.Phienban + " " + pb1;
                                var mm10 = new APIMindMap01() { name = phienBan, claimReason = i.Muctieu, ma = i.MaCT };
                                lstm1.Add(mm10);
                            }
                            else if (i.LoaiCT == LoaiHinhDT.CTDT_KH)
                            {
                                pb2 = i.Phienban;
                                var phienBan2 = Caymatran.Phienban + " " + pb2;
                                var mm11 = new APIMindMap01() { name = phienBan2, claimReason = i.Muctieu, ma = i.MaCT };
                                lstm2.Add(mm11);
                            }
                        }
                    }
                    var tenhe = new APIMindMap1() { name = item.Mota, color = Caymatran.Green, children = lstm0 };
                    lsthn1.Add(tenhe);

                }

            }
            //var mhe = new APIMindMap1() { name = tenhe, color = Caymatran.CLGreen, children = lstm0 };
            //var mhe2 = new APIMindMap1() { name = tenhe, color = Caymatran.CLGreen, children = lstm0 };
            return Json(new APIMindMap2()
            {
                name = Caymatran.HeDaoTao,
                children = lsthn1
            }, JsonRequestBehavior.AllowGet);

        }

    }
}