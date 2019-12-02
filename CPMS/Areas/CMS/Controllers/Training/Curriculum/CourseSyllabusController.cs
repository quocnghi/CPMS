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
    public class CourseSyllabusController : Controller
    {
        fit_misDBEntities db = new fit_misDBEntities();
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult CtdtList()
        {
            var lst = db.t_CTDaotao.ToList();
            return View(lst);
        }
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR)]
        public ActionResult CourseList(int ID)
        {
            //lấy mã ctdt gán vào cookie
            HttpCookie ctdt = new HttpCookie("CTDTfKHDT");
            ctdt.Value = ID.ToString();
            ctdt.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(ctdt);
            //var lsthp = db.tc_KHDaotao.Where(s => s.MaCTDT == ID).ToList();
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
            HttpCookie ctdt = Request.Cookies["CTDTfKHDT"];
            var id = ctdt.Value;
            int ctdtId = int.Parse(id);
            List<tc_KHDaotao> khdt = new List<tc_KHDaotao>();
            var role = db.AspNetUsers.Find(userid).AspNetRoles.FirstOrDefault().Name;
            if (role.Equals(ROLES.ADMIN) || role.Equals(ROLES.HEADOFEDITOR))
            {
                var lstkhdt = db.tc_KHDaotao.Where(s => s.MaCTDT == ctdtId).Select(s => new
                {
                    s.GioDA,
                    s.GioLT,
                    s.GioTH,
                    s.GioTT,
                    s.Hinhthuc,
                    s.Hocky,
                    s.MaCTDT,
                    MaHP = s.sc_Hocphan.MaHT,
                    s.MaKHDT,
                    s.MaKhoiKT,
                    s.MonHT,
                    s.MonSH,
                    s.MonTQ,
                    s.Mota,
                    s.MuctieuHP,
                    s.SoTC,
                    s.NgonnguGD,
                    s.NhiemvuSV,
                    s.Phanloai,
                    s.PPGD,
                    s.PPHT,
                    s.Thangdiem,
                    TenHP = s.sc_Hocphan.TenMH,
                    s.NgayTao,
                    s.NgayCN,
                    s.TrangthaiDC,
                    s.NgayHT,
                    TenCTDT = s.t_CTDaotao.TenCT,
                    GVien = s.tc_DecuongGV.Select(h => new { tenGV = h.AspNetUser.m_Nhanvien.Ten, hoGV = h.AspNetUser.m_Nhanvien.Ho, h.Trangthai })
                });
                return Json(new { khdt = lstkhdt });
            }
            //lấy danh sách học phần được phân công
            var lstKHDT = db.tc_DecuongGV.Where(s => s.NguoiST == userid).ToList();
            foreach (var item in lstKHDT)
            {
                var khdtao = db.tc_KHDaotao.Where(s => s.MaKHDT == item.MaKHDT && s.MaCTDT == ctdtId).ToList();
                foreach (var i in khdtao)
                {
                    var mt = db.tc_KHDaotao.Where(s => s.MaKHDT == i.MaKHDT && s.MaCTDT == ctdtId).FirstOrDefault();
                    khdt.Add(mt);
                }
            }
            //lấy dữ liệu từ bảng KHDaotao
            var listKHDT = khdt.Select(s => new {
                s.GioDA,
                s.GioLT,
                s.GioTH,
                s.GioTT,
                s.Hinhthuc,
                s.Hocky,
                s.MaCTDT,
                MaHP = s.sc_Hocphan.MaHT,
                s.MaKHDT,
                s.MaKhoiKT,
                s.MonHT,
                s.MonSH,
                s.MonTQ,
                s.Mota,
                s.MuctieuHP,
                s.SoTC,
                s.NgonnguGD,
                s.NhiemvuSV,
                s.Phanloai,
                s.PPGD,
                s.PPHT,
                s.Thangdiem,
                TenHP = s.sc_Hocphan.TenMH,
                s.NgayTao,
                s.NgayCN,
                s.TrangthaiDC,
                TenCTDT = s.t_CTDaotao.TenCT,
                s.NgayHT,
                GVien = s.tc_DecuongGV.Select(h => new { tenGV = h.AspNetUser.m_Nhanvien.Ten, hoGV = h.AspNetUser.m_Nhanvien.Ho, h.Trangthai })
            });
            return Json(new { khdt = listKHDT });
        }

        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public ActionResult Course(int id)
        {
            //gán mã KHDT vào cookie
            HttpCookie ck = new HttpCookie("idKHDT");
            ck.Value = id.ToString();
            ck.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(ck);
            return View();
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult loadInfo()
        {
            try
            {
                //gán mã KHDT vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //lấy dữ liệu tử bảng KHDaotao  tương ứng với mã đã nhớ
                var khdt = db.tc_KHDaotao.Where(s => s.MaKHDT == id).Select(s => new {
                    s.MaKhoiKT,
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
                    TenCTDT = s.t_CTDaotao.TenCT,
                    s.PhuongtienGD,
                    s.PhuongtienThi,
                    MaHP = s.sc_Hocphan.MaHT,
                    s.GhichuDG,
                    loaiPH = s.sc_LoaiPH.Select(h => new { h.MaLoaiPH, h.TenLoaiPH })
                }).FirstOrDefault();
                //lấy dữ liệu từ bảng chuẩn đầu ra CTDT
                var cdrctdt = db.t_CDR_CTDT.Where(s => s.MaCTDT == khdt.MaCTDT && s.tc_KHDaotao.Any(h=>h.MaKHDT == id)).Select(s => new { s.MaELO, s.MaHT, s.Mota, s.Phanloai });
                //lấy dữ liệu từ bảng khối kiến thức
                var kkt = db.sc_Khoikienthuc.Select(s => new { s.MaKhoiKT, MotaKT = s.Mota });
                //lấy dữ liệu từ bảng chuẩn đầu ra học phần
                var rs = db.tc_CDR_HP.Where(s => s.MaKHDT == id).Select(s => new { s.MaCELO, s.MaHT, s.Mota, s.Phanloai, MaHT1 = s.tc_MatranCDR.Select(h => new { MaCELO = h.tc_CDR_HP.MaHT, MaELO = h.t_CDR_CTDT.MaHT, h.Mucdo }) });
                //lấy dữ liệu từ bảng nội dung hp
                var nd = db.tc_NoidungHP.Where(s => s.MaKHDT == id).Select(s => new { s.TenHT, s.Phanloai, s.Noidung, s.Ghichu, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaHT }) });
                //lấy dữ liệu từ bảng tài liệu hp
                var tlhp = db.tc_TailieuHP.Where(s => s.MaKHDT == id).Select(s => new { s.MaTL, s.LoaiTL, s.TenTL, s.Tacgia, s.NhaXB, s.NamXB, s.Kieunhap });
                var loaiph = db.sc_LoaiPH.Select(s => new { s.MaLoaiPH, s.TenLoaiPH });
                string[] strHPTQ = null;
                string[] strHPHT = null;
                if(khdt.MonTQ != null)
                {
                    if (khdt.MonTQ.Contains(","))
                    {
                        strHPTQ = khdt.MonTQ.Split(',');
                    }
                }
                if(khdt.MonHT != null)
                {
                    if (khdt.MonHT.Contains(","))
                    {
                        strHPHT = khdt.MonHT.Split(',');
                    }
                }
              
                var lsthptq = new List<sc_Hocphan>();
                var lsthpht = new List<sc_Hocphan>();
                var lstkhdt = db.tc_KHDaotao.ToList();
                if(strHPTQ != null)
                {
                    foreach (var item in strHPTQ)
                    {
                        var makhdt = int.Parse(item);
                        var hptq = db.sc_Hocphan.Find(makhdt);
                        lsthptq.Add(hptq);
                    }
                }
                if (strHPHT != null)
                {
                    foreach (var item in strHPHT)
                    {
                        var makhdt = int.Parse(item);
                        var hpht = db.sc_Hocphan.Find(makhdt);
                        lsthpht.Add(hpht);
                    }
                }
                var listhptq = lsthptq.Select(s=>new { s.TenMH});
                var listhpht = lsthpht.Select(s => new { s.TenMH });
                return Json(new { mhoc = khdt, CDR = cdrctdt, KKT = kkt, cdrhp = rs, ndung = nd, tHP = tlhp, loaiPH = loaiph, lstht= listhpht, lsttq = listhptq });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult loadReviewInfo(int id)
        {
            try
            {
                //lấy dữ liệu tử bảng KHDaotao  tương ứng với mã đã nhớ
                var khdt = db.tc_KHDaotao.Where(s => s.MaKHDT == id).Select(s => new {
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
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult addCdr(MappingCDRMH mapping)
        {
            try
            {
                //lấy mã idKHDT gán vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //lấy mã KHDT trong bảng CDRMH giống với mã đã nhớ
                mapping.CDRMH.MaKHDT = id;
                //thêm các dữ liệu vào bảng chuẩn đầu ra học phần
                db.tc_CDR_HP.Add(mapping.CDRMH);
                //lưu dữ liệu
                db.SaveChanges();
                //lưu nhiều-nhiều giữa hai bảng chuẩn đầu ra hp và ma trận chuẩn đầu ra 
                var idHP = db.tc_CDR_HP.AsEnumerable().LastOrDefault().MaCELO;
                foreach (var item in mapping.MaELO)
                {
                    tc_MatranCDR tcmatrancdr = new tc_MatranCDR();
                    tcmatrancdr.MaCELO = idHP;
                    tcmatrancdr.MaELO = item;
                    db.tc_MatranCDR.Add(tcmatrancdr);
                    db.SaveChanges();
                }
                //lấy dữ liệu đã lưu để hiển thị
                var rs = db.tc_CDR_HP.Where(s => s.MaKHDT == id).Select(s => new { s.MaCELO, s.MaHT, s.Mota, s.Phanloai, MaHT1 = s.tc_MatranCDR.Select(h => new { MaCELO = h.tc_CDR_HP.MaHT, MaELO = h.t_CDR_CTDT.MaHT, h.Mucdo }) });

                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_ThemCDR, cdrhp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }
        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult getCdr(tc_CDR_HP cdrhp)
        {
            try
            {
                var rs = db.tc_CDR_HP.Where(s => s.MaCELO == cdrhp.MaCELO).Select(s => new { s.MaHT, s.Mota, s.Phanloai, s.MaCELO, MaHT1 = s.tc_MatranCDR.Select(h => new { h.MaELO,h.t_CDR_CTDT.MaHT }) }).FirstOrDefault();
                //var rr = db.tc_MatranCDR.Where(s => s.MaCELO == cdrhp.MaCELO).Select(s => new { s.MaELO, s.t_CDR_CTDT.MaHT, s.Mucdo }).FirstOrDefault();
                return Json(new { cdrhp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult editCdr(MappingCDRMH mapping)
        {
            try
            {
                //lấy mã idKHDT gán vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //tìm mã CELO tương ứng 
                var Hp = db.tc_CDR_HP.Find(mapping.CDRMH.MaCELO);
                //chỉnh sửa dữ liệu
                Hp.MaHT = mapping.CDRMH.MaHT;
                Hp.Mota = mapping.CDRMH.Mota;
                Hp.Phanloai = mapping.CDRMH.Phanloai;
                //thay đổi và lưu dữ liệu
                db.Entry(Hp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var matranelo = db.tc_MatranCDR.Where(s => s.MaCELO == mapping.CDRMH.MaCELO).ToList();
                foreach(var item in matranelo)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                foreach (var item in mapping.MaELO)
                {
                    tc_MatranCDR tcmatrancdr = new tc_MatranCDR();
                    tcmatrancdr.MaCELO = mapping.CDRMH.MaCELO;
                    tcmatrancdr.MaELO = item;
                    db.tc_MatranCDR.Add(tcmatrancdr);
                    db.SaveChanges();
                }
                var rs = db.tc_CDR_HP.Where(s => s.MaKHDT == id).Select(s => new { s.MaCELO, s.MaHT, s.Mota, s.Phanloai, MaHT1 = s.tc_MatranCDR.Select(h => new { MaCELO = h.tc_CDR_HP.MaHT, MaELO = h.t_CDR_CTDT.MaHT, h.Mucdo }) });

                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_SuaCDR, cdrhp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Chinhsuadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult deleteCdr(int id)
        {
            try
            {
                //tìm mã chuẩn đầu ra tương ứng
                var rs = db.tc_CDR_HP.Find(id);
                //lấy toàn bộ mã chuẩn đầu ra giống với mã đã tìm được
                var lstmatran = db.tc_MatranCDR.Where(s => s.MaCELO == rs.MaCELO).ToList();
                foreach (var item in lstmatran)
                {
                    //tìm và xóa mã chuẩn đầu ra trong bảng ma trận chuẩn đẩu ra
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    //lưu lại
                    db.SaveChanges();
                }
                //xóa mã chuẩn đầu ra trong bảng chuẩn đầu ra học phần
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                //lưu lại
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_XoaCDR });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Xoadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult addND(MappingNDGD mapping)
        {
            try
            {
                //lấy mã idKHDT gán vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //lấy mã KHDT trong bảng NDGD tương ứng với mã đã nhớ
                mapping.NDGD.MaKHDT = id;
                //thêm dữ liệu vào bảng nội dung
                db.tc_NoidungHP.Add(mapping.NDGD);
                //lưu dữ liệu
                db.SaveChanges();
                //lưu nhiều-nhiều giữa hai bảng nội dung hp và bảng CDR nội dung hp
                var idHP = db.tc_NoidungHP.AsEnumerable().LastOrDefault().MaND;
                foreach (var item in mapping.MaCELO)
                {
                    var hp = db.tc_CDR_HP.Find(item);
                    db.tc_NoidungHP.Find(idHP).tc_CDR_HP.Add(hp);
                    db.SaveChanges();
                }
                //lấy dữ liệu để hiển thị
                var ng = db.tc_NoidungHP.Where(s => s.MaKHDT == id).Select(s => new { s.TenHT, s.Phanloai, s.Noidung, s.Ghichu, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaHT }) });
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_ThemND, ndGiangDay = ng });

            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }

        }
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult getND(tc_NoidungHP nhp)
        {
            try
            {
                //lấy dữ liệu bảng nội dung hp
                var rs = db.tc_NoidungHP.Where(s => s.MaND == nhp.MaND).Select(s => new { s.Phanloai, s.Noidung, s.MaKHDT, s.TenHT, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaCELO,h.MaHT }) }).FirstOrDefault();
                //lấy dữ liệu bảng CDR nội dung hp
                //var rr = db.tc_CDR_NoidungHP.Where(s => s.MaND == nhp.MaND).Select(s => new { s.MaND, s.t_CDR_HP.MaHT, s.Ghichu }).FirstOrDefault();
                return Json(new { ndhp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult editND(MappingEditNDGD Hp)
        {
            try
            {
                //lấy mã idKHDT gán vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);

                var Ndhp = db.tc_NoidungHP.Find(Hp.NDGD.MaND);
                Ndhp.Noidung = Hp.NDGD.Noidung;
                Ndhp.Phanloai = Hp.NDGD.Phanloai;
                db.Entry(Ndhp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                foreach(var item in Ndhp.tc_CDR_HP.ToList())
                {
                    db.tc_NoidungHP.Find(Hp.NDGD.MaND).tc_CDR_HP.Remove(item);
                    db.SaveChanges();
                }

                foreach (var item in Hp.MaCELO)
                {
                    var hp = db.tc_CDR_HP.Find(item);
                    db.tc_NoidungHP.Find(Hp.NDGD.MaND).tc_CDR_HP.Add(hp);
                    db.SaveChanges();
                }
                var rs = db.tc_NoidungHP.Where(s => s.MaKHDT == id).Select(s => new { s.TenHT, s.Phanloai, s.Noidung, s.Ghichu, s.MaND, Mota = s.tc_CDR_HP.Select(h => new { h.MaHT }) });

                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_SuaND, ndunghp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Chinhsuadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult deleteND(int id)
        {
            try
            {
                //tìm mã nội dung hp tương ướng
                var rs = db.tc_NoidungHP.Find(id);
                foreach (var item in rs.tc_CDR_HP.ToList())
                {
                    db.tc_NoidungHP.Find(id).tc_CDR_HP.Remove(item);
                    //lưu lại
                    db.SaveChanges();
                }
                //tìm và xóa mã nội dung hp trong bảng nội dung hp
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                //lưu lại
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_XoaND });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Xoadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult addTailieu(tc_TailieuHP tailieuHP)
        {
            try
            {
                //gán mã KHDT vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //lấy mã KHDT từ bảng tài liệu hp giống với mã đã nhớ
                tailieuHP.MaKHDT = id;
                //thêm dữ liệu vào bảng tài liệu hp
                db.tc_TailieuHP.Add(tailieuHP);
                //lưu dữ liệu
                db.SaveChanges();
                //lấy dữ liệu để hiển thị
                var tlp = db.tc_TailieuHP.Where(s => s.MaKHDT == id).Select(s => new { s.MaTL, s.NamXB, s.NhaXB, s.Tacgia, s.TenTL, s.LoaiTL, s.Kieunhap });
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_ThemTL, tlthamkhao = tlp });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult getTailieu(tc_TailieuHP HP)
        {
            try
            {
                //lấy dữ liệu từ bản tài liệu hp tương ứng với mã đã có để hiển thị
                var rs = db.tc_TailieuHP.Where(s => s.MaTL == HP.MaTL).Select(s => new { s.MaTL, s.MaKHDT, s.LoaiTL, s.NamXB, s.NhaXB, s.Tacgia, s.TenTL, s.Kieunhap }).FirstOrDefault();
                return Json(new { tlhp = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_LoadData });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult editTailieu(tc_TailieuHP HpTk)
        {
            try
            {
                //gán mã KHDT vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //tìm mà tài liệu tương ứng đã có
                var TlHp = db.tc_TailieuHP.Find(HpTk.MaTL);
                //thay đổi dữ liệu
                TlHp.LoaiTL = HpTk.LoaiTL;
                if (HpTk != null)
                {
                    TlHp.NamXB = HpTk.NamXB;
                }
                TlHp.NhaXB = HpTk.NhaXB;
                TlHp.Tacgia = HpTk.Tacgia;
                TlHp.TenTL = HpTk.TenTL;
                //chỉnh sửa và lưu dữ liệu
                db.Entry(TlHp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //lấy dữ liệu để hiển thị
                var rs = db.tc_TailieuHP.Where(s => s.MaKHDT == id).Select(s => new { s.MaTL, s.LoaiTL, s.NamXB, s.NhaXB, s.Tacgia, s.TenTL, s.Kieunhap });

                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_SuaTL, tlthamkhao = rs });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Chinhsuadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult deleteTailieu(int id)
        {
            try
            {
                //tìm mã tài liệu được truyền vào
                var rs = db.tc_TailieuHP.Find(id);
                //xóa và lưu dữ liệu
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_XoaTL });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Xoadulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult hoanthanh(HoanthanhKHDT ht)
        {
            try
            {
                //gán mã KHDT vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //tìm mã KHDT đã nhớ trong bảng KHDaotao
                var kHDT = db.tc_KHDaotao.Find(id);
                //thay đổi dữ liệu
                kHDT.GioDA = ht.khdt.GioDA;
                kHDT.GioLT = ht.khdt.GioLT;
                kHDT.GioTH = ht.khdt.GioTH;
                kHDT.GioTT = ht.khdt.GioTT;
                kHDT.SoTC = ht.khdt.SoTC;
                kHDT.Hocky = ht.khdt.Hocky;
                kHDT.MuctieuHP = ht.khdt.MuctieuHP;
                kHDT.Mota = ht.khdt.Mota;
                kHDT.NgonnguGD = ht.khdt.NgonnguGD;
                kHDT.PPGD = ht.khdt.PPGD;
                kHDT.PPHT = ht.khdt.PPHT;
                kHDT.Hinhthuc = ht.khdt.Hinhthuc;
                kHDT.NhiemvuSV = ht.khdt.NhiemvuSV;
                kHDT.Thangdiem = ht.khdt.Thangdiem;
                kHDT.MonTQ = ht.khdt.MonTQ;
                kHDT.MonHT = ht.khdt.MonHT;
                kHDT.NoidungCN = ht.khdt.NoidungCN;
                kHDT.PhuongtienThi = ht.khdt.PhuongtienThi;
                kHDT.PhuongtienGD = ht.khdt.PhuongtienGD;
                //lấy ngày hiện tại cho ngày tạo hoặc ngày cập nhật
                if (kHDT.NgayTao == null)
                {
                    kHDT.NgayTao = DateTime.Now;
                }
                else
                {
                    kHDT.NgayCN = DateTime.Now;
                }
                //chuyển trạng thái thành chờ phê duyệt
                kHDT.TrangthaiDC = TrangThaiDc.ChoPheDuyet;
                //chỉnh sửa và lưu dữ liệu
                db.Entry(kHDT).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //gán mã ctdt vào cookie
                HttpCookie ctdt = Request.Cookies["CTDTfKHDT"];
                var idCTDT = ctdt.Value;
                int ctdtId = int.Parse(idCTDT);
                var lstKHDT = db.tc_KHDaotao.Where(s => s.MaCTDT == kHDT.MaCTDT).ToList();
                foreach (var item in lstKHDT)
                {
                    if (item.TrangthaiDC == "1")
                    {
                        return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_Hoanthanh, id = ctdtId });
                    }
                }
                if (ht.MaPH != null)
                {
                    foreach (var item in ht.MaPH)
                    {
                        var khdt = db.tc_KHDaotao.Find(id);
                        db.sc_LoaiPH.Find(item).tc_KHDaotao.Add(khdt);
                        db.SaveChanges();
                    }
                }
                var chuongtrinhDT = db.t_CTDaotao.Find(ht.khdt.MaCTDT);
                chuongtrinhDT.Trangthai = TrangThai.DanhGiaCTDT;
                db.Entry(chuongtrinhDT).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { msg = NotificationManagement.SuccessMessage.DCCT_Hoanthanh, id = ctdtId });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }
        }

        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult luunhap(LuunhapKHDT ln)
        {
            try
            {
                //gán mã KHDT vào cookie
                HttpCookie ck = Request.Cookies["idKHDT"];
                var userid = ck.Value;
                var id = int.Parse(userid);
                //tìm mã KHDT đã nhớ trong bảng KHDaotao
                var kHDT = db.tc_KHDaotao.Find(id);
                //thay đổi dữ liệu
                kHDT.GioDA = ln.khdt.GioDA;
                kHDT.GioLT = ln.khdt.GioLT;
                kHDT.GioTH = ln.khdt.GioTH;
                kHDT.GioTT = ln.khdt.GioTT;
                kHDT.SoTC = ln.khdt.SoTC;
                kHDT.Hocky = ln.khdt.Hocky;
                kHDT.MuctieuHP = ln.khdt.MuctieuHP;
                kHDT.Hinhthuc = ln.khdt.Hinhthuc;
                kHDT.Mota = ln.khdt.Mota;
                kHDT.NgonnguGD = ln.khdt.NgonnguGD;
                kHDT.PPGD = ln.khdt.PPGD;
                kHDT.PPHT = ln.khdt.PPHT;
                kHDT.NhiemvuSV = ln.khdt.NhiemvuSV;
                kHDT.Thangdiem = ln.khdt.Thangdiem;
                kHDT.MonTQ = ln.khdt.MonTQ;
                kHDT.MonHT = ln.khdt.MonHT;
                kHDT.NoidungCN = ln.khdt.NoidungCN;
                kHDT.PhuongtienThi = ln.khdt.PhuongtienThi;
                kHDT.PhuongtienGD = ln.khdt.PhuongtienGD;

                //lấy ngày hiện tạo cho ngày tạo và ngày cập nhật
                if (kHDT.NgayTao == null)
                {
                    kHDT.NgayTao = DateTime.Now;
                }
                else
                {
                    kHDT.NgayCN = DateTime.Now;
                }
                //chuyển trạng thái sang lưu tạm
                kHDT.TrangthaiDC = TrangThaiDc.LuuTam;
                //chỉnh sửa và lưu dữ liệu
                db.Entry(kHDT).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (ln.MaPH != null)
                {
                    foreach (var item in ln.MaPH)
                    {
                        var khdt = db.tc_KHDaotao.Find(id);
                        db.sc_LoaiPH.Find(item).tc_KHDaotao.Add(khdt);
                        db.SaveChanges();
                    }
                }
                //gán mã ctdt vài cookie
                HttpCookie ctdt = Request.Cookies["CTDTfKHDT"];
                var idCTDT = ctdt.Value;
                int ctdtId = int.Parse(idCTDT);
                return Json(new { mg = NotificationManagement.SuccessMessage.DCCT_Luutam, id = ctdtId });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }
        }
        //luu danh gia
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public JsonResult Adddanhgia(tc_KHDaotao monhoc)
        {
            try
            {
                var rs = db.tc_KHDaotao.Find(monhoc.MaKHDT);
                rs.GhichuDG = monhoc.GhichuDG;
                rs.TrangthaiDC = monhoc.TrangthaiDC;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HttpCookie ctdt = Request.Cookies["CTDTfKHDT"];
                var idCTDT = ctdt.Value;
                int ctdtId = int.Parse(idCTDT);
                return Json(new { mg = NotificationManagement.SuccessMessage.DCCT_Danhgia, id = ctdtId });
            }
            catch (Exception e)
            {
                return Json(new { msg = NotificationManagement.ErrorMessage.DCCT_Luudulieu });
            }
        }
    }
}