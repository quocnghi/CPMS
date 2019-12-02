using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capstone.Models;

namespace Capstone.Areas.CMS.Controllers.Tools
{
    public class SendEmail
    {
        SendNotification sNotitification = new SendNotification();
        public void SendEmailtoGV(int maql ,string userid,string username, string email, int id,string tenmh, string hocky, string nganh, int namhoc, string nguGui)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
			fit_misDBEntities db = new fit_misDBEntities();
            var rs = db.sf_EmailTemplate.Find(id);
            rs.Noidung = rs.Noidung.Replace("{TenGV}", username);
            rs.Noidung = rs.Noidung.Replace("{TenMH}", tenmh);
            rs.Noidung = rs.Noidung.Replace("{TenNganh}", nganh);
            rs.Noidung = rs.Noidung.Replace("{Hocky}", hocky);
            rs.Noidung = rs.Noidung.Replace("{TenNG}", nguGui);
            rs.Noidung = rs.Noidung.Replace("{NamHoc}", namhoc.ToString());
            string link = url.Action("HopThuDenGV", "Mail",
                    new { }, HttpContext.Current.Request.Url.Scheme);
            if (link.Contains("http://cntttest.vanlanguni.edu.vn"))
            {
                link = link.Replace("http://cntttest.vanlanguni.edu.vn", "http://cntttest.vanlanguni.edu.vn:18080/Cap21T14Prod/Curriculum");
            }
            rs.Noidung = rs.Noidung.Replace("{Url}", link);
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                   new System.Net.Mail.MailAddress("mnpostvn@gmail.com", rs.Chude),
                   new System.Net.Mail.MailAddress(email));
            m.Subject = rs.Chude;
            m.Body = string.Format(rs.Noidung);
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            smtp.Credentials = new System.Net.NetworkCredential("mnpostvn@gmail.com", "Mnpost_2018");
            smtp.EnableSsl = true;
            smtp.Send(m);
            sNotitification.SendNoticeAssignedtoGV(maql,userid, rs.Chude,rs.Noidung);
        }
    }
}