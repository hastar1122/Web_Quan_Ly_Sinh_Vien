using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Hubs
{
    public class ChatHub : Hub
    {
        public static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public override Task OnConnected()
        {
            tblUser tblUser = db.tblUsers.Find(long.Parse(Context.User.Identity.Name));
            tblUser.ConnectionId = Context.ConnectionId;
            db.SaveChanges();
            var tblFiends = db.tblFriends.Where(m => m.NguoiGui == tblUser.Id || m.NguoiNhan == tblUser.Id).ToList();
            List<string> connectionIds = new List<string>();
            foreach(var item in tblFiends)
            {
                if (item.NguoiGui == tblUser.Id)
                {
                    connectionIds.Add(item.tblUser1.ConnectionId);
                }
                else
                {
                    connectionIds.Add(item.tblUser.ConnectionId);
                }
            }
            Clients.Clients(connectionIds).ThongBaoOnline(tblUser.Id);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            tblUser tblUser = db.tblUsers.Find(long.Parse(Context.User.Identity.Name));
            tblUser.ConnectionId = null;
            db.SaveChanges();
            var tblFiends = db.tblFriends.Where(m => m.NguoiGui == tblUser.Id || m.NguoiNhan == tblUser.Id).ToList();
            List<string> connectionIds = new List<string>();
            foreach (var item in tblFiends)
            {
                if (item.NguoiGui == tblUser.Id)
                {
                    connectionIds.Add(item.tblUser1.ConnectionId);
                }
                else
                {
                    connectionIds.Add(item.tblUser.ConnectionId);
                }
            }
            Clients.Clients(connectionIds).ThongBaoOffline(tblUser.Id);
            return base.OnDisconnected(stopCalled);
        }

        public static void RecieveMessage(tblTinNhan tblTinNhan)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            String htmlString = @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhan.tblUser.Anh +  @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhan.ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhan.tblUser.Ho + " " + tblTinNhan.tblUser.Ten + " </div> <span class='text-break'>" + tblTinNhan.NoiDung + "</span></div> </div>";
            context.Clients.Clients(db.tblUsers.Where(m => m.Id == tblTinNhan.NguoiNhan).Select(m => m.ConnectionId).ToList()).message(tblTinNhan.NguoiGui,htmlString);
        }

        public static void UploadFiles(List<tblTinNhan> tblTinNhans)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            string htmlString = "";
            for (int i =0; i< tblTinNhans.Count(); i++)
            {
                if (tblTinNhans[i].LoaiTinNhan == 2)
                {
                    htmlString += @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhans[i].tblUser.Anh + @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhans[i].ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhans[i].tblUser.Ho + " " + tblTinNhans[i].tblUser.Ten + " </div> <div class='message-img mx-n2'> <img src='" + tblTinNhans[i].NoiDung + "'/> </div> </div> </div>";
                }
                else
                {
                    htmlString += @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhans[i].tblUser.Anh + @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhans[i].ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhans[i].tblUser.Ho + " " + tblTinNhans[i].tblUser.Ten + " </div> <a class='text-break' href='/Chat/Download?filename=" + tblTinNhans[i].NoiDung.Split('/').LastOrDefault() + "'>" + tblTinNhans[i].NoiDung.Split('/').LastOrDefault() +" </a> </div> </div>";
                }
            }
            long Id = (long)tblTinNhans[0].NguoiNhan;
            List<string> connectionIds = db.tblUsers.Where(m => m.Id == Id).Select(m => m.ConnectionId).ToList();
            context.Clients.Clients(connectionIds).uploadfile(tblTinNhans[0].NguoiGui, htmlString);
        }

        public static void UploadFilesGroup(List<string> ConnectionIds, List<tblTinNhan> tblTinNhans)
        {
            string htmlString = "";
            for (int i = 0; i < tblTinNhans.Count(); i++)
            {
                if (tblTinNhans[i].LoaiTinNhan == 2)
                {
                    htmlString += @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhans[i].tblUser.Anh + @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhans[i].ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhans[i].tblUser.Ho + " " + tblTinNhans[i].tblUser.Ten + " </div> <div class='message-img mx-n2'> <img src='" + tblTinNhans[i].NoiDung + "'/> </div> </div> </div>";
                }
                else
                {
                    htmlString += @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhans[i].tblUser.Anh + @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhans[i].ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhans[i].tblUser.Ho + " " + tblTinNhans[i].tblUser.Ten + " </div> <a class='text-break' href='/Chat/Download?filename=" + tblTinNhans[i].NoiDung.Split('/').LastOrDefault() + "'>" + tblTinNhans[i].NoiDung.Split('/').LastOrDefault() + " </a> </div> </div>";
                }
            }
            context.Clients.Clients(ConnectionIds).uploadfileGroup(tblTinNhans[0].Nhom, htmlString);
        }

        public static void RecieveMessageGroup(List<string> ConnectionId, tblTinNhan tblTinNhan)
        {
            String htmlString = @"<div class='chat-message-left pb-2'> <div>  <img src = """ + @tblTinNhan.tblUser.Anh + @""" class='rounded-circle mr-1' width='40' height='40'> <div class='text-muted small text-nowrap mt-2'> " + @String.Format("{0:t}", tblTinNhan.ThoiGian) + "</div></div> <div class='flex-shrink-1 bg-light rounded py-2 px-3 ml-3'><div class='font-weight-bold mb-1'> " + tblTinNhan.tblUser.Ho + " " + tblTinNhan.tblUser.Ten + " </div> <span class='text-break'>" + tblTinNhan.NoiDung + "</span></div> </div>";
            context.Clients.Clients(ConnectionId).messageGroup(tblTinNhan.Nhom, htmlString); ;
        }

        public static void DeleteUserFromGroup(long Id,List<long> UserId)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            context.Clients.Clients(db.tblUsers.Where(m => UserId.Contains(m.Id)).Select(m => m.ConnectionId).ToList()).ThongBaoBiKickKhoiNhom(Id,true);
        }

        public static void AddUserToGroup(List<long> UserId)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            context.Clients.Clients(db.tblUsers.Where(m => UserId.Contains(m.Id)).Select(m => m.ConnectionId).ToList()).ThongBaoAddVaoNhom(true);
        }

        public static void OfflineUser (long Id)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            var tblFiends = db.tblFriends.Where(m => m.NguoiGui == Id || m.NguoiNhan == Id).ToList();
            List<string> connectionIds = new List<string>();
            foreach (var item in tblFiends)
            {
                if (item.NguoiGui == Id)
                {
                    connectionIds.Add(item.tblUser1.ConnectionId);
                }
                else
                {
                    connectionIds.Add(item.tblUser.ConnectionId);
                }
            }
            context.Clients.Clients(connectionIds).ThongBaoOffline(Id);
        }

        public static void UpdateThongTinNhom (List<string> ConnectionId, long GroupId)
        {
            context.Clients.Clients(ConnectionId).ThongBaoUpdateThongTinNhom(GroupId,true);
        }

        public static void DeleteGroupChat (List<string> connectionIds, long GroupId)
        {
            context.Clients.Clients(connectionIds).ThongBaoXoaNhomChat(GroupId, true);
        }

        public static void DeleteMsg (long toUserId, long userId)
        {
            Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
            context.Clients.Clients(db.tblUsers.Where(m => m.Id == toUserId).Select(m => m.ConnectionId).ToList()).ThongBaoXoaTinNhan(userId, true);
        }
    }
}