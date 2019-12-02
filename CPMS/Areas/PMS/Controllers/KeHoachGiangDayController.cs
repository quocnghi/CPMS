using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Areas.PMS.Controllers;

namespace Capstone.Areas.PMS.Controllers
{
    public class KeHoachGiangDayController : Controller
    {
        APIController api = new APIController();
        fit_misDBEntities context = new fit_misDBEntities();

        // GET: KHDaotao
        [HttpGet]
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult XemKeHoachGiangDay()
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = api.dsHeDaotao();
            return View(listSystem);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult XemKeHoachGiangDay(string MaHN, string MaNG, string MaNH, string MaHK)
        {
            ViewBag.MaHN = MaHN;
            ViewBag.MaNG = MaNG;
            ViewBag.MaNH = MaNH;
            ViewBag.MaHK = MaHK;
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = api.dsHeDaotao();
            return View(listSystem);
        }

        // GET: LapKHDaotao
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult LapKeHoachGiangDay()
        {
            return View();
        }

        /// <summary>
        /// Purpose: Lấy tất cả danh sách các khối lớp có trong PC
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="MaNganh">Mã ngành</param> 
        /// <returns>Danh sách tất cả khối lớp PC theo mã ngành</returns>
        public string LayTatCaKhoiLopKeHoach(int MaNganh)
        {

            var khoiLops = context.sc_Khoilop.Join(context.t_CTDaotao, c => c.MaKhoi,
                d => d.MaKhoi, (c, d) => new {
                    MaKhoi = d.MaKhoi,
                    TenKhoi = c.TenKhoi,
                    MaHN = d.MaHN,
                    LoaiCT = d.LoaiCT
                }).Where(a => a.MaHN == MaNganh).Where(a => a.LoaiCT == "PC").GroupBy
                    (a => new { a.MaKhoi, a.TenKhoi }).Select(a => new
                    { MaKhoi = a.Key.MaKhoi, TenKhoi = a.Key.TenKhoi }).ToList();

            return JsonConvert.SerializeObject(khoiLops, Formatting.Indented);
        }

        /// <summary>
        /// Purpose: Lấy tất cả danh sách các khối lớp trong ngành
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="MaNganh">Mã ngành</param> 
        /// <returns>Danh sách tất cả khối lớp theo mã ngành</returns>
        public string LayTatCaKhoiLop(int MaNganh)
        {
            var khoiLops = context.sc_Khoilop.Join(context.t_CTDaotao, c => c.MaKhoi,
                d => d.MaKhoi, (c, d) => new {
                    MaKhoi = d.MaKhoi,
                    TenKhoi = c.TenKhoi,
                    MaHN = d.MaHN
                }).Where(a => a.MaHN == MaNganh).GroupBy
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


        /// <summary>
        /// Purpose: Tìm các học kỳ của PC the mã khối
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="MaKhoi">Mã khối</param> 
        /// <returns>Danh sách tất cả các học kỳ</returns>
        public string TimHocKyCTDTKeHoach(int MaKhoi)
        {
            var cTDaoTao = context.t_CTDaotao.Where(x => x.MaKhoi == MaKhoi).Where(x => x.LoaiCT == "PC").FirstOrDefault();
            var hocKys = cTDaoTao.tc_KHDaotao.GroupBy(x => x.Hocky).Select(x => x.Key).ToList();
            //return new JsonResult() { Data = khoiLops, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return JsonConvert.SerializeObject(hocKys, Formatting.Indented);
        }

        /// <summary>
        /// Purpose: Tìm các môn học theo học kỳ của khối lớp
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="MaKhoi">Mã khối</param> 
        /// <param name="HocKy">Học kỳ</param> 
        /// <returns>Danh sách tất cả môn học theo học kỳ</returns>
        public string TimMonHocTheoHocKy(int MaKhoi, string HocKy)
        {

            var cTDaoTao = context.t_CTDaotao.Where(x => x.MaKhoi == MaKhoi).Where(x => x.LoaiCT == "PC").FirstOrDefault();
            var hocPhan = cTDaoTao.tc_KHDaotao.Where(x => x.Hocky == HocKy).Select(c => new { c.MaKHDT, c.TenHP, c.MaHP }).ToList();
            //var hocPhan = kHDaoTao.Join(context.sc_Hocphan, c => c.MaHP, d => d.MaHP, (c, d) => new { MAKHDT = c.MaKHDT, TenMH = d.TenMH }).ToList();

            return JsonConvert.SerializeObject(hocPhan, Formatting.Indented);
        }

        public string KiemTraKeHoachGiangDay()
        {
            var NamHoc = HttpContext.Request.Form["namHoc"];
            var HocKy = HttpContext.Request.Form["hocky"];


            string danhSachlopandmontake = HttpContext.Request.Form["danhSachMonHoc"];
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachlopandmontake);

            string err = "";

            for (int i = 0; i < danhSachlopandmon.Count; i++)
            {
                var KhoiLop = danhSachlopandmon[i].lop;
                var timKhoiLop = context.sc_Khoilop.Where(x => x.TenKhoi == KhoiLop).FirstOrDefault();
                var khdt = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT,
                (c, d) => new { MAKHDT = c.MaKHDT, Namhoc = c.Namhoc, Hocky = c.Hocky, d.MaKhoi })
                .Where(x => x.Hocky == HocKy).Where(x => x.MaKhoi == timKhoiLop.MaKhoi).Where(x => x.Namhoc == NamHoc).FirstOrDefault();

                if (khdt != null)
                {
                    err += "Khoá " + KhoiLop + " đã tồn tại Học kỳ " + HocKy + " thuộc Năm học " + NamHoc + "\n";
                }
            }

