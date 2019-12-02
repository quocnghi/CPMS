using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using Capstone.Areas.PMS.Controllers;
using Capstone.Models;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.IO;
using System.Drawing;
using System.Collections;
using CommonRS.Resources;
using Newtonsoft.Json;

namespace Capstone.Areas.PMS.Controllers
{
    public class DeCuongMonHocController : Controller
    {
        APIController API = new APIController();
        fit_misDBEntities context = new fit_misDBEntities();

        // GET: View MonHocGiangDay for GiangVien
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult MonHocGiangDay()
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = API.dsHeDaotao();
            return View(listSystem);
        }
        // GET: View DeCuongMonHoc for TruongBoMon
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult DeCuongMonHoc()
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = API.dsHeDaotao();
            return View(listSystem);
        }

        // GET: View ChinhSuaDeCuongHocPhan for GiangVien, TruongBoMon
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult ChinhSuaDeCuongHocPhan(int id)
        {
            tp_KHDaotao khdt = context.tp_KHDaotao.Find(id);
            return View(khdt);
        }

        // Get: View XemChiTiet for GiangVien, TruongBoMon
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien)]
        public ActionResult XemChiTietDeCuong(int id)
        {
            tp_KHDaotao khdt = context.tp_KHDaotao.Find(id);
            return View(khdt);
        }

        // POST: View PheDuyetDeCuongHocPhan for TruongBoMon
        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult PheDuyetDeCuongHocPhan(int id)
        {
            tp_KHDaotao khdt = context.tp_KHDaotao.Find(id);
            return View(khdt);
        }

        // POST: PheDuyetDeCuongHocPhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string XacNhanPheDuyetDeCuong(int maKHDT, int trangThaiPheDuyet)
        {
            string message = "";
            try
            {
                fit_misDBEntities db = new fit_misDBEntities();
                var khdt = db.tp_KHDaotao.Find(maKHDT);
                if (khdt.TrangthaiDC == "2")
                {
                    if (trangThaiPheDuyet == 1)
                    {
                        khdt.TrangthaiDC = "1";
                        db.SaveChanges();
                        message = "Success";
                    }
                    else
                    {
                        khdt.TrangthaiDC = "0";
                        db.SaveChanges();
                        message = "Success";
                    }
                }
                else
                {
                    message = "Fail";
                }
            }
            catch (Exception e)
            {
                message = "Error";
            }
            return message;
        }

        // POST: PheDuyetDeCuongHocPhan
        [HttpPost]
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult TongQuanPheDuyetDeCuongHocPhan(int id)
        {
            tp_KHDaotao khdt = context.tp_KHDaotao.Find(id);
            return View(khdt);
        }

        Document doc = new Document();

        public string getListHoatDong()
        {

            Random random = new Random();
            int randomNumber = random.Next(0, 1000000);
            string fileName = "decuonghocphan.docx";
            string fileNameTemplate = "decuonghocphan" + randomNumber + ".docx";

            string sourcePath = Server.MapPath("~/");
            string targetPath = Server.MapPath("~/Content/Template/ProcessFile");

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileNameTemplate);

            // To copy a folder's contents to a new location:
            // Create a new target folder. 
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            System.IO.File.Delete(destFile);

            return destFile;
        }


        /// <summary>
        /// Purpose: Export File Word
        /// Developer: Nguyen Nguyen
        /// Date:
        /// </summary>
        /// <param name="maKHDT">Mã kế hoạch đào tạo</param> 
        /// <returns></returns>
        public ActionResult ExportWord(int maKHDT)
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 1000000);
                string fileName = "decuonghocphan.docx";
                string fileNameTemplate = "decuonghocphan" + randomNumber + ".docx";

                string sourcePath = Server.MapPath("~/Content/Template");
                string targetPath = Server.MapPath("~/Content/Template/ProcessFile");

                // Use Path class to manipulate file and directory paths.
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileNameTemplate);

                // To copy a folder's contents to a new location:
                // Create a new target folder. 
                // If the directory already exists, this method does not create a new directory.
                System.IO.Directory.CreateDirectory(targetPath);

                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(sourceFile, destFile, true);

                //Load file template
                doc.LoadFromFile(destFile);

                //Add default normal style and modify
                Style docStyle = doc.AddStyle(BuiltinStyle.Normal);
                docStyle.CharacterFormat.Font = new System.Drawing.Font("Times New Roman", 12);

                var khdt = context.tp_KHDaotao.Find(maKHDT);

                string tenHN = "";
                string tenKhoa = "";
                string tenMonTQ = "";
                string tenMonHT = "";
                string muctieuHP = "";
                string ngonNgu = "";
                string phuongPhapGiangDay = "";
                string phuongPhapHocTap = "";
                string moTaNoiDungHocPhan = "";
                string nhiemVuSinhVien = "";
                string thangDiem = "";
                string loaiPhongHoc = "";
                string phuongTienGD = "";
                string noiDungCN = "";
                string lanChinhSuaDC = "";
                string namHoc = "";
                string ngayXuat = "";
                string thangXuat = "";
                string namXuat = "";
                string gvCapNhat = "";
                string tenHocPhan = khdt.TenHP.ToString();
                string maHocPhan = khdt.MaHP.ToString();
                string soTinChi = khdt.SoTC.ToString();
                string gioLyThuyet = khdt.GioLT.ToString();
                string gioThucTien = khdt.GioTT.ToString();
                string gioTuHoc = khdt.GioTH.ToString();
                string hocKy = khdt.Hocky.ToString();

                var listTaiLieu = context.tp_TailieuHP.Where(s => s.MaKHDT == maKHDT).Select(s => new { s.LoaiTL, s.TenTL, s.Tacgia, s.NhaXB, s.NamXB, s.Kieunhap }).ToList();
                if (listTaiLieu != null)
                {
                    //TaiLieuHocTap
                    IList<Paragraph> replaceTLHT = new List<Paragraph>();
                    TextSelection selectionTLHT = doc.FindString("<tailieuhoctap>", false, true);
                    TextRangeLocation locationTLHT = new TextRangeLocation(selectionTLHT.GetAsOneRange());

                    ////GiaoTrinhChinh
                    IList<Paragraph> replaceGTC = new List<Paragraph>();
                    TextSelection selectionGTC = doc.FindString("<giaotrinhchinh>", false, true);
                    TextRangeLocation locationGTC = new TextRangeLocation(selectionGTC.GetAsOneRange());

                    ////TaiLieuKhac
                    IList<Paragraph> replaceTLK = new List<Paragraph>();
                    TextSelection selectionTLK = doc.FindString("<tailieukhac>", false, true);
                    TextRangeLocation locationTLK = new TextRangeLocation(selectionTLK.GetAsOneRange());

                    foreach (var taiLieu in listTaiLieu)
                    {
                        switch (int.Parse(taiLieu.LoaiTL))
                        {
                            case 1:
                                {
                                    if (int.Parse(taiLieu.Kieunhap) == 1)
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- " + taiLieu.NhaXB + " (" + taiLieu.NamXB + "), ");
                                        TextRange tenTL = p1.AppendText(taiLieu.TenTL);
                                        tenTL.CharacterFormat.Italic = true;
                                        replaceGTC.Add(p1);
                                    }
                                    else
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- ");
                                        TextRange linkTL = p1.AppendHyperlink(taiLieu.TenTL, taiLieu.TenTL, HyperlinkType.WebLink);
                                        replaceGTC.Add(p1);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (int.Parse(taiLieu.Kieunhap) == 1)
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- " + taiLieu.NhaXB + " (" + taiLieu.NamXB + "), ");
                                        TextRange tenTL = p1.AppendText(taiLieu.TenTL);
                                        tenTL.CharacterFormat.Italic = true;
                                        replaceTLK.Add(p1);
                                    }
                                    else
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- ");
                                        TextRange linkTL = p1.AppendHyperlink(taiLieu.TenTL, taiLieu.TenTL, HyperlinkType.WebLink);
                                        replaceTLK.Add(p1);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (int.Parse(taiLieu.Kieunhap) == 1)
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- " + taiLieu.NhaXB + " (" + taiLieu.NamXB + "), ");
                                        TextRange tenTL = p1.AppendText(taiLieu.TenTL);
                                        tenTL.CharacterFormat.Italic = true;
                                        replaceTLHT.Add(p1);
                                    }
                                    else
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- ");
                                        TextRange linkTL = p1.AppendHyperlink(taiLieu.TenTL, taiLieu.TenTL, HyperlinkType.WebLink);
                                        replaceTLHT.Add(p1);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (int.Parse(taiLieu.Kieunhap) == 1)
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- " + taiLieu.NhaXB + " (" + taiLieu.NamXB + "), ");
                                        TextRange tenTL = p1.AppendText(taiLieu.TenTL);
                                        tenTL.CharacterFormat.Italic = true;
                                        replaceTLHT.Add(p1);
                                    }
                                    else
                                    {
                                        Paragraph p1 = new Paragraph(doc);
                                        p1.AppendText("- ");
                                        TextRange linkTL = p1.AppendHyperlink(taiLieu.TenTL, taiLieu.TenTL, HyperlinkType.WebLink);
                                        replaceTLHT.Add(p1);
                                    }
                                    break;
                                }
                        }
                    }

                    //TaiLieuHocTap
                    Replace(locationTLHT, replaceTLHT);

                    //GiaoTrinhChinh
                    Replace(locationGTC, replaceGTC);

                    //TaiLieuKhac
                    Replace(locationTLK, replaceTLK);
                }

                if (khdt != null)
                {
                    tenHN = khdt.t_CTDaotao.sc_HeNganh.Mota;
                    tenKhoa = khdt.t_CTDaotao.sc_HeNganh.sc_Khoa.TenKhoa;
                    if (khdt.MonTQ != null)
                    {
                        string[] listMonTQ = khdt.MonTQ.ToString().Trim().Split(',');

                        foreach (string idMon in listMonTQ)
                        {
                            int idM = int.Parse(idMon);
                            string tenM = context.tp_KHDaotao.Where(s => s.MaHP == idM).Select(s => s.TenHP).FirstOrDefault();
                            if (tenMonTQ != "")
                            {
                                tenMonTQ = tenMonTQ + ", " + tenM;
                            }
                            else
                            {
                                tenMonTQ = tenMonTQ + tenM;
                            }
                        }
                    }
                    else
                    {
                        tenMonTQ = "Không";
                    }
                    if (khdt.MonHT != null)
                    {
                        string[] listMonHT = khdt.MonHT.ToString().Trim().Split(',');

                        foreach (string idMon in listMonHT)
                        {
                            int idM = int.Parse(idMon);
                            string tenM = context.tp_KHDaotao.Where(s => s.MaHP == idM).Select(s => s.TenHP).FirstOrDefault();
                            if (tenMonHT != "")
                            {
                                tenMonHT = tenMonHT + ", " + tenM;
                            }
                            else
                            {
                                tenMonHT = tenMonHT + tenM;
                            }
                        }
                    }
                    else
                    {
                        tenMonHT = "Không";
                    }
                    if (khdt.MuctieuHP != null)
                    {
                        muctieuHP = khdt.MuctieuHP.ToString().Trim();
                    }
                    if (khdt.NgonnguGD != null)
                    {
                        ngonNgu = khdt.NgonnguGD.ToString().Trim();
                    }
                    if (khdt.PPGD != null)
                    {
                        phuongPhapGiangDay = khdt.PPGD.ToString().Trim();
                    }
                    if (khdt.PPHT != null)
                    {
                        phuongPhapHocTap = khdt.PPHT.ToString().Trim();
                    }
                    if (khdt.Mota != null)
                    {
                        moTaNoiDungHocPhan = khdt.Mota.ToString().Trim();
                    }
                    if (khdt.NhiemvuSV != null)
                    {
                        nhiemVuSinhVien = khdt.NhiemvuSV.ToString().Trim();
                    }
                    if (khdt.Thangdiem != null)
                    {
                        thangDiem = khdt.Thangdiem.ToString().Trim();
                    }
                    if (khdt.PhuongtienGD != null)
                    {
                        phuongTienGD = khdt.PhuongtienGD.ToString().Trim();
                    }
                    var loaiPH = khdt.sc_LoaiPH.ToList();
                    if (loaiPH != null)
                    {
                        foreach (var phongHoc in loaiPH)
                        {
                            string tenPH = context.sc_LoaiPH.Where(s => s.MaLoaiPH == phongHoc.MaLoaiPH).Select(s => s.TenLoaiPH).FirstOrDefault();
                            if (loaiPH.IndexOf(phongHoc) != loaiPH.Count - 1)
                            {
                                loaiPhongHoc += tenPH + ", ";
                            }
                            else
                            {
                                loaiPhongHoc += tenPH;
                            }
                        }
                    }
                    var khoiKT = context.sc_Khoikienthuc.Find(khdt.MaKhoiKT);
                    var listKT = context.sc_Khoikienthuc.Where(s => s.MaQH == khoiKT.MaKhoiKT).ToList();
                    switch (khoiKT.MaKhoiKT)
                    {
                        case 2:
                            {
                                doc.Replace("<kktchuyennghiep>", '\u2611'.ToString(), false, true);
                                doc.Replace("<csnganh>", '\u2611'.ToString(), false, true);
                                doc.Replace("<chuyennganh>", '\u2610'.ToString(), false, true);
                                if (khdt.Hinhthuc == "BB")
                                {
                                    doc.Replace("<csnganhbb>", '\u2611'.ToString(), false, true);
                                    doc.Replace("<csnganhtc>", '\u2610'.ToString(), false, true);
                                }
                                else
                                {
                                    doc.Replace("<csnganhbb>", '\u2610'.ToString(), false, true);
                                    doc.Replace("<csnganhtc>", '\u2611'.ToString(), false, true);
                                }
                                doc.Replace("<chuyennganhbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<chuyennganhtc>", '\u2610'.ToString(), false, true);
                                doc.Replace("<kktdaicuong>", '\u2610'.ToString(), false, true);
                                doc.Replace("<daicuongbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<daicuongtc>", '\u2610'.ToString(), false, true);
                                break;
                            }
                        case 3:
                            {
                                doc.Replace("<kktchuyennghiep>", '\u2611'.ToString(), false, true);
                                doc.Replace("<csnganh>", '\u2610'.ToString(), false, true);
                                doc.Replace("<chuyennganh>", '\u2611'.ToString(), false, true);
                                if (khdt.Hinhthuc == "BB")
                                {
                                    doc.Replace("<chuyennganhbb>", '\u2611'.ToString(), false, true);
                                    doc.Replace("<chuyennganhtc>", '\u2610'.ToString(), false, true);
                                }
                                else
                                {
                                    doc.Replace("<chuyennganhbb>", '\u2610'.ToString(), false, true);
                                    doc.Replace("<chuyennganhtc>", '\u2611'.ToString(), false, true);
                                }
                                doc.Replace("<csnganhbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<csnganhtc>", '\u2610'.ToString(), false, true);
                                doc.Replace("<kktdaicuong>", '\u2610'.ToString(), false, true);
                                doc.Replace("<daicuongbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<daicuongtc>", '\u2610'.ToString(), false, true);
                                break;
                            }
                        case 4:
                            {
                                doc.Replace("<kktdaicuong>", '\u2611'.ToString(), false, true);
                                if (khdt.Hinhthuc == "BB")
                                {
                                    doc.Replace("<daicuongbb>", '\u2611'.ToString(), false, true);
                                    doc.Replace("<daicuongtc>", '\u2610'.ToString(), false, true);
                                }
                                else
                                {
                                    doc.Replace("<daicuongbb>", '\u2610'.ToString(), false, true);
                                    doc.Replace("<daicuongtc>", '\u2611'.ToString(), false, true);
                                }
                                doc.Replace("<kktchuyennghiep>", '\u2610'.ToString(), false, true);
                                doc.Replace("<csnganh>", '\u2610'.ToString(), false, true);
                                doc.Replace("<csnganhbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<csnganhtc>", '\u2610'.ToString(), false, true);
                                doc.Replace("<chuyennganh>", '\u2610'.ToString(), false, true);
                                doc.Replace("<chuyennganhbb>", '\u2610'.ToString(), false, true);
                                doc.Replace("<chuyennganhtc>", '\u2610'.ToString(), false, true);
                                break;
                            }
                    }
                    if (khdt.NoidungCN != null)
                    {
                        noiDungCN = khdt.NoidungCN;
                    }
                    if (khdt.SolancapnhatDC != null)
                    {
                        lanChinhSuaDC = khdt.SolancapnhatDC.ToString();
                    }
                    if (khdt.Namhoc != null)
                    {
                        namHoc = khdt.Namhoc;
                    }
                    if (khdt.NguoiCN != null)
                    {
                        gvCapNhat = context.AspNetUsers.Find(khdt.NguoiCN).UserName.ToString();
                    }
                }

                var listChuyenCan = context.tp_DanhgiaHP.Where(s => s.MaKHDT == maKHDT && s.Thanhphan == "Chuyên cần").ToList();
                var listGiuaKy = context.tp_DanhgiaHP.Where(s => s.MaKHDT == maKHDT && s.Thanhphan == "Giữa kỳ").ToList();
                var listCuoiKy = context.tp_DanhgiaHP.Where(s => s.MaKHDT == maKHDT && s.Thanhphan == "Cuối kỳ").ToList();

                int? phanTramChuyenCan = 0;
                int? phanTramGiuaKy = 0;
                int? phanTramCuoiKy = 0;
                string noiDungChuyenCan = "";
                string noiDungGiuaKy = "";
                string noiDungCuoiKy = "";
                if (listChuyenCan != null && listGiuaKy != null && listCuoiKy != null)
                {
                    for (int i = 0; i < listChuyenCan.Count; i++)
                    {
                        phanTramChuyenCan += listChuyenCan[i].Trongso;
                        if (i != listChuyenCan.Count - 1)
                        {
                            noiDungChuyenCan += listChuyenCan[i].Noidung + ", ";
                        }
                        else
                        {
                            noiDungChuyenCan += listChuyenCan[i].Noidung;
                        }
                    }
                    for (int i = 0; i < listGiuaKy.Count; i++)
                    {
                        phanTramGiuaKy += listGiuaKy[i].Trongso;
                        if (i != listGiuaKy.Count - 1)
                        {
                            noiDungGiuaKy += listGiuaKy[i].Noidung + ", ";
                        }
                        else
                        {
                            noiDungGiuaKy += listGiuaKy[i].Noidung;
                        }
                    }
                    for (int i = 0; i < listCuoiKy.Count; i++)
                    {
                        phanTramCuoiKy += listCuoiKy[i].Trongso;
                        if (i != listCuoiKy.Count - 1)
                        {
                            noiDungCuoiKy += listCuoiKy[i].Noidung + ", ";
                        }
                        else
                        {
                            noiDungCuoiKy += listCuoiKy[i].Noidung;
                        }
                    }
                }

                //Replacing <tenthe> by data which got from Database
                doc.Replace("<khoa>", tenKhoa.ToUpper(), false, true);
                doc.Replace("<nganh>", tenHN.ToUpper(), false, true);
                doc.Replace("<tenhocphan>", tenHocPhan.ToUpper(), false, true);
                doc.Replace("<mahocphan>", maHocPhan, false, true);
                doc.Replace("<sotinchi>", soTinChi, false, true);
                doc.Replace("<giolythuyet>", gioLyThuyet, false, true);
                doc.Replace("<giothuctien>", gioThucTien, false, true);
                doc.Replace("<giotuhoc>", gioTuHoc, false, true);
                doc.Replace("<hocphantienquyet>", tenMonTQ, false, true);
                doc.Replace("<hocphanhoctruoc>", tenMonHT, false, true);
                doc.Replace("<hocky>", hocKy, false, true);
                doc.Replace("<nganhhoc>", tenHN, false, true);
                doc.Replace("<tenkhoa>", tenKhoa, false, true);
                doc.Replace("<ngonngu>", ngonNgu, false, true);
                doc.Replace("<muctieuhocphan>", muctieuHP, false, true);
                doc.Replace("<phuongphapgiangday>", phuongPhapGiangDay, false, true);
                doc.Replace("<phuongphaphoctap>", phuongPhapHocTap, false, true);
                doc.Replace("<motanoidunghocphan>", moTaNoiDungHocPhan, false, true);
                doc.Replace("<thangdiem>", thangDiem, false, true);
                doc.Replace("<nhiemvucuasinhvien>", nhiemVuSinhVien, false, true);
                doc.Replace("<loaiphonghoc>", loaiPhongHoc, false, true);
                doc.Replace("<phuongtiengiangday>", phuongTienGD, false, true);
                doc.Replace("<noidungcn>", noiDungCN, false, true);
                doc.Replace("<lanchinhsuadecuong>", lanChinhSuaDC, false, true);
                doc.Replace("<namhoc>", namHoc, false, true);
                doc.Replace("<phantramchuyencan>", phanTramChuyenCan.ToString(), false, true);
                doc.Replace("<chuyencan>", noiDungChuyenCan, false, true);
                doc.Replace("<phantramgiuaky>", phanTramGiuaKy.ToString(), false, true);
                doc.Replace("<giuaky>", noiDungGiuaKy, false, true);
                doc.Replace("<phantramcuoiky>", phanTramCuoiKy.ToString(), false, true);
                doc.Replace("<cuoiky>", noiDungCuoiKy, false, true);
                doc.Replace("<gvcapnhat>", gvCapNhat, false, true);

                //Get time for replace date of creating document
                DateTime homNay = DateTime.Now;
                ngayXuat = homNay.Day.ToString();
                thangXuat = homNay.Month.ToString();
                namXuat = homNay.Year.ToString();
                doc.Replace("<ngayxuat>", ngayXuat, false, true);
                doc.Replace("<thangxuat>", thangXuat, false, true);
                doc.Replace("<namxuat>", namXuat, false, true);

                //Create table
                //Table KetQuaHocTapMongDoi
                addTableKQHTMD(maKHDT);
                //Table MaTranDanhGia
                addTableMTDG(maKHDT);
                //Table MaTranKetQuaHocTapMongDoi
                addTableMaTranKQHTMD(maKHDT);
                //Table ChiTietHocPhan
                addTableChiTietHP(maKHDT);
                //Table Rubric
                addTableRubric(maKHDT);
                //Table Teacher
                addTableTeacher(maKHDT);

                string[] tenHocPhanNotSpace = tenHocPhan.Split(null);
                string tenHocPhanMoi = "";
                for (int i = 0; i < tenHocPhanNotSpace.Length; i++)
                {
                    tenHocPhanMoi += tenHocPhanNotSpace[i];
                }

                System.IO.File.Delete(destFile);

                MemoryStream m = new MemoryStream();
                doc.SaveToStream(m, FileFormat.Docx);
                return File(m.ToArray(), "apllication/msword", tenHocPhanMoi + ".docx");

                //WordDocViewer(khdt.MaKHDT + "-decuonghocphan.docx");
            }
            catch (Exception e)
            {
                return Redirect("~/Error/BadRequest/");
            }
        }

        private void addTableChiTietHP(int maKHDT)
        {
            try
            {
                Section section = doc.Sections[0];
                TextSelection selection = doc.FindString("<chitiethocphan>", true, true);
                TextRange range = selection.GetAsOneRange();
                Paragraph paragraph = range.OwnerParagraph;
                Body body = paragraph.OwnerTextBody;
                int index = body.ChildObjects.IndexOf(paragraph);

                String[] header = { "Tuần", "Buổi", "Nội dung", "KQHTMĐ của Học phần" };

                var listBuoi = context.GetListNoiDung(maKHDT).ToList();

                ArrayList DCSVN = new ArrayList();

                foreach (var buoi in listBuoi)
                {
                    var maND = buoi.MaND;
                    var listHoatDong = context.GetListAction(maKHDT).ToList();
                    string hoatDongTL = "";
                    string hoatDongTN = "";
                    string cdr = "";
                    string danhGia = "";
                    List<string> listDG = new List<string>();
                    foreach (var hoatdong in listHoatDong)
                    {
                        if (hoatdong.Buoi == buoi.Buoi && hoatdong.Tuan == buoi.Tuan && hoatdong.TenNoiDung == buoi.TenNoiDung)
                        {
                            string nguoiThucHien = "";
                            switch (hoatdong.Phanloai.Trim())
                            {
                                case "GV":
                                    {
                                        nguoiThucHien = "Giảng viên";
                                        break;
                                    }
                                case "SV":
                                    {
                                        nguoiThucHien = "Sinh viên";
                                        break;
                                    }
                                case "NSV":
                                    {
                                        nguoiThucHien = "Nhóm sinh viên";
                                        break;
                                    }
                            }
                            if (hoatdong.HDTL == true)
                            {
                                hoatDongTL += "- " + nguoiThucHien + " (" + hoatdong.Sogio + " giờ):\r" + hoatdong.Mota + "\r";
                            }
                            else
                            {
                                hoatDongTN += "- " + nguoiThucHien + " (" + hoatdong.Sogio + " giờ):\r" + hoatdong.Mota + "\r";
                            }
                            var listDGia = context.tp_TacvuHP.Where(s => s.MaCTHP == hoatdong.MaCTHP).Select(s => s.MaTV).ToList();
                            if (listDGia != null)
                            {
                                foreach (var dGia in listDGia)
                                {
                                    var danhG = context.tp_TacvuHP.Find(dGia).tp_DanhgiaHP.Select(s => s.Noidung).ToList();
                                    foreach (var noidung in danhG)
                                    {
                                        listDG.Add(noidung.ToString().Trim());
                                    }
                                }
                            }
                        }
                    }
                    List<string> listDanhGiaUnique = listDG.Distinct().ToList();
                    for (int i = 0; i < listDanhGiaUnique.Count; i++)
                    {
                        danhGia += "- " + listDanhGiaUnique[i] + "\n";
                    }
                    var listCELO = context.tp_NoidungHP.Find(buoi.MaND).tp_CDR_HP.Select(s => s.MaHT).ToList();
                    foreach (var celo in listCELO)
                    {
                        cdr += celo + "\r";
                    }
                    DCSVN.Add(new ChiTietHP(buoi.Tuan, buoi.Buoi, buoi.TenNoiDung, hoatDongTL, hoatDongTN, danhGia.TrimEnd(), cdr));
                }

                Spire.Doc.Table table = section.AddTable(true);
                table.ResetCells(1 + DCSVN.Count * 4, header.Length);

                // ***************** First Row *************************
                TableRow row = table.Rows[0];
                row.IsHeader = true;
                row.Height = 40;    //unit: point, 1point = 0.3528 mm
                row.HeightType = TableRowHeightType.Exactly;
                //row.RowFormat.BackColor = Color.Gray;
                for (int i = 0; i < header.Length; i++)
                {
                    if (i == 2)
                    {
                        row.Cells[i].Width = 900;
                    }
                    else if (i == 0 || i == 1)
                    {
                        row.Cells[i].Width = 10;
                    }
                    else
                    {
                        row.Cells[i].Width = 30;
                    }
                    row.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph p = row.Cells[i].AddParagraph();
                    p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRange = p.AppendText(header[i]);
                    txtRange.CharacterFormat.Bold = true;
                    txtRange.CharacterFormat.FontSize = 12;
                }

                int numberRow = 1;
                foreach (ChiTietHP item in DCSVN)
                {
                    if (item != null)
                    {
                        TableRow dataRowFirst = table.Rows[numberRow];
                        numberRow++;
                        TableRow dataRowSecond = table.Rows[numberRow];
                        numberRow++;
                        TableRow dataRowThird = table.Rows[numberRow];
                        numberRow++;
                        TableRow dataRowFourth = table.Rows[numberRow];
                        numberRow++;
                        //dataRow.Height = 20;
                        dataRowFirst.HeightType = TableRowHeightType.Exactly;
                        dataRowFirst.RowFormat.BackColor = Color.Empty;

                        dataRowFirst.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol1 = dataRowFirst.Cells[0].AddParagraph();
                        pCol1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        TextRange txtRangeCol1 = pCol1.AppendText(item.Tuan.Trim());
                        txtRangeCol1.CharacterFormat.FontSize = 12;
                        dataRowFirst.Cells[0].CellWidthType = CellWidthType.Point;

                        dataRowFirst.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol2 = dataRowFirst.Cells[1].AddParagraph();
                        pCol2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        TextRange txtRangeCol2 = pCol2.AppendText(item.Buoi.Trim());
                        txtRangeCol2.CharacterFormat.FontSize = 12;
                        dataRowFirst.Cells[1].CellWidthType = CellWidthType.Point;

                        dataRowFirst.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol3 = dataRowFirst.Cells[2].AddParagraph();
                        pCol3.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRangeCol3 = pCol3.AppendText(item.TenNoiDung.Trim());
                        txtRangeCol3.CharacterFormat.FontSize = 12;
                        txtRangeCol3.CharacterFormat.Bold = true;
                        dataRowFirst.Cells[2].CellWidthType = CellWidthType.Point;

                        dataRowFirst.Cells[3].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol4 = dataRowFirst.Cells[3].AddParagraph();
                        pCol4.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        TextRange txtRangeCol4 = pCol4.AppendText(item.CDR.Trim());
                        txtRangeCol4.CharacterFormat.FontSize = 12;
                        dataRowFirst.Cells[3].CellWidthType = CellWidthType.Point;

                        dataRowSecond.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol5 = dataRowSecond.Cells[2].AddParagraph();
                        pCol5.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRangeCol5Title = pCol5.AppendText("A. Nội dung giảng dạy trên lớp:\n");
                        txtRangeCol5Title.CharacterFormat.FontSize = 12;
                        txtRangeCol5Title.CharacterFormat.Bold = true;
                        TextRange txtRangeCol5Content = pCol5.AppendText(item.HoatDongTaiLop.Trim());
                        txtRangeCol5Content.CharacterFormat.FontSize = 12;
                        dataRowSecond.Cells[2].CellWidthType = CellWidthType.Point;

                        dataRowThird.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol6 = dataRowThird.Cells[2].AddParagraph();
                        pCol6.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRangeCol6Title = pCol6.AppendText("B. Các nội dung cần tự học tại nhà:\n");
                        txtRangeCol6Title.CharacterFormat.FontSize = 12;
                        txtRangeCol6Title.CharacterFormat.Bold = true;
                        TextRange txtRangeCol6Content = pCol6.AppendText(item.HoatDongTaiNha.Trim());
                        txtRangeCol6Content.CharacterFormat.FontSize = 12;
                        dataRowThird.Cells[2].CellWidthType = CellWidthType.Point;

                        dataRowFourth.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph pCol7 = dataRowFourth.Cells[2].AddParagraph();
                        pCol7.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRangeCol7Title = pCol7.AppendText("C. Đánh giá kết quả học tập:\n");
                        txtRangeCol7Title.CharacterFormat.FontSize = 12;
                        txtRangeCol7Title.CharacterFormat.Bold = true;
                        TextRange txtRangeCol7Meta = pCol7.AppendText("Phương pháp đánh giá:\r");
                        TextRange txtRangeCol7Content = pCol7.AppendText(item.DanhGia.Trim());
                        txtRangeCol7Meta.CharacterFormat.FontSize = 12;
                        txtRangeCol5Content.CharacterFormat.FontSize = 12;
                        dataRowFourth.Cells[2].CellWidthType = CellWidthType.Point;

                        table.ApplyVerticalMerge(0, numberRow - 4, numberRow - 1);
                        table.ApplyVerticalMerge(1, numberRow - 4, numberRow - 1);
                        table.ApplyVerticalMerge(3, numberRow - 4, numberRow - 1);
                    }
                }

                body.ChildObjects.Remove(paragraph);
                body.ChildObjects.Insert(index, table);
            }
            catch (Exception e)
            {
                doc.Replace("<chitiethocphan>", "", false, true);
            }
        }

        private void addTableKQHTMD(int maKHDT)
        {
            try
            {
                Section section = doc.Sections[0];
                TextSelection selection = doc.FindString("<kqhtmd>", true, true);
                TextRange range = selection.GetAsOneRange();
                Paragraph paragraph = range.OwnerParagraph;
                Body body = paragraph.OwnerTextBody;
                int index = body.ChildObjects.IndexOf(paragraph);

                String[] header = { "Ký hiệu", "KQHTMĐ của học phần \r\n Hoàn thành học phần này, sinh viên có thể", "CĐR của CTĐT" };

                ArrayList DCS = new ArrayList();

                var listKQHTMDKienThuc = context.tp_CDR_HP.Where(s => s.MaKHDT == maKHDT && s.Phanloai == "Kiến thức").Select(s => new { s.MaCELO, s.MaHT, s.Mota }).ToList();
                var listKQHTMDKyNang = context.tp_CDR_HP.Where(s => s.MaKHDT == maKHDT && s.Phanloai == "Kỹ năng").Select(s => new { s.MaCELO, s.MaHT, s.Mota }).ToList();
                var listKQHTMDThaiDo = context.tp_CDR_HP.Where(s => s.MaKHDT == maKHDT && s.Phanloai == "Thái độ và phẩm chất đạo đức").Select(s => new { s.MaCELO, s.MaHT, s.Mota }).ToList();

                if (listKQHTMDKienThuc != null)
                {
                    DCS.Add(new StringElement("", "Kiến thức", ""));
                    foreach (var kqhtmd in listKQHTMDKienThuc)
                    {
                        var listElo = context.tp_CDR_HP.Find(kqhtmd.MaCELO).tp_MatranCDR.Select(s => s.MaELO).ToList();
                        string el = "";
                        for (int j = 0; j < listElo.Count; j++)
                        {
                            string tenElo = context.t_CDR_CTDT.Find(listElo[j]).MaHT;
                            if (j != listElo.Count - 1)
                            {
                                el = el + tenElo + "\r\n";
                            }
                            else
                            {
                                el = el + tenElo;
                            }
                        }
                        DCS.Add(new StringElement(kqhtmd.MaHT, kqhtmd.Mota, el));
                    }
                }
                if (listKQHTMDThaiDo != null)
                {
                    DCS.Add(new StringElement("", "Thái độ và phẩm chất đạo đức", ""));
                    foreach (var kqhtmd in listKQHTMDThaiDo)
                    {
                        var listElo = context.tp_CDR_HP.Find(kqhtmd.MaCELO).tp_MatranCDR.Select(s => s.MaELO).ToList();
                        string el = "";
                        for (int j = 0; j < listElo.Count; j++)
                        {
                            string tenElo = context.t_CDR_CTDT.Find(listElo[j]).MaHT;
                            if (j != listElo.Count - 1)
                            {
                                el = el + tenElo + "\r\n";
                            }
                            else
                            {
                                el = el + tenElo;
                            }
                        }
                        DCS.Add(new StringElement(kqhtmd.MaHT, kqhtmd.Mota, el));
                    }
                }
                if (listKQHTMDKyNang != null)
                {
                    DCS.Add(new StringElement("", "Kỹ năng", ""));
                    foreach (var kqhtmd in listKQHTMDKyNang)
                    {
                        var listElo = context.tp_CDR_HP.Find(kqhtmd.MaCELO).tp_MatranCDR.Select(s => s.MaELO).ToList();
                        string el = "";
                        for (int j = 0; j < listElo.Count; j++)
                        {
                            string tenElo = context.t_CDR_CTDT.Find(listElo[j]).MaHT;
                            if (j != listElo.Count - 1)
                            {
                                el = el + tenElo + "\r\n";
                            }
                            else
                            {
                                el = el + tenElo;
                            }
                        }
                        DCS.Add(new StringElement(kqhtmd.MaHT, kqhtmd.Mota, el));
                    }
                }

                Spire.Doc.Table table = section.AddTable(true);
                table.ResetCells(DCS.Count + 1, header.Length);

                // ***************** First Row *************************
                TableRow row = table.Rows[0];
                row.IsHeader = true;
                row.Height = 40;    //unit: point, 1point = 0.3528 mm
                row.HeightType = TableRowHeightType.Exactly;
                //row.RowFormat.BackColor = Color.Gray;
                for (int i = 0; i < header.Length; i++)
                {
                    row.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph p = row.Cells[i].AddParagraph();
                    p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRange = p.AppendText(header[i]);
                    txtRange.CharacterFormat.Bold = true;
                    txtRange.CharacterFormat.FontSize = 10;
                }

                foreach (StringElement item in DCS)
                {
                    int numberOfRow = DCS.IndexOf(item) + 1;
                    TableRow dataRow = table.Rows[numberOfRow];
                    //dataRow.Height = 20;
                    dataRow.HeightType = TableRowHeightType.Exactly;
                    dataRow.RowFormat.BackColor = Color.Empty;

                    dataRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pCol1 = dataRow.Cells[0].AddParagraph();
                    pCol1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRangeCol1 = pCol1.AppendText(item.KyHieu);
                    txtRangeCol1.CharacterFormat.FontSize = 10;
                    table.Rows[numberOfRow].Cells[0].CellWidthType = CellWidthType.Point;
                    table.Rows[numberOfRow].Cells[0].Width = 40;

                    if (item.KetQuaHTMD == "Kỹ năng" || item.KetQuaHTMD == "Thái độ và phẩm chất đạo đức" || item.KetQuaHTMD == "Kiến thức")
                    {
                        dataRow.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph p = dataRow.Cells[1].AddParagraph();
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRange = p.AppendText(item.KetQuaHTMD);
                        txtRange.CharacterFormat.Bold = true;
                        txtRange.CharacterFormat.FontSize = 10;
                    }
                    else
                    {
                        Paragraph p = dataRow.Cells[1].AddParagraph();
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;
                        TextRange txtRange = p.AppendText(item.KetQuaHTMD);
                        table.Rows[numberOfRow].Cells[1].CellWidthType = CellWidthType.Point;
                        table.Rows[numberOfRow].Cells[1].Width = 590;
                        //dataRow.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        table.Rows[numberOfRow].Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    }

                    dataRow.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pCol2 = dataRow.Cells[2].AddParagraph();
                    pCol2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRangeCol2 = pCol2.AppendText(item.ChuanDR);
                    txtRangeCol2.CharacterFormat.FontSize = 10;
                    table.Rows[numberOfRow].Cells[2].CellWidthType = CellWidthType.Point;
                    table.Rows[numberOfRow].Cells[2].Width = 70;
                }

                body.ChildObjects.Remove(paragraph);
                body.ChildObjects.Insert(index, table);
            }
            catch (Exception e)
            {
                doc.Replace("<kqhtmd>", "", false, true);
            }
        }

        private void addTableMTDG(int maKHDT)
        {
            try
            {
                Section section = doc.Sections[0];
                TextSelection selection = doc.FindString("<matrandg>", true, true);
                TextRange range = selection.GetAsOneRange();
                Paragraph paragraph = range.OwnerParagraph;
                Body body = paragraph.OwnerTextBody;
                int index = body.ChildObjects.IndexOf(paragraph);

                //int maCTDT = context.tp_KHDaotao.Find(maKHDT).MaCTDT;

                int maCTDT = context.tp_KHDaotao.Find(maKHDT).MaCTDT;
                int? maThamChieu = context.t_CTDaotao.Find(maCTDT).MaTC;

                var listElo = context.t_CDR_CTDT.Where(s => s.MaCTDT == maThamChieu).Select(s => s.MaHT).ToList();

                listElo.Sort();

                string header = "CĐR của CTĐT";

                Spire.Doc.Table table = section.AddTable(true);
                table.ResetCells(3, 1 + listElo.Count);

                // ***************** First Row *************************
                TableRow row = table.Rows[0];
                row.IsHeader = true;
                row.Height = 25;    //unit: point, 1point = 0.3528 mm
                row.HeightType = TableRowHeightType.Exactly;
                row.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p = row.Cells[0].AddParagraph();
                p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                TextRange txtRange = p.AppendText(header);
                txtRange.CharacterFormat.FontSize = 10;

                for (int i = 1; i <= listElo.Count; i++)
                {
                    row.Cells[1].AddParagraph().AppendText("");
                }
                table.ApplyHorizontalMerge(0, 0, listElo.Count);

                int numberOfRow = 1;
                TableRow dataRow = table.Rows[numberOfRow];
                dataRow.Height = 25;
                dataRow.HeightType = TableRowHeightType.Exactly;
                dataRow.RowFormat.BackColor = Color.Empty;

                for (int i = 0; i < listElo.Count + 1; i++)
                {
                    dataRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pCol1 = dataRow.Cells[i].AddParagraph();
                    pCol1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRangeCol1;
                    if (i == 0)
                    {

                        txtRangeCol1 = pCol1.AppendText("Mã HP");
                    }
                    else
                    {
                        txtRangeCol1 = pCol1.AppendText(listElo[i - 1]);
                    }
                    txtRangeCol1.CharacterFormat.FontSize = 10;
                    table.Rows[numberOfRow].Cells[i].CellWidthType = CellWidthType.Point;
                    table.Rows[numberOfRow].Cells[i].Width = 80;
                }

                TableRow dataRow3 = table.Rows[2];
                dataRow3.Height = 25;
                dataRow3.HeightType = TableRowHeightType.Exactly;
                dataRow3.RowFormat.BackColor = Color.Empty;

                for (int i = 0; i < listElo.Count + 1; i++)
                {
                    dataRow3.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    Paragraph pCol1 = dataRow3.Cells[i].AddParagraph();
                    pCol1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TextRange txtRangeCol1 = pCol1.AppendText("");
                    txtRangeCol1.CharacterFormat.FontSize = 10;
                    table.Rows[numberOfRow].Cells[i].CellWidthType = CellWidthType.Point;
                    table.Rows[numberOfRow].Cells[i].Width = 80;
                }

                body.ChildObjects.Remove(paragraph);
                body.ChildObjects.Insert(index, table);
            }
            catch (Exception e)
            {
                doc.Replace("<matrandg>", "", false, true);
            }
        }

        private void addTableMaTranKQHTMD(int maKHDT)
        {
            try
            {
                Section section = doc.Sections[0];
                TextSelection selection = doc.FindString("<matrankqhtmd>", true, true);
                TextRange range = selection.GetAsOneRange();
                Paragraph paragraph = range.OwnerParagraph;
                Body body = paragraph.OwnerTextBody;
                int index = body.ChildObjects.IndexOf(paragraph);

                var listDG = context.tp_DanhgiaHP.Where(s => s.MaKHDT == maKHDT).ToList();
                List<List<String>> MaTranDG = new ArrayList<List<String>>();

                List<String> row1 = new List<String>();
                row1.Add("CELOs");
                row1.Add("PHƯƠNG PHÁP ĐÁNH GIÁ");
                for (int i = 0; i < listDG.Count - 1; i++)
                {
                    row1.Add("");
                }
                row1.Add("CÔNG CỤ ĐÁNH GIÁ");
                row1.Add("THỜI ĐIỂM ĐÁNH GIÁ");
                List<String> row2 = new List<String>();
                for (int i = 0; i < listDG.Count + 3; i++)
                {

                    if (i != listDG.Count - 1)
                    {
                        row2.Add("");
                    }
                    else if (i != listDG.Count - 2)
                    {
                        row2.Add("");
                    }
                    else if (i == 0)
                    {
                        row2.Add("");
                    }
                    else
                    {
                        row2.Add("");
                    }
                }
                for (int i = 0; i < listDG.Count; i++)
                {
                    row2[i + 1] = listDG[i].Noidung + " (" + listDG[i].Trongso + "%)";
                }
                MaTranDG.Add(row1);
                MaTranDG.Add(row2);

                var listCDR = context.tp_CDR_HP.Where(s => s.MaKHDT == maKHDT).ToList();

                Spire.Doc.Table table = section.AddTable(true);
                table.ResetCells(2 + listCDR.Count, 3 + listDG.Count);

                // ***************** First Row *************************
                for (int r = 0; r < MaTranDG.Count; r++)
                {
                    TableRow row = table.Rows[r];
                    //row.IsHeader = true;
                    row.HeightType = TableRowHeightType.Exactly;
                    //row.RowFormat.BackColor = Color.Gray;
                    for (int c = 0; c < MaTranDG[r].Count; c++)
                    {
                        row.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        Paragraph p = row.Cells[c].AddParagraph();
                        p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                        TextRange txtRange = p.AppendText(MaTranDG[r][c]);
                        txtRange.CharacterFormat.Bold = true;
                        txtRange.CharacterFormat.FontSize = 10;
                        if (r == 1)
                        {
                            txtRange.CharacterFormat.Bold = false;
                        }
                    }
                }

                table.ApplyVerticalMerge(0, 0, 1);
                table.ApplyHorizontalMerge(0, 1, listDG.Count);
                table.ApplyVerticalMerge(1 + listDG.Count, 0, 1);
                table.ApplyVerticalMerge(1 + listDG.Count + 1, 0, 1);
                table.ApplyHorizontalMerge(0, 1, 3);

                for (int i = 0; i < listCDR.Count; i++)
                {
                    List<String> row = new List<String>();
                    row.Add(listCDR[i].MaHT);
                    var listTPDG = context.tp_CDR_HP.Find(listCDR[i].MaCELO).tp_DanhgiaHP.Select(s => s.MaDG).ToList();
                    for (int j = 0; j < listDG.Count; j++)
                    {
                        if (checkExists(listDG[j].MaDG, listTPDG))
                        {
                            row.Add("X");
                        }
                        else
                        {
                            row.Add("");
                        }
                    }
                    if (listCDR[i].CongCuDanhGia != null)
                    {
                        row.Add(listCDR[i].CongCuDanhGia.ToString());
                    }
                    else
                    {
                        row.Add("");
                    }
                    if (listCDR[i].ThoiDiemDanhGia != null)
                    {
                        row.Add(listCDR[i].ThoiDiemDanhGia.ToString());
                    }
                    else
                    {
                        row.Add("");
                    }
                    MaTranDG.Add(row);
                }

                // ***************** Body Table *************************
                for (int r = 2; r < MaTranDG.Count; r++)
                {
                    TableRow row = table.Rows[r];
                    //row.IsHeader = true;
                    //row.Height = 40;    //unit: point, 1point = 0.3528 mm
                    row.HeightType = TableRowHeightType.Exactly;
                    for (int c = 0; c < MaTranDG[r].Count; c++)
                    {
                        if (c == 0)
                        {
                            row.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph p = row.Cells[c].AddParagraph();
                            p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRange = p.AppendText(MaTranDG[r][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 10;
                        }
                        else
                        {
                            row.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph p = row.Cells[c].AddParagraph();
                            p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRange = p.AppendText(MaTranDG[r][c]);
                            txtRange.CharacterFormat.Bold = false;
                            txtRange.CharacterFormat.FontSize = 10;
                        }

                    }
                }

                body.ChildObjects.Remove(paragraph);
                body.ChildObjects.Insert(index, table);
            }
            catch (Exception e)
            {
                doc.Replace("<matrankqhtmd>", "", false, true);
            }
        }

        /// <summary>
        /// Purpose: Tạo table rubric file word
        /// Developer: Nguyen Nguyen
        /// 
        /// Date: 9/4/2019 
        /// </summary>
        /// <param name=""></param> 
        /// <returns></returns>
        private void addTableRubric(int maKHDT)
        {
            try
            {
                //Get List DanhGia which have data
                var listDGia = context.tp_DanhgiaHP
                    .Where(s => s.MaKHDT == maKHDT)
                    .Join(context.tp_TieuchiDG, s => s.MaDG, t => t.MaDG, (s, t) => new { MaDG = t.MaDG, NoiDung = s.Noidung })
                    .Distinct()
                    .Select(d => new { MaDG = d.MaDG, NoiDung = d.NoiDung }).ToList();

                if (listDGia != null)
                {
                    doc.Replace("<phuluc2>", "PHỤ LỤC 2", false, true);
                    doc.Replace("<rubricdanhgia>", "RUBRIC ĐÁNH GIÁ", false, true);

                    //Count loop
                    int co = 1;

                    //Get location of tag
                    Section section = doc.Sections[0];
                    TextSelection selection = doc.FindString("<rubric>", false, true);
                    TextRange range = selection.GetAsOneRange();
                    Paragraph paragraph = range.OwnerParagraph;
                    Body body = paragraph.OwnerTextBody;
                    int index = body.ChildObjects.IndexOf(paragraph);

                    for (int i = listDGia.Count - 1; i >= 0; i--)
                    {
                        //Storing data in Paragraph - Rubric Title
                        List<Paragraph> listPara = new List<Paragraph>();
                        Paragraph rubricTitle = new Paragraph(doc);
                        TextRange rubricBold = rubricTitle.AppendText("Rubric " + (i + 1) + ": ");
                        rubricBold.CharacterFormat.Bold = true;
                        rubricTitle.AppendText(listDGia[i].NoiDung.ToString().Trim());
                        listPara.Add(rubricTitle);

                        //Getting the evaluation component according to MaDanhGia and MaKeHoachDaoTao
                        var listTP = context.pr_GetGroupTP(maKHDT, listDGia[i].MaDG).ToList();

                        //Handling data for table header
                        List<String> row1 = new List<String>();
                        row1.Add("Tiêu chí");
                        row1.Add("");
                        row1.Add("Trọng số" + "\r" + "(%)");
                        for (int k = 0; k < listTP.Count; k++)
                        {
                            row1.Add(listTP[k].Mucdo3 + "\r" + listTP[k].Trongso3 + "%");
                        }

                        //Handling data for table body
                        //Each row in Rubric is a row in table
                        ArrayList Rubric = new ArrayList();
                        int totalRow = 0;
                        //Group By Level 1, list all level 1 according to MaDanhGia
                        var listLV1 = context.pr_GetGroupLevel(maKHDT, listDGia[i].MaDG)
                            .GroupBy(s => new { s.MaTC, s.Mota })
                            .Select(s => new { MaTC = s.Key.MaTC, Mota = s.Key.Mota })
                            .ToList();

                        foreach (var level1 in listLV1)
                        {
                            //List TieuChi according to Level1 
                            var listLV2 = context.pr_GetGroupLevel(maKHDT, listDGia[i].MaDG)
                                .Where(s => s.MaTC == level1.MaTC)
                                .GroupBy(s => new { s.Mota2, s.Trongso2, s.MATC2 })
                                .Select(s => new { MoTa = s.Key.Mota2, TrongSo = s.Key.Trongso2, MaTC = s.Key.MATC2 })
                                .ToList();
                            List<List<String>> listTieuChi = new ArrayList<List<String>>();
                            //Adding data to Level 2
                            foreach (var level2 in listLV2)
                            {
                                List<String> thongTinTC = new List<String>();
                                thongTinTC.Add(level2.MoTa);
                                thongTinTC.Add(level2.TrongSo.ToString());
                                thongTinTC.Add(level2.MaTC.ToString());
                                listTieuChi.Add(thongTinTC);
                                totalRow = totalRow + 1;
                            }
                            //Adding all data to Object is inheritted from ThanhphanDanhGia - This is a row in table
                            Rubric.Add(new ThanhPhanDanhGia(level1.MaTC, level1.Mota, listTieuChi));
                        }

                        Spire.Doc.Table table = section.AddTable(true);
                        table.ResetCells(1 + totalRow, 3 + listTP.Count);

                        // ***************** First Row *************************
                        //Handling Table Header
                        TableRow row = table.Rows[0];
                        row.IsHeader = true;
                        row.Height = 40;    //unit: point, 1point = 0.3528 mm
                        row.HeightType = TableRowHeightType.Exactly;
                        //row.RowFormat.BackColor = Color.Gray;
                        for (int c = 0; c < row1.Count; c++)
                        {
                            row.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph p = row.Cells[c].AddParagraph();
                            p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRange = p.AppendText(row1[c].ToString());
                            txtRange.CharacterFormat.Bold = true;
                            txtRange.CharacterFormat.FontSize = 12;
                        }

                        table.ApplyHorizontalMerge(0, 0, 1);

                        //// ***************** Body Table *************************
                        //Handling Table Body
                        int numberOfRow = 1;
                        foreach (ThanhPhanDanhGia tpdg in Rubric)
                        {
                            int startNumber = numberOfRow;
                            TableRow rowT = table.Rows[numberOfRow];

                            //Column 1 - Name of Level 1
                            rowT.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph p = rowT.Cells[0].AddParagraph();
                            p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRange = p.AppendText(tpdg.TenDanhGia);
                            txtRange.CharacterFormat.Bold = true;
                            txtRange.CharacterFormat.FontSize = 12;

                            //Column 2 - Name of Level 2
                            rowT.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph pFirst = rowT.Cells[1].AddParagraph();
                            pFirst.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRangeFirst = pFirst.AppendText(tpdg.TieuChi[0][0]);
                            txtRangeFirst.CharacterFormat.Bold = false;
                            txtRangeFirst.CharacterFormat.FontSize = 12;

                            //Column 3 - Percent of Level 2
                            rowT.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            Paragraph pSecond = rowT.Cells[2].AddParagraph();
                            pSecond.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                            TextRange txtRangeSecond = pSecond.AppendText(tpdg.TieuChi[0][1]);
                            txtRangeSecond.CharacterFormat.Bold = false;
                            txtRangeSecond.CharacterFormat.FontSize = 12;

                            //Mapping with evaluation component in header in line one
                            var listMapTP = context.pr_GetGroupLevel(maKHDT, listDGia[i].MaDG)
                                .Where(s => s.MaTC == tpdg.MaDanhGia && s.MATC2 == int.Parse(tpdg.TieuChi[0][2]))
                                .Select(s => new { MucDo3 = s.Mucdo3, TrongSo3 = s.Trongso3, Mota3 = s.Mota3 })
                                .ToList();
                            for (int l = 0; l < listTP.Count; l++)
                            {
                                for (int j = 0; j < listMapTP.Count; j++)
                                {
                                    //Comparing row and column for mapping and insert description of Level 3
                                    if (listMapTP[j].MucDo3 == listTP[l].Mucdo3 && listMapTP[j].TrongSo3 == listTP[l].Trongso3)
                                    {
                                        rowT.Cells[l + 3].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                        Paragraph pThird = rowT.Cells[l + 3].AddParagraph();
                                        pThird.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                                        TextRange txtRangeThird = pThird.AppendText(listMapTP[j].Mota3);
                                        txtRangeThird.CharacterFormat.Bold = false;
                                        txtRangeThird.CharacterFormat.FontSize = 12;
                                    }
                                }
                            }

                            numberOfRow++;

                            //Handling from row 2
                            for (int e = 1; e < tpdg.TieuChi.Count; e++)
                            {
                                //Handling from column 2 in row 2
                                TableRow rowS = table.Rows[numberOfRow];
                                rowS.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                Paragraph pA = rowS.Cells[1].AddParagraph();
                                pA.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                                TextRange txtRangeA = pA.AppendText(tpdg.TieuChi[e][0]);
                                txtRangeA.CharacterFormat.Bold = false;
                                txtRangeA.CharacterFormat.FontSize = 12;

                                //Handling from column 3 in row 2
                                rowS.Cells[2].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                Paragraph pB = rowS.Cells[2].AddParagraph();
                                pB.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                                TextRange txtRangeB = pB.AppendText(tpdg.TieuChi[e][1]);
                                txtRangeB.CharacterFormat.Bold = false;
                                txtRangeB.CharacterFormat.FontSize = 12;

                                numberOfRow++;

                                //Mapping with evaluation component
                                var listMapTPRow = context.pr_GetGroupLevel(maKHDT, listDGia[i].MaDG)
                                .Where(s => s.MaTC == tpdg.MaDanhGia && s.MATC2 == int.Parse(tpdg.TieuChi[e][2]))
                                .Select(s => new { MucDo3 = s.Mucdo3, TrongSo3 = s.Trongso3, Mota3 = s.Mota3 })
                                .ToList();
                                for (int k = 0; k < listTP.Count; k++)
                                {
                                    for (int j = 0; j < listMapTPRow.Count; j++)
                                    {
                                        if (listMapTPRow[j].MucDo3 == listTP[k].Mucdo3 && listMapTPRow[j].TrongSo3 == listTP[k].Trongso3)
                                        {
                                            rowS.Cells[k + 3].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                            Paragraph pFourth = rowS.Cells[k + 3].AddParagraph();
                                            pFourth.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                                            TextRange txtRangeFourth = pFourth.AppendText(listMapTPRow[j].Mota3);
                                            txtRangeFourth.CharacterFormat.Bold = false;
                                            txtRangeFourth.CharacterFormat.FontSize = 12;
                                        }
                                    }
                                }
                            }
                            int endNumber = numberOfRow;

                            table.ApplyVerticalMerge(0, startNumber, endNumber - 1);
                        }
                        if (co == 1)
                        {
                            body.ChildObjects.Remove(paragraph);
                        }
                        Paragraph lineBreak = new Paragraph(doc);
                        lineBreak.AppendBreak(BreakType.LineBreak);

                        body.ChildObjects.Insert(index, lineBreak);
                        body.ChildObjects.Insert(index, table);
                        body.ChildObjects.Insert(index, rubricTitle);
                        co++;
                    }
                }
                else
                {
                    doc.Replace("<rubric>", "", false, true);
                    doc.Replace("<phuluc2>", "", false, true);
                    doc.Replace("<rubricdanhgia>", "", false, true);
                }
            }
            catch (Exception e)
            {
                doc.Replace("<rubric>", "", false, true);
                doc.Replace("<phuluc2>", "", false, true);
                doc.Replace("<rubricdanhgia>", "", false, true);
            }
        }

        private void addTableTeacher(int maKHDT)
        {
            try
            {
                var listGV = context.GetRoleGVHocPhan(maKHDT).ToList();

                //Storing list Teacher by type "Giang vien thinh giang" or "Giang vien co huu"
                ArrayList GVCH = new ArrayList();
                ArrayList GVTG = new ArrayList();

                //Add teacher information into List upon
                foreach (var gv in listGV)
                {
                    if (gv.LoaiGV == "0")
                    {
                        GVCH.Add(new GiangVien(gv.UserName, gv.SDT, gv.Thongtinhocvi, gv.LoaiGV, gv.Diachi, gv.Ngaysinh, gv.Thongtinhocham, gv.Email, "", ""));
                    }
                    else if (gv.LoaiGV == "1")
                    {
                        GVTG.Add(new GiangVien(gv.UserName, gv.SDT, gv.Thongtinhocvi, gv.LoaiGV, gv.Diachi, gv.Ngaysinh, gv.Thongtinhocham, gv.Email, "", ""));
                    }
                }

                //Creating table by type
                if (GVCH.Count > 0)
                {
                    addTableTeacherByType("<giangviencohuu>", GVCH);
                }
                else
                {
                    doc.Replace("<giangviencohuu>", "", false, true);
                }
                if (GVTG.Count > 0)
                {
                    addTableTeacherByType("<giangvienthinhgiang>", GVTG);
                }
                else
                {
                    doc.Replace("<giangvienthinhgiang>", "", false, true);
                }
            }
            catch (Exception e)
            {
                doc.Replace("<giangviencohuu>", "", false, true);
                doc.Replace("<giangvienthinhgiang>", "", false, true);

            }
        }

        private void addTableTeacherByType(string tagWord, ArrayList listGV)
        {
            //Count loop
            int co = 1;

            //Get location of tag
            Section section = doc.Sections[0];
            TextSelection selection = doc.FindString(tagWord, false, true);
            TextRange range = selection.GetAsOneRange();
            Paragraph paragraph = range.OwnerParagraph;
            Body body = paragraph.OwnerTextBody;
            int index = body.ChildObjects.IndexOf(paragraph);

            foreach (GiangVien gv in listGV)
            {
                Spire.Doc.Table table = section.AddTable(true);
                table.ResetCells(4, 2);

                //// ***************** Body Table *************************
                //Row 1
                TableRow row1 = table.Rows[0];
                row1.Height = 30;    //unit: point, 1point = 0.3528 mm
                row1.HeightType = TableRowHeightType.Exactly;
                //Column 1
                row1.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p1 = row1.Cells[0].AddParagraph();
                p1.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange1 = p1.AppendText("Họ và tên: ");
                txtRange1.CharacterFormat.FontSize = 12;
                p1.AppendText(gv.HoTen);
                //Column 2
                row1.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p2 = row1.Cells[1].AddParagraph();
                p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange2 = p2.AppendText("Học hàm, học vị: ");
                txtRange2.CharacterFormat.FontSize = 12;
                p2.AppendText(gv.HocHam + ", " + gv.HocVi);

                //Row 2
                TableRow row2 = table.Rows[1];
                row2.Height = 30;    //unit: point, 1point = 0.3528 mm
                row2.HeightType = TableRowHeightType.Exactly;
                //Column 1
                row2.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p3 = row2.Cells[0].AddParagraph();
                p3.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange3 = p3.AppendText("Địa chỉ cơ quan: ");
                txtRange3.CharacterFormat.FontSize = 12;
                p3.AppendText(VI.info_SchoolAddress);
                //Column 2
                row2.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p4 = row2.Cells[1].AddParagraph();
                p4.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange4 = p4.AppendText("Điện thoại liên hệ: ");
                txtRange4.CharacterFormat.FontSize = 12;
                p4.AppendText(gv.SoDienThoai);

                //Row 3
                TableRow row3 = table.Rows[2];
                row3.Height = 30;    //unit: point, 1point = 0.3528 mm
                row3.HeightType = TableRowHeightType.Exactly;
                //Column 1
                row3.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p5 = row3.Cells[0].AddParagraph();
                p5.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange5 = p5.AppendText("Email: ");
                txtRange5.CharacterFormat.FontSize = 12;
                p5.AppendText(gv.Email);
                //Column 2
                row3.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p6 = row3.Cells[1].AddParagraph();
                p6.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Left;
                TextRange txtRange6 = p6.AppendText("Trang web: ");
                txtRange6.CharacterFormat.FontSize = 12;
                p6.AppendText(gv.Website);

                //Row 4
                TableRow row4 = table.Rows[3];
                row4.Height = 40;    //unit: point, 1point = 0.3528 mm
                row4.HeightType = TableRowHeightType.Exactly;
                //Column 1
                row4.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                Paragraph p7 = row4.Cells[0].AddParagraph();
                p7.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                TextRange txtRange7 = p7.AppendText("Cách liên lạc với Giảng viên: ");
                txtRange7.CharacterFormat.FontSize = 12;
                p7.AppendText(gv.CachLienLac);

                table.ApplyHorizontalMerge(3, 0, 1);

                if (co == 1)
                {
                    body.ChildObjects.Remove(paragraph);
                }
                Paragraph lineBreak = new Paragraph(doc);
                lineBreak.AppendBreak(BreakType.LineBreak);

                body.ChildObjects.Insert(index, lineBreak);
                body.ChildObjects.Insert(index, table);
                co++;
            }
        }

        private void WordDocViewer(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch { }
        }

        /// <summary>
        /// Purpose: Replace  
        /// Developer: Nguyen Nguyen
        /// Date: 9/4/2019 
        /// </summary>
        /// <param name=""></param> 
        /// <returns></returns>
        private void Replace(TextRangeLocation location, IList<Paragraph> replacement)
        {
            //will be replaced
            TextRange textRange = location.Text;

            //textRange index
            int index = location.Index;

            //owener paragraph
            Paragraph paragraph = location.Owner;

            //owner text body
            Body sectionBody = paragraph.OwnerTextBody;

            //get the index of paragraph in section
            int paragraphIndex = sectionBody.ChildObjects.IndexOf(paragraph);

            int replacementIndex = -1;
            if (index == 0)
            {
                //remove
                paragraph.ChildObjects.RemoveAt(0);

                replacementIndex = sectionBody.ChildObjects.IndexOf(paragraph);
            }
            else if (index == paragraph.ChildObjects.Count - 1)
            {
                paragraph.ChildObjects.RemoveAt(index);
                replacementIndex = paragraphIndex + 1;
            }
            else
            {

                //split owner paragraph
                Paragraph paragraph1 = (Paragraph)paragraph.Clone();
                while (paragraph.ChildObjects.Count > index)
                {
                    paragraph.ChildObjects.RemoveAt(index);
                }
                for (int i = 0, count = index + 1; i < count; i++)
                {
                    paragraph1.ChildObjects.RemoveAt(0);
                }
                sectionBody.ChildObjects.Insert(paragraphIndex + 1, paragraph1);

                replacementIndex = paragraphIndex + 1;
            }

            //insert replacement
            for (int i = 0; i < replacement.Count; i++)
            {
                sectionBody.ChildObjects.Insert(replacementIndex + i, replacement[i].Clone());
            }
        }

        private class ArrayList<T> : List<List<string>>
        {
        }

        private bool checkExists(int item, List<int> listCheck)
        {
            bool flag = false;
            for (int i = 0; i < listCheck.Count; i++)
            {
                if (listCheck[i] == item)
                {
                    flag = true;
                }
            }
            return flag;
        }
    }

    //Get Range Location
    public class TextRangeLocation : IComparable<TextRangeLocation>
    {
        public TextRangeLocation()
        {
        }

        public TextRangeLocation(TextRange text)
        {
            this.Text = text;
        }

        public TextRange Text { get; set; }

        public Paragraph Owner
        {
            get
            {
                return this.Text.OwnerParagraph;
            }
        }

        public int Index
        {
            get
            {
                return this.Owner.ChildObjects.IndexOf(this.Text);
            }
        }

        public int CompareTo(TextRangeLocation other)
        {
            return -(this.Index - other.Index);
        }
    }

    //Storing data in table ChuanDauRa
    public class StringElement
    {
        private string kyHieu;
        private string ketQuaHTMD;
        private string chuanDR;
        public StringElement(string kyHieu, string ketQuaHTMD, string chuanDR)
        {
            this.kyHieu = kyHieu;
            this.ketQuaHTMD = ketQuaHTMD;
            this.chuanDR = chuanDR;
        }
        public string KyHieu
        {
            get { return kyHieu; }
            set { kyHieu = value; }
        }
        public string KetQuaHTMD
        {
            get { return ketQuaHTMD; }
            set { ketQuaHTMD = value; }
        }
        public string ChuanDR
        {
            get { return chuanDR; }
            set { chuanDR = value; }
        }
    }

    //Storing data in table ChiTietHocPhan
    public class ChiTietHP
    {
        private string tuan;
        private string buoi;
        private string tenNoiDung;
        private string hoatDongTaiLop;
        private string hoatDongTaiNha;
        private string danhGia;
        private string cdr;
        public ChiTietHP(string tuan, string buoi, string tenNoiDung, string hoatDongTaiLop, string hoatDongTaiNha, string danhGia, string cdr)
        {
            this.tuan = tuan;
            this.buoi = buoi;
            this.tenNoiDung = tenNoiDung;
            this.hoatDongTaiLop = hoatDongTaiLop;
            this.hoatDongTaiNha = hoatDongTaiNha;
            this.danhGia = danhGia;
            this.cdr = cdr;
        }
        public string Tuan
        {
            get { return this.tuan; }
            set { this.tuan = value; }
        }
        public string Buoi
        {
            get { return this.buoi; }
            set { this.buoi = value; }
        }
        public string TenNoiDung
        {
            get { return this.tenNoiDung; }
            set { this.tenNoiDung = value; }
        }
        public string HoatDongTaiLop
        {
            get { return this.hoatDongTaiLop; }
            set { this.hoatDongTaiLop = value; }
        }
        public string HoatDongTaiNha
        {
            get { return this.hoatDongTaiNha; }
            set { this.hoatDongTaiNha = value; }
        }
        public string DanhGia
        {
            get { return this.danhGia; }
            set { this.danhGia = value; }
        }
        public string CDR
        {
            get { return this.cdr; }
            set { this.cdr = value; }
        }
    }

    //Storing data in table Rubric
    public class ThanhPhanDanhGia
    {
        private int? maDanhGia;
        private string tenDanhGia;
        private List<List<String>> tieuChi;

        public ThanhPhanDanhGia(int? maDG, string tenDG, List<List<string>> tieuChi)
        {
            this.maDanhGia = maDG;
            this.tenDanhGia = tenDG;
            this.tieuChi = tieuChi;
        }

        public List<List<String>> TieuChi
        {
            get { return this.tieuChi; }
            set { this.tieuChi = value; }
        }
        public int? MaDanhGia
        {
            get { return this.maDanhGia; }
            set { this.maDanhGia = value; }
        }
        public string TenDanhGia
        {
            get { return this.tenDanhGia; }
            set { this.tenDanhGia = value; }
        }
    }

    public class GiangVien
    {
        private string hoTen;
        private string soDienThoai;
        private DateTime? ngaySinh;
        private string hocVi;
        private string loaiGV;
        private string diaChi;
        private string hocHam;
        private string email;
        private string cachLienLac;
        private string website;
        public GiangVien(string hoTen, string soDienThoai, string hocVi, string loaiGV, string diaChi, DateTime? ngaySinh, string hocHam, string email, string cachLienLac, string website)
        {
            this.hoTen = hoTen;
            this.soDienThoai = soDienThoai;
            this.hocHam = hocHam;
            this.email = email;
            this.cachLienLac = cachLienLac;
            this.website = website;
            this.ngaySinh = ngaySinh;
            this.hocVi = hocVi;
            this.diaChi = diaChi;
            this.loaiGV = loaiGV;
        }
        public string HoTen
        {
            get { return this.hoTen; }
            set { this.hoTen = value; }
        }
        public string SoDienThoai
        {
            get { return this.soDienThoai; }
            set { this.soDienThoai = value; }
        }
        public string HocHam
        {
            get { return this.hocHam; }
            set { this.hocHam = value; }
        }
        public string CachLienLac
        {
            get { return this.cachLienLac; }
            set { this.cachLienLac = value; }
        }
        public string Website
        {
            get { return this.website; }
            set { this.website = value; }
        }
        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }
        public DateTime? NgaySinh
        {
            get { return this.ngaySinh; }
            set { this.ngaySinh = value; }
        }
        public string HocVi
        {
            get { return this.hocVi; }
            set { this.hocVi = value; }
        }
        public string DiaChi
        {
            get { return this.diaChi; }
            set { this.diaChi = value; }
        }
        public string LoaiGV
        {
            get { return this.loaiGV; }
            set { this.loaiGV = value; }
        }
    }
}