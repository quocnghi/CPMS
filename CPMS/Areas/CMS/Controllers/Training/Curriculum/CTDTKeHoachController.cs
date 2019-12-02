// <copyright file="CTDTKeHoachController.cs" company="PlaceholderCompany">
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
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using Capstone.Areas.CMS.Controllers.Tools;
    using CommonLibrary;
    using Capstone.Areas.CMS.Models;
    #endregion

    /// <summary>
    /// Manage CTDT Ke hoach, such as Index and Create new.
    /// </summary>
    public class CTDTKeHoachController : Controller
    {
        private fit_misDBEntities db = new fit_misDBEntities();
        private SendEmail sEmail = new SendEmail();

        /// <summary>
        /// List all CTDT Ke hoach existing on the system.
        /// </summary>
        /// <returns>A webpage.</returns>
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult Index()
        {
            // Load all CTDT Ke hoach from database
            var ctdtKeHoachs = this.db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KH)).ToList();

            // Load all He nganh from database
            var heNganhs = this.db.sc_HeNganh.ToList();

            return this.View(new Tuple<IEnumerable<t_CTDaotao>, IEnumerable<sc_HeNganh>>(ctdtKeHoachs, heNganhs));
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult loadList()
        {
            try
            {
                var rs = db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KH)).ToList();
                var json = rs.Select(x => new { x.MaCT, x.Trinhdo, x.Phienban, x.Tinhtrang, x.sc_HeNganh.Mota }).AsEnumerable().Select(x => new t_CTDaotao { MaCT = x.MaCT, Trinhdo = x.Trinhdo, Phienban = x.Phienban, Tinhtrang = x.Tinhtrang, Ghichu = x.Mota });
                return Json(new { data = json });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public JsonResult changeNganh(ChangeNganh cn)
        {
            try
            {
                int mahe = int.Parse(cn.He);
                var rs = db.sc_HeNganh.Where(s => s.MaQH == mahe).Select(s => new {s.MaHN,s.Mota });
                return Json(new { nganh = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Find(FindCTDTKhung ctdt)
        {
            string msg = "";
            try
            {
                //gọi ctdt khung từ db lên và tìm theo ngành và hệ
                var ctdtkhung = db.t_CTDaotao.Where(s => s.sc_HeNganh.MaHN == ctdt.manganh && s.Trinhdo.Equals(ctdt.trinhdo) && s.LoaiCT.Equals(LoaiHinhDT.CTDT_KHUNG) && s.Tinhtrang.Equals(TinhTrang.HieuLuc)).FirstOrDefault();
                //kiểm tra dữ liệu
                if (ctdtkhung != null)
                {
                    //tìm các môn học thuộc ct đào tạo đó   
                    var khdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ctdtkhung.MaCT).ToList();
                    if (khdt != null)
                    {
                        //Gọi bảng khối lớp
                        var klop = db.sc_Khoilop.ToList();
                        //Thêm trường thông tin cần hiện
                        var ctdaotao = new CTDTKeHoach
                        {
                            id = ctdtkhung.MaCT,
                            tenct = ctdtkhung.TenCT,
                            trinhdokhung = ctdtkhung.Trinhdo,
                            tenkhoa = ctdtkhung.sc_HeNganh.Mota,
                            mahn = (int)ctdtkhung.MaHN,
                            loaihinhdt = ctdtkhung.HinhthucDT,
                            soqd = ctdtkhung.SoQD,
                            ngayqd = ctdtkhung.NgayQD,
                            muctieu = ctdtkhung.Muctieu,
                            tgiandt = ctdtkhung.ThoigianDT,
                            khoiluongkt = ctdtkhung.Khoiluongkienthuc,
                            doituong = ctdtkhung.Doituong,
                            quytrinh = ctdtkhung.QuytrinhDT,
                            thangdiem = ctdtkhung.Thangdiem,
                            csvc = ctdtkhung.CosoVC
                        };
                        //Thêm danh sách các môn học vào class
                        var jsondata = khdt.Select(x => new { x.sc_Hocphan.MaMH, x.sc_Hocphan.TenMH, x.sc_Hocphan.MaHT, x.Hocky, x.GioDA, x.GioLT, x.GioTH, x.GioTT, x.SoTC, x.Hinhthuc, Ten = x.sc_Khoikienthuc.Mota, x.MaHP, x.MaKhoiKT }).AsEnumerable().Select(x => new tc_KHDaotao { MaHP = x.MaMH, TenHP = x.TenMH, Hocky = x.Hocky, GioDA = x.GioDA, GioLT = x.GioLT, GioTH = x.GioTH, GioTT = x.GioTT, SoTC = x.SoTC, Hinhthuc = x.Hinhthuc, MaKhoiKT = x.MaKhoiKT, Mota = x.Ten });
                        //Thêm danh sách khối lớp
                        var khoilop = klop.Select(x => new { x.MaKhoi, x.TenKhoi, x.NamBD, x.NamKT }).AsEnumerable().Select(x => new sc_Khoilop { MaKhoi = x.MaKhoi, TenKhoi = x.TenKhoi, NamBD = x.NamBD, NamKT = x.NamKT });
                        //Gán vào class để truyền qua json
                        JsonReturnCTDTKeHoach json = new JsonReturnCTDTKeHoach { ctdtao = ctdaotao, khdaotao = jsondata, khoilop = khoilop };
                        // nếu dữ liệu không null thì trả về json cho view
                        return Json(json);
                    }
                    var data = new CTDTKeHoach
                    {
                        id = ctdtkhung.MaCT,
                        tenct = ctdtkhung.TenCT,
                        trinhdokhung = ctdtkhung.Trinhdo,
                        tenkhoa = ctdtkhung.sc_HeNganh.Mota,
                        loaihinhdt = ctdtkhung.HinhthucDT,
                        soqd = ctdtkhung.SoQD,
                        ngayqd = ctdtkhung.NgayQD,
                        muctieu = ctdtkhung.Muctieu,
                        tgiandt = ctdtkhung.ThoigianDT,
                        khoiluongkt = ctdtkhung.Khoiluongkienthuc,
                        doituong = ctdtkhung.Doituong,
                        quytrinh = ctdtkhung.QuytrinhDT,
                        thangdiem = ctdtkhung.Thangdiem,
                        csvc = ctdtkhung.CosoVC
                    };
                    return Json(data);
                }
                //nếu dữ liệu null thì trả về thông báo
                msg = NotificationManagement.ErrorMessage.TimCTDT;
                return Json(new { Message = msg, Status = 200 });
            }
            catch (Exception e)
            {
                // nếu xảy ra lỗi thì trả về thông báo
                msg = NotificationManagement.ErrorMessage.LoadData;
                return Json(new { Message = msg, Status = 500 });
            }
        }

        [HttpPost]
        //[Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult Create(GetCTDTKeHoach json)
        {
            string msg = "";
            try
            {
                var rs = db.t_CTDaotao.Where(s => s.Tinhtrang.Equals(TinhTrang.HieuLuc) && s.MaKhoi == json.khoilop && s.MaHN == json.ctdtao.mahn).FirstOrDefault();
                CTDTKeHoach ctdt = json.ctdtao;
                var rsctdt = new t_CTDaotao();
                rsctdt.MaTC = ctdt.id;
                rsctdt.TenCT = ctdt.tenct;
                rsctdt.Trinhdo = ctdt.trinhdokhung;
                rsctdt.MaHN = ctdt.mahn;
                rsctdt.HinhthucDT = ctdt.loaihinhdt;
                rsctdt.SoQD = ctdt.soqd;
                rsctdt.NgayQD = ctdt.ngayqd;
                rsctdt.Muctieu = ctdt.muctieu;
                rsctdt.ThoigianDT = ctdt.tgiandt;
                rsctdt.Khoiluongkienthuc = ctdt.khoiluongkt;
                rsctdt.Doituong = ctdt.doituong;
                rsctdt.QuytrinhDT = ctdt.quytrinh;
                rsctdt.Thangdiem = ctdt.thangdiem;
                rsctdt.CosoVC = ctdt.csvc;
                rsctdt.Tinhtrang = TinhTrang.HieuLuc;
                if (rs != null)
                {
                    rs.Tinhtrang = TinhTrang.LuuTru;
                    db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }            
                rsctdt.LoaiCT = LoaiHinhDT.CTDT_KH;
                db = new fit_misDBEntities();
                var henganh = db.sc_HeNganh.Find(ctdt.mahn);
                var pb = db.sc_Ma.Where(s => s.LoaiMa == Ma.Phienban).FirstOrDefault();
                string tema = pb.TenMa;
                tema = tema.Replace("{LoaiCT}", LoaiHinhDT.CTDT_KH);
                tema = tema.Replace("{TenRG}", henganh.Tenrutgon);
                tema = tema.Replace("{Nam}", DateTime.Now.Year.ToString());
                var ctdtkh = db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KH) && s.MaHN == ctdt.mahn).AsEnumerable().LastOrDefault();
                if (ctdtkh != null)
                {
                    var pbtdt = int.Parse(ctdtkh.Phienban.Substring(ctdtkh.Phienban.Length - 3));
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
                    rsctdt.Phienban = tema + "-" + phienban;
                }
                else
                {
                    rsctdt.Phienban = tema + "-" + pb.Phienban;
                }

                rsctdt.Trangthai = TrangThai.KhoiTao;
                rsctdt.MaKhoi = json.khoilop;
                db.t_CTDaotao.Add(rsctdt);
                db.SaveChanges();
                var newctdtkehoach = db.t_CTDaotao.AsEnumerable().LastOrDefault();
                foreach (var item in json.khdaotao)
                {
                    var khdt = new tc_KHDaotao();
                    khdt.Mota = item.Mota;
                    khdt.Hinhthuc = item.Hinhthuc;
                    khdt.GioDA = item.GioDA;
                    khdt.GioLT = item.GioLT;
                    khdt.GioTH = item.GioTH;
                    khdt.GioTT = item.GioTT;
                    khdt.Hocky = item.Hocky;
                    khdt.MaCTDT = newctdtkehoach.MaCT;
                    khdt.MaHP = item.MaHP;
                    khdt.MaKhoiKT = item.MaKhoiKT;
                    khdt.MonSH = item.MonSH;
                    khdt.MonHT = item.MonHT;
                    khdt.MonTQ = item.MonTQ;
                    khdt.MuctieuHP = item.MuctieuHP;
                    khdt.TenHP = item.TenHP;
                    //khdt.NgayCN = item.NgayCN;
                    //khdt.Ngaytao = item.Ngaytao;
                    khdt.NgonnguGD = item.NgonnguGD;
                    khdt.NhiemvuSV = item.NhiemvuSV;
                    khdt.PPGD = item.PPGD;
                    khdt.PPHT = item.PPHT;
                    khdt.SoTC = item.SoTC;
                    khdt.Trinhdo = item.Trinhdo;
                    db.tc_KHDaotao.Add(khdt);
                    db.SaveChanges();
                }
                msg = NotificationManagement.SuccessMessage.ThemCTDTKH;
                var jsondata = db.t_CTDaotao.Where(s => s.LoaiCT.Equals(LoaiHinhDT.CTDT_KH)).Select(x => new { x.MaCT, x.Trinhdo, x.Phienban, x.Tinhtrang, x.sc_HeNganh.Mota, x.sc_Khoilop.TenKhoi });
                return Json(new { Message = msg, data = jsondata });
            }
            catch (Exception e)
            {
                msg = NotificationManagement.ErrorMessage.LoadData;
                return Json(new { Message = msg });
            }

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
        //Edit Bước 1 
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTKehoachStep1(int id)
        {
            HttpCookie ck = new HttpCookie("EditCTDTKHId");
            ck.Value = id.ToString();
            ck.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(ck);
            var rs = db.t_CTDaotao.Where(st => st.MaCT == id && st.LoaiCT.Equals(LoaiHinhDT.CTDT_KH));
            var lst = db.sc_HeNganh.Where(s => s.MaHN == rs.FirstOrDefault().sc_HeNganh.MaQH).ToList();
            CTDTData d = new CTDTData();
            d.ctdt = rs;
            d.henganh = lst;
            List<CTDTData> ls = new List<CTDTData>();
            ls.Add(d);
            return View(ls);
        }

        //Lưu dữ liệu xuông data
        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTKehoachStep1(string Phienbanctdt
            , string Cosovatchat
            , string Thangdiem, string Quytrinhdaotaodieukientotnghiep, string Doituongtuyensinh
            , string Thoigiandaotao, string Loaihinhdaotao
            , string Trinhdodaotao, string Tenctdt
            , string nganhdaotao)
        {
            HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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
            return RedirectToAction("EditCTDTKehoachStep2", "CTDTKeHoach", new { id = ctdt.MaCT });
        }


        //Edit Bước 2
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTKehoachStep2(string ID)
        {
            //HttpCookie ck = Request.Cookies["EditCTDTKHId"];
            //var idCTDT = ck.Value;
            int id = int.Parse(ID);
            var rs = db.t_CTDaotao.Where(st => st.MaCT == id);
            var mtctdt = db.t_MT_CTDT.Where(s => s.MaCTDT == id).ToList();
            var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == id).ToList();
            CTDTData d = new CTDTData();
            d.ctdt = rs;
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
        public ActionResult EditCTDTKehoachStep2(string Muctieudaotaochung, string mahienthi)
        {
            HttpCookie ck = Request.Cookies["EditCTDTKHId"];
            var idCTDT = ck.Value;
            int ID = int.Parse(idCTDT);
            var ctdt = db.t_CTDaotao.Find(ID);
            CTDTData data = new CTDTData();
            ctdt.Muctieu = Muctieudaotaochung;
            db.Entry(ctdt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditCTDTKehoachStep3", "CTDTKeHoach", new { id = ctdt.MaCT });
        }


        //Lưu kết quả ma trận Mục tiêu đào tạo cụ thể và Chuẩn đầu ra CTDT ở Edit step 2
        //[Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        //public JsonResult Ketquamatranedit(List<t_MTCDR_CTDT> listMTCDR)
        //{
        //    string msg = "";
        //    try
        //    {
        //        using (var context = new CMSEntities())
        //        {
        //            int maMT = listMTCDR[0].MaMT;
        //            var ctdt = context.t_MT_CTDT.Find(maMT).t_CTDaotao;
        //            foreach (t_MT_CTDT item in ctdt.t_MT_CTDT)
        //            {
        //                var mtcdrTim = context.t_MTCDR_CTDT.Where(x => x.MaMT == item.MaMT).ToList();
        //                context.t_MTCDR_CTDT.RemoveRange(mtcdrTim);
        //                context.SaveChanges();
        //            }
        //            context.t_MTCDR_CTDT.AddRange((List<t_MTCDR_CTDT>)listMTCDR);
        //            context.SaveChanges();
        //            return Json(new { msg = NotificationManagement.SuccessMessage.Luumatranthanhcong });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        msg = NotificationManagement.ErrorMessage.Loikhongdulieu;
        //        return Json(new { Message = msg });
        //    }


        //}

        //Edit Bước 3
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult EditCTDTKehoachStep3()
        {
            HttpCookie ck = Request.Cookies["EditCTDTKHId"];
            var idCTDT = ck.Value;
            int ID = int.Parse(idCTDT);
            var rs = db.t_CTDaotao.Find(ID);
            var khungdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ID).ToList();
            var kienthuc = db.sc_Khoikienthuc.Where(s => s.MaQH == null).ToList();
            var chuandaura = db.t_CDR_CTDT.Where(s => s.MaCTDT == ID).ToList();
            HocphanData d = new HocphanData();
            d.ctdt = rs;
            d.khdaotao = khungdt;
            d.khoikienthuc = kienthuc;
            d.cdrctdt = chuandaura;
            List<HocphanData> ls = new List<HocphanData>();
            ls.Add(d);
            return View(ls);
        }

        //Edit bước 3
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult EditCTDTKehoachStep3(tc_KHDaotao khdt, sc_Hocphan hocphan)
        {

            string msg = "";
            try
            {
                HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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
        public ActionResult EditCTDTKehoachStep4()
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
                HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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
                HttpCookie cook = Request.Cookies["EditCTDTKHId"];
                var idCTDT = cook.Value;
                var idctdt = int.Parse(idCTDT);
                var jsonMonhoc = db.tc_KHDaotao.Where(s => s.MaCTDT == idctdt).Select(x => new { x.MaKHDT, x.TenHP, gVien = x.tc_DecuongGV.Select(s => new { s.AspNetUser.m_Nhanvien.Ho, s.AspNetUser.m_Nhanvien.Ten }) });
                var jsonNhanVien = db.AspNetUsers.Where(s => s.AspNetRoles.Any(h => h.Name.Equals(ROLES.EDITOR))).Select(s => new { s.Id, s.UserName, s.m_Nhanvien.Ho, s.m_Nhanvien.Ten });
                return Json(new { msg = NotificationManagement.SuccessMessage.KH_Suagv, tenGVPC = jsonMonhoc, nv = jsonNhanVien });
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
                HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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
                            string ngNhan = sendEmail.m_Nhanvien.Ho + " " + sendEmail.m_Nhanvien.Ten;
                            string ngGui = Gui.m_Nhanvien.Ho + " " + Gui.m_Nhanvien.Ten;
                            sEmail.SendEmailtoGV(i.MaQL, rs.NguoiST, ngNhan, sendEmail.Email, id, item.TenHP, item.Hocky, ctdtkhung.sc_HeNganh.Mota, hk, ngGui);
                        }
                        else
                        {
                            hk = (hk / 2) + 1;
                            string ngNhan = sendEmail.m_Nhanvien.Ho + " " + sendEmail.m_Nhanvien.Ten;
                            string ngGui = Gui.m_Nhanvien.Ho + " " + Gui.m_Nhanvien.Ten;
                            sEmail.SendEmailtoGV(i.MaQL, rs.NguoiST, ngNhan, sendEmail.Email, id, item.TenHP, item.Hocky, ctdtkhung.sc_HeNganh.Mota, hk, ngGui



                                );
                        }
                        //dcGV.Add(rs);
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
        public JsonResult EditshowEmail(GetNgayHT ngayht)
        {
            try
            {
                var lstEmail = db.sf_EmailTemplate.Select(s => new { s.MaET, s.Chude, s.Noidung });
                HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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
                    //addHT.NgayHT = ngayht.NgayHT;
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
                    HttpCookie ck = Request.Cookies["EditCTDTKHId"];
                    var idCTDT = ck.Value;
                    var idctdt = int.Parse(idCTDT);
                    //var idctdt = 7;
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

                    msg = NotificationManagement.SuccessMessage.KH_Themmuctieu;
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
                HttpCookie ck = Request.Cookies["EditCTDTKHId"];
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

        //Lấy dữ liệu Chương trình đào tạo cho Đánh giá
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public JsonResult LaydulieudanhgiaCTDT(t_CTDaotao ttctdt)
        {

            try
            {
                var ctdt = db.t_CTDaotao.Find(ttctdt.MaCT);
                //lấy dữ liệu mục tiêu đào tạo chung của CTDT
                var muctieuchung = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.Muctieu }).FirstOrDefault();

                var kqdanhgia = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.Ghichu }).FirstOrDefault();

                //lấy dữ liệu mục tiêu đào tạo cụ thể của CTDT
                var mtdtcuthe = db.t_MT_CTDT.Where(s => s.MaCTDT == ctdt.MaTC).Select(s => new { s.Phanloai, s.Mota, s.MaMT, CDR = s.t_CDR_CTDT.Select(m => new { m.Mota }) }).ToList();

                //lấy dữ liệu kết quả học tập mong đợi của CTDT
                var chuandaura = db.t_CDR_CTDT.Where(s => s.MaCTDT == ctdt.MaTC).Select(s => new { s.MaHT, s.Mota }).ToList();

                //lấy danh sách học phần ứng với mỗi CTDT
                var dshocphan = db.tc_KHDaotao.Where(x => x.MaCTDT == ttctdt.MaCT).Select(s => new { s.sc_Hocphan.MaHT, s.sc_Hocphan.TenMH, CDR = s.t_CDR_CTDT.Select(m => new { m.Mota }) }).ToList();

                //lấy dữ liệu thông tin chung của CTDT
                var thongtinctdt = db.t_CTDaotao.Where(s => s.MaCT == ttctdt.MaCT).Select(s => new { s.MaCT, s.TenCT, s.sc_HeNganh.MaHN, s.Phienban, s.Trinhdo, s.HinhthucDT, s.ThoigianDT, s.QuytrinhDT, s.Doituong, s.CosoVC, s.Muctieu, s.Thangdiem }).FirstOrDefault();
                string tenHN = db.sc_HeNganh.Find(thongtinctdt.MaHN).Mota;
                return Json(new { thongtinctdt = thongtinctdt, tenHN = tenHN, dshocphan = dshocphan, mtdtcuthe = mtdtcuthe, chuandaura = chuandaura, kqdanhgia = kqdanhgia, });
            }
            catch (Exception e)
            {
                return Json(new { Message = NotificationManagement.ErrorMessage.Loichung });

            }

        }
    }
}