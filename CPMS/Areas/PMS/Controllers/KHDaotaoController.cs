using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Areas.PMS.Controllers;
using CommonRS.Resources;

namespace Capstone.Areas.PMS.Controllers
{
    public class KHDaotaoController : Controller
    {
        APIController api = new APIController();

        // GET: KHDaotao
        [HttpGet]
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult XemKeHoachDaoTao()
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = api.ListSystem();
            return View(listSystem);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult XemKeHoachDaoTao(string MaHN, string MaNG, string MaNH, string MaHK)
        {
            ViewBag.MaHN = MaHN;
            ViewBag.MaNG = MaNG;
            ViewBag.MaNH = MaNH;
            ViewBag.MaHK = MaHK;
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = api.ListSystem();
            return View(listSystem);
        }

        // GET: LapKHDaotao
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult LapKeHoachDaoTao()
        {
            return View();
        }

        public string LayTatCaKhoiLopKeHoach()
        {
            using (var context = new fit_misDBEntities())
            {
                int maNganh = int.Parse(Request.QueryString["MaNganh"]);

                var khoiLops = context.sc_Khoilop.Join(context.t_CTDaotao, c => c.MaKhoi, 
                    d => d.MaKhoi, (c, d) => new { MaKhoi = d.MaKhoi, TenKhoi = c.TenKhoi,
                        MaHN = d.MaHN, LoaiCT = d.LoaiCT }).Where(a => a.MaHN == maNganh).Where(a => a.LoaiCT == "PC").GroupBy
                        (a => new {  a.MaKhoi, a.TenKhoi }).Select(a => new
                        { MaKhoi = a.Key.MaKhoi, TenKhoi = a.Key.TenKhoi }).ToList();

                return JsonConvert.SerializeObject(khoiLops, Formatting.Indented);
            }
        }

        public string LayTatCaKhoiLop()
        {
            using (var context = new fit_misDBEntities())
            {
                int maNganhKH = int.Parse(Request.QueryString["MaNganh"]);
                var khoiLops = context.sc_Khoilop.Join(context.t_CTDaotao, c => c.MaKhoi,
                    d => d.MaKhoi, (c, d) => new {
                        MaKhoi = d.MaKhoi,
                        TenKhoi = c.TenKhoi,
                        MaHN = d.MaHN
                    }).Where(a => a.MaHN == maNganhKH).GroupBy
                        (a => new { a.MaKhoi, a.TenKhoi }).Select(a => new
                        { MaKhoi = a.Key.MaKhoi, TenKhoi = a.Key.TenKhoi }).ToList();
                //var khoiLops = context.sc_Khoilop.Join(context.t_CTDaotao, c => c.MaKhoi,
                //    d => d.MaKhoi, (c, d) => new {
                //        MaKhoi = d.MaKhoi,
                //        TenKhoi = c.TenKhoi,
                //        MaHN = d.MaHN,
                //        LoaiCT = d.LoaiCT
                //    }).Where(a => a.MaHN == maNganhKH).GroupBy
                //        (a => new { a.MaKhoi, a.TenKhoi }).Select(a => new
                //        { MaKhoi = a.Key.MaKhoi, TenKhoi = a.Key.TenKhoi }).ToList();

                return JsonConvert.SerializeObject(khoiLops, Formatting.Indented);
            }
        }

        public string TimHocKyCTDTKeHoach()
        {
            using (var context = new fit_misDBEntities())
            {
                var MaKhoi = Request.QueryString["MaKhoi"];
                int maKhoi = int.Parse(MaKhoi);
                var cTDaoTao = context.t_CTDaotao.Where(x => x.MaKhoi == maKhoi).Where(x => x.LoaiCT == "PC").FirstOrDefault();
                var hocKys = cTDaoTao.tc_KHDaotao.GroupBy(x => x.Hocky).Select(x => x.Key).ToList();
                //return new JsonResult() { Data = khoiLops, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                return JsonConvert.SerializeObject(hocKys, Formatting.Indented);
            }
        }

