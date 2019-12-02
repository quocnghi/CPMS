using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone.Models;

namespace Capstone.Areas.CMS.Models
{
    public class MmapData
    {
        public List<t_CDR_CTDT> cdr_cd { get; set; }
        public List<t_CTDaotao> ctdt { get; set; }
        public List<sc_HeNganh> henganh { get; set; }
        public List<sc_Khoa> kh { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
    }
    public class NoiDungCtdt
    {
        public t_CTDaotao ctdt { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<sc_Khoikienthuc> khoikienthuc { get; set; }
        public List<sc_Hocphan> hocphan { get; set; }
    }
    public class CTDTData
    {

        public IQueryable<t_CTDaotao> ctdt { get; set; }
        public List<sc_HeNganh> henganh { get; set; }
        public List<sc_Khoa> kh { get; set; }
        public List<AspNetUser> nv { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<sc_Khoikienthuc> kkt { get; set; }
        public List<t_MT_CTDT> mt_ctdt { get; set; }
        public List<t_CDR_CTDT> cdr_ctdt { get; set; }
        public List<t_MTCDR_CTDT> mt_cdr_ctdt { get; set; }
    }

    public class CTDTkhungData
    {
        public List<t_CTDaotao> ctdt { get; set; }
        public IQueryable<sc_HeNganh> henganh { get; set; }
        public IQueryable<sc_Khoa> kh { get; set; }
        public IQueryable<AspNetUser> nv { get; set; }
        public IQueryable<tc_KHDaotao> khdt { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public IQueryable<sc_Khoikienthuc> kkt { get; set; }
    }
    public class PhancongData
    {
        public t_CTDaotao ctdt { get; set; }
        public List<AspNetUser> nhanvien { get; set; }
        public List<tc_KHDaotao> khctdt { get; set; }
        public List<tc_DecuongGV> gvien { get; set; }
    }
    public class HocphanData
    {
        public t_CTDaotao ctdt { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<sc_Khoikienthuc> khoikienthuc { get; set; }
        public List<t_MT_CTDT> mtctdt { get; set; }
        public List<t_CDR_CTDT> cdrctdt { get; set; }
        public List<t_MTCDR_CTDT> mtcdrctdt { get; set; }
        public List<tc_MatranMH> mtmh { get; set; }
        public List<sc_Hocphan> hocphan { get; set; }
    }

    public class CTDTKH
    {
        public List<t_CTDaotao> ctdt { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<sc_Khoikienthuc> khoikienthuc { get; set; }
        public List<sc_HeNganh> henganh { get; set; }
        public List<t_MT_CTDT> mtctdt { get; set; }
        public List<t_CDR_CTDT> cdrctdt { get; set; }
        public List<t_MTCDR_CTDT> mtcdrctdt { get; set; }
        public List<tc_MatranMH> mtmh { get; set; }

    }

    public class FindCTDTKhung
    {
        public int manganh { get; set; }
        public string trinhdo { get; set; }
    }

    public class JsonReturnCTDTKeHoach
    {
        public IEnumerable<tc_KHDaotao> khdaotao { get; set; }
        public IEnumerable<sc_Khoilop> khoilop { get; set; }
        public CTDTKeHoach ctdtao { get; set; }
    }

    public class GetCTDTKeHoach
    {
        public IEnumerable<tc_KHDaotao> khdaotao { get; set; }
        public CTDTKeHoach ctdtao { get; set; }
        public int khoilop { get; set; }
    }

    public class CTDTKeHoach
    {
        public int id { get; set; }
        public string tenct { get; set; }
        public string trinhdokhung { get; set; }
        public string tenkhoa { get; set; }
        public string loaihinhdt { get; set; }
        public string soqd { get; set; }
        public DateTime? ngayqd { get; set; }
        public string muctieu { get; set; }
        public string tgiandt { get; set; }
        public string khoiluongkt { get; set; }
        public string doituong { get; set; }
        public string quytrinh { get; set; }
        public string thangdiem { get; set; }
        public string csvc { get; set; }
        public int mahn { get; set; }
        public string phienban { get; set; }
    }