            return err;
        }

        /// <summary>
        /// Purpose: Lập kế hoach giảng dạy IC theo PC
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="danhSachMonHoc">Danh sách môn học giảng dạy</param> 
        /// <param name="hocky">Học kỳ</param> 
        /// <param name="namHoc">Năm học</param> 
        /// <param name="ngayBatDau">ngày bắt đầu học kỳ</param> 
        /// <param name="ngayKetThuc">ngày kế thúc học kỳ</param> 
        /// <param name="ngaycn">Ngày hết hạn nộp đề cương</param> 
        /// <param name="maHN">Mã Hệ ngành</param> 
        /// <returns>string check hàm đã chạy xong chưa</returns>
        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string LuuKeHoachGiangDay(string danhSachMonHoc, string hocky, string namHoc, string ngayBatDau, string ngayKetThuc, string ngaycn, string maHN)
        {
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachMonHoc);
            //KHDT
            for (int i = 0; i < danhSachlopandmon.Count; i++)
            {
                var tenlop = danhSachlopandmon[i].lop;
                var lop = context.sc_Khoilop.Where(x => x.TenKhoi == tenlop).FirstOrDefault();
                for (int j = 0; j < danhSachlopandmon[i].danhsachmonhoc.Count; j++)
                {
                    int makhdtPC = int.Parse(danhSachlopandmon[i].danhsachmonhoc[j].id);
                    context.CreateKHDTIC(makhdtPC, namHoc, hocky, DateTime.Parse(ngayBatDau), DateTime.Parse(ngayKetThuc), DateTime.Parse(ngaycn), lop.MaKhoi);
                }
            }

