using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone.Models;

namespace Capstone.Areas.CMS.Controllers.Tools
{
    public class SendNotification
    {
		fit_misDBEntities db = new fit_misDBEntities();
        public void SendNoticeAssignedtoGV(int maql, string userid, string chude, string noidung)
        {
            var noti = new sf_Notification();
            noti.DaXem = false;
            noti.Ngaytao = DateTime.Now;
            noti.NguoiNhan= userid;
            noti.Chude = chude;
            noti.Thongtin = noidung;
            noti.MaDC = maql;
            db.sf_Notification.Add(noti);
            db.SaveChanges();
        }
    }
}