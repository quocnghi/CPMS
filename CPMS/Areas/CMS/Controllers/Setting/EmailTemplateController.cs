using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Capstone.Models;
using CommonLibrary;

namespace Capstone.Areas.CMS.Controllers.Setting
{
    public class EmailTemplateController : Controller
    {
        fit_misDBEntities db = new fit_misDBEntities();
        // GET: EmailTemplate
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public ActionResult Index()
        {
            List<sf_EmailTemplate> data = loadEmailList();
            return View(data);
        }

        /// <summary>
        /// lấy danh sách email
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public List<sf_EmailTemplate> loadEmailList()
        {
            List<sf_EmailTemplate> lstemail = db.sf_EmailTemplate.ToList();
            return lstemail;
        }

        /// <summary>
        /// thêm mới 1 email template
        /// </summary>
        /// <param name="mahienthi"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        [ValidateInput(false)]
        public ActionResult AddEmail(string mahienthi, string Description)
        {
            sf_EmailTemplate eMail = new sf_EmailTemplate();
            eMail.Chude = mahienthi;
            eMail.Noidung = Description;
            try
            {
                db.sf_EmailTemplate.Add(eMail);
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("/");
        }

        /// <summary>
        /// lấy thông tin cần chỉnh sửa 1 email theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        public PartialViewResult EditEmail(int id)
        {
            sf_EmailTemplate dataInfo = new sf_EmailTemplate();

            var rs = db.sf_EmailTemplate.Find(id);
            return PartialView(rs);
        }

        /// <summary>
        /// lưu thông tin chỉnh sửa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mahienthi"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR)]
        [ValidateInput(false)]
        public ActionResult SaveEditEmail(int id, string mahienthi, string Description)
        {
            sf_EmailTemplate eMail = new sf_EmailTemplate();
            try
            {
                var rs = db.sf_EmailTemplate.Find(id);
                rs.Chude = mahienthi;
                rs.Noidung = Description;
                db.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
            }
            return RedirectToAction("/");
        }

        /// <summary>
        /// xóa 1 email 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ROLES.ADMIN_HEADOFEDITOR_EDITOR)]
        public bool DeleteEmail(int id)
        {
            try
            {
                //tìm mã biểu mẫu tương ướng
                var rs = db.sf_EmailTemplate.Find(id);
                //tìm và xóa biểu mẫu trong bảng biểu mẫu Email
                db.Entry(rs).State = System.Data.Entity.EntityState.Deleted;
                //lưu lại
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}