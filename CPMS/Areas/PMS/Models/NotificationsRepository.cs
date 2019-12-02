using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone.HubMain;
using Capstone.Areas.PMS;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using Capstone.Models;

namespace Capstone.Areas.PMS.Models
{
    public class NotificationsRepository
    {
        int numChange = 0;
        fit_misDBEntities db = new fit_misDBEntities();
        readonly string _connString =
        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<object> getListNotiForHub(bool checkBol, string emailUser)
        {
            //var listNotification = db.sf_Notification.ToList();

            var listThonBao = new List<object>();
            using (var connection = new SqlConnection(_connString))
            {
                string sqlquerystr = @"SELECT [MaTB],[Kieu],[NguoiGui],[NguoiNhan],[Nguon],[TrangThai],[ThongTin],[DaXem],[Ngaytao] FROM [dbo].[sf_Notification] 
                WHERE [DaXem] = '{checkBol}' AND [NguoiNhan] = '{emailUser}'
                ORDER BY [dbo].[sf_Notification].[Ngaytao] DESC";

                sqlquerystr = sqlquerystr.Replace("{checkBol}", checkBol.ToString()).Replace("{emailUser}", emailUser);

                connection.Open();
                using (var command = new SqlCommand(sqlquerystr, connection))
                {
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    //SqlNotificationRequest sqlNotiReq = new SqlNotificationRequest();
                    //sqlNotiReq.Timeout = 2000;
                    //command.Notification = sqlNotiReq;


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string emailNguoiNhan = (string)reader["NguoiNhan"];
                        string emailNguoiGui = (string)reader["NguoiGui"];
                        listThonBao.Add(item: new
                        {
                            MANO = reader["MaTB"],
                            Kieu = reader["Kieu"],
                            idNguoiGui = emailNguoiGui,
                            NguoiGui = db.AspNetUsers.FirstOrDefault(s => s.Email == emailNguoiGui).UserName,
                            idNguoiNhan = emailNguoiNhan,
                            NguoiNhan = db.AspNetUsers.FirstOrDefault(s => s.Email == emailNguoiNhan).UserName,
                            Nguon = reader["Nguon"],
                            TrangThai = reader["TrangThai"],
                            ThongTin = reader["ThongTin"],
                            DaXem = reader["DaXem"],
                            ThoiGianRaDoi = reader["Ngaytao"]
                        });
                    }

                    return listThonBao;
                }
            }

        }


        //Occur when table Notification change
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change && numChange == 0)
            {
                Notification.updatedNoti();
                SqlDependency dependency = sender as SqlDependency;
                dependency.OnChange -= dependency_OnChange;
                numChange++;

            }
        }

    }
}