        public string TimMonHocTheoHocKy()
        {
            using (var context = new fit_misDBEntities())
            {
                var MaKhoi = Request.QueryString["MaKhoi"];
                var HocKy = Request.QueryString["HocKy"];
                int maKhoi = int.Parse(MaKhoi);
                var cTDaoTao = context.t_CTDaotao.Where(x => x.MaKhoi == maKhoi).Where(x => x.LoaiCT == "PC").FirstOrDefault();
                var hocPhan = cTDaoTao.tc_KHDaotao.Where(x => x.Hocky == HocKy).Select(c => new {c.MaKHDT,c.TenHP,c.MaHP }).ToList();
                //var hocPhan = kHDaoTao.Join(context.sc_Hocphan, c => c.MaHP, d => d.MaHP, (c, d) => new { MAKHDT = c.MaKHDT, TenMH = d.TenMH }).ToList();


                return JsonConvert.SerializeObject(hocPhan, Formatting.Indented);
            }
        }

        public string KiemTraKeHoachDaoTao()
        {
            var NamHoc = HttpContext.Request.Form["namHoc"]; 
            var HocKy = HttpContext.Request.Form["hocky"]; 
            

            string danhSachlopandmontake = HttpContext.Request.Form["danhSachMonHoc"];
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachlopandmontake);

            string err = "";

            using (var context = new fit_misDBEntities())
            {
                for(int i = 0; i < danhSachlopandmon.Count; i++)
                {
                    var KhoiLop = danhSachlopandmon[i].lop;
                    var timKhoiLop = context.sc_Khoilop.Where(x => x.TenKhoi == KhoiLop).FirstOrDefault();
                    var khdt = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT,
                   (c, d) => new { MAKHDT = c.MaKHDT, Namhoc = c.Namhoc, Hocky = c.Hocky, d.MaKhoi })
                   .Where(x => x.Hocky == HocKy).Where(x => x.MaKhoi == timKhoiLop.MaKhoi).Where(x => x.Namhoc == NamHoc).FirstOrDefault();

                    if (khdt != null)
                    {
                        err += "Khoá " + KhoiLop + " đã tồn tại Học kỳ " + HocKy + " thuộc Năm học " + NamHoc+"\n";
                    }
                }

                return err;
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string LuuKeHoachDaoTao()
        {
            string danhSachlopandmontake = HttpContext.Request.Form["danhSachMonHoc"];
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachlopandmontake);
            //KHDT
            string hocky = HttpContext.Request.Form["hocky"];
            string namHoc = HttpContext.Request.Form["namHoc"];
            string ngayBatDau = HttpContext.Request.Form["ngayBatDau"];
            string ngayKetThuc = HttpContext.Request.Form["ngayKetThuc"];
            string ngayCN = HttpContext.Request.Form["ngaycn"];
            string maHN = HttpContext.Request.Form["maHN"];