    public class DCuongGV
    {
        public int MaKHDT { get; set; }
        public List<string> Id { get; set; }
        public List<int> MaQL { get; set; }
    }

    public class EditDCuongGV
    {
        public int MaKHDT { get; set; }
        public List<string> Id { get; set; }
        public List<int> MaQL { get; set; }
    }

    public class GetNgayHT
    {
        public DateTime? NgayHT { get; set; }
    }

    public class Decuongchitiet
    {
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<tc_CDR_HP> cdrHocphan { get; set; }
        public List<tc_NoidungHP> noidungHp { get; set; }
        public List<t_CDR_CTDT> cdrCtdt { get; set; }
        public List<sc_Khoikienthuc> khoiKT { get; set; }
    }

    public class Muctieudaotaocuthe
    {
        public string Mota1 { get; set; }
        public string Mota2 { get; set; }
        public string Mota3 { get; set; }

        public string Phanloai1 { get; set; }
        public string Phanloai2 { get; set; }
        public string Phanloai3 { get; set; }

    }

    public class Ketquahoctapmongdoi
    {
        public string MaHT1 { get; set; }
        public string MaHT2 { get; set; }
        public string MaHT3 { get; set; }

        public string Mota1 { get; set; }
        public string Mota2 { get; set; }
        public string Mota3 { get; set; }

    }


    public class MappingCDRMH
    {
        public tc_CDR_HP CDRMH { get; set; }
        public List<int> MaELO { get; set; }
    }

    public class MatranCDR
    {
        public string MaHT { get; set; }
        public string Mucdo { get; set; }
    }

    public class DanhgiaCTDT
    {
        public List<t_CTDaotao> ctdt { get; set; }
        public List<tc_KHDaotao> khdaotao { get; set; }
        public List<sc_Khoikienthuc> khoikienthuc { get; set; }
        public List<sc_HeNganh> henganh { get; set; }
        public List<t_MT_CTDT> mtctdt { get; set; }
        public List<t_CDR_CTDT> cdrctdt { get; set; }
        public List<t_MTCDR_CTDT> mtcdrctdt { get; set; }
        public List<tc_MatranMH> mtmh { get; set; }
    }

    public class MappingNDGD
    {
        public tc_NoidungHP NDGD { get; set; }
        public List<int> MaCELO { get; set; }
    }

    public class MappingEditNDGD
    {
        public tc_NoidungHP NDGD { get; set; }
        public List<int> MaCELO { get; set; }
    }

    public class MatranNDGD
    {
        public string Ghichu { get; set; }
        public string MaHT { get; set; }
    }

    public class QuanLyUser
    {
        public string Id { get; set; }
        public int MaNV { get; set; }
        public string Vaitro { get; set; }
    }

    public class HoanthanhKHDT
    {
        public tc_KHDaotao khdt { get; set; }
        public List<int> MaPH { get; set; }
    }

    public class LuunhapKHDT
    {
        public tc_KHDaotao khdt { get; set; }
        public List<int> MaPH { get; set; }
    }

    public class CheckPB
    {
        public string NganhDT { get; set; }
    }

    public class ChangeNganh
    {
        public string He { get; set; }
    }
    public class HenganhDT
    {
        public sc_HeNganh Henganh { get; set; }
        public string TenHe { get; set; }
    }
    public class KhoiKienThuc
    {
        public sc_Khoikienthuc khoikienthuc { get; set; }
        public string TenLoai { get; set; }
    }

    public class PDFFile
    {
        public HttpPostedFileBase FileExcel { get; set; }
        public string MaCT { get; set; }
    }

    public class StringPDF
    {
        public string pdf { get; set; }
    }
}