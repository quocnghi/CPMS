using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class ROLES
    {
        public const string ADMIN = "Admin";
        public const string HEADOFEDITOR = "Trưởng ban soạn thảo";
        public const string EVALUATOR = "Người đánh giá";
        public const string EDITOR = "Người soạn thảo";
        public const string ADMIN_HEADOFEDITOR = "Admin, Trưởng ban soạn thảo";
        public const string EDITOR_EVALUATOR = "Người soạn thảo, Người đánh giá";
        public const string ADMIN_HEADOFEDITOR_EDITOR_EVALUATOR = "Admin, Trưởng ban soạn thảo,Người soạn thảo, Người đánh giá";
        public const string ADMIN_HEADOFEDITOR_EDITOR = "Admin, Trưởng ban soạn thảo,Người soạn thảo";
        public const string ADMIN_EVALUATOR = "Admin, Người đánh giá";
        public const string ALLROLES = "Admin, Trưởng ban soạn thảo,Người soạn thảo, Người đánh giá, Giảng viên, Giảng viên cơ hữu, Giảng viên thỉnh giảng, Trợ lý giáo vụ Khoa, Trưởng bộ môn, Trưởng Khoa";
    }
    public class LoaiHinhDT
    {
        public const string CTDT_KH = "PC";
        public const string CTDTKH = "Chương trình đào tạo kế hoạch";
        public const string CTDT_KHUNG = "SC";
        public const string CTDTKHUNG = "Chương trình đào tạo khung";
    }
    public class TinhTrang
    {
        public const string HieuLuc = "Hiệu lực";
        public const string LuuTam = "Lưu tạm";
        public const string LuuTru = "Lưu trữ";
    }

    public class TrangThai
    {
        public const string KhoiTao = "0";
        public const string XayDungDeCuongHocPhan = "1";
        public const string DanhGiaCTDT = "2";
        public const string DaPheDuyet = "3";

    }
    public class TrangThaiDc
    {
        public const string ChuaCapNhat = "0";
        public const string LuuTam = "1";
        public const string ChoPheDuyet = "2";
        public const string DaPheDuyet = "3";
        public const string TuChoi = "4";

    }

    public class Ma
    {
        public const string Phienban = "1";
        public const string Monhoc = "2";
        public const string Nhanvien = "3";
        public const string KQHTMD = "4";
        public const string KQHTMDHP = "5";

    }
    public class Caymatran
    {
        public const string HeDaoTao = "Hệ Đào Tạo";
        public const string Green = "green";
        public const string Phienban = "Phiên bản";
    }
}