            using (var context = new fit_misDBEntities()) {
                for(int i=0; i < danhSachlopandmon.Count; i++)
                {
                    var tenlop = danhSachlopandmon[i].lop;
                    var lop = context.sc_Khoilop.Where(x => x.TenKhoi == tenlop).FirstOrDefault();
                    for (int j=0; j < danhSachlopandmon[i].danhsachmonhoc.Count; j++)
                    {
                        int makhdtPC = int.Parse(danhSachlopandmon[i].danhsachmonhoc[j].id);
                        context.CreateKHDTIC(makhdtPC, namHoc, hocky, DateTime.Parse(ngayBatDau), DateTime.Parse(ngayKetThuc), DateTime.Parse(ngayCN), lop.MaKhoi);
                    }
                }

                ////Ctdt
                //int maDk = 5953;
                //int maTC;
                //string loaiCT = "IC";
                //string tinhTrang = "Lưu tạm";
                ////list chứa các id của ctdt
                //List<int> lsCTDT = new List<int>();

                ////check xem ctdt thực thi này đã có chưa nếu chưa có (HK1) thì sẽ tạo
                //for(int i = 0; i < danhSachlopandmon.Count; i++)
                //{
                //    var lop = danhSachlopandmon[i].lop;
                //    var item = context.t_CTDaotao.Join(context.sc_Khoilop
                //        , c => c.MaKhoi, d => d.MaKhoi, (c, d) =>
                //           new { MaCTDT = c.MaCT, TenKhoi = d.TenKhoi, MaKhoi = d.MaKhoi, LoaiCT = c.LoaiCT }).Where(x => x.TenKhoi == lop&& x.LoaiCT == "IC").FirstOrDefault();
                //    var itemlaymaTC = context.t_CTDaotao.Join(context.sc_Khoilop
                //        , c => c.MaKhoi, d => d.MaKhoi, (c, d) =>
                //           new { MaCTDT = c.MaCT, TenKhoi = d.TenKhoi, MaKhoi = d.MaKhoi, LoaiCT = c.LoaiCT }).Where(x => x.TenKhoi == lop&& x.LoaiCT == "PC").FirstOrDefault();
                //    maTC = itemlaymaTC.MaCTDT;
                //    //tạo ctdt
                //    if (item == null)
                //    {
                //        t_CTDaotao ctdt = new t_CTDaotao();
                //        ctdt.MaHN = int.Parse(maHN);
                //        ctdt.MaKhoi = context.sc_Khoilop.Where(x=>x.TenKhoi == lop).FirstOrDefault().MaKhoi;
                //        ctdt.MaTC = maTC;
                //        ctdt.LoaiCT = loaiCT;
                //        ctdt.Tinhtrang = tinhTrang;
                //        ctdt.TenCT = "CT Thực thi";
                //        context.t_CTDaotao.Add(ctdt);
                //        context.SaveChanges();
                //        lsCTDT.Add(ctdt.MaCT);
                //    }
                //    else
                //    {
                //        lsCTDT.Add(item.MaCTDT);
                //    }

                //}

                


                //for (int i=0;i< danhSachlopandmon.Count; i++)
                //{
                //    for(int j=0;j< danhSachlopandmon[i].danhsachmonhoc.Count; j++)
                //    {
                //        var khdttuongung = context.tp_KHDaotao.Find(int.Parse(danhSachlopandmon[i].danhsachmonhoc[j].id));
                //        tp_KHDaotao khdt = new tp_KHDaotao();
                //        khdt.MaCTDT = lsCTDT[i];
                //        khdt.MaHP = khdttuongung.MaHP;
                //        khdt.TenHP = khdttuongung.TenHP;
                //        khdt.MaKhoiKT = khdttuongung.MaKhoiKT;
                //        khdt.SoTC = khdttuongung.SoTC;
                //        khdt.GioLT = khdttuongung.GioLT;
                //        khdt.GioTH = khdttuongung.GioTH;
                //        khdt.GioDa = khdttuongung.GioDa;
                //        khdt.GioTT = khdttuongung.GioTT;
                //        khdt.TrangthaiKH = ResourcePM.Resources.Variables.TrangthaiKH_KhoiTao;
                //        khdt.NgayBD = DateTime.Parse(ngayBatDau);
                //        khdt.NgayKT = DateTime.Parse(ngayKetThuc);
                //        khdt.NgayHT = DateTime.Parse(ngayCN);


                //        khdt.Hocky = hocky;
                //        khdt.Namhoc = namHoc;

                //        context.tp_KHDaotao.Add(khdt);
                //        context.SaveChanges();
                //    }
                //}
            }

            var sMessage = "Success";
            return sMessage;
        }
        
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult ChinhSuaKeHoachDaoTao()
        {
            return View();
        }

