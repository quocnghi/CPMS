// <copyright file="CTDTKhungController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Capstone.Areas.CMS.Controllers.Training.Curriculum
{
    #region Using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Capstone.Models;
    using Capstone.Areas.CMS.Controllers.Tools;
    using Capstone.Areas.CMS.Models;
    using System.Data.Entity.Infrastructure;
    using CommonLibrary;
    using System.Threading.Tasks;
    using System.Data;
    using System.IO;
    #endregion

    /// <summary>
    /// Manages CTDTKhung such as Listing, Creating.
    /// </summary>
    public class CTDTKhungController : Controller
    {
        private fit_misDBEntities db = new fit_misDBEntities();
        private SendEmail sEmail = new SendEmail();

        /// <summary>
        /// For testing, showing all CTDT in databases.
        /// </summary>
        /// <returns>A webpage.</returns>
        public ActionResult Index()
        {
            return this.View(this.db.t_CTDaotao.ToList());
        }

        /// <summary>
        /// Lists all CTDT Khung in database.
        /// </summary>
        /// <returns>a Webpage.</returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult List()
        {
            // Load all CTDT Khung from database
            var ctdtKhungs = this.db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KHUNG)).ToList();

            return this.View(ctdtKhungs);
        }

        /// <summary>
        /// Get list of NganhDaoTao based on maHe.
        /// </summary>
        /// <param name="maHe">Code of HeDaoTao.</param>
        /// <returns>Json of CacHeNganh.</returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult GetNganhDaoTao(int maHe)
        {
            var cacHeNganh = this.db.sc_HeNganh.Where(x => x.MaQH == maHe).Select(x => new { x.MaHN, x.Mota, x.Tenrutgon }).ToList();
            return this.Json(cacHeNganh, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate version for CTDT based on idHeNganh.
        /// </summary>
        /// <param name="idHeNganh">id of Nganh DaoTao.</param>
        /// <returns>Version value.</returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public string GetPhienBanCTDT(int idHeNganh)
        {
            var heNganh = this.db.sc_HeNganh.Find(idHeNganh);
            var ma = this.db.sc_Ma.Where(s => s.LoaiMa == Ma.Phienban).FirstOrDefault();
            var tenMa = ma.TenMa;
            tenMa = tenMa.Replace("{LoaiCT}", LoaiHinhDT.CTDT_KHUNG);
            tenMa = tenMa.Replace("{TenRG}", heNganh.Tenrutgon);
            tenMa = tenMa.Replace("{Nam}", DateTime.Now.Year.ToString());
            string phienBan = string.Empty;
            var ctdt = this.db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KHUNG) && s.MaHN == idHeNganh).AsEnumerable().LastOrDefault();
            if (ctdt != null)
            {
                var pbtdt = int.Parse(ctdt.Phienban.Substring(ctdt.Phienban.Length - 3));
                pbtdt++;
                if (pbtdt < 10)
                {
                    phienBan = "00" + pbtdt;
                }
                else if (pbtdt >= 10 && pbtdt < 100)
                {
                    phienBan = "0" + pbtdt;
                }
                else if (pbtdt > 100)
                {
                    phienBan = string.Empty + pbtdt;
                }

                phienBan = tenMa + "-" + phienBan;
            }
            else
            {
                phienBan = tenMa + "-" + ma.Phienban;
            }

            return phienBan;
        }

        /// <summary>
        /// Create CTDT Khung, Step 1.
        /// </summary>
        /// <param name="maCT">id of CTDT.</param>
        /// <returns>a Webpage.</returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult CreateCTDTStep1(int? maCT)
        {
            if (maCT.HasValue)
            {
                this.ViewBag.CacHeNganh = this.db.sc_HeNganh.Where(x => x.MaQH == null).ToList();
                this.ViewBag.NganhDaoTao = this.db.sc_HeNganh.ToList();
                return this.View(this.db.t_CTDaotao.Find(maCT) ?? new t_CTDaotao());
            }
            else
            {
                var model = this.db.t_CTDaotao.Where(p => p.Trangthai == TrangThai.KhoiTao && p.Tinhtrang == TinhTrang.LuuTam);
                return this.View("CreateCTDTStep0", model.ToList());
            }
        }

        /// <summary>
        /// Create CTDT and save into database.
        /// </summary>
        /// <param name="model">Model of data.</param>
        /// <returns>Message.</returns>
        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public string CreateCTDTStep1(t_CTDaotao model)
        {
            try
            {
                // tạo mới CTDT hoac tiep tuc
                var ctdt = this.db.t_CTDaotao.Find(model.MaCT) ?? new t_CTDaotao();
                // Lưu những thông tin chung của CTDT
                ctdt.TenCT = model.TenCT;
                ctdt.MaHN = model.MaHN;
                ctdt.MaKhoi = model.MaKhoi;
                ctdt.Phienban = model.Phienban;
                ctdt.HinhthucDT = model.HinhthucDT;
                ctdt.LoaiCT = LoaiHinhDT.CTDT_KHUNG;
                ctdt.Trinhdo = model.Trinhdo;
                ctdt.ThoigianDT = model.ThoigianDT;
                ctdt.Doituong = model.Doituong;
                ctdt.QuytrinhDT = model.QuytrinhDT;
                ctdt.Thangdiem = model.Thangdiem;
                ctdt.CosoVC = model.CosoVC;
                ctdt.Trangthai = TrangThai.KhoiTao;
                ctdt.Tinhtrang = TinhTrang.LuuTam;

                if (model.MaCT > 0)
                {
                    // cập nhật hoặc
                    this.db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // thêm vào Database
                    this.db.t_CTDaotao.Add(ctdt);
                }

                this.db.SaveChanges();

                return null; // successfully.
            }
            catch (Exception e)
            {
                return e.GetBaseException().Message;
            }
        }

        /// <summary>
        /// Edit CTDT Khung, Step 1.
        /// </summary>
        /// <param name="id">id of CTDT Khung</param>
        /// <returns>a Webpage.</returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep1(int id)
        {
            var ctdtKhung = this.db.t_CTDaotao.SingleOrDefault(st => st.MaCT == id && st.LoaiCT.Equals(LoaiHinhDT.CTDT_KHUNG));
            var heNganhs = this.db.sc_HeNganh.Where(s => s.MaHN == ctdtKhung.sc_HeNganh.MaQH).ToList();

            return this.View(new Tuple<t_CTDaotao, IEnumerable<sc_HeNganh>>(ctdtKhung, heNganhs));
        }

        //Lưu dữ liệu xuông data
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep1(string Phienbanctdt
            , string Cosovatchat
            , string Thangdiem, string Quytrinhdaotaodieukientotnghiep, string Doituongtuyensinh
            , string Thoigiandaotao, string Loaihinhdaotao
            , string Trinhdodaotao, string Tenctdt
            , string nganhdaotao)
        {
            HttpCookie ck = Request.Cookies["EditCTDTId"];
            var idCTDT = ck.Value;
            int ID = int.Parse(idCTDT);
            //int henganhh = int.Parse(henganh);
            var ctdt = db.t_CTDaotao.Find(ID);
            ctdt.TenCT = Tenctdt;
            //ctdt.Phienban = Phienbanctdt;
            ctdt.Trinhdo = Trinhdodaotao;
            //ctdt.MaHN = henganhh;
            ctdt.HinhthucDT = Loaihinhdaotao;
            ctdt.ThoigianDT = Thoigiandaotao;
            ctdt.Doituong = Doituongtuyensinh;
            ctdt.QuytrinhDT = Quytrinhdaotaodieukientotnghiep;
            ctdt.Thangdiem = Thangdiem;
            ctdt.CosoVC = Cosovatchat;
            db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditCTDTStep2", "CTDTKhung", new { id = ctdt.MaCT });
        }

        //Lưu nội dung đánh giá CTĐT
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult Hoanthanhdanhgia(t_CTDaotao ctdt)
        {
            string msg = "";
            try
            {
                var timmactdt = db.t_CTDaotao.Find(ctdt.MaCT);
                timmactdt.Ghichu = ctdt.Ghichu;
                timmactdt.Xetduyet = ctdt.Xetduyet;
                timmactdt.Tinhtrang = TinhTrang.HieuLuc;
                db.SaveChanges();
                msg = NotificationManagement.SuccessMessage.DanhgiaCTDTkhungthanhcong;
                return Json(new { Message = msg });
            }
            catch (Exception e)
            {
                return Json(new { Message = msg });
            }
        }


        //Lấy dữ liệu Chương trình đào tạo cho Đánh giá
        [Authorize(Roles = ROLES.ADMIN_EVALUATOR)]
        public JsonResult LaydulieudanhgiaCTDT(t_CTDaotao ttctdt)
        {

            try
            {
                //lấy dữ liệu mục tiêu đào tạo chung của CTDT
                var muctieuchung = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.Muctieu }).FirstOrDefault();

                var kqdanhgia = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.Ghichu }).FirstOrDefault();

                //lấy dữ liệu mục tiêu đào tạo cụ thể của CTDT
                var mtdtcuthe = db.t_MT_CTDT.Where(s => s.MaCTDT == ttctdt.MaCT).Select(s => new { s.Phanloai, s.Mota, s.MaMT, CDR = s.t_CDR_CTDT.Select(m => new { m.Mota,m.MaHT }) }).ToList();

                //lấy dữ liệu kết quả học tập mong đợi của CTDT
                var chuandaura = db.t_CDR_CTDT.Where(s => s.MaCTDT == ttctdt.MaCT).Select(s => new { s.MaHT, s.Mota, HP = s.tc_KHDaotao.Select(m=> new { m.TenHP}) }).ToList();

                //lấy danh sách học phần ứng với mỗi CTDT
                var dshocphan = db.tc_KHDaotao.Where(x => x.MaCTDT == ttctdt.MaCT).Select(s => new { s.sc_Hocphan.MaHT,s.SoTC,s.Hocky,s.sc_Khoikienthuc.Mota,s.sc_Khoikienthuc.MaKhoiKT, s.sc_Hocphan.TenMH, CDR = s.t_CDR_CTDT.Select(m => new { m.Mota,m.MaHT }) }).ToList();

                //lấy dữ liệu thông tin chung của CTDT
                var thongtinctdt = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.MaCT, s.TenCT, s.sc_HeNganh.MaHN, s.Phienban, s.Trinhdo, s.HinhthucDT, s.ThoigianDT, s.QuytrinhDT, s.Doituong, s.CosoVC, s.Muctieu, s.Thangdiem,s.BanScan }).FirstOrDefault();
                string tenHN = db.sc_HeNganh.Find(thongtinctdt.MaHN).Mota;
                return Json(new { thongtinctdt = thongtinctdt, tenHN = tenHN, dshocphan = dshocphan, mtdtcuthe = mtdtcuthe, chuandaura = chuandaura, kqdanhgia = kqdanhgia, });
            }
            catch (Exception e)
            {
                return Json(new { Message = NotificationManagement.ErrorMessage.Loichung });
            }
        }

        [HttpPost]
        public async Task<string> getPDF(HttpPostedFileBase FileExcel, string mact = "")
        {
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["FilePDF"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["FilePDF"].FileName);

                    if (fileExtension == ".pdf")
                    {
                        string fileLocation = Server.MapPath("~/Areas/CMS/Documents/") + Request.Files["FilePDF"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["FilePDF"].SaveAs(fileLocation);
                        var id = int.Parse(mact);
                        var rs = db.t_CTDaotao.Find(id);
                        rs.BanScan = Request.Files["FilePDF"].FileName;
                        db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
               
                return "done";
            }
            catch (Exception ex)
            {
                return "fail";
            }
        }



        //Create Bước 2 (Cho nút tiếp tục)
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult CreateCTDTStep2(t_CTDaotao ctdt)
        {
            {
                string msg = "";
                try
                {
                    //lấy mã CTDT cuối cùng
                    var lst = db.t_CTDaotao.AsEnumerable().LastOrDefault();
                    //lưu mục tiêu chung của CTDT
                    lst.Muctieu = ctdt.Muctieu;
                    db.Entry(lst).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { Message = msg });
                }
                catch (Exception e)
                {
                    msg = NotificationManagement.ErrorMessage.LoithemmoiCTDTkhung;
                    return Json(new { Message = msg });
                }
            }
        }

        //Create Bước 2
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult CreateCTDTStep2(string flag = "false")
        {
            if (flag.Equals("false"))
            {
                var lst = db.t_CTDaotao.AsEnumerable().LastOrDefault();
                HttpCookie ck = new HttpCookie("CTDTId");
                ck.Value = lst.MaCT.ToString();
                ck.Expires = DateTime.Now.AddDays(10);
                Response.Cookies.Add(ck);
                //load danh sách kết quả học tập mong đợi và mục tiêu đào tạo cụ thể ra bảng
                var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == lst.MaCT).ToList();
                var mtdtct = db.t_MT_CTDT.Where(s => s.MaCTDT == lst.MaCT).ToList();

                //lấy mã ctdt 
                int mactdt = lst.MaCT;

                //tìm các mục tiêu của ctdt đó
                var mtctdts = db.t_CTDaotao.Find(mactdt).t_MT_CTDT.ToList();
                //tạo ra ma trận cdr mt để load ra UI
                List<t_MTCDR_CTDT> mtcdrctdt = new List<t_MTCDR_CTDT>();

                foreach (t_MT_CTDT mt in mtctdts)
                {
                    var mtcdr = mt.t_CDR_CTDT.Select(ct => new t_MTCDR_CTDT
                    {
                        MaELO = ct.MaELO,
                        MaMT = mt.MaMT
                    }).ToList();
                    mtcdrctdt.AddRange(mtcdr);
                }

                HocphanData d = new HocphanData();
                d.mtctdt = mtdtct;
                d.cdrctdt = cdrctdt;
                d.mtcdrctdt = mtcdrctdt;
                List<HocphanData> ls = new List<HocphanData>();
                ls.Add(d);
                return View(ls);
            }
            if (flag.Equals("true"))
            {
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var idctdt = int.Parse(idCTDT);
                //load danh sách kết quả học tập mong đợi và mục tiêu đào tạo cụ thể ra bảng
                var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == idctdt).ToList();
                var mtdtct = db.t_MT_CTDT.Where(s => s.MaCTDT == idctdt).ToList();

                //lấy mã ctdt 
                int mactdt = idctdt;

                //tìm các mục tiêu của ctdt đó
                var mtctdts = db.t_CTDaotao.Find(mactdt).t_MT_CTDT.ToList();
                //tạo ra ma trận cdr mt để load ra UI
                List<t_MTCDR_CTDT> mtcdrctdt = new List<t_MTCDR_CTDT>();

                foreach (t_MT_CTDT mt in mtctdts)
                {
                    var mtcdr = mt.t_CDR_CTDT.Select(ct => new t_MTCDR_CTDT
                    {
                        MaELO = ct.MaELO,
                        MaMT = mt.MaMT
                    }).ToList();
                    mtcdrctdt.AddRange(mtcdr);
                }

                HocphanData d = new HocphanData();
                d.mtctdt = mtdtct;
                d.cdrctdt = cdrctdt;
                d.mtcdrctdt = mtcdrctdt;
                List<HocphanData> ls = new List<HocphanData>();
                ls.Add(d);
                return View(ls);
            }
            return View();
        }

        //Thêm mục tiêu đào tạo cụ thể
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Themmtdtct(Muctieudaotaocuthe mtdtct)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    HttpCookie ck = Request.Cookies["CTDTId"];
                    var idCTDT = ck.Value;
                    var idctdt = int.Parse(idCTDT);
                    if (mtdtct.Mota1 != null)
                    {

                        string[] splitmota = mtdtct.Mota1.Trim().Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai1;
                            lst.MaCTDT = idctdt;

                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }

                    if (mtdtct.Mota2 != null)
                    {

                        string[] splitmota = mtdtct.Mota2.Trim().Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai2;
                            lst.MaCTDT = idctdt;
                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }

                    if (mtdtct.Mota3 != null)
                    {

                        string[] splitmota = mtdtct.Mota3.Trim().Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai3;
                            lst.MaCTDT = idctdt;
                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }
                    msg = NotificationManagement.SuccessMessage.ThemmoimuctieudaotaocutheCTDTkhungthanhcong;
                    var lstmtctdt = db.t_MT_CTDT.Where(s => s.MaCTDT == idctdt).ToList();
                    var json = lstmtctdt.Select(x => new { x.Phanloai, x.Mota }).AsEnumerable().Select(x => new t_MT_CTDT { Phanloai = x.Phanloai, Mota = x.Mota });
                    return Json(new { Message = msg, mtdtct = json });
                }
            }
            catch (RetryLimitExceededException)
            {
                msg = NotificationManagement.ErrorMessage.LoithemmoimuctieudaotaocutheCTDTkhung;
                return Json(new { Message = msg });
            }
            return Json(mtdtct);

        }

        //Cấu hình MaHT chuẩn đầu ra CTDT
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getELO()
        {
            HttpCookie ck = Request.Cookies["CTDTId"];
            var idCTDT = ck.Value;
            var idctdt = int.Parse(idCTDT);
            var pb = db.sc_Ma.Where(s => s.LoaiMa == Ma.KQHTMD).FirstOrDefault();
            string phienBan = "";
            var rs = db.t_CDR_CTDT.Where(s => s.MaCTDT == idctdt).AsEnumerable().LastOrDefault();
            if (rs != null)
            {
                var pbtdt = int.Parse(rs.MaHT.Substring(rs.MaHT.Length - 3));
                pbtdt++;
                string phienban = "";
                if (pbtdt < 10)
                {
                    phienban = "00" + pbtdt;
                }
                else if (pbtdt >= 10 && pbtdt < 100)
                {
                    phienban = "0" + pbtdt;
                }
                else if (pbtdt > 100)
                {
                    phienban = "" + pbtdt;
                }
                phienBan = pb.TenMa + "-" + phienban;
                return Json(new { ma = phienBan });
            }
            phienBan = pb.TenMa + "-" + pb.Phienban;
            return Json(new { ma = phienBan });
        }

        //Thêm kết quả học tập mong đợi
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Themkqhtmongdoi(Ketquahoctapmongdoi cdrctdt)
        {
            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var idctdt = int.Parse(idCTDT);
                if (cdrctdt.Mota1 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT1;
                    lst.Mota = cdrctdt.Mota1;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                if (cdrctdt.Mota2 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT2;
                    lst.Mota = cdrctdt.Mota2;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                if (cdrctdt.Mota3 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT3;
                    lst.Mota = cdrctdt.Mota3;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                msg = NotificationManagement.SuccessMessage.ThemmoiketquahoctapmongdoiCTDTkhungthanhcong;
                var lstmtctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == idctdt).ToList();
                var json = lstmtctdt.Select(x => new { x.MaHT, x.Mota }).AsEnumerable().Select(x => new t_CDR_CTDT { MaHT = x.MaHT, Mota = x.Mota });
                return Json(new { Message = msg, chuandaura = json });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.LoithemmoiketquahoctapmongdoiCTDTkhung;
                return Json(new { Message = msg });
            }

        }


        //Lưu kết quả ma trận Mục tiêu đào tạo cụ thể và Chuẩn đầu ra CTDT
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Ketquamatran(List<t_MTCDR_CTDT> listMTCDR)
        {
            string msg = "";
            try
            {
                using (var context = new fit_misDBEntities())
                {
                    int maMT = listMTCDR[0].MaMT;
                    var ctdt = context.t_MT_CTDT.Find(maMT).t_CTDaotao;
                    foreach (t_MT_CTDT item in ctdt.t_MT_CTDT)
                    {
                        var mtcdrTim = context.t_MT_CTDT.Find(item.MaMT);
                        mtcdrTim.t_CDR_CTDT.Clear();
                        context.SaveChanges();
                    }
                    foreach (var item in listMTCDR)
                    {
                        var mt = context.t_MT_CTDT.Find(item.MaMT);
                        var cdr = context.t_CDR_CTDT.Find(item.MaELO);
                        mt.t_CDR_CTDT.Add(cdr);
                        cdr.t_MT_CTDT.Add(mt);
                    }

                    context.SaveChanges();
                    return Json(new { msg = NotificationManagement.SuccessMessage.Luumatranthanhcong });
                }
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loikhongdulieu;
                return Json(new { Message = msg });
            }


        }

        //Create Bước 3 load loại kiến thức
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getLoaikt(int maKhoi)
        {

            var khoikienthuc = db.sc_Khoikienthuc.Where(x => x.MaQH == maKhoi).Select(x => new { x.MaKhoiKT, x.Mota }).ToList();
            return Json(khoikienthuc, JsonRequestBehavior.AllowGet);
        }

        //Create Bước 3
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult CreateCTDTStep3()
        {
            HttpCookie ck = Request.Cookies["CTDTId"];
            var idCTDT = ck.Value;
            var id = int.Parse(idCTDT);
            ViewBag.idCTDT = id;
            var khdt = db.tc_KHDaotao.Where(s => s.MaCTDT == id).ToList();
            var kienthuc = db.sc_Khoikienthuc.Where(x=>x.MaQH ==null).ToList();
            var chuandaura = db.t_CDR_CTDT.ToList();
            HocphanData d = new HocphanData();
            d.khdaotao = khdt;
            d.khoikienthuc = kienthuc;
            d.cdrctdt = chuandaura;

            //d.mtmh = new List<tc_MatranMH>();
            //for(int i = 0;i < khdt.Count; i++)
            //{
            //    var a = khdt[i].tc_MatranMH.ToList();
            //    d.mtmh.AddRange(a);
            //}
            List<HocphanData> ls = new List<HocphanData>();
            ls.Add(d);
            return View(ls);
        }

        //Thêm môn học Step 3
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult CreateCTDTStep3(tc_KHDaotao khdt, sc_Hocphan hocphan)
        {
            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var idctdt = int.Parse(idCTDT);
                var lst = new sc_Hocphan();
                lst.TenMH = hocphan.TenMH;
                lst.MaHT = hocphan.MaHT;
                db.sc_Hocphan.Add(lst);
                db.SaveChanges();
                var idhp = db.sc_Hocphan.AsEnumerable().LastOrDefault().MaMH;
                var khdaotao = new tc_KHDaotao();
                khdaotao.MaHP = idhp;
                khdaotao.MaKhoiKT = khdt.MaKhoiKT;
                if (khdt.MonTQ != null)
                {
                    khdaotao.MonTQ = khdt.MonTQ;
                }
                if (khdt.MonHT != null)
                {
                    khdaotao.MonHT = khdt.MonHT;
                }
                khdaotao.SoTC = khdt.SoTC;
                khdaotao.GioLT = khdt.GioLT;
                khdaotao.TrangthaiDC = TrangThaiDc.ChuaCapNhat;
                khdaotao.GioTH = khdt.GioTH;
                khdaotao.GioTT = khdt.GioTT;
                khdaotao.GioDA = khdt.GioDA;
                khdaotao.Hinhthuc = khdt.Hinhthuc;
                khdaotao.Hocky = khdt.Hocky;
                khdaotao.MuctieuHP = khdt.MuctieuHP;
                khdaotao.Mota = khdt.Mota;
                khdaotao.MaCTDT = idctdt;
                khdaotao.TenHP = lst.TenMH;
                HttpCookie td = Request.Cookies["thangdiem"];
                var thangdiem = td.Value;
                khdaotao.Thangdiem = thangdiem;
                db.tc_KHDaotao.Add(khdaotao);
                db.SaveChanges();
                msg = NotificationManagement.SuccessMessage.Themmoihocphanthanhcong;
                var lstkhdt = db.tc_KHDaotao.Where(s => s.MaCTDT == idctdt).ToList();
                var json = lstkhdt.Select(x => new { x.SoTC, x.Hocky, x.GioLT, x.GioTH, x.GioTT, x.GioDA, x.Hinhthuc, x.TenHP, x.MuctieuHP, x.Mota, x.MaHP, x.MonTQ, x.MonHT }).AsEnumerable().Select(x => new tc_KHDaotao { GioDA = x.GioDA, GioLT = x.GioLT, GioTH = x.GioTH, GioTT = x.GioTT, Hocky = x.Hocky, Hinhthuc = x.Hinhthuc, SoTC = x.SoTC, TenHP = x.TenHP, MuctieuHP = x.MuctieuHP, Mota = x.Mota, MaHP = x.MaHP, MonTQ = x.MonTQ, MonHT = x.MonHT });
                return Json(new { Message = msg, khdt = json });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loithemmoihocphan;
                return Json(new { Message = msg });
            }

        }


        //Lưu ma trận Step 3
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult luumatranstep3(List<tc_MatranMH> mtmh, List<int> arrkhdt)
        {
            string msg = "";
            try
            {
                using (var context = new fit_misDBEntities())
                {
                    int maCDR = mtmh[0].MaELO;
                    var ctdt = context.t_CDR_CTDT.Find(maCDR).t_CTDaotao;
                    foreach (t_CDR_CTDT item in ctdt.t_CDR_CTDT)
                    {
                        var hpcdrtim = context.t_CDR_CTDT.Find(item.MaELO);
                        hpcdrtim.tc_KHDaotao.Clear();
                        context.SaveChanges();
                    }
                    foreach (var item in mtmh)
                    {
                        var khdt = context.tc_KHDaotao.Find(item.MaKHDT);
                        var cdr = context.t_CDR_CTDT.Find(item.MaELO);
                        khdt.t_CDR_CTDT.Add(cdr);
                        cdr.tc_KHDaotao.Add(khdt);
                    }
                    context.SaveChanges();
                    return Json(new { msg = NotificationManagement.SuccessMessage.Luumatranthanhcong });
                }

            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loikhongdulieu;
                return Json(new { Message = msg });
            }
        }

        //Create bước 4
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult CreateCTDTStep4()
        {
            return View();
        }

        //Load thông tin của giảng viên
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadInfo()
        {
            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var id = int.Parse(idCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == id).ToList();
                var nhanVien = db.AspNetUsers.Where(s => s.AspNetRoles.Any(h=>h.Name.Equals(ROLES.EDITOR))).ToList();
                var jsonMonhoc = monHoc.Select(x => new { x.MaKHDT, x.sc_Hocphan.TenMH, gVien = x.tc_DecuongGV.Select(s => new { s.MaQL, s.AspNetUser.m_Nhanvien.Ho, s.AspNetUser.m_Nhanvien.Ten, s.NguoiST }) });
                var jsonNhanvien = nhanVien.Select(x => new { x.UserName, x.Id, x.m_Nhanvien.Ten, x.m_Nhanvien.Ho });
                return Json(new { monHoc = jsonMonhoc, nhanVien = jsonNhanvien });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loichung;
                return Json(new { Message = msg });
            }
        }

        //Thêm phân công giảng viên
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult addGV(DCuongGV dc)
        {
            try
            {
                HttpCookie ck = Request.Cookies["Id"];
                var id = ck.Value;
                if (dc.MaQL != null)
                {
                    foreach (var item in dc.MaQL)
                    {
                        var rs = db.tc_DecuongGV.Find(item);
                        db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
                foreach (var item in dc.Id)
                {
                    var decuong = new tc_DecuongGV();
                    decuong.MaKHDT = dc.MaKHDT;
                    decuong.NgayPC = DateTime.Now;
                    decuong.NguoiPC = id;
                    decuong.NguoiST = item;
                    db.tc_DecuongGV.Add(decuong);
                    db.SaveChanges();
                    //sNotitification.SendNoticeAssignedtoGV(item);
                }
                HttpCookie cook = Request.Cookies["CTDTId"];
                var idCTDT = cook.Value;
                var idctdt = int.Parse(idCTDT);
                var jsonMonhoc = db.tc_KHDaotao.Where(s => s.MaCTDT == idctdt).Select(x => new { x.MaKHDT, x.TenHP, gVien = x.tc_DecuongGV.Select(s => new { s.AspNetUser.m_Nhanvien.Ho, s.AspNetUser.m_Nhanvien.Ten }) });
                var jsonNhanVien = db.AspNetUsers.Where(s => s.AspNetRoles.Any(h => h.Name.Equals(ROLES.EDITOR))).Select(s => new { s.Id, s.UserName, s.m_Nhanvien.Ho, s.m_Nhanvien.Ten });
                return Json(new { msg = NotificationManagement.SuccessMessage.Themgv, tenGVPC = jsonMonhoc, nv = jsonNhanVien });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult saveEmail(sf_EmailTemplate et)
        {
            try
            {
                var rs = db.sf_EmailTemplate.Find(et.MaET);
                rs.Noidung = et.Noidung;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DM_Luumail });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        //Gửi Email
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult sendEmail(int id)
        {
            try
            {
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var maCTDT = int.Parse(idCTDT);
                var ctdtkhung = db.t_CTDaotao.Find(maCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == maCTDT).ToList();
                List<tc_DecuongGV> dcGV = new List<tc_DecuongGV>();
                foreach (var item in monHoc)
                {
                    var dCuong = db.tc_DecuongGV.Where(s => s.MaKHDT == item.MaKHDT).ToList();
                    foreach (var i in dCuong)
                    {
                        var rs = db.tc_DecuongGV.Find(i.MaQL);
                        var sendEmail = db.AspNetUsers.Find(rs.NguoiST);
                        var Gui = db.AspNetUsers.Find(rs.NguoiPC);
                        int hk = int.Parse(item.Hocky);
                        if (hk % 2 == 0)
                        {
                            hk = hk / 2;
                        }
                        else
                        {
                            hk = (hk / 2) + 1;
                        }
                        string ngNhan = sendEmail.m_Nhanvien.Ho + " " + sendEmail.m_Nhanvien.Ten;
                        string ngGui = Gui.m_Nhanvien.Ho + " " + Gui.m_Nhanvien.Ten;
                        sEmail.SendEmailtoGV(i.MaQL, rs.NguoiST, ngNhan, sendEmail.Email, id, item.TenHP, item.Hocky, ctdtkhung.sc_HeNganh.Mota, hk, ngGui);
                    }
                }
                var ctdt = db.t_CTDaotao.Find(maCTDT);
                ctdt.Trangthai = TrangThai.XayDungDeCuongHocPhan;
                db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.TaoCTDTkhungthanhcong });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.Loichung });
            }
        }
        //Email Template
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult showEmail(GetNgayHT ngay)
        {
            try
            {
                var lstEmail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung });
                HttpCookie ck = Request.Cookies["CTDTId"];
                var idCTDT = ck.Value;
                var maCTDT = int.Parse(idCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == maCTDT).ToList();
                List<tc_DecuongGV> dcGV = new List<tc_DecuongGV>();
                foreach (var item in monHoc)
                {
                    var dCuong = db.tc_DecuongGV.Where(s => s.MaKHDT == item.MaKHDT).ToList();
                    foreach (var i in dCuong)
                    {
                        var rs = db.tc_DecuongGV.Where(s => s.MaQL == i.MaQL).FirstOrDefault();
                        dcGV.Add(rs);
                    }
                    var addHT = db.tc_KHDaotao.Find(item.MaKHDT);
                    addHT.NgayHT = ngay.NgayHT;
                    db.Entry(addHT).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var decuongGV = dcGV.Select(s => new { s.NguoiST, s.AspNetUser.UserName, s.AspNetUser.m_Nhanvien.Ten, s.AspNetUser.m_Nhanvien.Ho });
                HttpCookie us = Request.Cookies["Id"];
                var id = us.Value;
                var user = db.AspNetUsers.Where(s => s.Id == id).Select(s => new { s.UserName, s.m_Nhanvien.Ho, s.m_Nhanvien.Ten }).FirstOrDefault();
                return Json(new { email = lstEmail, gVien = decuongGV, ngGui = user });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        //Xóa kết quả học tập mong đợi
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult deleteKetquahtmd(t_CDR_CTDT cdrctdt)
        {
            //try
            //{
            var ketquahtmd = db.t_CDR_CTDT.Find(cdrctdt.MaELO);
            db.Entry(ketquahtmd).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return Json(new { NotificationManagement.SuccessMessage.XoaketquahoctapmongdoiCTDTkhungthanhcong });
            //}
            //catch(Exception e)
            //{
            //    return Json(new { msg = NotificationManagement.SuccessMessage.XoaketquahoctapmongdoiCTDTkhungthanhcong });
            //}

        }

        //Xóa mục tiêu đào tạo cụ thể
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult DeleteMuctieudtct(t_MT_CTDT mtctdt, t_CDR_CTDT cdrctdt)
        {

            //try
            //{
            var muctieudaotaoct = db.t_MT_CTDT.Find(mtctdt.MaMT);
            db.Entry(muctieudaotaoct).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new { msg = NotificationManagement.SuccessMessage.XoamuctieudaotaocutheCTDTkhungthanhcong }); ;

            //}
            //catch (Exception e)
            //{
            //    return Json(new { msg = NotificationManagement.SuccessMessage.XoamuctieudaotaocutheCTDTkhungthanhcong }); ;
            //}
        }

        //Lấy dữ liệu kết quả học tập mong đợi
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getDataKQHTMD(t_CDR_CTDT cdrctdt)
        {
            try
            {
                var ketquahtmd = db.t_CDR_CTDT.Where(s => s.MaELO == cdrctdt.MaELO).Select(s => new { s.MaHT, s.Mota, s.MaELO }).FirstOrDefault();
                return Json(new { kqhtmd = ketquahtmd });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.Loichung });
            }

        }
        //Chỉnh sửa dữ liệu kết quả học tập mong đợi
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editKQHTMD(t_CDR_CTDT cdrctdt)
        {
            try
            {
                var ketquahtmd = db.t_CDR_CTDT.Find(cdrctdt.MaELO);
                ketquahtmd.MaHT = cdrctdt.MaHT;
                ketquahtmd.Mota = cdrctdt.Mota;
                db.Entry(ketquahtmd).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.Chinhsuaketquahoctapmongdoithanhcong });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.LoichinhsuaketquahoctapmongdoiCTDTkhung });
            }

        }

        //Lấy dữ liệu mục tiêu đào tạo cụ thể
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getDataMTDTCT(t_MT_CTDT mtctdt)
        {
            try
            {
                var muctieudtct = db.t_MT_CTDT.Where(s => s.MaMT == mtctdt.MaMT).Select(s => new { s.Phanloai, s.Mota, s.MaMT }).FirstOrDefault();
                return Json(new { muctieu = muctieudtct });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.Loichung });
            }

        }

        //Chỉnh sửa thông tin mục tiêu đào tạo cụ thể
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editMTDTCT(t_MT_CTDT mtctdt)
        {
            try
            {
                var muctieu = db.t_MT_CTDT.Find(mtctdt.MaMT);
                muctieu.Mota = mtctdt.Mota;
                muctieu.Phanloai = mtctdt.Phanloai;
                db.Entry(muctieu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.Chinhsuamuctieudaotaocuthethanhcong });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.LoichinhsuamuctieudaotaocutheCTDTkhung });
            }

        }

        //Xóa môn học
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult deleteSubject(tc_KHDaotao khdt)
        {
            var xoahocphan = db.tc_KHDaotao.Find(khdt.MaKHDT);
            db.Entry(xoahocphan).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new { msg = NotificationManagement.SuccessMessage.Xoahocphanthanhcong });
        }

        //Lấy dữ liệu thông tin môn học
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult getDatathongtinmonhoc(tc_KHDaotao ttmonhoc)
        {
            try
            {
                var thongtinmonhoc = db.tc_KHDaotao.Where(s => s.MaKHDT == ttmonhoc.MaKHDT).Select(s => new { s.MaKHDT, s.sc_Khoikienthuc.MaQH,s.sc_Khoikienthuc.MaKhoiKT, s.MaHP, s.SoTC, s.GioLT, s.GioTH, s.GioDA, s.GioTT, s.Hinhthuc, s.Hocky, s.MuctieuHP, s.Mota, s.TenHP, s.MonTQ, s.MonHT, s.sc_Hocphan.MaHT, s.sc_Hocphan.TenMH }).FirstOrDefault();
                return Json(new { monhoc = thongtinmonhoc });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.Loichung });
            }

        }

        //Chỉnh sửa dữ liệu thông tin môn học
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editthongtinmonhoc(tc_KHDaotao khdt, sc_Hocphan hocphan)
        {
            try
            {
                var monhoc = db.tc_KHDaotao.Find(khdt.MaKHDT);
                monhoc.MaKhoiKT = khdt.MaKhoiKT;
                monhoc.MonTQ = khdt.MonTQ;
                monhoc.MonHT = khdt.MonHT;
                monhoc.SoTC = khdt.SoTC;
                monhoc.GioLT = khdt.GioLT;
                monhoc.GioTH = khdt.GioTH;
                monhoc.GioDA = khdt.GioDA;
                monhoc.GioTT = khdt.GioTT;
                monhoc.Hinhthuc = khdt.Hinhthuc;
                monhoc.Hocky = khdt.Hocky;
                monhoc.MuctieuHP = khdt.MuctieuHP;
                monhoc.Mota = khdt.Mota;
                db.Entry(monhoc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var hphan = db.sc_Hocphan.Find(hocphan.MaMH);
                hphan.MaHT = hocphan.MaHT;
                hphan.TenMH = hocphan.TenMH;
                db.Entry(hphan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.Chinhsuahocphanthanhcong });
            }
            catch (Exception e)
            {
                return Json(new { Msg = NotificationManagement.ErrorMessage.Loichinhsuahocphan });
            }
        }

        //Edit Bước 2
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep2()
        {
            HttpCookie ck = Request.Cookies["EditCTDTId"];
            var idCTDT = ck.Value;
            int id = int.Parse(idCTDT);
            var ctdt = db.t_CTDaotao.Where(st => st.MaCT == id);
            var mtctdt = db.t_MT_CTDT.Where(s => s.MaCTDT == id).ToList();
            var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == id).ToList();
            CTDTData d = new CTDTData();
            d.ctdt = ctdt;
            d.mt_ctdt = mtctdt;
            d.cdr_ctdt = cdrctdt;
            List<CTDTData> ls = new List<CTDTData>();
            ls.Add(d);
            return View(ls);
        }
        //Edit Bước 2 các trường thông tin
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep2(string muctieudaotaochung, string mahienthi)
        {
            HttpCookie ck = Request.Cookies["EditCTDTId"];
            var idCTDT = ck.Value;
            int ID = int.Parse(idCTDT);
            var ctdt = db.t_CTDaotao.Find(ID);
            CTDTData data = new CTDTData();
            ctdt.Muctieu = muctieudaotaochung;
            db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditCTDTStep3", "CTDTKhung", new { id = ctdt.MaCT });
        }



        //Edit Bước 3
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep3()
        {
            HttpCookie ck = Request.Cookies["EditCTDTId"];
            var idCTDT = ck.Value;
            int ID = int.Parse(idCTDT);
            var rs = db.t_CTDaotao.Find(ID);
            var khungdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ID).ToList();
            var kienthuc = db.sc_Khoikienthuc.Where(s=>s.MaQH==null).ToList();
            var chuandaura = db.t_CDR_CTDT.Where(s => s.MaCTDT == ID).ToList();
            HocphanData d = new HocphanData();
            d.ctdt = rs;
            d.khdaotao = khungdt;
            d.khoikienthuc = kienthuc;
            d.cdrctdt = chuandaura;
            //d.mtmh = new List<tc_MatranMH>();
            //for (int i = 0; i < khungdt.Count; i++)
            //{
            //    var a = khungdt[i].tc_MatranMH.ToList();
            //    d.mtmh.AddRange(a);
            //}
            List<HocphanData> ls = new List<HocphanData>();
            ls.Add(d);
            return View(ls);
        }

        //Edit bước 3
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult EditCTDTStep3(tc_KHDaotao khdt, sc_Hocphan hocphan)
        {

            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["EditCTDTId"];
                var idCTDT = ck.Value;
                var idctdt = int.Parse(idCTDT);
                var lst = new sc_Hocphan();
                lst.TenMH = hocphan.TenMH;
                lst.MaHT = hocphan.MaHT;
                db.sc_Hocphan.Add(lst);
                db.SaveChanges();
                var idhp = db.sc_Hocphan.AsEnumerable().LastOrDefault().MaMH;
                var khdaotao = new tc_KHDaotao();
                khdaotao.MaHP = idhp;
                khdaotao.TenHP = hocphan.TenMH;
                khdaotao.MaKhoiKT = khdt.MaKhoiKT;
                if (khdt.MonTQ != null)
                {
                    khdaotao.MonTQ = khdt.MonTQ;
                }
                if (khdt.MonHT != null)
                {
                    khdaotao.MonHT = khdt.MonHT;
                }
                khdaotao.SoTC = khdt.SoTC;
                khdaotao.GioLT = khdt.GioLT;
                khdaotao.TrangthaiDC = TrangThaiDc.ChuaCapNhat;
                khdaotao.GioTH = khdt.GioTH;
                khdaotao.GioTT = khdt.GioTT;
                khdaotao.GioDA = khdt.GioDA;
                khdaotao.Hinhthuc = khdt.Hinhthuc;
                khdaotao.Hocky = khdt.Hocky;
                khdaotao.MuctieuHP = khdt.MuctieuHP;
                khdaotao.Mota = khdt.Mota;
                khdaotao.MaCTDT = idctdt;
                db.tc_KHDaotao.Add(khdaotao);
                db.SaveChanges();
                msg = NotificationManagement.SuccessMessage.Themmoihocphanthanhcong;
                var lstkhdt = db.tc_KHDaotao.Where(s => s.MaCTDT == idctdt).ToList();
                var json = lstkhdt.Select(x => new { x.SoTC, x.Hocky, x.GioLT, x.GioTH, x.GioTT, x.GioDA, x.Hinhthuc, x.TenHP, x.MuctieuHP, x.Mota, x.MaHP, x.MonTQ, x.MonHT }).AsEnumerable().Select(x => new tc_KHDaotao { GioDA = x.GioDA, GioLT = x.GioLT, GioTH = x.GioTH, GioTT = x.GioTT, Hocky = x.Hocky, Hinhthuc = x.Hinhthuc, SoTC = x.SoTC, TenHP = x.TenHP, MuctieuHP = x.MuctieuHP, Mota = x.Mota, MaHP = x.MaHP, MonTQ = x.MonTQ, MonHT = x.MonHT });
                return Json(new { Message = msg, khdt = json });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loithemmoihocphan;
                return Json(new { Message = msg });
            }
        }

        //Edit Bước 4
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTStep4()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult loadEditInfo()
        {
            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["EditCTDTId"];
                var idCTDT = ck.Value;
                var id = int.Parse(idCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == id).ToList();
                var nhanVien = db.AspNetUsers.Where(s => s.AspNetRoles.Any(h => h.Name.Equals(ROLES.EDITOR))).ToList();
                var jsonMonhoc = monHoc.Select(x => new { x.MaKHDT, x.sc_Hocphan.TenMH, gVien = x.tc_DecuongGV.Select(s => new { s.MaQL, s.AspNetUser.m_Nhanvien.Ho, s.AspNetUser.m_Nhanvien.Ten, s.NguoiST }) });
                var jsonNhanvien = nhanVien.Select(x => new { x.UserName, x.Id, x.m_Nhanvien.Ten, x.m_Nhanvien.Ho });
                return Json(new { monHoc = jsonMonhoc, nhanVien = jsonNhanvien });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.Loichung;
                return Json(new { Message = msg });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult editGV(EditDCuongGV dc)
        {
            try
            {
                HttpCookie ck = Request.Cookies["Id"];
                var id = ck.Value;
                if (dc.MaQL != null)
                {
                    foreach (var item in dc.MaQL)
                    {
                        var rs = db.tc_DecuongGV.Find(item);
                        var nt = db.sf_Notification.Where(s => s.MaDC == item).ToList();
                        foreach (var i in nt)
                        {
                            var tn = db.sf_Notification.Find(i.MaTB);
                            db.Entry(tn).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();

                        }
                        db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
                foreach (var item in dc.Id)
                {
                    var decuong = new tc_DecuongGV();
                    decuong.MaKHDT = dc.MaKHDT;
                    decuong.NgayPC = DateTime.Now;
                    decuong.NguoiPC = id;
                    decuong.NguoiST = item;
                    db.tc_DecuongGV.Add(decuong);
                    db.SaveChanges();

                }
                HttpCookie cook = Request.Cookies["EditCTDTId"];
                var idCTDT = cook.Value;
                var idctdt = int.Parse(idCTDT);
                var jsonMonhoc = db.tc_KHDaotao.Where(s => s.MaCTDT == idctdt).Select(x => new { x.MaKHDT, x.TenHP, gVien = x.tc_DecuongGV.Select(s => new { s.AspNetUser.m_Nhanvien.Ho, s.AspNetUser.m_Nhanvien.Ten }) });
                var jsonNhanVien = db.AspNetUsers.Where(s => s.AspNetRoles.Any(h => h.Name.Equals(ROLES.EDITOR))).Select(s => new { s.Id, s.UserName, s.m_Nhanvien.Ho, s.m_Nhanvien.Ten });
                return Json(new { msg = NotificationManagement.SuccessMessage.Suagv, tenGVPC = jsonMonhoc, nv = jsonNhanVien });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult EditsendEmail(int id)
        {
            try
            {
                HttpCookie ck = Request.Cookies["EditCTDTId"];
                var idCTDT = ck.Value;
                var maCTDT = int.Parse(idCTDT);
                var ctdtkhung = db.t_CTDaotao.Find(maCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == maCTDT).ToList();
                List<tc_DecuongGV> dcGV = new List<tc_DecuongGV>();
                foreach (var item in monHoc)
                {
                    var dCuong = db.tc_DecuongGV.Where(s => s.MaKHDT == item.MaKHDT).ToList();
                    foreach (var i in dCuong)
                    {
                        var rs = db.tc_DecuongGV.Find(i.MaQL);
                        var sendEmail = db.AspNetUsers.Find(rs.NguoiST);
                        var Gui = db.AspNetUsers.Find(rs.NguoiPC);
                        int hk = int.Parse(item.Hocky);
                        if (hk % 2 == 0)
                        {
                            hk = hk / 2;
                        }
                        else
                        {
                            hk = (hk / 2) + 1;
                        }
                        string ngNhan = sendEmail.m_Nhanvien.Ho + " " + sendEmail.m_Nhanvien.Ten;
                        string ngGui = Gui.m_Nhanvien.Ho + " " + Gui.m_Nhanvien.Ten;
                        sEmail.SendEmailtoGV(i.MaQL, rs.NguoiST, ngNhan, sendEmail.Email, id, item.TenHP, item.Hocky, ctdtkhung.sc_HeNganh.Mota, hk, ngGui);
                    }
                }
                var ctdt = db.t_CTDaotao.Find(maCTDT);
                ctdt.Trangthai = TrangThai.XayDungDeCuongHocPhan;
                db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.ChinhsuaCTDTkhungthanhcong });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.Loichung });
            }
        }
        //Email Template
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult EditshowEmail(GetNgayHT ngay)
        {
            try
            {
                var lstEmail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung });
                HttpCookie ck = Request.Cookies["EditCTDTId"];
                var idCTDT = ck.Value;
                var maCTDT = int.Parse(idCTDT);
                var monHoc = db.tc_KHDaotao.Where(s => s.MaCTDT == maCTDT).ToList();
                List<tc_DecuongGV> dcGV = new List<tc_DecuongGV>();
                foreach (var item in monHoc)
                {
                    var dCuong = db.tc_DecuongGV.Where(s => s.MaKHDT == item.MaKHDT).ToList();
                    foreach (var i in dCuong)
                    {
                        var rs = db.tc_DecuongGV.Where(s => s.MaQL == i.MaQL).FirstOrDefault();
                        dcGV.Add(rs);
                    }
                    var addHT = db.tc_KHDaotao.Find(item.MaKHDT);
                    addHT.NgayHT = ngay.NgayHT;
                    db.Entry(addHT).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var decuongGV = dcGV.Select(s => new { s.NguoiST, s.AspNetUser.UserName, s.AspNetUser.m_Nhanvien.Ten, s.AspNetUser.m_Nhanvien.Ho });
                HttpCookie us = Request.Cookies["Id"];
                var Id = us.Value;
                var user = db.AspNetUsers.Where(s => s.Id == Id).Select(s => new { s.UserName, s.m_Nhanvien.Ho, s.m_Nhanvien.Ten }).FirstOrDefault();
                return Json(new { email = lstEmail, gVien = decuongGV, ngGui = user });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }
        //Thêm mục tiêu đào tạo cụ thể - edit 
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Themmtdtctedit(Muctieudaotaocuthe mtdtct)
        {
            string msg = "";
            try
            {
                if (ModelState.IsValid)
                {
                    HttpCookie ck = Request.Cookies["EditCTDTId"];
                    var idCTDT = ck.Value;
                    var idctdt = int.Parse(idCTDT);
                    if (mtdtct.Mota1 != null)
                    {

                        string[] splitmota = mtdtct.Mota1.Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai1;
                            lst.MaCTDT = idctdt;
                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }

                    if (mtdtct.Mota2 != null)
                    {
                        string[] splitmota = mtdtct.Mota2.Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai2;
                            lst.MaCTDT = idctdt;
                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }

                    if (mtdtct.Mota3 != null)
                    {

                        string[] splitmota = mtdtct.Mota3.Split('\n');
                        foreach (string motatungphan in splitmota)
                        {
                            var lst = new t_MT_CTDT();
                            lst.Mota = motatungphan;
                            lst.Phanloai = mtdtct.Phanloai3;
                            lst.MaCTDT = idctdt;
                            db.t_MT_CTDT.Add(lst);
                            db.SaveChanges();
                        }
                    }

                    msg = NotificationManagement.SuccessMessage.ThemmoimuctieudaotaocutheCTDTkhungthanhcong;
                    var lstmtctdt = db.t_MT_CTDT.Where(s => s.MaCTDT == idctdt).ToList();
                    var json = lstmtctdt.Select(x => new { x.Phanloai, x.Mota }).AsEnumerable().Select(x => new t_MT_CTDT { Phanloai = x.Phanloai, Mota = x.Mota });
                    return Json(new { Message = msg, mtdtct = json });
                }
            }
            catch (RetryLimitExceededException)
            {
                msg = NotificationManagement.ErrorMessage.LoithemmoimuctieudaotaocutheCTDTkhung;
                return Json(new { Message = msg });
            }
            return Json(mtdtct);

        }

        //Thêm kết quả học tập mong đợi - edit
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Themkqhtmongdoiedit(Ketquahoctapmongdoi cdrctdt)
        {
            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["EditCTDTId"];
                var idCTDT = ck.Value;
                var idctdt = int.Parse(idCTDT);
                if (cdrctdt.Mota1 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT1;
                    lst.Mota = cdrctdt.Mota1;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                if (cdrctdt.Mota2 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT2;
                    lst.Mota = cdrctdt.Mota2;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                if (cdrctdt.Mota3 != null)
                {
                    var lst = new t_CDR_CTDT();
                    lst.MaHT = cdrctdt.MaHT3;
                    lst.Mota = cdrctdt.Mota3;
                    lst.MaCTDT = idctdt;
                    db.t_CDR_CTDT.Add(lst);
                    db.SaveChanges();
                }

                msg = NotificationManagement.SuccessMessage.ThemmoiketquahoctapmongdoiCTDTkhungthanhcong;
                var lstmtctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == idctdt).ToList();
                var json = lstmtctdt.Select(x => new { x.MaHT, x.Mota }).AsEnumerable().Select(x => new t_CDR_CTDT { MaHT = x.MaHT, Mota = x.Mota });
                return Json(new { Message = msg, chuandaura = json });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.LoithemmoiketquahoctapmongdoiCTDTkhung;
                return Json(new { Message = msg });
            }

        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult loadKHDTList()
        {
            //lấy mã ctdt gán vào cookie
            HttpCookie ctdt = Request.Cookies["CTDTfKHDT"];
            var id = ctdt.Value;
            int ctdtId = int.Parse(id);
            List<tc_KHDaotao> khdt = new List<tc_KHDaotao>();
            var lstkhdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ctdtId).Select(s => new { s.Hocky, s.MaCTDT, MaHP = s.sc_Hocphan.MaHT, s.MaKHDT, s.SoTC, TenHP = s.sc_Hocphan.TenMH });
            return Json(new { khdt = lstkhdt });

        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult loadReviewInfo(int id)
        {
            try
            {
                //lấy dữ liệu tử bảng KHDaotao  tương ứng với mã đã nhớ
                var khdt = db.tc_KHDaotao.Where(s => s.MaKHDT == id).Select(s => new
                {
                    s.MaKHDT,
                    s.MaKhoiKT,
                    MotaKienthuc = s.sc_Khoikienthuc.Mota,
                    s.GioLT,
                    s.GioDA,
                    s.Hocky,
                    s.SoTC,
                    s.GioTH,
                    s.GioTT,
                    s.Hinhthuc,
                    TenHP = s.sc_Hocphan.TenMH,
                    s.MuctieuHP,
                    s.Mota,
                    s.NgonnguGD,
                    s.PPGD,
                    s.PPHT,
                    s.NhiemvuSV,
                    s.Thangdiem,
                    s.MonTQ,
                    s.MonHT,
                    s.NoidungCN,
                    s.MaCTDT,
                    s.PhuongtienGD,
                    s.PhuongtienThi,
                    s.TrangthaiDC,
                    s.GhichuDG,
                    TenCTDT = s.t_CTDaotao.TenCT,
                    MaHP = s.sc_Hocphan.MaHT
                }).FirstOrDefault();
                //lấy dữ liệu từ bảng chuẩn đầu ra CTDT
                var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == khdt.MaCTDT).Select(s => new { s.MaELO, s.MaHT, s.Mota, s.Phanloai });
                //lấy dữ liệu từ bảng khối kiến thức
                var kkt = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, MotaKT = s.Mota });
                //lấy dữ liệu từ bảng chuẩn đầu ra học phần
                var rs = db.tc_CDR_HP.Where(s => s.MaKHDT == id).Select(s => new { s.MaCELO, s.MaHT, s.Mota, s.Phanloai, MaHT1 = s.tc_MatranCDR.Select(h => new { MaCELO = h.tc_CDR_HP.MaHT, MaELO = h.t_CDR_CTDT.MaHT, h.Mucdo }) });
                //lấy dữ liệu từ bảng nội dung hp
                var nd = db.tc_NoidungHP.Where(s => s.MaKHDT == id).Select(s => new { s.TenHT, s.Phanloai, s.Noidung, s.Ghichu, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaHT }) });
                //lấy dữ liệu từ bảng tài liệu hp
                var tlhp = db.tc_TailieuHP.Where(s => s.MaKHDT == id).Select(s => new { s.MaTL, s.LoaiTL, s.TenTL, s.Tacgia, s.NhaXB, s.NamXB, s.Kieunhap });
                var loaihp = db.sc_LoaiPH.Select(s => new { s.MaLoaiPH, s.TenLoaiPH });

                return Json(new { mhoc = khdt, CDR = cdrctdt, KKT = kkt, cdrhp = rs, ndung = nd, tHP = tlhp, loaiPH = loaihp });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_LoadData });
            }
        }

        [HttpPost]
        public ActionResult ShowFile(StringPDF id)
        {
            //id += ".pdf";
            var path = Server.MapPath("~/Areas/CMS/Documents/");
            var file = Path.Combine(path, id.pdf);
            file = Path.GetFullPath(file);
            if (!file.StartsWith(path))
            {
                // someone tried to be smart and sent 
                // ?filename=..\..\creditcard.pdf as parameter
                throw new HttpException(403, "Forbidden");
            }
            return File(file, "application/pdf");
        }
    }
}
