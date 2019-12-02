using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Capstone.Models;
using CommonRS.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;


namespace Capstone.Areas.PMS.Controllers
{
    public class APIController : Controller
    {
        fit_misDBEntities db = new fit_misDBEntities();

        /// <summary>
        /// Purpose: GET API - List User account with mail and name
        /// Developer: Nguyen Nguyen
        /// Date: 9/4/2019 
        /// </summary>
        /// <param name=""></param> 
        /// <returns></returns>
        public JsonResult DanhSachTaiKhoan()
        {
            var listU = db.AspNetUsers.Select(s => new { id = s.Id, userEmail = s.Email, userName = s.UserName }).ToList();
            return Json(listU, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: GET API - List User roles with email
        /// Developer: Nguyen Nguyen
        /// Date: 9/4/2019 
        /// </summary>
        /// <param name=""></param> 
        /// <returns></returns>
        public ActionResult DanhSachRoles(string id)
        {
            var userRoles = db.AspNetUsers.Find(id).AspNetRoles.Select(s => s.Name).ToList();
            return Json(userRoles, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: GET API - List user roles with UserID
        /// Developer: Nguyen Nguyen
        /// Date: 8/4/2019 
        /// </summary>
        /// <param name=""></param> 
        /// <returns></returns>
        public ActionResult LayUserRoles()
        {
            var userRoles = db.AspNetUsers.Find(User.Identity.GetUserId()).AspNetRoles.Select(s => s.Name).ToList();
            return Json(userRoles, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: GET API - Lấy ra danh sách hệ đào tạo
        /// Developer: Nguyen Kha Minh
        /// Date:  
        /// </summary>
        /// <param name=""></param> 
        /// <returns>Danh sách hệ đào tạo từ table sc_HeNganh</returns>
        public List<sc_HeNganh> dsHeDaotao()
        {
            List<sc_HeNganh> system = new List<sc_HeNganh>();
            system = db.sc_HeNganh.ToList();
            List<sc_HeNganh> an_system = new List<sc_HeNganh>();
            foreach (var item in system)
            {
                if (item.MaQH == null)
                {
                    an_system.Add(item);
                }
            }
            return an_system;
        }

        /// <summary>
        /// Purpose: GET API - Lấy ra danh sách ngành đào tạo
        /// Developer: Nguyen Kha Minh
        /// Date:  
        /// </summary>
        /// <param name=""></param> 
        /// <returns>Danh sách ngành đào tạo từ table sc_HeNganh</returns>
        [HttpGet]
        public JsonResult GetListBranch(int id_system)
        {
            var jsonBranch = ListBranch(id_system);
            return Json(jsonBranch, JsonRequestBehavior.AllowGet);
        }

        public Object ListBranch(int id_system)
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = db.sc_HeNganh.Where(s => s.MaQH == id_system).ToList();
            List<object> an_branch = new List<object>();
            foreach (var item in listSystem)
            {
                object meo = new { _id = item.MaHN, name = item.Mota, maqh = item.MaQH };
                an_branch.Add(meo);
            }
            return an_branch;
        }

        /// <summary>
        /// Purpose: GET API - Lấy ra danh sách năm học
        /// Developer: Nguyen Kha Minh
        /// Date:  
        /// </summary>
        /// <param name="id_branch">Mã ngành đào tạo do người dùng chọn</param> 
        /// <returns>Danh sách các năm học của hệ ngành đã chọn từ table tp_KHDaotao</returns>
        [HttpGet]
        public JsonResult GetListYear(int? id_branch)
        {
            var jsonYear = ListYearBranch(id_branch);
            return Json(jsonYear, JsonRequestBehavior.AllowGet);
        }
        public Object ListYearBranch(int? id_branch)
        {
            List<t_CTDaotao> ctDaotao = new List<t_CTDaotao>();
            ctDaotao = db.t_CTDaotao.Where(s => s.MaHN == id_branch && s.LoaiCT == "IC").ToList();

            List<object> listYear = new List<object>();
            foreach (var sub in ctDaotao)
            {
                var meo = from item in db.tp_KHDaotao
                          where item.MaCTDT == sub.MaCT
                          group item by item.Namhoc into newGroup
                          orderby newGroup.Key
                          select newGroup;
                foreach (var item in meo)
                {
                    object subMeo = new { year = item.Key };
                    listYear.Add(subMeo);
                }
            }
            return listYear;
        }

        /// <summary>
        /// Purpose: GET tất cả các Năm học của các IC
        /// Developer: Nguyen Kha Minh
        /// Date:  
        /// </summary>
        /// <param name="mahn">Mã ngành</param> 
        /// <param name="loaict">Loại Chương trình default là IC</param> 
        /// <returns>Danh sách các năm học của IC theo JSON</returns>
        /**
         *  Group Year for typeMACT 
         *  params mahn and loaict
         *  loaict default = IC
         *  version 1.1 (New)
         */
        [HttpGet]

        public JsonResult GetListYearNew(int mahn, string loaict = "IC")
        {
            var json = listYearNew(mahn, loaict);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public Object listYearNew(int mahn, string loaict)
        {
            var shushi = (from c in db.t_CTDaotao
                          join k in db.tp_KHDaotao on c.MaCT equals k.MaCTDT
                          where c.MaHN == mahn && c.LoaiCT == loaict
                          group k by new
                          {
                              //c.TenCT,
                              //c.LoaiCT,
                              k.Namhoc
                          } into newGroup
                          select new
                          {
                              //TenCT = newGroup.Key.TenCT,
                              //LoaiCT = newGroup.Key.LoaiCT,
                              Namhoc = newGroup.Key.Namhoc
                          }).OrderByDescending(a => a.Namhoc);
            List<object> listC = new List<object>();
            foreach (var item in shushi)
            {
                object meo = new { /*tenct = item.TenCT, loaict = item.LoaiCT, */namhoc = item.Namhoc };
                listC.Add(meo);
            }
            return listC;
        }

        /// <summary>
        /// Purpose: Trả về danh sách cần thiết cho view xem KHDT
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="pr_branch">Mã ngành</param> 
        /// <param name="pr_year">Năm học</param> 
        /// <param name="pr_semester">Học kỳ</param> 
        /// <param name="pr_loaict">Loại chương trình default là IC</param> 
        /// <returns name="listC">danh sách các môn học IC theo học kỳ và năm học</returns>
        /// <returns name="allSub">danh sách các môn học để thêm nhanh</returns>
        ///  <returns name="allBranch">danh sách các Khối lớp theo Ngành và loại IC</returns>
        /**
         *  Get infomation detail all branch for screen View (Subject)
         *  pr_loaict default =  IC
         */
        [HttpGet]
        public JsonResult GetListSub(int pr_branch, string pr_year, string pr_semester, string pr_loaict = "IC")
        {
            var json = listSubBranch(pr_branch, pr_loaict, pr_year, pr_semester);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public Object listSubBranch(int pr_branch, string pr_loaict, string pr_year, string pr_semester)
        {
            List<object> json = new List<object>();
            // if (pr_loaict == "IC")
            // {
            //var listSubject = from c in db.t_CTDaotao
            //                  join kl in db.sc_Khoilop on c.MaKhoi equals kl.MaKhoi
            //                  join k in db.tp_KHDaotao on c.MaCTDT equals k.MaCTDT
            //                  join hp in db.sc_Hocphan on k.MaHP equals hp.MaHP
            //                  where c.MaHN == pr_branch && c.LoaiCT == pr_loaict && k.Hocky == pr_semester && k.Namhoc == pr_year
            //                  group k by new
            //                  {
            //                      makh = kl.MaKhoi,
            //                      tenkh = kl.TenKhoi,
            //                      mahp = hp.MaHP,
            //                      tenmh = hp.TenMH,
            //                      mactdt = k.MaCTDT,
            //                      sotc = k.SoTC,
            //                      giolt = k.GioLT,
            //                      gioth = k.GioTH,
            //                      tinhtrang = k.Tinhtrang,
            //                      hocky = k.Hocky
            //                  } into gNew
            //                  select gNew.Key;

            //foreach (var item in listSubject)
            //{
            //    object meo = new
            //    {
            //        mactdt = item.mactdt,
            //        makh = item.makh,
            //        tenkh = item.tenkh,
            //        mahp = item.mahp,
            //        tenmh = item.tenmh,
            //        hocky = item.hocky,
            //        tinhtrang = item.tinhtrang,
            //        sotc = item.sotc,
            //        giolt = item.giolt,
            //        gioth = item.gioth
            //    };
            //    json.Add(meo);
            //}
            //}
            //else if (pr_loaict == "PC")
            //{
            var listSubject = from c in db.t_CTDaotao
                              join kl in db.sc_Khoilop on c.MaKhoi equals kl.MaKhoi
                              join k in db.tp_KHDaotao on c.MaCT equals k.MaCTDT
                              // join hp in db.sc_Hocphan on k.MaHP equals hp.MaHP
                              where c.MaHN == pr_branch && c.LoaiCT == pr_loaict && k.Hocky == pr_semester && k.Namhoc == pr_year
                              group k by new
                              {
                                  makh = kl.MaKhoi,
                                  tenkh = kl.TenKhoi,
                                  makhdt = k.MaKHDT,
                                  // mahp = hp.MaHP,
                                  mahp = 0,
                                  tenmh = k.TenHP,
                                  matc = c.MaTC,
                                  mactdt = k.MaCTDT,
                                  sotc = k.SoTC,
                                  giolt = k.GioLT,
                                  gioth = k.GioTH,
                                  gioda = k.GioDa,
                                  giott = k.GioTT,
                                  hocky = k.Hocky,
                                  trangthaiKH = k.TrangthaiKH
                              } into gNew
                              select gNew.Key;
            List<object> listC = new List<object>();
            foreach (var item in listSubject)
            {
                object meo = new
                {
                    mactdt = item.mactdt,
                    makh = item.makh,
                    makhdt = item.makhdt,
                    matc = item.matc,
                    tenkh = item.tenkh,
                    trangthaiKH = item.trangthaiKH,
                    mahp = item.mahp,
                    tenmh = item.tenmh,
                    hocky = item.hocky,
                    sotc = item.sotc,
                    giolt = item.giolt,
                    gioth = item.gioth,
                    gioda = item.gioda,
                    giott = item.giott
                };
                listC.Add(meo);
            }
            var allSubject = from c in db.t_CTDaotao
                             join kl in db.sc_Khoilop on c.MaKhoi equals kl.MaKhoi
                             join k in db.tc_KHDaotao on c.MaCT equals k.MaCTDT
                             // join hp in db.sc_Hocphan on k.MaHP equals hp.MaHP
                             where c.MaHN == pr_branch && c.LoaiCT == "PC"
                             group k by new
                             {
                                 makh = kl.MaKhoi,
                                 tenkh = kl.TenKhoi,
                                 makhdt = k.MaKHDT,
                                 // mahp = hp.MaHP,
                                 mahp = k.MaHP,
                                 tenmh = k.TenHP,
                                 mactdt = c.MaCT,
                                 sotc = k.SoTC,
                                 giolt = k.GioLT,
                                 gioth = k.GioTH,
                                 gioda = k.GioDA,
                                 giott = k.GioTT,
                                 hocky = k.Hocky
                             } into subNew
                             select subNew.Key;
            List<object> allSub = new List<object>();
            foreach (var sub in allSubject)
            {
                int matcdtIC;
                var ctic = db.t_CTDaotao.FirstOrDefault(s => s.MaTC == sub.mactdt);
                if (ctic == null)
                {
                    break;
                }
                else
                {
                    matcdtIC = ctic.MaCT;
                }
                tp_KHDaotao item = new tp_KHDaotao();
                item = db.tp_KHDaotao.FirstOrDefault(s => s.MaCTDT == matcdtIC && s.TenHP == sub.tenmh && s.Hocky == pr_semester && s.Namhoc == pr_year);
                if (item == null)
                {
                    object meo = new
                    {
                        mactdt = sub.mactdt,
                        makhdt = sub.makhdt,
                        makh = sub.makh,
                        tenkh = sub.tenkh,
                        matcIC = db.t_CTDaotao.FirstOrDefault(s => s.MaTC == sub.mactdt).MaCT,
                        mahp = sub.mahp,
                        tenmh = sub.tenmh,
                        hocky = sub.hocky,
                        sotc = sub.sotc,
                        giolt = sub.giolt,
                        gioth = sub.gioth,
                        gioda = sub.gioda,
                        giott = sub.giott
                    };
                    allSub.Add(meo);
                }

            }
            List<object> allBranch = new List<object>();
            var lBranch = from item in db.t_CTDaotao
                          join k in db.sc_Khoilop on item.MaKhoi equals k.MaKhoi
                          where item.MaHN == pr_branch && item.LoaiCT == pr_loaict
                          group k by new
                          {
                              mactdt = item.MaCT,
                              mahn = item.MaHN,
                              makh = k.MaKhoi,
                              tenkh = k.TenKhoi
                          } into gNew
                          select gNew.Key;
            foreach (var item in lBranch)
            {
                object meo = new
                {
                    mactdt = item.mactdt,
                    mahn = item.mahn,
                    matc = db.t_CTDaotao.FirstOrDefault(s => s.MaCT == item.mactdt).MaTC,
                    makh = item.makh,
                    tenkh = item.tenkh
                };
                allBranch.Add(meo);
            }
            json.Add(listC);
            json.Add(allSub);
            json.Add(allBranch);
            // }
            return json;
        }

        /// <summary>
        /// Purpose: Lưu môn thêm nhanh và cập nhật số giờ của KHDT
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="arrSubject">Danh sách các KHDT</param> 
        /// <returns>json thông báo thành công</returns>
        ///**
        // *  API Save Subject in PC (CTDT Ke Hoach)
        // *  MAKH + MAHN ==> MACTDT after connect to DB KHDAOTAO Update Subject
        // *  params 
        // */
        [HttpPost]
        public JsonResult SaveSubBranch(List<Subject> arrSubject)
        {
            if (arrSubject.Count != 0)
            {
                foreach (var item in arrSubject)
                {
                    //tp_KHDaotao subject = new tp_KHDaotao();
                    //subject = db.tp_KHDaotao.SingleOrDefault(s => s.MaCTDT == item.Mactdt && s.MaHP == item.Mamh);
                    //subject.Hocky = item.Hocky;
                    //subject.Namhoc = item.Year;
                    //db.SaveChanges();
                    //tp_KHDaotao subject = new tp_KHDaotao();
                    //subject = db.tp_KHDaotao.SingleOrDefault(s => s.MaCTDT == item.Mactdt && s.MaHP == item.Mamh && s.Namhoc == item.Year);
                    //if (subject == null)
                    //{

                    if (item.Status == "new")
                    {
                        tp_KHDaotao subSub = new tp_KHDaotao();
                        subSub.TrangthaiKH = Variables.TrangthaiKH_KhoiTao;
                        subSub.Hocky = item.Hocky;
                        subSub.Namhoc = item.Year;
                        subSub.MaCTDT = item.Mactdt;


                        subSub.SoTC = item.Sotc;
                        subSub.GioLT = item.Giolt;
                        subSub.GioTH = item.Gioth;
                        subSub.GioDa = item.Gioda;
                        subSub.GioTT = item.Giott;
                        subSub.MaKhoiKT = 1;

                        var timKHDT = db.tp_KHDaotao.FirstOrDefault(s => s.MaKHDT == item.MaKhdt);
                        subSub.TenHP = timKHDT.TenHP;
                        subSub.MaHP = timKHDT.MaHP;

                        //Tim ke hoach dao tao get ngay bat dau ngay ket thuc
                        var timKHDTNBDNKT = db.tp_KHDaotao.Where(x => x.t_CTDaotao.MaCT == item.Mactdt).Where(x => x.Hocky == item.Hocky).Where(x => x.Namhoc == item.Year).FirstOrDefault();
                        if (timKHDTNBDNKT != null)
                        {
                            subSub.NgayBD = timKHDTNBDNKT.NgayBD;
                            subSub.NgayKT = timKHDTNBDNKT.NgayKT;
                        }

                        db.tp_KHDaotao.Add(subSub);
                        db.SaveChanges();
                    }
                    else
                    {
                        tp_KHDaotao subSub = db.tp_KHDaotao.SingleOrDefault(s => s.MaKHDT == item.MaKhdt);
                        subSub.TrangthaiKH = Variables.TrangthaiKH_KhoiTao;
                        subSub.Hocky = item.Hocky;
                        subSub.Namhoc = item.Year;
                        subSub.MaCTDT = item.Mactdt;

                        subSub.SoTC = item.Sotc;
                        subSub.GioLT = item.Giolt;
                        subSub.GioTH = item.Gioth;
                        subSub.GioDa = item.Gioda;
                        subSub.GioTT = item.Giott;
                        subSub.MaKhoiKT = 1;
                        // db.tp_KHDaotao.Add(subSub);
                        db.SaveChanges();
                    }


                    //}
                }
                object json = new { status = 200, message = "Lưu thành công" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                object json = new { status = 404, message = "Lưu không thành công" };
                return Json(json, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Purpose: Lấy tất cả cách học kỳ theo năm học
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name=""></param> 
        /// <returns>json các học kỳ</returns>
        ///**
        // * Groupby semester follow year
        // * ToDo
        // */
        [HttpGet]
        public JsonResult GetGrSemester(int mahn, string year)
        {
            var listSe = grSemesYear(mahn, year);
            return Json(listSe, JsonRequestBehavior.AllowGet);
        }
        public Object grSemesYear(int mahn, string year)
        {
            var lSemester = from item in db.t_CTDaotao
                            join k in db.tp_KHDaotao on item.MaCT equals k.MaCTDT
                            where item.MaHN == mahn && k.Namhoc == year
                            group k by new
                            {
                                hocky = k.Hocky
                            } into gNew
                            select gNew.Key;
            List<object> lSe = new List<object>();
            foreach (var item in lSemester)
            {
                object meo = new { hocky = item.hocky };
                lSe.Add(meo);
            }
            return lSe;
        }

        /// <summary>
        /// Purpose: Lấy danh sách tất cả giảng viên
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name=""></param> 
        /// <returns>json các giáo viên</returns>
        /**
         *  PhanCongGiangVien
         *  Danh sach Tat ca giang vien
         */

        public string LayTatCaGiangVien()
        {
            var role = db.AspNetRoles.Where(x => x.Name == UserRoles.roleGiaoVien).FirstOrDefault();
            var giaoViens = role.AspNetUsers.Select(x => new { id = x.Id, name = x.UserName }).ToList();

            return JsonConvert.SerializeObject(giaoViens, Formatting.Indented);

        }

        /// <summary>
        /// Purpose: Lấy tất cả danh sách giáo viên là giáo viên cơ hữu
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name=""></param> 
        /// <returns>json các giáo viên cơ hữu</returns>
        public string LayTatCaGiangVienCoHuu()
        {
            var role = db.AspNetRoles.Where(x => x.Name == UserRoles.roleGiaoVienCoHuu).FirstOrDefault();
            var giaoViens = role.AspNetUsers.Select(x => new { id = x.Id, name = x.UserName }).ToList();

            return JsonConvert.SerializeObject(giaoViens, Formatting.Indented);

        }

        /// <summary>
        /// Purpose: Lấy tất cả giáo viên là giáo viên thỉnh giảng
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="">json các giáo viên thỉnh giảng</param> 
        /// <returns></returns>
        public string LayTatCaGiangVienThinhGiang()
        {
            var role = db.AspNetRoles.Where(x => x.Name == UserRoles.roleGiaoVienThinhGiang).FirstOrDefault();
            var giaoViens = role.AspNetUsers.Select(x => new { id = x.Id, name = x.UserName }).ToList();

            return JsonConvert.SerializeObject(giaoViens, Formatting.Indented);

        }

        /**
         *  PhanCongGiangVien
         *  Danh sach danh sach giao vien theo MonTQ va MonHT
         *  Goi y giao vien
         */
        /// <summary>
        /// Purpose: Lấy danh sách các giáo viên gợi ý dựa trên mã KHDT
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã khdt</param> 
        /// <returns>json các giáo viên được gợi ý</returns>
        public string LayGiaoVienGoiY(int maKHDT)
        {
            //int maKHDT = int.Parse(Request.QueryString["maKHDT"]);

            var giangviens = db.GetGiangVienGoiY(maKHDT);
            var result = giangviens.Select(x => new { id = x.Id, name = x.UserName }).ToList();
            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }


        /**
         *  PhanCongGiangVien
         *  Danh sach KHDT
         */
        /// <summary>
        /// Purpose: Lấy danh sách KHDT để phân công
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maNganh">Mã Ngành</param> 
        /// <param name="namHoc">Mã Năm học</param> 
        /// <param name="hocKy">Mã Học kỳ</param> 
        /// <returns>json Danh sách các khdt để phân công theo Ngành năm học và học kỳ</returns>

        public string LayDanhSachKHDTPhanCong(int maNganh, string namHoc, string hocKy)
        {
            var khdts = db.tp_KHDaotao.Join(db.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d)// Join KHDT vs CTDT
                   => new { c.TenHP, c.MaKHDT, d.LoaiCT, c.TrangthaiKH, d.MaHN, c.Namhoc, c.Hocky, d.MaKhoi, c.MaHP, c.GioLT, c.GioTH, c.GioDa, c.GioTT, c.SoTC, c.SolopDuKien })
                   .Where(x => x.MaHN == maNganh) // Where các biến khi join 2 bảng KHDT vào CTDT
                   .Where(x => x.Hocky == hocKy)
                   .Where(x => x.Namhoc == namHoc)
                   .Where(x => x.LoaiCT == "IC")
                    //Chỉ trạng thái lưu tạm được set giáo viên
                    .Join(db.sc_Khoilop, c => c.MaKhoi, d => d.MaKhoi, (c, d)// Join KHDT vs CTDT vs KhoiLop Để lấy tên khối
                    => new { TenMH = c.TenHP, c.MaKHDT, d.TenKhoi, c.MaHP, c.GioLT, c.GioTH, c.GioDa, c.GioTT, c.SoTC, c.TrangthaiKH, c.SolopDuKien }).ToList();
            //.Join(db.sc_Hocphan, c => c.MaHP, d => d.MaHP, (c, d)// Join KHDT vs CTDT vs KhoiLop vs HocPhan lấy tên môn học
            //=> new { MaKHDT = c.MaKHDT, TenKhoi = c.TenKhoi, TenMH = d.TenMH, GioLT = c.GioLT, GioTH = c.GioTH, GioDa = c.GioDa, GioTT = c.GioTT, SoTC = c.SoTC }).ToList();

            return JsonConvert.SerializeObject(khdts, Formatting.Indented);
        }

        /**
         *  PhanCongGiangVien
         *  Lấy danh sách giáo viên đã được phân công
         */
        /// <summary>
        /// Purpose: Lấy dánh sách giáo viên đã được phân công
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maNganh">Mã Ngành</param> 
        /// <param name="namHoc">Mã Năm học</param> 
        /// <param name="hocKy">Mã Học kỳ</param> 
        /// <returns>json Danh sách các KHDT và các giảng viên được phân công cho KHDT đó và trạng thái của giảng viên đó</returns>


        public string LayDanhSachGiaoVienDuocPhanCong(int maNganh, string namHoc, string hocKy)
        {

            var deCuongGV = db.tp_DecuongGV.Join(db.tp_KHDaotao, c => c.MaKHDT, d => d.MaKHDT, (c, d)
                => new { d.MaCTDT, c.MaKHDT, d.Namhoc, d.Hocky, d.NguoiCN, d.SolopDuKien }).Join(db.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d)
                           => new { d.MaHN, c.MaKHDT, c.Namhoc, c.Hocky, c.NguoiCN, c.SolopDuKien })
                .Where(x => x.MaHN == maNganh)
                .Where(x => x.Namhoc == namHoc)
                .Where(x => x.Hocky == hocKy).GroupBy(x => new { x.MaKHDT, x.NguoiCN, x.SolopDuKien }).Select(x => new { MaKHDT = x.Key.MaKHDT, NguoiCN = x.Key.NguoiCN, SolopDuKien = x.Key.SolopDuKien }).ToList();

            if (deCuongGV.Count != 0)
            {

                List<KHDTVaGiangVienVaTrangThai> danhSachKHDTvaListGV = new List<KHDTVaGiangVienVaTrangThai>();
                foreach (var el in deCuongGV)
                {
                    var deCuongGVTheoKHDT = db.tp_DecuongGV.Where(x => x.MaKHDT == el.MaKHDT).ToList();
                    if (deCuongGVTheoKHDT.Count != 0)
                    {
                        KHDTVaGiangVienVaTrangThai col = new KHDTVaGiangVienVaTrangThai();
                        col.khdt = el.MaKHDT.ToString();
                        if (el.SolopDuKien != null)
                        {
                            col.solopdukien = (int)el.SolopDuKien;
                        }
                        col.gvduocpc = el.NguoiCN;
                        foreach (var dc in deCuongGVTheoKHDT)
                        {
                            GiangVienVaTrangThai gv = new GiangVienVaTrangThai();
                            gv.magv = dc.NguoiCN.ToString();
                            gv.trangthai = dc.Trangthai;
                            gv.lydo = dc.Ghichu;
                            col.listgv.Add(gv);
                        }

                        danhSachKHDTvaListGV.Add(col);
                    }
                }
                return JsonConvert.SerializeObject(danhSachKHDTvaListGV, Formatting.Indented);
            }

            return "[]";
        }

        /// <summary>
        /// Purpose: Lấy ngày hết hạn đề cương
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKhdt">Mã KHDT</param> 
        /// <returns>string date của ngày hết hạn theo KHDT</returns>

        public string LayNgayHetHanDeCuong(string maKhdt)
        {
            int maKH = int.Parse(maKhdt);

            var khdt = db.tp_KHDaotao.Find(maKH);
            //string nguoiPhuTrach = khdt.NguoiCN;
            //string ngayHT = khdt.tp_DecuongGV.Where(x => x.NguoiCN == nguoiPhuTrach).FirstOrDefault().NgayHT.ToString();
            DateTime ngayHT = (DateTime)khdt.NgayHT;

            return String.Format("{0:yyyy-MM-dd}", ngayHT);
        }

        /**
        *  PhanCongGiangVien
        *  Lưu (cập nhật) danh sách giáo viên phân công 
        *  
        */
        /// <summary>
        /// Purpose: Lưu danh sách các giáo viên được phân công cho KHDT
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="listAddGiaoVien">Danh sách giáo viên được phân công theo KHDT</param> 
        /// <returns>string done để check xem hàm đã hoàn thành hay chưa</returns>

        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string LuuDanhSachGiaoVienPhanCong(string listAddGiaoVien)
        {
            List<KHDTVaGiangVienVaTrangThai> listAddGiaoViene = JsonConvert.DeserializeObject<List<KHDTVaGiangVienVaTrangThai>>(listAddGiaoVien);
            string idEmailMeo = User.Identity.GetUserId();
            List<tp_DecuongGV> listDecuongAdd = new List<tp_DecuongGV>();

            foreach (var ele in listAddGiaoViene)
            {
                int maKHDT = int.Parse(ele.khdt);
                bool isHaveStatusOne = false;

                var khdtTim = db.tp_KHDaotao.Find(maKHDT);
                khdtTim.SolopDuKien = ele.solopdukien;
                db.SaveChanges();

                if (ele.listgv.Count > 0)
                {
                    //Lưu người phụ trách đề cương
                    var khdt = db.tp_KHDaotao.Find(maKHDT);
                    khdt.NguoiCN = ele.listgv[0].magv;
                    db.SaveChanges();

                    foreach (var gv in ele.listgv)
                    {
                        var deCuongGV = db.tp_DecuongGV.Where(x => x.MaKHDT == maKHDT).Where(x => x.NguoiCN == gv.magv).FirstOrDefault();
                        //Nếu chưa có trong Db thì tạo
                        if (deCuongGV == null)
                        {
                            tp_DecuongGV deCuong = new tp_DecuongGV();
                            deCuong.MaKHDT = maKHDT;
                            deCuong.Trangthai = gv.trangthai;
                            deCuong.NguoiPC = idEmailMeo;
                            deCuong.NguoiCN = gv.magv;
                            deCuong.NgayPC = DateTime.Now;
                            if (gv.trangthai == "1")
                            {
                                isHaveStatusOne = true;
                            }

                            listDecuongAdd.Add(deCuong);
                        }
                        //Nếu có rồi thì clone ra để lấy lại các thông tin có sẵn
                        else
                        {
                            tp_DecuongGV deCuong = new tp_DecuongGV();
                            deCuong.MaKHDT = maKHDT;
                            deCuong.Trangthai = gv.trangthai;
                            deCuong.NguoiPC = idEmailMeo;
                            deCuong.NguoiCN = gv.magv;
                            deCuong.Ghichu = deCuongGV.Ghichu;
                            deCuong.NgayPC = DateTime.Now;
                            if (gv.trangthai == "1")
                            {
                                isHaveStatusOne = true;
                            }

                            listDecuongAdd.Add(deCuong);

                        }
                    }


                }

                //Nếu có trạng thái 1 đồng ý đề cương chuyển trạng thái đề cương
                if (isHaveStatusOne)
                {
                    db.tp_KHDaotao.Find(maKHDT).TrangthaiDC = "3";
                }
                else
                {
                    db.tp_KHDaotao.Find(maKHDT).TrangthaiDC = null;
                }

                //xoá đi tất cả các DeCuongGV có mã KHDT hiện có trong db
                var listDeCuongGvDelete = db.tp_DecuongGV.Where(x => x.MaKHDT == maKHDT).ToList();
                if (listDeCuongGvDelete.Count > 0)
                {
                    db.tp_DecuongGV.RemoveRange(listDeCuongGvDelete);
                    db.SaveChanges();
                }





                ////add lại tất cả các DeCuongGV có mã KHDT có trong danh sách (Update lại)
                //foreach (var gv in ele.listgv)
                //{
                //    tp_DecuongGV deCuongGV = new tp_DecuongGV();
                //    deCuongGV.MaKHDT = maKHDT;
                //    deCuongGV.Trangthai = gv.trangthai;
                //    deCuongGV.NguoiPC = idEmailMeo;
                //    deCuongGV.NguoiCN = gv.magv;


                //    db.tp_DecuongGV.Add(deCuongGV);
                //    db.SaveChanges();
                //}
            }

            //Add listDecuongAdd vào db
            db.tp_DecuongGV.AddRange((IEnumerable<tp_DecuongGV>)listDecuongAdd);
            db.SaveChanges();

            return "done";
        }


        /**
       *  NoiDungEmailPhanCongGV
       *  Gửi mail cho các giáo viên
       *  
       */
        /// <summary>
        /// Purpose: Gửi email cho các giáo viên về môn học giảng dạy của mình đồng thời cũng tạo các Notification cho account các giảng viên được gửi mail
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="listGiaoVienChuaGuiMail">Danh sách giáo viên Chuẩn bị gửi email</param> 
        /// <param name="TenNganh">Tên ngành</param> 
        /// <param name="NamHoc">Năm học</param> 
        /// <param name="HocKy">Học kỳ</param> 
        /// <returns>string sussess để check xem hàm đã hoàn thành hay chưa</returns>
        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string GuiMailChoCacGiaoVien(string listGiaoVienChuaGuiMail, string TenNganh, string NamHoc, string HocKy)
        {
            List<KHDTVaGiangVienVaTrangThai> listAddGiaoVien = JsonConvert.DeserializeObject<List<KHDTVaGiangVienVaTrangThai>>(listGiaoVienChuaGuiMail);
            string idEmailMeo = User.Identity.GetUserId();
            var emailGiaoVu = db.AspNetUsers.FirstOrDefault(s => s.Id == idEmailMeo).Email;

            List<GiangVienKHDT> listGiangVienKHDT = new List<GiangVienKHDT>();

            //chuyển danh sách KHDTVaGiangVienVaTrangThai thành danh sách GiangVienKHDT
            foreach (var ele in listAddGiaoVien)
            {
                foreach (var gv in ele.listgv)

                {
                    if (listGiangVienKHDT.Count > 0)
                    {

                        for (var i = 0; i < listGiangVienKHDT.Count; i++)
                        {
                            if (listGiangVienKHDT[i].magv == gv.magv)
                            {
                                listGiangVienKHDT[i].listKHDT.Add(ele.khdt);
                                break;
                            }
                            //chạy đến cuối mảng listGiangVienKHDT mà vẫn không có thì tạo
                            if (i == listGiangVienKHDT.Count - 1)
                            {
                                GiangVienKHDT GiangVienKHDT = new GiangVienKHDT();
                                GiangVienKHDT.magv = gv.magv;
                                GiangVienKHDT.listKHDT = new List<string>();
                                GiangVienKHDT.listKHDT.Add(ele.khdt);

                                listGiangVienKHDT.Add(GiangVienKHDT);
                                break;
                            }
                        }

                    }
                    //chưa có phần tử nào trong mảng listGiangVienKHDT thì tạo phần từ đầu tiên
                    else
                    {
                        GiangVienKHDT GiangVienKHDT = new GiangVienKHDT();
                        GiangVienKHDT.magv = gv.magv;
                        GiangVienKHDT.listKHDT = new List<string>();
                        GiangVienKHDT.listKHDT.Add(ele.khdt);

                        listGiangVienKHDT.Add(GiangVienKHDT);
                    }

                }
            }


            //Gọi tới hàm gửi Email
            foreach (var ele in listGiangVienKHDT)
            {
                //Tạo notification
                var giaoVienTim = db.AspNetUsers.Find(ele.magv);
                //Chứa các mã KHDT để lưu vào mã QH trong Notification
                var maKHDTs = "";
                foreach (var khdt in ele.listKHDT)
                {
                    maKHDTs += khdt + ",";
                }

                string message = "Thư mời Giảng dạy môn học Học kỳ " + HocKy + ", Năm học " + NamHoc;
                sf_Notification notifiSend = new sf_Notification();
                notifiSend.DaXem = false;
                notifiSend.Trangthai = false;
                notifiSend.Thongtin = message;
                notifiSend.NguoiGui = emailGiaoVu;
                notifiSend.NguoiNhan = giaoVienTim.Email;
                notifiSend.Nguon = "Email";
                notifiSend.Kieu = "Lời mời giảng dạy";
                notifiSend.MaQH = maKHDTs;
                notifiSend.Ngaytao = DateTime.Now;
                db.sf_Notification.Add(notifiSend);
                db.SaveChanges();

                //Gửi mail cho giảng viên
                GuiMailXacNhanGiaoVienPhuTrachMonHoc(ele, TenNganh, HocKy, NamHoc, notifiSend.MaTB);

            }

            string sMessage = "Success";

            return sMessage;
        }


        /**
       *  Function gửi mail
       *  
       *  
       */

        /// <summary>
        /// Purpose: Gửi email cho giảng viên
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="giangVienKHDT">Danh sách các KHDT theo giảng viên</param> 
        /// <param name="tenNganh">Tên ngành</param> 
        /// <param name="namHoc">Năm học</param> 
        /// <param name="soHK">Học kỳ</param> 
        /// <param name="maNotifi">Mã thông báo</param> 
        /// <returns>string done để check xem hàm đã hoàn thành hay chưa</returns>

        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public string GuiMailXacNhanGiaoVienPhuTrachMonHoc(GiangVienKHDT giangVienKHDT, string tenNganh, string soHK, string namHoc, int maNotifi)
        {
            string mailBodyhtml = "";
            string mailSubject = "";
            string u = Request.Url.AbsoluteUri.ToString();
            string hostUrl = u.Substring(0, u.LastIndexOf("/"));

            string acceptlink = hostUrl + "/Mail/ChiTietThongBao/" + maNotifi;
            string khungMail = "";

            var mailEle = db.sf_EmailTemplate.Where(x => x.Phanloai == "Mail Phân Công").FirstOrDefault();
            var giaovien = db.AspNetUsers.Find(giangVienKHDT.magv);
            string tenGV = giaovien.UserName;
            string Email = giaovien.Email;
            string maGV = giaovien.Id;
            khungMail = mailEle.Khungnoidung;
            khungMail = khungMail.Replace(@"{tenGV}", tenGV);

            foreach (var ele in giangVienKHDT.listKHDT)
            {
                string noidung = mailEle.Noidung;
                int maKHDT = int.Parse(ele);
                //var khdt = context.tp_KHDaotao.Join(context.sc_Hocphan, c => c.MaHP, d => d.MaHP,
                //            (c, d) => new { c.MaKHDT, d.TenMH }).Where(x => x.MaKHDT == maKHDT).FirstOrDefault();
                var khdt = db.tp_KHDaotao.Where(x => x.MaKHDT == maKHDT).Select(c => new { c.MaKHDT, c.TenHP }).FirstOrDefault();

                string tenMH = khdt.TenHP;

                //Replace nội dung
                mailBodyhtml += noidung.Replace(@"{tenMH}", tenMH).Replace(@"{tenNganh}", tenNganh).Replace(@"{soHK}", soHK).Replace(@"{namHoc}", namHoc).Replace(@"{acceptlink}", acceptlink);


                //Đổi trạng thái DecuongGV trong db sang đã gửi mail
                var DecuongGV = db.tp_DecuongGV.Where(x => x.MaKHDT == maKHDT).Where(x => x.NguoiCN == giaovien.Id).FirstOrDefault();
                if (DecuongGV != null)
                {
                    DecuongGV.Trangthai = "3";
                    db.SaveChanges();
                }
            }

            khungMail = khungMail.Replace(@"{noiDung}", mailBodyhtml);
            mailSubject = mailEle.Chude;

            var msg = new MailMessage("vanlangsender@gmail.com", Email, mailSubject, khungMail);

            msg.From = new MailAddress("vanlangsender@gmail.com");
            msg.To.Add(Email);
            msg.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com", 587); //if your from email address is "from@hotmail.com" then host should be "smtp.hotmail.com"
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("vanlangsender@gmail.com", "Ex2XzGuF_Z5Nc37");
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);

            return "done";
        }

        //protected void btnSubmit_Click(string tenMH, string tenGV, string tenNganh, string soHK, string namHoc, string acceptlink, string Email)
        //{
        //    //Fetching Settings from WEB.CONFIG file.  
        //    string emailSender = ConfigurationManager.AppSettings["emailsender"].ToString();
        //    string emailSenderPassword = ConfigurationManager.AppSettings["password"].ToString();
        //    string emailSenderHost = ConfigurationManager.AppSettings["smtpserver"].ToString();
        //    int emailSenderPort = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
        //    Boolean emailIsSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);


        //    //Fetching Email Body Text from EmailTemplate File.  
        //    string FilePath = "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    //Repalce [newusername] = signup user name   
        //    MailText = MailText.Replace("[newusername]", txtUserName.Text.Trim());


        //    string subject = "Welcome to CSharpCorner.Com";

        //    //Base class for sending email  
        //    MailMessage _mailmsg = new MailMessage();

        //    //Make TRUE because our body text is html  
        //    _mailmsg.IsBodyHtml = true;

        //    //Set From Email ID  
        //    _mailmsg.From = new MailAddress(emailSender);

        //    //Set To Email ID  
        //    _mailmsg.To.Add(txtUserName.Text.ToString());

        //    //Set Subject  
        //    _mailmsg.Subject = subject;

        //    //Set Body Text of Email   
        //    _mailmsg.Body = MailText;


        //    //Now set your SMTP   
        //    SmtpClient _smtp = new SmtpClient();

        //    //Set HOST server SMTP detail  
        //    _smtp.Host = emailSenderHost;

        //    //Set PORT number of SMTP  
        //    _smtp.Port = emailSenderPort;

        //    //Set SSL --> True / False  
        //    _smtp.EnableSsl = emailIsSSL;

        //    //Set Sender UserEmailID, Password  
        //    NetworkCredential _network = new NetworkCredential(emailSender, emailSenderPassword);
        //    _smtp.Credentials = _network;

        //    //Send Method will send your MailMessage create above.  
        //    _smtp.Send(_mailmsg);



        //}

        //protected string GuiMailXacNhanGiaoVienPhuTrachMonHoc(string tenMH, string tenGV, string tenNganh, string soHK, string namHoc, string acceptlink, string Email)
        //{
        //    //Fetching Settings from WEB.CONFIG file.  
        //    //string emailSender = ConfigurationManager.AppSettings["emailsender"].ToString();
        //    //string emailSenderPassword = ConfigurationManager.AppSettings["password"].ToString();
        //    //string emailSenderHost = ConfigurationManager.AppSettings["smtpserver"].ToString();
        //    //int emailSenderPort = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
        //    //Boolean emailIsSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);


        //    //Fetching Email Body Text from EmailTemplate File.  
        //    string FilePath = "C:\\Study\\program\\ProgramManagement\\EmailTemplate\\SignUp.html";
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();

        //    //Repalce [newusername] = signup user name   
        //    //MailText = MailText.Replace("[newusername]", txtUserName.Text.Trim());


        //    string subject = "Phân công giảng viên";

        //    ////Base class for sending email  
        //    //MailMessage _mailmsg = new MailMessage();

        //    ////Make TRUE because our body text is html  
        //    //_mailmsg.IsBodyHtml = true;

        //    ////Set From Email ID  
        //    //_mailmsg.From = new MailAddress(emailSender);

        //    ////Set To Email ID  
        //    //_mailmsg.To.Add(txtUserName.Text.ToString());

        //    ////Set Subject  
        //    //_mailmsg.Subject = subject;

        //    ////Set Body Text of Email   
        //    //_mailmsg.Body = MailText;


        //    ////Now set your SMTP   
        //    //SmtpClient _smtp = new SmtpClient();

        //    ////Set HOST server SMTP detail  
        //    //_smtp.Host = emailSenderHost;

        //    ////Set PORT number of SMTP  
        //    //_smtp.Port = emailSenderPort;

        //    ////Set SSL --> True / False  
        //    //_smtp.EnableSsl = emailIsSSL;

        //    ////Set Sender UserEmailID, Password  
        //    //NetworkCredential _network = new NetworkCredential(emailSender, emailSenderPassword);
        //    //_smtp.Credentials = _network;

        //    ////Send Method will send your MailMessage create above.  
        //    //_smtp.Send(_mailmsg);

        //    MailMessage msg = new MailMessage("vanlangsender@gmail.com", Email, subject, MailText);

        //    msg.From = new MailAddress("vanlangsender@gmail.com");
        //    msg.To.Add(Email);
        //    msg.IsBodyHtml = true;
        //    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); //if your from email address is "from@hotmail.com" then host should be "smtp.hotmail.com"
        //    smtpClient.UseDefaultCredentials = false;
        //    smtpClient.Credentials = new NetworkCredential("vanlangsender@gmail.com", "Ex2XzGuF_Z5Nc37");
        //    smtpClient.EnableSsl = true;
        //    smtpClient.Send(msg);

        //    return "done";
        //}

        /**
         * * ==============================================================================================================================
         * * View: ThongKeThoiGianGiangVien
         * * Controller: GiangVien
         */

        /**
         *  Data for Chart
         **/

        //public List<DtChartThongKe> lDGvChart(List<string> listGv, string year, int id_branch, string hk)
        //{
        //    var listItem = from dcGv in db.tp_DecuongGV
        //                   join item in db.AspNetUsers on dcGv.NguoiPC equals item.Id
        //                   join khdt in db.tp_KHDaotao on dcGv.MaKHDT equals khdt.MaKHDT
        //                   join ctDt in db.t_CTDaotao on khdt.MaCTDT equals ctDt.MaCT
        //                   where ctDt.LoaiCT == "IC" && khdt.Namhoc == year && khdt.Hocky == hk && ctDt.MaHN == id_branch && khdt.Tinhtrang == "Lưu tạm"
        //                   group item by new
        //                   {
        //                       item.Id,
        //                       item.UserName,
        //                       item.Email,
        //                       khdt.MaKHDT,
        //                       khdt.MaCTDT,
        //                       khdt.GioLT,
        //                       khdt.GioTH,
        //                       khdt.GioTT,
        //                       khdt.GioDa,
        //                       khdt.SoTC
        //                   }
        //                   into g
        //                   select g.Key;
        //    List<DtChartThongKe> listGvChart = new List<DtChartThongKe>();
        //    foreach (var item in listItem)
        //    {
        //        // TODO constraint DK 
        //        //var match = listGv.Where(s => s.Contains(item.id));
        //        //if (match!=null)
        //        //{
        //        DtChartThongKe meo = new DtChartThongKe();
        //        meo.Id = item.Id;
        //        meo.Username = item.UserName;
        //        meo.Email = item.Email;
        //        meo.Giolt = item.GioLT;
        //        meo.Giott = item.GioTT;
        //        meo.Gioda = item.GioDa;
        //        meo.Gioth = item.GioTH;
        //        meo.Sotc = item.SoTC;
        //        meo.Mactdt = item.MaCTDT;
        //        meo.Makhdt = item.MaKHDT;

        //        listGvChart.Add(meo);
        //        //}
        //    }
        //    return listGvChart;
        //}

        /*
         * API Modified Notification Is Seen
         */
        /// <summary>
        /// Purpose: Thanh đổi trạng thái đã xem của thông báo
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="MANO">Mã thông báo</param> 
        /// <returns>json để check xem hàm đã hoàn thành hay chưa</returns>
        [HttpGet]
        public JsonResult modifiedNotifi(int MANO)
        {
            sf_Notification entiNo = db.sf_Notification.FirstOrDefault(s => s.MaTB == MANO);
            if (entiNo == null)
            {
                return Json(new { check = false, message = VI.mes_KhongTimThay }, JsonRequestBehavior.AllowGet);
            }
            entiNo.DaXem = true;
            try
            {
                db.sf_Notification.Attach(entiNo);
                db.Entry(entiNo).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { check = true, message = VI.mes_LuuThanhCong }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { check = false, message = VI.mes_KhongLuuDuoc }, JsonRequestBehavior.AllowGet);
            }

        }
        //==================================================== HopThuDenGiangVien =======================================================//
        //[HttpGet]
        //public JsonResult GetLSTeacher(string hocky, string namhoc, string userId)
        //{
        //    var listSubject = from dcGv in db.tp_DecuongGV
        //                      join meoUser in db.AspNetUsers on dcGv.NguoiCN equals meoUser.Id
        //                      join khdt in db.tp_KHDaotao on dcGv.MaKHDT equals khdt.MaKHDT
        //                      join ctdt in db.t_CTDaotao on khdt.MaCTDT equals ctdt.MaCTDT
        //                      join hngah in db.sc_HeNganh on ctdt.MaHN equals hngah.MaHN
        //                      join hp in db.sc_Hocphan on khdt.MaHP equals hp.MaHP
        //                      where khdt.Hocky == hocky && khdt.Namhoc == namhoc && dcGv.NguoiCN == userId
        //                      group dcGv by new
        //                      {
        //                          meoUser.Id,
        //                          meoUser.Email,
        //                          meoUser.UserName,
        //                          hp.MaHP,
        //                          hp.TenMH,
        //                          hngah.TenHeNganh,
        //                          hngah.MaHN,
        //                          dcGv.NguoiPC
        //                      } into gKey
        //                      select new
        //                      {
        //                          userId = gKey.Key.Id,
        //                          Email = gKey.Key.Email,
        //                          UserName = gKey.Key.UserName,
        //                          MaHP = gKey.Key.MaHP,
        //                          TenMH = gKey.Key.TenMH,
        //                          MaHN = gKey.Key.MaHN,
        //                          TenHeNganh = gKey.Key.TenHeNganh,
        //                          idNguoiPC = gKey.Key.NguoiPC,
        //                          NguoiPC = db.AspNetUsers.FirstOrDefault(s => s.Id == gKey.Key.NguoiPC).UserName
        //                      };
        //    List<object> meo = new List<object>();
        //    string id = "";
        //    object sub = new object();
        //    foreach(var item in listSubject)
        //    {
        //        id = item.userId;
        //        if (id.Length <= 0)
        //        {
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //        }
        //        if(id == item.userId)
        //        {
        //            /// viet tiep ham xu li tra ve json gom nhom vao mot mang chung
        //        }
        //    }

        //    return Json(listSubject, JsonRequestBehavior.AllowGet);
        //}


        //================================================DeCuongGiaoVien======================================//


        /// <summary>
        /// Purpose: Lấy danh sách tất cả các môn học cho view môn học giảng dạy
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="idBranch">mã Ngành</param> 
        /// <param name="idYear">Năm học</param> 
        /// <param name="idSemester">Học kỳ</param> 
        /// <param name="idUser">Id user</param> 
        /// <returns>json danh sách môn học có kèm trạng thái</returns>

        [HttpGet]
        public JsonResult DC_MonHocGiangDayGV(int idBranch, string idYear, string idSemester, string idUser)
        {
            var listYourSubTeach = from ctdt in db.t_CTDaotao
                                   join khdt in db.tp_KHDaotao on ctdt.MaCT equals khdt.MaCTDT
                                   // join hp in db.sc_Hocphan on khdt.MaHP equals hp.MaHP
                                   join dcgv in db.tp_DecuongGV on khdt.MaKHDT equals dcgv.MaKHDT
                                   where ctdt.MaHN == idBranch && ctdt.LoaiCT == "IC"
                                   && khdt.Namhoc == idYear
                                   && khdt.Hocky == idSemester
                                   && dcgv.NguoiCN == idUser
                                   group dcgv by new
                                   {
                                       ctdt.TenCT,
                                       khdt.MaKHDT,
                                       khdt.TenHP,
                                       khdt.NgayHT,
                                       khdt.TrangthaiDC,
                                       khdt.TrangthaiKH
                                   } into gKey
                                   select new
                                   {
                                       khoa = gKey.Key.TenCT,
                                       makhdt = gKey.Key.MaKHDT,
                                       tenhp = gKey.Key.TenHP,
                                       ngayhh = gKey.Key.NgayHT.ToString(),
                                       trangthai = gKey.Key.TrangthaiDC,
                                       trangthaiKH = gKey.Key.TrangthaiKH
                                   };
            List<object> danhSachDeCuong = new List<object>();
            foreach (var item in listYourSubTeach)
            {
                string timeHH = "";
                if (item.ngayhh == "")
                {
                    timeHH = "30/04/1975";
                }
                else
                {
                    timeHH = Convert.ToDateTime(item.ngayhh).ToString("dd/MM/yyyy");
                }
                string[] khoi = item.khoa.Split('-');
                object meo = new { TenKhoi = khoi[1], MaKhdt = item.makhdt, TenMH = item.tenhp, NgayHh = timeHH, TrangThai = item.trangthai, TrangthaiKH = item.trangthaiKH };
                danhSachDeCuong.Add(meo);
            }
            return Json(danhSachDeCuong, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Purpose: Lấy danh sách tất cả các môn học cho view đề cương
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="idBranch">mã Ngành</param> 
        /// <param name="idYear">Năm học</param> 
        /// <param name="idSemester">Học kỳ</param> 
        /// <param name="idUser">Id user</param> 
        /// <returns>json danh sách môn học có kèm trạng thái</returns>
        /// 
        public JsonResult DC_DeCuongMonHocGV(int idBranch, string idYear, string idSemester)
        {
            if (User.IsInRole(UserRoles.roleTruongBoMon))
            {
                var listYourSubTeach = from ctdt in db.t_CTDaotao
                                       join khdt in db.tp_KHDaotao on ctdt.MaCT equals khdt.MaCTDT
                                       where ctdt.MaHN == idBranch && ctdt.LoaiCT == "IC"
                                       && khdt.Namhoc == idYear
                                       && khdt.Hocky == idSemester
                                       && khdt.TrangthaiDC != null
                                       group khdt by new
                                       {
                                           ctdt.TenCT,
                                           khdt.MaKHDT,
                                           khdt.TenHP,
                                           khdt.NgayHT,
                                           khdt.TrangthaiDC,
                                           khdt.TrangthaiKH
                                       } into gKey
                                       select new
                                       {
                                           khoa = gKey.Key.TenCT,
                                           makhdt = gKey.Key.MaKHDT,
                                           tenhp = gKey.Key.TenHP,
                                           ngayhh = gKey.Key.NgayHT.ToString(),
                                           trangthai = gKey.Key.TrangthaiDC,
                                           trangthaiKH = gKey.Key.TrangthaiKH
                                       };
                List<object> danhSachDeCuong = new List<object>();
                foreach (var item in listYourSubTeach)
                {
                    string timeHH = "";
                    if (item.ngayhh == "")
                    {
                        timeHH = "30/04/1975";
                    }
                    else
                    {
                        timeHH = Convert.ToDateTime(item.ngayhh).ToString("dd/MM/yyyy");
                    }

                    string[] khoi = item.khoa.Split('-');
                    object meo = new { TenKhoi = khoi[1], MaKhdt = item.makhdt, TenMH = item.tenhp, NgayHh = timeHH, TrangThai = item.trangthai, trangthaiKH = item.trangthaiKH };
                    danhSachDeCuong.Add(meo);
                }
                return Json(danhSachDeCuong, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Not Allowed", JsonRequestBehavior.AllowGet);
            }

        }


        /**
      *  ChinhSuaDeCuongHocPhan
      *  Lấy các chuẩn đầu ra CTDT
      */

        /// <summary>
        /// Purpose: Lấy danh sách các CDR-CTDT
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="mactdt">mã CTDT</param> 
        /// <returns>json danh sách CDR-CTDT theo CTDT</returns>
        /// 

        public JsonResult GetCDRCTDT(int mactdt)
        {
            int mactdtpc = (int)db.t_CTDaotao.Find(mactdt).MaTC;
            var lstCDRCTDT = db.t_CDR_CTDT.Where(x => x.MaCTDT == mactdt || x.MaCTDT == mactdtpc).Select(x => new { x.MaELO, x.MaHT, x.Mota }).ToList();
            return Json(lstCDRCTDT, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Purpose: Lấy danh sách các CDR-HP
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">mã KHDT</param> 
        /// <returns>json danh sách CDR-HP theo KHDT</returns>
        /// 

        public JsonResult GetCDRHP(int maKHDT)
        {
            var cdrhps = db.tp_CDR_HP.Where(x => x.MaKHDT == maKHDT).ToList();
            List<ModelChinhSuaDeCuongHocPhanPage1> result = new List<ModelChinhSuaDeCuongHocPhanPage1>();
            foreach (var cdrhp in cdrhps)
            {
                ModelChinhSuaDeCuongHocPhanPage1 modelcdrhp = new ModelChinhSuaDeCuongHocPhanPage1();
                modelcdrhp.moTa = cdrhp.Mota;
                modelcdrhp.phanLoai = cdrhp.Phanloai;
                modelcdrhp.tenKQHT = cdrhp.MaHT;
                modelcdrhp.macelo = cdrhp.MaCELO;
                //Tạo list chưa CDR-CTDT cho model
                List<ModelcdrCTDTChinhSuaDeCuongHocPhanPage1> listCDRCTDT = new List<ModelcdrCTDTChinhSuaDeCuongHocPhanPage1>();

                var cdrCTDTs = db.tp_MatranCDR.Where(x => x.MaCELO == cdrhp.MaCELO).ToList();
                foreach (var cdrCTDT in cdrCTDTs)
                {
                    ModelcdrCTDTChinhSuaDeCuongHocPhanPage1 CDRCTDT = new ModelcdrCTDTChinhSuaDeCuongHocPhanPage1();
                    CDRCTDT.id = cdrCTDT.MaELO;
                    CDRCTDT.text = cdrCTDT.t_CDR_CTDT.MaHT;

                    listCDRCTDT.Add(CDRCTDT);
                }
                modelcdrhp.cdrCTDT = listCDRCTDT;
                result.Add(modelcdrhp);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Purpose: Lưu page 1 chỉnh sửa đề cương
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="listRow">Danh sách cách CDR HP</param> 
        /// <param name="MucTieuHP">Mục tiêu học phần</param> 
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>string check hàm đã hoàn thành chưa</returns>
        /// 
        public string LuuDanhSachCDRHP(string listRow, string MucTieuHP, int maKHDT)
        {
            try
            {
                List<ModelChinhSuaDeCuongHocPhanPage1> listR = JsonConvert.DeserializeObject<List<ModelChinhSuaDeCuongHocPhanPage1>>(listRow);

                //Lưu mục tiêu HP vào KHDT
                var khdt = db.tp_KHDaotao.Find(maKHDT);
                khdt.MuctieuHP = MucTieuHP;
                khdt.NgayCN = DateTime.Now;
                db.SaveChanges();

                var listRemovecdrHP = db.tp_CDR_HP.Where(x => x.MaKHDT == maKHDT).ToList();
                foreach (var cdrHp in listR)
                {
                    var cdrTim = db.tp_CDR_HP.Find(cdrHp.macelo);
                    //if exist just Edit
                    if (cdrTim != null)
                    {
                        cdrTim.Mota = cdrHp.moTa;
                        cdrTim.Phanloai = cdrHp.phanLoai;
                        cdrTim.MaHT = cdrHp.tenKQHT;
                        db.SaveChanges();
                        //Remove from list Remove
                        listRemovecdrHP.Remove(cdrTim);
                        //Remove all old MatranCDR
                        var listMatranCDR = db.tp_MatranCDR.Where(x => x.MaCELO == cdrTim.MaCELO);
                        db.tp_MatranCDR.RemoveRange(listMatranCDR);
                        db.SaveChanges();

                        foreach (var cdrCTDT in cdrHp.cdrCTDT)
                        {
                            tp_MatranCDR mtcdrmoi = new tp_MatranCDR();
                            mtcdrmoi.MaCELO = cdrTim.MaCELO;
                            mtcdrmoi.MaELO = cdrCTDT.id;
                            db.tp_MatranCDR.Add(mtcdrmoi);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //if not exist create new
                        tp_CDR_HP cdrMoi = new tp_CDR_HP();
                        cdrMoi.MaKHDT = maKHDT;
                        cdrMoi.Mota = cdrHp.moTa;
                        cdrMoi.Phanloai = cdrHp.phanLoai;
                        cdrMoi.MaHT = cdrHp.tenKQHT;

                        db.tp_CDR_HP.Add(cdrMoi);
                        db.SaveChanges();
                        foreach (var cdrctdt in cdrHp.cdrCTDT)
                        {
                            tp_MatranCDR matrancdrMoi = new tp_MatranCDR();
                            matrancdrMoi.MaCELO = cdrMoi.MaCELO;
                            matrancdrMoi.MaELO = cdrctdt.id;

                            db.tp_MatranCDR.Add(matrancdrMoi);
                            db.SaveChanges();
                        }
                    }
                }

                if (listRemovecdrHP.Count > 0)
                {
                    db.tp_CDR_HP.RemoveRange(listRemovecdrHP);
                    db.SaveChanges();
                }

                if (khdt.TrangthaiDC == Variables.TrangthaiDC_ChuaCapNhat)
                {
                    khdt.TrangthaiDC = Variables.TrangthaiDC_LuuNhap;
                    db.SaveChanges();
                }

                return "done";

            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// Purpose: Lấy danh sách CDR-CTDT
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="mactdt">Mã CTDT</param> 
        /// <returns>json danh sách CDR-CTDT</returns>
        /// 
        // Get CMU from MKHDT
        public JsonResult getCDRwKHDT(int mactdt)
        {
            // int idCTDT = db.tp_KHDaotao.FirstOrDefault(s => s.MaKHDT == makhdt).MaCTDT;
            int mactdtpc = (int)db.t_CTDaotao.Find(mactdt).MaTC;
            var ctrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == mactdt || s.MaCTDT == mactdtpc).ToList();
            if (ctrctdt.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<object> meoList = new List<object>();
                foreach (var item in ctrctdt)
                {
                    var meo = new
                    {
                        MaELO = item.MaELO,
                        Mota = item.Mota,
                        PhanLoai = item.Phanloai,
                        MaHT = item.MaHT
                    };
                    meoList.Add(meo);
                }
                return Json(meoList, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Purpose: Lưu page 5 chỉnh sửa đề cương HP
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="dsTaiLieuHP">Danh sách tài liệu</param> 
        /// <param name="thangDiem">Thanh điểm</param> 
        /// <param name="pPGD">Phương pháp giáo dục</param> 
        /// <param name="pPHT">Phương pháp học tập</param> 
        /// <param name="nVSV">Nhiệm vụ sinh viên</param> 
        /// <param name="nNGD">Ngôn ngữ giảng dạy</param> 
        /// <param name="moTa">Mô tả</param> 
        /// <param name="lPH">Loại phòng học</param> 
        /// <param name="pTPVGD">Phương tiện phục vụ giảng dạy</param> 
        /// <param name="pTT">Phương thức thi</param> 
        /// <param name="nHDCBS">năm học đề cương biên soạn</param> 
        /// <param name="cSDCLT">Số lần cập nhật đề cương</param> 
        /// <param name="nDCS">nội dung cập nhật</param> 
        /// <returns>string check hàm đã chạy xong chưa</returns>
        /// 
        //View DeCuongHocPhanDanhGia
        public string LuuDeCuongHocPhanDanhGia(List<tp_TailieuHP> dsTaiLieuHP, string thangDiem, string pPGD, string pPHT, string nVSV, string nNGD, string moTa, List<int> lPH, string pTPVGD, string pTT, string nHDCBS, int cSDCLT, string nDCS)
        {
            try
            {
                //Tìm mã KHDT ở phần tử đầu danh sách Tài liệu học phần
                int maKHDT = (int)dsTaiLieuHP[0].MaKHDT;
                //Xoá đi các Tài liệu học phần cũ
                var taiLieuHPcu = db.tp_TailieuHP.Where(x => x.MaKHDT == maKHDT).ToList();
                db.tp_TailieuHP.RemoveRange(taiLieuHPcu);
                db.SaveChanges();


                //Add list Tài liệu học phần
                db.tp_TailieuHP.AddRange(dsTaiLieuHP);
                db.SaveChanges();

                //Add các thông tin của KHDT
                var khdtTim = db.tp_KHDaotao.Find(maKHDT);
                khdtTim.NgonnguGD = nNGD;
                khdtTim.Mota = moTa;
                khdtTim.PhuongtienGD = pTPVGD;
                khdtTim.Phuongtienthi = pTT;
                khdtTim.Ngaytao = Convert.ToDateTime(nHDCBS);
                khdtTim.SolancapnhatDC = cSDCLT;
                khdtTim.NoidungCN = nDCS;
                khdtTim.Thangdiem = thangDiem;
                khdtTim.PPGD = pPGD;
                khdtTim.PPHT = pPHT;
                khdtTim.NhiemvuSV = nVSV;
                db.SaveChanges();

                //Xoá tất cả các LoaiPH cũ 
                if (khdtTim.sc_LoaiPH.Count > 0)
                {
                    List<sc_LoaiPH> lsLPHcu = khdtTim.sc_LoaiPH.ToList();
                    foreach (sc_LoaiPH lphcu in lsLPHcu)
                    {
                        khdtTim.sc_LoaiPH.Remove(lphcu);
                        db.SaveChanges();
                    }
                }

                //Add các LoaiPH mới
                foreach (int maLoaiPH in lPH)
                {
                    khdtTim.sc_LoaiPH.Add(db.sc_LoaiPH.Find(maLoaiPH));
                    db.SaveChanges();
                }

                db.tp_KHDaotao.Find(maKHDT).NgayCN = DateTime.Now;
                db.SaveChanges();
                return "done";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// Purpose: Lấy danh sách các tài liệu học phần
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>json danh sách các tài liệu học phần theo KHDT</returns>
        /// 
        public JsonResult LayDanhSachTaiLieuHP(int maKHDT)
        {
            var listTaiLieuHP = db.tp_TailieuHP.Where(x => x.MaKHDT == maKHDT).Select(x => new { x.MaTL, x.MaKHDT, x.NamXB, x.Kieunhap, x.LoaiTL, x.NhaXB, x.TenTL, x.Tacgia }).ToList();
            return Json(listTaiLieuHP, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: Lấy các CDRHP
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>json danh sách các CDR-HP</returns>
        /// 
        //ChinhSuaDeCuongHocPhan Page2
        public JsonResult LayTatCaCDRHP(int makhdt)
        {
            var listCDRHP = db.tp_CDR_HP.Where(x => x.MaKHDT == makhdt).Select(x => new { x.MaCELO, x.MaHT }).ToList();
            return Json(listCDRHP, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Purpose: Lưu page 2 cập nhật đề cương học phần
        /// Developer: Nguyễn Đình Nguyên
        /// Date:  
        /// </summary>
        /// <param name="listDanhgiaHP">Danh sách các đánh giá học phần</param> 
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <param name="listRowCCDG">Danh sách các công cụ đánh giá của CDR-HP</param> 
        /// <param name="listNoiDung">Danh sách các nội dung học phần</param> 
        /// <returns>string check hàm đã hoàn thành chưa</returns>
        /// 
        public string LuuDanhgiaHP(List<ModelDanhgiaHPStep2> listDanhgiaHP, int makhdt, List<ModelPage2CongCuDG> listRowCCDG, List<ModelNoiDungHPStep2> listNoiDung)
        {
            try
            {
                //Get list remove
                var listRemoveDanhGiaHP = db.tp_DanhgiaHP.Where(x => x.MaKHDT == makhdt).ToList();

                //var maKHDT = db.tp_KHDaotao.Where(x => x.MaKHDT == makhdt).FirstOrDefault();

                foreach (var item in listDanhgiaHP)
                {
                    var danhgiaHPTim = db.tp_DanhgiaHP.Find(item.madg);
                    //If exist just edit
                    if (danhgiaHPTim != null)
                    {
                        danhgiaHPTim.MaKHDT = makhdt;
                        danhgiaHPTim.Thanhphan = item.thanhphan;
                        danhgiaHPTim.Noidung = item.mota;
                        danhgiaHPTim.Trongso = item.trongso;
                        db.SaveChanges();

                        listRemoveDanhGiaHP.Remove(danhgiaHPTim);

                        //Remove all MatranDG
                        var matranDG = danhgiaHPTim.tp_CDR_HP.ToList();
                        foreach (var mt in matranDG)
                        {
                            danhgiaHPTim.tp_CDR_HP.Remove(mt);
                            db.SaveChanges();
                        }

                        //Add MatranDG
                        foreach (var kqhtmongdoi in item.kqhtmongdoi)
                        {
                            var cdrHP = db.tp_CDR_HP.Find(kqhtmongdoi.id);
                            danhgiaHPTim.tp_CDR_HP.Add(cdrHP);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        tp_DanhgiaHP danhgiaHP = new tp_DanhgiaHP();
                        danhgiaHP.MaKHDT = makhdt;
                        danhgiaHP.Thanhphan = item.thanhphan;
                        danhgiaHP.Noidung = item.mota;
                        danhgiaHP.Trongso = item.trongso;

                        db.tp_DanhgiaHP.Add(danhgiaHP);
                        db.SaveChanges();

                        foreach (var kqhtmongdoi in item.kqhtmongdoi)
                        {
                            var cdrhp = db.tp_CDR_HP.Find(kqhtmongdoi.id);
                            danhgiaHP.tp_CDR_HP.Add(cdrhp);
                            db.SaveChanges();
                        }
                    }
                }

                if (listRemoveDanhGiaHP.Count > 0)
                {
                    foreach (var removeDanhgiaHP in listRemoveDanhGiaHP)
                    {
                        var removeMatran = removeDanhgiaHP.tp_CDR_HP.ToList();
                        foreach (var mt in removeMatran)
                        {
                            removeDanhgiaHP.tp_CDR_HP.Remove(mt);
                            db.SaveChanges();
                        }

                    }
                    db.tp_DanhgiaHP.RemoveRange(listRemoveDanhGiaHP);
                    db.SaveChanges();
                }

                var listCDRcu = db.tp_CDR_HP.Where(x => x.MaKHDT == makhdt).ToList();

                foreach (var cdr in listCDRcu)
                {
                    cdr.ThoiDiemDanhGia = null;
                    cdr.CongCuDanhGia = null;
                    db.SaveChanges();
                }

                if (listRowCCDG.Count > 0)
                {
                    foreach (var item in listRowCCDG)
                    {
                        tp_CDR_HP meo = db.tp_CDR_HP.Find(item.maCELO);
                        meo.CongCuDanhGia = item.congCuDanhGia;
                        meo.ThoiDiemDanhGia = item.thoiDiemDanhGia;
                        db.SaveChanges();
                    }
                }

                /// noi dung
                var listRemoveNoiDung = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt).ToList();

                try
                {
                    var c = listNoiDung.Count;
                    foreach (var item in listNoiDung)
                    {
                        var cont = db.tp_NoidungHP.Find(item.id);
                        //Create new content if it wasn't exist
                        if (cont == null)
                        {
                            tp_NoidungHP newContent = new tp_NoidungHP();
                            newContent.MaKHDT = makhdt;
                            newContent.Noidung = item.noiDung;
                            newContent.Phanloai = item.phanLoai;
                            newContent.TenHT = item.noiDungNgan;
                            db.tp_NoidungHP.Add(newContent);
                            db.SaveChanges();

                            var noiDungID = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt && s.TenHT == item.noiDungNgan).Select(s => s.MaND).FirstOrDefault();

                            foreach (var cdrItem in item.kqht)
                            {
                                var cdrId = db.tp_CDR_HP.Find(cdrItem.id);
                                db.tp_NoidungHP.Find(noiDungID).tp_CDR_HP.Add(cdrId);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //Edit the content if it was exist
                            cont.Noidung = item.noiDung;
                            cont.Phanloai = item.phanLoai;
                            cont.TenHT = item.noiDungNgan;

                            var listCDR = db.tp_NoidungHP.Find(cont.MaND).tp_CDR_HP.Select(s => s.MaCELO).ToList();
                            foreach (var itemCDR in listCDR)
                            {
                                tp_CDR_HP cdr = db.tp_NoidungHP.Find(item.id).tp_CDR_HP.Where(s => s.MaCELO == itemCDR).FirstOrDefault();
                                db.tp_NoidungHP.Find(cont.MaND).tp_CDR_HP.Remove(cdr);
                                db.SaveChanges();
                            }

                            var noiDungID = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt && s.TenHT == item.noiDungNgan).Select(s => s.MaND).FirstOrDefault();

                            foreach (var cdrItem in item.kqht)
                            {
                                var cdrId = db.tp_CDR_HP.Find(cdrItem.id);
                                db.tp_NoidungHP.Find(noiDungID).tp_CDR_HP.Add(cdrId);
                                db.SaveChanges();
                            }
                            listRemoveNoiDung.Remove(cont);
                        }

                    }
                    if (listRemoveNoiDung.Count > 0)
                    {
                        db.tp_NoidungHP.RemoveRange(listRemoveNoiDung);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    var idND = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt).Select(s => s.MaND).FirstOrDefault();

                    var listNDCDR = db.tp_NoidungHP.Find(idND).tp_CDR_HP.Select(s => s.MaCELO).ToList();
                    foreach (var item in listNDCDR)
                    {
                        tp_CDR_HP cdr = db.tp_CDR_HP.Find(item);
                        db.tp_NoidungHP.Find(idND).tp_CDR_HP.Remove(cdr);
                        db.SaveChanges();
                    }

                    List<tp_NoidungHP> nd = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt).ToList();
                    db.tp_NoidungHP.RemoveRange(nd);
                    db.SaveChanges();
                }

                db.tp_KHDaotao.Find(makhdt).NgayCN = DateTime.Now;
                db.SaveChanges();

                return "done";

            }
            catch (Exception e)
            {
                return "";
            }
        }


        /// <summary>
        /// Purpose: Lưu page 2 cập nhật đề cương học phần
        /// Developer: Nguyễn Đình Nguyên
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Danh sách các mã nôi dung và tên Hiển thị theo mã KHDT</returns>
        /// 
        public JsonResult LayIDNoiDung(int makhdt)
        {
            var a = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt).Select(s => new { id = s.MaND, tenHT = s.TenHT }).ToList();
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: Lấy danh sách Đánh giá học phần
        /// Developer: Nguyễn Đình Nguyên
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Danh sách các Đánh giá HP theo mã KHDT</returns>
        /// 
        public JsonResult LayDanhgiaHP(int makhdt)
        {
            try
            {
                var listDanhGiaHP = db.tp_DanhgiaHP.Where(x => x.MaKHDT == makhdt);

                List<ModelDanhgiaHPStep2> listResult = new List<ModelDanhgiaHPStep2>();
                //Add all in listDanhGiaHP sent
                foreach (var item in listDanhGiaHP)
                {
                    ModelDanhgiaHPStep2 ModeldanhgiaHP = new ModelDanhgiaHPStep2();

                    ModeldanhgiaHP.madg = item.MaDG;
                    ModeldanhgiaHP.thanhphan = item.Thanhphan;
                    ModeldanhgiaHP.mota = item.Noidung;
                    ModeldanhgiaHP.trongso = (int)item.Trongso;
                    if (item.Thanhphan == null)
                        ModeldanhgiaHP.trangthai = false;
                    else
                        ModeldanhgiaHP.trangthai = true;

                    List<ModelkqhtmongdoiStep2> listModelKQHT = new List<ModelkqhtmongdoiStep2>();
                    foreach (var cdrHP in item.tp_CDR_HP)
                    {
                        ModelkqhtmongdoiStep2 ModelKQHT = new ModelkqhtmongdoiStep2();
                        ModelKQHT.id = cdrHP.MaCELO;
                        ModelKQHT.text = cdrHP.MaHT;
                        listModelKQHT.Add(ModelKQHT);
                    }

                    ModeldanhgiaHP.kqhtmongdoi = listModelKQHT;

                    listResult.Add(ModeldanhgiaHP);
                }
                return Json(listResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// Purpose: Lấy danh sách nội dung học phần 
        /// Developer: Nguyễn Đình Nguyên
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Danh sách các nội dung học phần theo mã KHDT</returns>
        /// 
        public JsonResult LayNoiDungHP(int makhdt)
        {
            try
            {
                var listNoiDungHP = db.tp_NoidungHP.Where(s => s.MaKHDT == makhdt);

                List<ModelNoiDungHPStep2> listRe = new List<ModelNoiDungHPStep2>();

                foreach (var item in listNoiDungHP)
                {
                    ModelNoiDungHPStep2 cont = new ModelNoiDungHPStep2();
                    cont.id = item.MaND;
                    cont.noiDungNgan = item.TenHT;
                    cont.phanLoai = item.Phanloai;
                    cont.noiDung = item.Noidung;

                    var listCDRHP = db.tp_NoidungHP.Find(item.MaND).tp_CDR_HP.Select(s => s.MaCELO).ToList();

                    List<ModelkqhtmongdoiStep2> listCDR = new List<ModelkqhtmongdoiStep2>();
                    foreach (var cdrHP in item.tp_CDR_HP)
                    {
                        ModelkqhtmongdoiStep2 cdr = new ModelkqhtmongdoiStep2();
                        cdr.id = cdrHP.MaCELO;
                        cdr.text = cdrHP.MaHT;
                        listCDR.Add(cdr);
                    }

                    cont.kqht = listCDR;
                    listRe.Add(cont);
                }
                return Json(listRe, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Purpose: Lấy danh sách loại phòng học
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <returns>Danh sách các loại phòng học</returns>
        /// 
        public JsonResult LayTatCaLoaiPH()
        {
            var lstLoaiPH = db.sc_LoaiPH.Select(x => new { id = x.MaLoaiPH, text = x.TenLoaiPH }).ToList();
            return Json(lstLoaiPH, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Purpose: Lấy danh sách các phòng học của theo môn
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Danh sách loại phòng học theo KHDT</returns>
        /// 
        public JsonResult LayLoaiPHTheoKHDT(int makhdt)
        {
            var lstLoaiPHTheoKHDT = db.tp_KHDaotao.Find(makhdt).sc_LoaiPH.Select(x => x.MaLoaiPH).ToList();
            return Json(lstLoaiPHTheoKHDT, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Purpose: Lấy công cụ đánh giá của CDR-HP
        /// Developer: Nguyễn Đình Nguyên
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>json Danh sách công cụ đánh giá của CDR-HP theo KHDT</returns>
        /// 
        public JsonResult LayCDRCongCuDanhGia(int makhdt)
        {
            try
            {
                List<ModelPage2CongCuDGReturn> res = new List<ModelPage2CongCuDGReturn>();
                var lsCdrHPDG = db.tp_CDR_HP.Where(x => x.MaKHDT == makhdt).Where(x => x.CongCuDanhGia != null).Where(x => x.ThoiDiemDanhGia != null).ToList();
                foreach (var cdr in lsCdrHPDG)
                {
                    ModelPage2CongCuDGReturn mre = new ModelPage2CongCuDGReturn();
                    mre.ccDG = cdr.CongCuDanhGia;
                    mre.tdDG = cdr.ThoiDiemDanhGia;
                    mre.madg = 0;
                    ModelPage2CongCuDGReturnInner imre = new ModelPage2CongCuDGReturnInner();
                    imre.id = cdr.MaCELO.ToString();
                    imre.text = cdr.MaHT;
                    mre.cdr = imre;

                    res.Add(mre);
                }

                return Json(new { status = 200, message = VI.mes_TaiThanhCong, listRowCCDG = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = VI.mes_KhongTaiDuoc }, JsonRequestBehavior.AllowGet);
            }
        }

        /********************************* Rubric Tieu Chi Danh Gia ****************************************/
        /***
         * Material Active 
         * MaKHDT       Params
         * return       List Material
         */
        /// <summary>
        /// Purpose: Lấy tiêu chí đánh giá Page 3
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Lấy tiêu chí đánh giá và thành phần đánh giá theo KHDT</returns>
        /// 
        [HttpGet]
        public JsonResult MaterialActive(int MaKHDT)
        {
            //var List = db.tp_DanhgiaHP.Where(s => s.MaKHDT == MaKHDT).Select(w => new { w.MaDG, w.MaKHDT, w.Thanhphan, w.Noidung, w.Trongso, w.Ghichu }).ToList();

            var listActive = from dghp in db.tp_DanhgiaHP
                             join tcdg in db.tp_TieuchiDG on dghp.MaDG equals tcdg.MaDG into gj
                             from subpet in gj.DefaultIfEmpty()
                             where dghp.MaKHDT == MaKHDT
                             select new
                             {
                                 MaDG = dghp.MaDG,
                                 MaKHDT = dghp.MaKHDT,
                                 Thanhphan = dghp.Thanhphan,
                                 Noidung = dghp.Noidung,
                                 Trongso = dghp.Trongso,
                                 Ghichu = dghp.Ghichu,
                                 Tieuchi = subpet.MaTC != null ? new
                                 {
                                     subpet.MaTC,
                                     subpet.MaDG,
                                     subpet.MaQH,
                                     subpet.Mota,
                                     subpet.Trongso,
                                     subpet.Mucdo
                                 } : null
                             };

            //var listActive = from dghp in db.tp_DanhgiaHP
            //                 where dghp.MaKHDT == 5213
            //                 select new
            //                 {
            //                     MaDG = dghp.MaDG,
            //                     MaKHDT = dghp.MaKHDT,
            //                     Thanhphan = dghp.Thanhphan,
            //                     Noidung = dghp.Noidung,
            //                     Trongso = dghp.Trongso,
            //                     Ghichu = dghp.Ghichu,
            //                 };

            return Json(listActive, JsonRequestBehavior.AllowGet);
        }

        /**
         * Save Data from client with Rubric
         * Save All Level2 (Contrainst Level3) first Because it's alway shoud
         * After Save Level1 and Contrainst referrence
         */
        /// <summary>
        /// Purpose: Lưu tiêu chí đánh giá Page 3
        /// Developer: Nguyễn Khả Minh
        /// Date:  
        /// </summary>
        /// <param name="ListLevel1">danh sách các tiêu chí</param> 
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>Lưu các tiêu chí đánh giá</returns>
        /// 
        [HttpPost]
        public JsonResult SaveRubric(List<LevelSaT> ListLevel1, int makhdt)
        {
            try
            {
                db.clearPurepose(makhdt);

                if (ListLevel1 != null)
                {
                    saveRecursion(ListLevel1, null);
                }
                db.tp_KHDaotao.Find(makhdt).NgayCN = DateTime.Now;
                db.SaveChanges();

                return Json(new { status = 200, message = VI.mes_LuuThanhCong }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = VI.mes_KhongLuuDuoc }, JsonRequestBehavior.AllowGet);
            }
        }

        public void saveRecursion(List<LevelSaT> ListLevel, int? idMeo = null)
        {
            foreach (var item in ListLevel)
            {
                tp_TieuchiDG recore = new tp_TieuchiDG();
                recore.MaDG = item.MaDG;
                recore.MaQH = idMeo;
                recore.Trongso = item.Trongso;
                recore.Mucdo = item.MucDo;
                recore.Mota = item.Mota;

                db.tp_TieuchiDG.Add(recore);
                db.SaveChanges();
                if (item.LevelNext != null)
                {
                    saveRecursion(item.LevelNext, recore.MaTC);
                }
            }
        }


        /********************************* TacvuHP Buổi và Tuần Page 4 ****************************************/
        /***
         * Material Active 
         * MaKHDT       string strModelChitiet, int maKHDT
         * return       List Material
         */
        /// <summary>
        /// Purpose: Lưu chitiết và tác vụ Page 4
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="strModelChitiet">string json chitiết học phần có chứa các tác vụ học phần</param> 
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>json check trạng thái hàm đã hoàn thành chưa</returns>
        /// 
        public JsonResult LuuTacVuHP(string strModelChitiet, int maKHDT)
        {
            try
            {
                List<ModelChitietHPPage4> lsModelChitiet = JsonConvert.DeserializeObject<List<ModelChitietHPPage4>>(strModelChitiet);
                //Xoa chitiet cu
                var lsChitietCu = db.tp_ChitietHP.Where(x => x.MaKHDT == maKHDT).ToList();
                db.tp_ChitietHP.RemoveRange(lsChitietCu);
                db.SaveChanges();

                //Add chitiet moi
                int countBuoi = 1;
                foreach (var iChitiet in lsModelChitiet)
                {
                    tp_ChitietHP chiTietMoi = new tp_ChitietHP();
                    chiTietMoi.MaKHDT = maKHDT;
                    chiTietMoi.Buoi = countBuoi.ToString();
                    chiTietMoi.Tuan = iChitiet.Tuan;
                    List<tp_TacvuHP> listHoatDong = new List<tp_TacvuHP>();


                    //Add hoat dong trong lop
                    int countSTTTL = 1;
                    foreach (var iHoatDongTrongLop in iChitiet.I)
                    {

                        tp_TacvuHP tacVuMoi = new tp_TacvuHP();
                        tacVuMoi.Phanloai = iHoatDongTrongLop.txtNth;
                        tacVuMoi.Sogio = iHoatDongTrongLop.txtHour;
                        tacVuMoi.Mota = iHoatDongTrongLop.txaDes;
                        tacVuMoi.Douutien = countSTTTL;
                        tacVuMoi.STT = countSTTTL;
                        tacVuMoi.HDTL = true;
                        tacVuMoi.MaND = int.Parse(iHoatDongTrongLop.txtNoidung);

                        //foreach (var iNoiDung in iHoatDongTrongLop.txtNoidung)
                        //{
                        //    int idNoidung = int.Parse(iNoiDung);
                        //    tacVuMoi.tp_NoidungHP.Add(db.tp_NoidungHP.Find(idNoidung));
                        //}


                        foreach (var iDanhGia in iHoatDongTrongLop.txtRating)
                        {
                            int idDanhGia = int.Parse(iDanhGia);
                            tacVuMoi.tp_DanhgiaHP.Add(db.tp_DanhgiaHP.Find(idDanhGia));
                        }

                        countSTTTL++;
                        listHoatDong.Add(tacVuMoi);
                    }

                    int countSTTNL = 1;
                    //Add hoat dong ngoai lop
                    foreach (var iHoatDongNgoai in iChitiet.O)
                    {
                        tp_TacvuHP tacVuMoi = new tp_TacvuHP();
                        tacVuMoi.Phanloai = iHoatDongNgoai.txtNth;
                        tacVuMoi.Sogio = iHoatDongNgoai.txtHour;
                        tacVuMoi.Mota = iHoatDongNgoai.txaDes;
                        tacVuMoi.Douutien = countSTTNL;
                        tacVuMoi.STT = countSTTNL;
                        tacVuMoi.HDTL = false;
                        tacVuMoi.MaND = int.Parse(iHoatDongNgoai.txtNoidung);

                        //foreach (var iNoiDung in iHoatDongNgoai.txtNoidung)
                        //{
                        //    int idNoidung = int.Parse(iNoiDung);
                        //    tacVuMoi.tp_NoidungHP.Add(db.tp_NoidungHP.Find(idNoidung));
                        //}

                        foreach (var iDanhGia in iHoatDongNgoai.txtRating)
                        {
                            int idDanhGia = int.Parse(iDanhGia);
                            tacVuMoi.tp_DanhgiaHP.Add(db.tp_DanhgiaHP.Find(idDanhGia));
                        }

                        countSTTNL++;
                        listHoatDong.Add(tacVuMoi);
                    }

                    chiTietMoi.tp_TacvuHP = listHoatDong;
                    db.tp_ChitietHP.Add(chiTietMoi);
                    db.SaveChanges();

                    countBuoi++;
                }

                db.tp_KHDaotao.Find(maKHDT);
                db.SaveChanges();

                return Json(new { status = 200, message = VI.mes_LuuThanhCong }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = VI.mes_KhongLuuDuoc }, JsonRequestBehavior.AllowGet);

            }

        }

        /// <summary>
        /// Purpose: Lấy chitiết và tác vụ Page 4
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns name="lsModelChiTietHP">json danh sách chi tiết và tác vụ</returns>
        /// <returns name="arrWeek">json chứa các array mô tả các tuần</returns>
        /// <returns name="countIDSolid">Id lớn hơn ID của Chitiết để xử lý generate ra id trong View</returns>
        /// <returns name="countWeek">Đếm số tuần hiện có</returns>
        /// 
        public JsonResult LayTacVuHP(int maKHDT)
        {
            try
            {
                List<ModelChitietHPPage4> lsModelChiTietHP = new List<ModelChitietHPPage4>();
                List<List<string>> arrWeek = new List<List<string>>();
                int countIDSolid = 0;
                int countWeek = 0;
                int countTacVu = 1;

                var lsChiTietHPHienTai = db.tp_ChitietHP.Where(x => x.MaKHDT == maKHDT).ToList();

                //Them vao lsModelChiTietHP
                foreach (var iChitietHT in lsChiTietHPHienTai)
                {
                    ModelChitietHPPage4 mChitiet = new ModelChitietHPPage4();
                    mChitiet.Tuan = (iChitietHT.Tuan != null && iChitietHT.Tuan != " ") ? iChitietHT.Tuan.Trim().ToString() : null;
                    var lsTacVuHienTai = iChitietHT.tp_TacvuHP.ToList();
                    foreach (var iTacvu in lsTacVuHienTai)
                    {
                        ModelTacvuPage4 mTacvu = new ModelTacvuPage4();
                        mTacvu.idKey = countTacVu;
                        mTacvu.txaDes = iTacvu.Mota;
                        mTacvu.txtHour = (int)iTacvu.Sogio;
                        mTacvu.txtNth = iTacvu.Phanloai;
                        //List<string> lsNoiDungHienTai = iTacvu.tp_NoidungHP.Select(x => x.MaND.ToString()).ToList();
                        List<string> lsDanhGiaHienTai = iTacvu.tp_DanhgiaHP.Select(x => x.MaDG.ToString()).ToList();
                        mTacvu.txtNoidung = iTacvu.MaND.ToString();
                        mTacvu.txtRating = lsDanhGiaHienTai;
                        if (iTacvu.HDTL == true)
                        {
                            mChitiet.I.Add(mTacvu);
                        }
                        else
                        {
                            mChitiet.O.Add(mTacvu);
                        }
                        countTacVu++;
                    }
                    lsModelChiTietHP.Add(mChitiet);
                    countIDSolid = iChitietHT.MaCTHP;
                }

                countIDSolid++;

                //Them vao arrWeek
                var lsBuoi = lsChiTietHPHienTai.OrderBy(x => x.Tuan).ToList();
                if (lsBuoi.Count > 0)
                {
                    var lsTuan = lsBuoi.Where(x => x.Tuan != null && x.Tuan.Trim() != "").GroupBy(x => x.Tuan).OrderBy(x => x.Key).Select(x => x.Key);
                    foreach (var iTuan in lsTuan)
                    {
                        List<string> mTuan = new List<string>();
                        mTuan = lsBuoi.Where(x => x.Tuan == iTuan).Select(x => x.Buoi.Trim()).ToList();
                        arrWeek.Add(mTuan);
                        countWeek++;
                    }

                }



                return Json(new { status = 200, message = VI.mes_TaiThanhCong, lsModelChiTietHP, arrWeek, countIDSolid, countWeek }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = 500, message = VI.mes_KhongTaiDuoc }, JsonRequestBehavior.AllowGet);

            }

        }

        /// <summary>
        /// Purpose: Thay đổi trạng thái khi nộp đề cương học phần
        /// Developer: Trần An Bình
        /// Date:  
        /// </summary>
        /// <param name="maKHDT">Mã KHDT</param> 
        /// <returns>string check hàm đã chạy hoàn tất</returns>
        /// 
        public string NopDeCuongMonHoc(int makhdt)
        {
            var khdt = db.tp_KHDaotao.Find(makhdt).TrangthaiDC = Variables.TrangthaiDC_ChoPheDuyet;
            db.SaveChanges();
            return "done";
        }
        public ActionResult LayDanhSachCDRPheDuyetDC(int maKHDT)
        {
            var listCDR = db.tp_CDR_HP.Where(s => s.MaKHDT == maKHDT)
                .Select(s => new { MaCELO = s.MaCELO, TenCDR = s.MaHT, PhanLoai = s.Phanloai, MoTa = s.Mota })
                .OrderBy(s => s.MaCELO).ToList();
            return Json(listCDR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayDanhSachNoiDungPheDuyetDC(int maCELO)
        {
            var listNoiDung = db.tp_CDR_HP.Find(maCELO).tp_NoidungHP
                .Select(s => new { s.MaND, TenNoiDung = s.TenHT, s.Phanloai, s.Noidung })
                .ToList();
            return Json(listNoiDung, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayTacVuNoiDungPheDuyetDC(int maKHDT, int maCELO, int maND)
        {
            var listHoatDong = db.GetListCDR_NoiDung_TacVu(maKHDT, maCELO, maND).ToList();
            List<TacVuHocPhan> listTVHoatDong = new List<TacVuHocPhan>();
            List<TacVuHocPhan> listTVHoatDongMoi = new List<TacVuHocPhan>();
            List<int> listNumber = new List<int>();
            foreach (var hoatDong in listHoatDong)
            {
                TacVuHocPhan tvHP = new TacVuHocPhan();
                tvHP.maTV = hoatDong.MaTV;
                tvHP.moTaTV = hoatDong.Mota;
                tvHP.phanLoai = hoatDong.Phanloai;
                tvHP.hoatDongTrongLop = hoatDong.HDTL;
                tvHP.soGio = hoatDong.Sogio;
                tvHP.noiDung = hoatDong.Noidung;
                listTVHoatDong.Add(tvHP);
            }
            for (int i = 0; i < listHoatDong.Count; i++)
            {
                if (!listNumber.Contains(listHoatDong[i].MaTV))
                {
                    string noiDung = "";
                    for (int j = 0; j < listHoatDong.Count; j++)
                    {
                        if (listHoatDong[j].MaTV == listHoatDong[i].MaTV)
                        {
                            noiDung += listHoatDong[j].Noidung.Trim() + ", ";
                        }
                    }
                    TacVuHocPhan tvHP = new TacVuHocPhan();
                    tvHP.maTV = listHoatDong[i].MaTV;
                    tvHP.moTaTV = listHoatDong[i].Mota;
                    tvHP.phanLoai = listHoatDong[i].Phanloai;
                    tvHP.hoatDongTrongLop = listHoatDong[i].HDTL;
                    tvHP.noiDung = noiDung;
                    tvHP.soGio = listHoatDong[i].Sogio;

                    listNumber.Add(listHoatDong[i].MaTV);

                    listTVHoatDongMoi.Add(tvHP);
                }
            }
            return Json(listTVHoatDongMoi, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayDanhGiaPheDuyetDC(int maCELO)
        {
            var listDanhGia = db.tp_CDR_HP.Find(maCELO).tp_DanhgiaHP.Select(s => new { s.Noidung, s.Trongso }).ToList();
            return Json(listDanhGia, JsonRequestBehavior.AllowGet);
        }
    }

    /**
     *  Class Rec JSON for Save Subject
     */
    public class Subject
    {
        int mactdt;
        int mamh;
        int makhdt;
        string year;
        string hocky;
        int sotc;
        int giolt;
        int gioth;
        int gioda;
        int giott;
        string status;


        public int Mactdt { get => mactdt; set => mactdt = value; }
        public int Mamh { get => mamh; set => mamh = value; }
        public int MaKhdt { get => mamh; set => mamh = value; }
        public string Year { get => year; set => year = value; }
        public string Hocky { get => hocky; set => hocky = value; }
        public int Sotc { get => sotc; set => sotc = value; }
        public int Giolt { get => giolt; set => giolt = value; }
        public int Gioth { get => gioth; set => gioth = value; }
        public int Gioda { get => gioda; set => gioda = value; }
        public int Giott { get => giott; set => giott = value; }
        public string Status { get => status; set => status = value; }
    }

    /**
     *  Class Json for page4 
     */
    public class ModelChitietHPPage4
    {
        public string Tuan;

        public List<ModelTacvuPage4> I;

        public List<ModelTacvuPage4> O;

        public ModelChitietHPPage4()
        {
            I = new List<ModelTacvuPage4>();
            O = new List<ModelTacvuPage4>();
        }
    }

    public class ModelTacvuPage4
    {
        public int idKey { get; set; }
        public string txaDes { get; set; }
        public int txtHour { get; set; }
        public string txtNoidung { get; set; }
        public string txtNth { get; set; }
        public List<string> txtRating { get; set; }

        public ModelTacvuPage4()
        {
            txtRating = new List<string>();
        }
    }




    /**
     * Class Return Json cho LayDanhSachGiaoVienDuocPhanCong
     */
    public class KHDTVaGiangVienVaTrangThai
    {
        public string khdt;

        public string gvduocpc;

        public string ngayhethandecuong;

        public List<GiangVienVaTrangThai> listgv;

        public KHDTVaGiangVienVaTrangThai()
        {
            listgv = new List<GiangVienVaTrangThai>();
        }

        public int solopdukien;
    }

    public class GiangVienVaTrangThai
    {
        public string trangthai;

        public string magv;

        public string lydo;
    }


    /**
    * Class gửi mail cho giảng viên 1 giảng viên có thể có dạy nhiều KHDT
    */
    public class GiangVienKHDT
    {
        public string magv;

        public List<string> listKHDT;

        public GiangVienKHDT()
        {
            listKHDT = new List<string>();
        }

    }


    //Model for view ChinhSuaDeCuongHocPhanPage1 to save CDR-HP
    public class ModelChinhSuaDeCuongHocPhanPage1
    {
        public int macelo;
        public List<ModelcdrCTDTChinhSuaDeCuongHocPhanPage1> cdrCTDT { get; set; }
        public string moTa { get; set; }
        public string phanLoai { get; set; }
        public string tenKQHT { get; set; }

    }

    public class ModelcdrCTDTChinhSuaDeCuongHocPhanPage1
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    public class ModelDanhgiaHPStep2
    {
        public int madg { get; set; }
        public List<ModelkqhtmongdoiStep2> kqhtmongdoi { get; set; }
        public string mota { get; set; }
        public string thanhphan { get; set; }
        public bool trangthai { get; set; }
        public int trongso { get; set; }

    }

    public class ModelNoiDungHPStep2
    {
        public int id { get; set; }
        public string idNoiDung { get; set; }
        public List<ModelkqhtmongdoiStep2> kqht { get; set; }
        public string noiDungNgan { get; set; }
        public string noiDung { get; set; }
        public string phanLoai { get; set; }
    }

    public class ModelkqhtmongdoiStep2
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    public class LevelSaT
    {
        public int MaDG { get; set; }
        public int? MaQH { get; set; }
        public string Mota { get; set; }
        public int? Trongso { get; set; }
        public string MucDo { get; set; }

        public List<LevelSaT> LevelNext { get; set; }
    }


    //Model page 2
    public class ModelPage2CongCuDG
    {
        public int maCELO { get; set; }
        public string congCuDanhGia { get; set; }
        public string thoiDiemDanhGia { get; set; }
    }

    public class ModelPage2CongCuDGReturn
    {
        public string ccDG { get; set; }
        public ModelPage2CongCuDGReturnInner cdr { get; set; }
        public int madg { get; set; }
        public string tdDG { get; set; }
    }

    public class ModelPage2CongCuDGReturnInner
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class TacVuHocPhan
    {
        public int maTV { get; set; }
        public string moTaTV { get; set; }
        public string phanLoai { get; set; }
        public int? soGio { get; set; }
        public bool? hoatDongTrongLop { get; set; }
        public string noiDung { get; set; }
    }
}