            var sMessage = "Success";
            return sMessage;
        }

        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult ChinhSuaKeHoachGiangDay()
        {
            return View();
        }

        /// <summary>
        /// Purpose: Lấy môn học kế hoach giảng dạy IC 
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="namhoc">Năm học giảng dạy</param> 
        /// <param name="hocky">Học kỳ</param> 
        /// <param name="namHoc">Năm học</param> 
        /// <param name="khoilop">Tên khối lớp</param> 
        /// <returns>json các môn học giảng dạy IC theo năm học học kỳ và khối lớp</returns>
        //Load ra các KHDT cho view chỉnh sửa
        public string LayKHDT(string namhoc, string hocky, string khoilop)
        {

            using (var context = new fit_misDBEntities())
            {
                var lop = context.sc_Khoilop.Where(x => x.TenKhoi == khoilop).FirstOrDefault();
                var khdts = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT,
                    (c, d) => new { c.TenHP, MaHP = c.MaHP, c.MaKHDT, Namhoc = c.Namhoc, Hocky = c.Hocky, MaKhoi = d.MaKhoi, NgayBD = c.NgayBD, NgayKT = c.NgayKT, NgayCN = c.NgayHT })
                    .Where(x => x.Namhoc == namhoc).Where(x => x.Hocky == hocky)
                    .Where(x => x.MaKhoi == lop.MaKhoi).Select(x => new { TenMH = x.TenHP, MaHP = x.MaHP, NgayBD = x.NgayBD, NgayKT = x.NgayKT, NgayCN = x.NgayCN }).ToList();

                LopAndMonJson LopvaMons = new LopAndMonJson();

                LopvaMons.danhsachmonhoc = new List<MonJson>();
                LopvaMons.lop = khoilop;

                for (int i = 0; i < khdts.Count; i++)
                {

                    MonJson mon = new MonJson();
                    var mahp = khdts[i].MaHP;
                    //mon.id = khdts[i].MaHP.ToString();
                    var khdtKehoachTuongUng = context.tc_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d) =>
                     new { MaKHDT = c.MaKHDT, MaHP = c.MaHP, LoaiCT = d.LoaiCT }).Where(x => x.LoaiCT == "PC").Where(x => x.MaHP == mahp).FirstOrDefault();
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


        /// <summary>
        /// Purpose: Chỉnh sửa các môn học giảng dạy
        /// Developer: Trần An Bình
        /// Date: 
        /// </summary>
        /// <param name="danhSachMonHoc">Danh sách môn học giảng dạy</param> 
        /// <param name="hocky">Học kỳ</param> 
        /// <param name="namHoc">Năm học</param> 
        /// <param name="ngayBatDau">ngày bắt đầu học kỳ</param> 
        /// <param name="ngayKetThuc">ngày kế thúc học kỳ</param> 
        /// <param name="ngaycn">Ngày hết hạn nộp đề cương</param> 
        /// <param name="maHN">Mã Hệ ngành</param> 
        /// <param name="tenlop">Tên khối lớp</param> 
        /// <returns>string check hàm đã hoàn thành hay chưa</returns>
        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string XoaVaLuuKeHoachGiangDay(string danhSachMonHoc, string maHN, string hocky, string namHoc, string ngayBatDau, string ngayKetThuc, string ngaycn, string tenlop)
        {
            List<LopAndMonJson> danhSachlopandmon = JsonConvert.DeserializeObject<List<LopAndMonJson>>(danhSachMonHoc);


            using (var context = new fit_misDBEntities())
            {

                //list chứa các id của ctdt
                List<int> lsCTDT = new List<int>();


                //Xoá tất cả các Kế hoạch đào tạo cũ
                var makhoilop = context.sc_Khoilop.Where(x => x.TenKhoi == tenlop).FirstOrDefault().MaKhoi;
                var khdtxoa = context.tp_KHDaotao.Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d) =>
                       new { MaCTDT = c.MaCTDT, MaHP = c.MaHP, MaKHDT = c.MaKHDT, Namhoc = c.Namhoc, Hocky = c.Hocky, MaKhoi = d.MaKhoi, LoaiCT = d.LoaiCT }).Where(x => x.Hocky == hocky)
                .Where(x => x.Namhoc == namHoc).Where(x => x.MaKhoi == makhoilop).Where(x => x.LoaiCT == "IC").ToList();

                List<tc_KHDaotao> lsKHDTTuongUng = new List<tc_KHDaotao>();
                if (danhSachlopandmon.Count != 0)
                {
                    for (int i = 0; i < danhSachlopandmon[0].danhsachmonhoc.Count; i++)
                    {
                        int maKHDT = int.Parse(danhSachlopandmon[0].danhsachmonhoc[i].id);
                        var khdttuongung = context.tc_KHDaotao.Find(maKHDT);
                        lsKHDTTuongUng.Add(khdttuongung);

                        var khdtTims = khdtxoa.Where(x => x.MaHP == khdttuongung.MaHP).ToList();
                        //Nếu tìm không thấy, trong KHDT cũ. Nghĩa là vừa được thêm vào => tạo mới
                        if (khdtTims.Count == 0)
                        {
                            DateTime ngaybd = DateTime.Parse(ngayBatDau);
                            DateTime ngaykt = DateTime.Parse(ngayKetThuc);
                            DateTime ngayht = DateTime.Parse(ngaycn);
                            context.CreateKHDTIC(khdttuongung.MaKHDT, namHoc, hocky, ngaybd, ngaykt, ngayht, makhoilop);

                            context.SaveChanges();
                        }
                        else
                        {
                            foreach (var ele in khdtTims)
                            {
                                tp_KHDaotao KHDTAO = context.tp_KHDaotao.Find(ele.MaKHDT);
                                KHDTAO.NgayBD = DateTime.Parse(ngayBatDau);
                                KHDTAO.NgayKT = DateTime.Parse(ngayKetThuc);
                                KHDTAO.NgayHT = DateTime.Parse(ngaycn);
                                context.SaveChanges();
                            }
                        }
                    }
                }


                for (int j = 0; j < khdtxoa.Count; j++)
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