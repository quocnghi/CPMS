using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using CommonRS.Resources;

namespace Capstone.Areas.PMS.Controllers
{
    public class MailController : Controller
    {
        fit_misDBEntities context = new fit_misDBEntities();

        // GET: Mail
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult HopThuDen()
        {
            return View();
        }
        // GET: Mailbox for GiangVien
        [Authorize(Roles = UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult HopThuDenGiangVien()
        {
            return View();
        }

        // <summary>
        /// Purpose: View xem chi tiết thông báo
        /// Developer: Trần An Bình
        /// Date:
        /// </summary>
        /// <param name="id">Mã thông báo</param>
        /// <returns>Trả về view chi tiết thông báo tuỳ theo kiểu của thông báo</returns>
        // GET: Detail invitation for Giangvien
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public ActionResult ChiTietThongBao(string id)
        {
            int maNO = int.Parse(id);
            var notifi = context.sf_Notification.Find(maNO);
            notifi.DaXem = true;
            context.SaveChanges();

            var nguoiGui = context.AspNetUsers.Where(x => x.Email == notifi.NguoiGui).FirstOrDefault();

            //Lưu kiểu notifi vào viewBag để qua view check
            ViewBag.kieuNotification = notifi.Kieu;
            ViewBag.thoiGianGui = notifi.Ngaytao;
            ViewBag.ThongTin = notifi.Thongtin;
            ViewBag.EmailNguoiGui = nguoiGui.Email;
            ViewBag.nguoiGui = nguoiGui.UserName;
            ViewBag.maNotifi = notifi.MaTB;
            ViewBag.chude = notifi.Chude;

            //Khi kiểu notification là Lời mời giảng dạy
            //Trạng thái sẽ vẫn là Chưa trả lời
            if (notifi.Kieu == @"Lời mời giảng dạy")
            {
                //Lấy ra MaQH là các mã KHDT
                string[] lstMaKHDTstr = notifi.MaQH.Split(',');
                List<ModelMonHocAcceptDeny> lstResult = new List<ModelMonHocAcceptDeny>();
                string maUserHienTai = User.Identity.GetUserId();

                for (int i = 0; i < lstMaKHDTstr.Count() - 1; i++)
                {
                    int maKHDT = int.Parse(lstMaKHDTstr[i]);
                    var DecuongGV = context.tp_DecuongGV.Where(x => x.MaKHDT == maKHDT).Where(x => x.NguoiCN == maUserHienTai)
                        .Join(context.tp_KHDaotao, c => c.MaKHDT, d => d.MaKHDT, (c, d) => new { d.TenHP, d.NguoiCN, c.MaQL, d.SoTC, d.GioTH, d.GioLT, d.GioDa, d.GioTT, d.MaCTDT, d.MaHP })
                        .Join(context.t_CTDaotao, c => c.MaCTDT, d => d.MaCT, (c, d) => new { c.TenHP, c.NguoiCN, c.MaQL, c.SoTC, c.GioTH, c.GioLT, c.GioDa, c.GioTT, c.MaCTDT, c.MaHP, d.MaKhoi }).FirstOrDefault();
                    //.Join(context.sc_Hocphan, c => c.MaHP, d => d.MaHP, (c, d) => new { c.NguoiCN,c.MaQL, c.SoTC, c.GioTH, c.GioLT, c.GioDa, c.GioTT, c.MaCTDT, c.MaHP, c.MaKhoi, d.TenMH }).FirstOrDefault();

                    //Tạo một thành phần vào list gửi về
                    ModelMonHocAcceptDeny monHocAcceptDeny = new ModelMonHocAcceptDeny();
                    monHocAcceptDeny.TenKhoa = context.sc_Khoilop.Find(DecuongGV.MaKhoi).TenKhoi;
                    monHocAcceptDeny.TenMonHoc = DecuongGV.TenHP;
                    monHocAcceptDeny.SoTC = DecuongGV.SoTC.ToString();
                    monHocAcceptDeny.GioDa = DecuongGV.GioDa.ToString();
                    monHocAcceptDeny.GioTT = DecuongGV.GioTT.ToString();
                    monHocAcceptDeny.GioLT = DecuongGV.GioLT.ToString();
                    monHocAcceptDeny.GioTH = DecuongGV.GioTH.ToString();
                    monHocAcceptDeny.MaDeCuongGV = DecuongGV.MaQL.ToString();
                    if (maUserHienTai == DecuongGV.NguoiCN)
                    {
                        monHocAcceptDeny.capnhatdecuong = true;
                    }
                    else
                    {
                        monHocAcceptDeny.capnhatdecuong = false;

                    }

                    lstResult.Add(monHocAcceptDeny);
                }

                return View(lstResult);
            }
            //Khi Kiểu Notification không phải kiểu đặc biệt **kiểu noti nội dung chỉ hiển thị đoạn message** thì đổi trạng thái thành đã trả lời khi vừa hiển thị
            notifi.Trangthai = true;
            context.SaveChanges();

            return View();
        }


        // <summary>
        /// Purpose: Thay đổi trạng thái đề cương giảng viên sau khi giáo viên hoàn thành trả lời các lời mời giảng dạy
        /// Developer: Trần An Bình
        /// Date:
        /// </summary>
        /// <param name="listDeCuongGVChapNhanTuChoi">Danh sách các đề cương giảng viên chấp nhận từ chối giảng dạy</param>
        /// <param name="lyDo">Lý do từ chối giảng dạy</param>
        /// <param name="emailNguoiGui">Email của người trả lời</param>
        /// <param name="maThongBaoHienTai">Mã thông báo lời mời giảng dạy mà người dùng vừa trả lời</param>
        /// <returns>Trả về string check ham đã thành công hay chưa</returns>
        // GET: Detail invitation for Giangvien
        [Authorize(Roles = UserRoles.roleTruongBoMon + "," + UserRoles.roleGiaoVien + "," + UserRoles.roleGiaoVienThinhGiang + "," + UserRoles.roleGiaoVienCoHuu)]
        public string ThayDoiTrangThaiDeCuongGV(string listDeCuongGVChapNhanTuChoi, string lyDo, string emailNguoiGui, string maThongBaoHienTai)
        {
            List<ModelThayDoiTrangThaiDeCuongGV> listDeCuongGVvaTinhTrang = JsonConvert.DeserializeObject<List<ModelThayDoiTrangThaiDeCuongGV>>(listDeCuongGVChapNhanTuChoi);

            //Bổ xung thông tin cho notification
            string thongTinCacMon = "";

            foreach (var Decuong in listDeCuongGVvaTinhTrang)
            {
                int maDeCuong = int.Parse(Decuong.maDeCuongGV);
                var deCuongTim = context.tp_DecuongGV.Find(maDeCuong);

                if (Decuong.giaTri)
                {
                    deCuongTim.Trangthai = Variables.TrangthaiDCGV_DongY;
                    deCuongTim.tp_KHDaotao.TrangthaiDC = Variables.TrangthaiDC_ChuaCapNhat;
                }
                else
                {
                    deCuongTim.Trangthai = Variables.TrangthaiDCGV_TuChoi;
                    deCuongTim.Ghichu = lyDo;
                }
                context.SaveChanges();
                thongTinCacMon += deCuongTim.tp_KHDaotao.t_CTDaotao.sc_Khoilop.TenKhoi + " Năm học " + deCuongTim.tp_KHDaotao.Namhoc + " học kỳ " + deCuongTim.tp_KHDaotao.Hocky + " " + deCuongTim.tp_KHDaotao.TenHP + (Decuong.giaTri ? " chấp nhận" : " từ chối");
                thongTinCacMon += "<br>";


            }

            //Khoá thông báo người dùng lại không cho tiếp tục cập nhật môn
            int maThongBaoHienTaii = int.Parse(maThongBaoHienTai);
            var notifiHienTai = context.sf_Notification.Find(maThongBaoHienTaii);
            notifiHienTai.Kieu = "Đã trả lời mời";
            notifiHienTai.DaXem = true;
            notifiHienTai.Trangthai = true;
            context.SaveChanges();

            //Tạo thông báo gửi lại cho trưởng bộ môn
            var maNguoiDungHienTai = User.Identity.GetUserId();
            var nguoiDungHienTai = context.AspNetUsers.Find(maNguoiDungHienTai);
            sf_Notification thongBao = new sf_Notification();
            thongBao.Nguon = "Email";
            thongBao.Thongtin = "Thầy/Cô " + nguoiDungHienTai.UserName + " đã trả lời yêu cầu giảng dạy <br> " + thongTinCacMon + "<br>Lý do: " + lyDo;
            thongBao.Kieu = "Trả lời lời mời giảng dạy";
            thongBao.NguoiGui = nguoiDungHienTai.Email;
            thongBao.NguoiNhan = emailNguoiGui;
            thongBao.DaXem = false;
            thongBao.Trangthai = false;
            thongBao.Ngaytao = DateTime.Now;

            context.sf_Notification.Add(thongBao);
            context.SaveChanges();

            return "done";
        }
    }

    public class ModelMonHocAcceptDeny
    {
        public string TenKhoa;
        public string TenMonHoc;
        public string SoTC;
        public string GioLT;
        public string GioTH;
        public string GioDa;
        public string GioTT;
        public string MaDeCuongGV;
        public bool capnhatdecuong;
    }

    public class ModelThayDoiTrangThaiDeCuongGV
    {
        public string maDeCuongGV;
        public bool giaTri;
    }
}