        //Load ra các KHDT cho view chỉnh sửa
        public string LayKHDT()
        {
            var namHoc = Request.QueryString["namhoc"];
            var hocKy = Request.QueryString["hocky"];
            var khoiLop = Request.QueryString["khoilop"];

            using(var context = new fit_misDBEntities())
            {
                var lop = context.sc_Khoilop.Where(x => x.TenKhoi == khoiLop).FirstOrDefault();
                var khdts = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT,
                    (c, d) => new {c.TenHP ,MaHP = c.MaHP, c.MaKHDT ,Namhoc = c.Namhoc, Hocky = c.Hocky, MaKhoi = d.MaKhoi, NgayBD=c.NgayBD, NgayKT =c.NgayKT, NgayCN = c.NgayHT })
                    .Where(x => x.Namhoc == namHoc).Where(x => x.Hocky == hocKy)
                    .Where(x => x.MaKhoi == lop.MaKhoi).Select(x=> new { TenMH = x.TenHP, MaHP = x.MaHP, NgayBD = x.NgayBD, NgayKT = x.NgayKT, NgayCN = x.NgayCN }).ToList();

                LopAndMonJson LopvaMons = new LopAndMonJson();

                LopvaMons.danhsachmonhoc = new List<MonJson>();
                LopvaMons.lop = khoiLop;

                for(int i = 0; i < khdts.Count; i++)
                {
                    
                    MonJson mon = new MonJson();
                    var mahp = khdts[i].MaHP; 
                    //mon.id = khdts[i].MaHP.ToString();
                    var khdtKehoachTuongUng = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d) =>
                     new { MaKHDT = c.MaKHDT, MaHP = c.MaHP, LoaiCT = d.LoaiCT }).Where(x => x.LoaiCT == "PC").Where(x=>x.MaHP== mahp).FirstOrDefault();
                    mon.id = khdtKehoachTuongUng.MaKHDT.ToString();
                    mon.name = khdts[i].TenMH.ToString();
                    mon.mamon = khdts[i].MaHP.ToString();
                    LopvaMons.danhsachmonhoc.Add(mon);
                }
                LopAndMonJsonAndTGHK listTraVe = new LopAndMonJsonAndTGHK();
                listTraVe.lopAndMonJson = LopvaMons;
                //listTraVe.ngayBD = khdts[0].NgayBD.ToString();
                //listTraVe.ngayKT = khdts[0].NgayKT.ToString();

                if (khdts.Count > 0)
                {
                    listTraVe.ngayBD = String.Format("{0:dd/MM/yyyy}", khdts[0].NgayBD);
                    listTraVe.ngayKT = String.Format("{0:dd/MM/yyyy}", khdts[0].NgayKT);
                    listTraVe.ngayCN = String.Format("{0:dd/MM/yyyy}", khdts[0].NgayCN);
                }


                return JsonConvert.SerializeObject(listTraVe, Formatting.Indented);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string XoaVaLuuKeHoachDaoTao()
        {
            string danhSachlopandmontake = HttpContext.Request.Form["danhSachMonHoc"];
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachlopandmontake);


            using (var context = new fit_misDBEntities())
            {
                string maHN = HttpContext.Request.Form["maHN"];
               
                //list chứa các id của ctdt
                List<int> lsCTDT = new List<int>();
                string hocky = HttpContext.Request.Form["hocky"];
                string namHoc = HttpContext.Request.Form["namHoc"];
                string ngayBatDau = HttpContext.Request.Form["ngayBatDau"];
                string ngayKetThuc = HttpContext.Request.Form["ngayKetThuc"];
                string ngaycn = HttpContext.Request.Form["ngaycn"];


                //Xoá tất cả các Kế hoạch đào tạo cũ
                var tenlop = danhSachlopandmon[0].lop;
                var makhoilop = context.sc_Khoilop.Where(x => x.TenKhoi == tenlop).FirstOrDefault().MaKhoi;
                var khdtxoa = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d) =>
                       new { MaCTDT = c.MaCTDT, MaHP = c.MaHP, MaKHDT = c.MaKHDT, Namhoc = c.Namhoc, Hocky = c.Hocky, MaKhoi = d.MaKhoi, LoaiCT = d.LoaiCT }).Where(x => x.Hocky == hocky)
                .Where(x => x.Namhoc == namHoc).Where(x => x.MaKhoi == makhoilop).Where(x => x.LoaiCT == "IC").ToList();

                List<tp_KHDaotao> lsKHDTTuongUng = new List<tp_KHDaotao>();
                for(int i = 0; i < danhSachlopandmon[0].danhsachmonhoc.Count; i++)
                {
                    int maKHDT = int.Parse(danhSachlopandmon[0].danhsachmonhoc[i].id);
                    var khdttuongung = context.tp_KHDaotao.Find(maKHDT);
                    lsKHDTTuongUng.Add(khdttuongung);

                    var khdtTims = khdtxoa.Where(x => x.MaHP == khdttuongung.MaHP).ToList();
                    //Nếu tìm không thấy, trong KHDT cũ. Nghĩa là vừa được thêm vào => tạo mới
                    if (khdtTims.Count == 0)
                    {
                        tp_KHDaotao khdt = new tp_KHDaotao();
                        khdt.MaCTDT = khdtxoa[0].MaCTDT;
                        khdt.MaHP = khdttuongung.MaHP;
                        khdt.TenHP = khdttuongung.TenHP;
                        khdt.MaKhoiKT = khdttuongung.MaKhoiKT;
                        khdt.SoTC = khdttuongung.SoTC;
                        khdt.GioLT = khdttuongung.GioLT;
                        khdt.GioTH = khdttuongung.GioTH;
                        khdt.GioDa = khdttuongung.GioDa;
                        khdt.GioTT = khdttuongung.GioTT;
                        khdt.TrangthaiKH = Variables.TrangthaiKH_KhoiTao;
                        khdt.NgayBD = DateTime.Parse(ngayBatDau);
                        khdt.NgayKT = DateTime.Parse(ngayKetThuc);
                        khdt.NgayHT = DateTime.Parse(ngaycn);

                        khdt.Hocky = hocky;
                        khdt.Namhoc = namHoc;

                        context.tp_KHDaotao.Add(khdt);
                        context.SaveChanges();
                    }
                    else
                    {
                        foreach(var ele in khdtTims)
                        {
                            tp_KHDaotao KHDTAO = context.tp_KHDaotao.Find(ele.MaKHDT);
                            KHDTAO.NgayBD = DateTime.Parse(ngayBatDau);
                            KHDTAO.NgayKT = DateTime.Parse(ngayKetThuc);
                            KHDTAO.NgayHT = DateTime.Parse(ngaycn);
                            context.SaveChanges();
                        }
                    }
                }

                for(int j = 0; j < khdtxoa.Count; j++)
                {
                    var a = lsKHDTTuongUng.Where(x => x.MaHP == khdtxoa[j].MaHP).ToList();
                    //Không tìm thấy trong list khdt cũ thì xoá đi
                    if (a.Count == 0)
                    {
                        tp_KHDaotao kh = context.tp_KHDaotao.Find(khdtxoa[j].MaKHDT);
                        if (kh != null)
                        {
                            context.tp_KHDaotao.Remove(kh);
                            context.SaveChanges();
                        }
                        else
                        {
                            kh.NgayBD = DateTime.Parse(ngayBatDau);
                            kh.NgayKT = DateTime.Parse(ngayKetThuc);
                            context.SaveChanges();
                        }
                    }
                    
                }
                
                var sMessage = "Success";
                return sMessage;
            }
        }

    }
    
    public class MonJson
    {
        public string id;
        public string name;
        public string mamon;
    }
    public class LopAndMonJson
    {
        public string lop;
        public List<MonJson> danhsachmonhoc;
    }

    public class LopAndMonJsonAndTGHK
    {
        public LopAndMonJson lopAndMonJson;
        public string ngayBD;
        public string ngayKT;
        public string ngayCN;
    }
    
}