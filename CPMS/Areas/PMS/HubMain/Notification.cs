using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Capstone.Areas.PMS.Controllers;
using Capstone.Models;

namespace Capstone.HubMain
{
    public class Notification : Hub
    {
        fit_misDBEntities db = new fit_misDBEntities();
        public static readonly string _connString =
        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        /**
         ** params False is Not seen
         *  Params True is Seen
         **/

        //public void getListNoti(bool checkBol, string emailUser)
        //{
        //    //var listNotification = db.sf_Notification.ToList();
        //    var listThonBao = (from item in db.sf_Notification
        //                      where item.DaXem == checkBol && item.NguoiNhan== emailUser
        //                      orderby item.ThoiGianRaDoi
        //                      select new
        //                      {
        //                          MANO = item.MANO,
        //                          Kieu = item.Kieu,
        //                          idNguoiGui = item.NguoiGui,
        //                          NguoiGui = db.AspNetUsers.FirstOrDefault(s=> s.Email == item.NguoiGui).UserName,
        //                          idNguoiNhan = item.NguoiNhan,
        //                          NguoiNhan = db.AspNetUsers.FirstOrDefault(s => s.Email == item.NguoiNhan).UserName,
        //                          Nguon = item.Nguon,
        //                          TrangThai = item.TrangThai,
        //                          ThongTin = item.ThongTin,
        //                          DaXem = item.DaXem,
        //                          GhiChu = item.GhiChu,
        //                          ThoiGianRaDoi = item.ThoiGianRaDoi,
        //                      }).OrderByDescending(d=> d.ThoiGianRaDoi);
        //    if (checkBol)
        //    {
        //        Clients.All.listSeen(listThonBao);
        //    }else
        //    {
        //        Clients.All.listNotSeen(listThonBao);
        //        Clients.All.listNotSeenContent(listThonBao);
        //    }

        //}
        public void sendMeo(string name)
        {
            Clients.All.sendSi(name);
        }

        //public void getListNoti(bool checkBol, string emailUser)
        //{
        //    List<object> listThonBao;
        //    NotificationsRepository notiRes = new NotificationsRepository();
        //    listThonBao = notiRes.getListNotiForHub(checkBol,emailUser);


        //    if (checkBol)
        //    {
        //        Clients.All.listSeen(listThonBao);
        //    }
        //    else
        //    {
        //        Clients.All.listNotSeen(listThonBao);
        //        Clients.All.listNotSeenContent(listThonBao);
        //    }

        //}

        public static void updatedNoti()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Notification>();
            context.Clients.All.notifUpdated();
        }



    }
}