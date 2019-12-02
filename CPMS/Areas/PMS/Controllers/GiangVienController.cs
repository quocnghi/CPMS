using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using Capstone.Areas.PMS.Controllers;
using Capstone.Models;
using Newtonsoft.Json;

namespace Capstone.Areas.PMS.Controllers
{
    public class GiangVienController : Controller
    {
        APIController API = new APIController();
        fit_misDBEntities context = new fit_misDBEntities();

        // GET: GiangVien
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult PhanCongGiangVien()
        {
            List<sc_HeNganh> listSystem = new List<sc_HeNganh>();
            listSystem = API.dsHeDaotao();
            return View(listSystem);
        }

        // GET
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        [HttpPost]
        public ActionResult XemTruocPhanCongGiangVien(string HeHoc, string MaNganh, string NamHoc, string HocKy)
        {
            ViewBag.MaNganh = MaNganh;
            ViewBag.NamHoc = NamHoc;
            ViewBag.HocKy = HocKy;
            ViewBag.HeHoc = HeHoc;

            var manganh = int.Parse(MaNganh);
            ViewBag.TenNganh = context.sc_HeNganh.Find(manganh).Mota;

            return View();
        }

        // GET
        [Authorize(Roles = UserRoles.roleTruongBoMon)]
        public ActionResult NoiDungEmailPhanCongGV(string MaNganh, string NamHoc, string HocKy, string listGiaoVienChuaGuiMail, string TenNganh)
        {
            ViewBag.MaNganh = MaNganh;
            ViewBag.NamHoc = NamHoc;
            ViewBag.HocKy = HocKy;
            ViewBag.TenNganh = TenNganh;
            ViewBag.listGiaoVienChuaGuiMail = listGiaoVienChuaGuiMail;
            List<KHDTVaGiangVienVaTrangThai> listAddGiaoVien = JsonConvert.DeserializeObject<List<KHDTVaGiangVienVaTrangThai>>(listGiaoVienChuaGuiMail);

            //Lưu ngày hết hạn nộp đề cương
            foreach (var ele in listAddGiaoVien)
            {
                ////lấy ra người đầu tiền trong list gv để ngán hết hạn đề cương
                //string maGV = ele.listgv[0].magv;
                //int maKHDT = int.Parse(ele.khdt);

                //var decuongGV = context.tp_DecuongGV.Where(x => x.NguoiCN == maGV).Where(x => x.MaKHDT == maKHDT).FirstOrDefault();
                //decuongGV.NgayHT = DateTime.Parse(ele.ngayhethandecuong);

                int maKHDT = int.Parse(ele.khdt);
                var khdt = context.tp_KHDaotao.Find(maKHDT);
                khdt.NgayHT = DateTime.Parse(ele.ngayhethandecuong);

                //context.SaveChanges();--Hong
                context.Entry(khdt).State = System.Data.Entity.EntityState.Modified;
            }
            context.SaveChanges();

            sf_EmailTemplate mailEle = context.sf_EmailTemplate.Where(x => x.Phanloai == "Mail Phân Công").FirstOrDefault();
            return View(mailEle);
        }
    }
}