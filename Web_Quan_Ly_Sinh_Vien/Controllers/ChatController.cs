using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Hubs;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetUsers(string s)
        {
            string sql = "SELECT *, CONVERT(varchar,CONCAT(Ho, ' ', Ten)) as firstlast FROM tblUser WHERE CONVERT(varchar,CONCAT(Ho, ' ', Ten)) like '%" + s + "%' and Id != " + long.Parse(User.Identity.Name);
            List<tblUser> tblUsers = db.tblUsers.SqlQuery(sql).ToList();
            return PartialView(tblUsers);
        }

        public PartialViewResult GetUsersToAddGroup(long Id, string s)
        {
            var tblUserId = db.tblChiTietNhomChats.Where(m => m.MaNhom == Id).Select(m => m.User);
            string sql = "SELECT *, CONVERT(varchar,CONCAT(Ho, ' ', Ten)) as firstlast FROM tblUser WHERE CONVERT(varchar,CONCAT(Ho, ' ', Ten)) like '%" + s + "%' and Id != " + long.Parse(User.Identity.Name);
            List<tblUser> tblUsers = db.tblUsers.SqlQuery(sql).ToList();
            return PartialView(tblUsers.Where(m => !tblUserId.Contains(m.Id)).ToList());
        }

        public PartialViewResult LoadListTinNhanRieng()
        {
            long Id = long.Parse(User.Identity.Name);
            var tblFriends = db.tblFriends.Where(m => m.NguoiGui == Id || m.NguoiNhan == Id).OrderByDescending(m => m.Update_At);
            return PartialView(tblFriends.ToList());
        }

        public PartialViewResult LoadListNhomChat()
        {
            long Id = long.Parse(User.Identity.Name);
            var tblNhomChats = from x in db.tblNhomChats join y in db.tblChiTietNhomChats on x.Id equals y.MaNhom where y.User == Id && x.MaLHP == null select x;
            return PartialView(tblNhomChats.OrderByDescending(m => m.Update_At).ToList());
        }

        public PartialViewResult LoadThongTinTinNhan(long? id)
        {
            return PartialView(db.tblUsers.Find(id));
        }

        public PartialViewResult LoadAllTinNhanRieng(long? id)
        {
            long Id = long.Parse(User.Identity.Name);
            var tblTinNhans = db.tblTinNhans.Where(m => (m.NguoiGui == Id && m.NguoiNhan == id) || (m.NguoiGui == id && m.NguoiNhan == Id)).OrderBy(m => m.ThoiGian).ToList();
            return PartialView(tblTinNhans);
        }

        public PartialViewResult LoadAllTinNhanNhom(long? id)
        {
            return PartialView("LoadAllTinNhanRieng", db.tblTinNhans.Where(m => m.Nhom == id));
        }

        [HttpPost]
        public JsonResult CreateNhomChat(tblNhomChat tblNhomChat)
        {
            try
            {
                DateTime a;
                if (tblNhomChat.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblNhomChat.ImageFile.FileName);
                    string extension = Path.GetExtension(tblNhomChat.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblNhomChat.Avatar = "/Images/NhomChat/" + fileName;
                    fileName = Path.Combine(Server.MapPath("/Images/NhomChat/"), fileName);
                    tblNhomChat.ImageFile.SaveAs(fileName);
                }
                else
                {
                    tblNhomChat.Avatar = "/Images/NhomChat/avatar.png";
                }
                tblNhomChat.Create_At = DateTime.Now;
                a = (DateTime)tblNhomChat.Create_At;
                tblNhomChat.Update_At = a;
                db.tblNhomChats.Add(tblNhomChat);
                db.SaveChanges();
                //tblNhomChat b = db.tblNhomChats.SqlQuery("select * from tblNhomChat where FORMAT(Create_At , 'yyyy-MM-dd HH:mm:ss') ='" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", a) + "' and TenNhom = '" + tblNhomChat.TenNhom + "'").FirstOrDefault();
                var tblChiTietNhomChat = new tblChiTietNhomChat();
                tblChiTietNhomChat.User = long.Parse(User.Identity.Name);
                tblChiTietNhomChat.MaNhom = tblNhomChat.Id;
                tblChiTietNhomChat.IsAdmin = true;
                db.tblChiTietNhomChats.Add(tblChiTietNhomChat);
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(new { status = false });
            }
        }

        public PartialViewResult LoadThongTinNhom(long? id)
        {
            return PartialView(db.tblChiTietNhomChats.Find(long.Parse(User.Identity.Name), id));
        }

        [HttpPost]
        public PartialViewResult UploadFiles(FormCollection form)
        {
            long toUserId = long.Parse(Request.Form["toUserId"]);
            bool isChatNhom = bool.Parse(Request.Form["isChatNhom"]);
            HttpFileCollectionBase files = Request.Files;
            if (isChatNhom == false)
            {
                tblUser user = db.tblUsers.Find(long.Parse(User.Identity.Name));
                List<tblTinNhan> tblTinNhans = new List<tblTinNhan>();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToUpperInvariant()))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                        string f = "/Images/TinNhan/" + fileName;
                        fileName = Path.Combine(Server.MapPath("/Images/TinNhan/"), fileName);
                        file.SaveAs(fileName);
                        tblTinNhan tblTinNhan = new tblTinNhan();
                        tblTinNhan.NguoiGui = user.Id;
                        tblTinNhan.NguoiNhan = toUserId;
                        tblTinNhan.Nhom = null;
                        DateTime a = DateTime.Now;
                        tblTinNhan.ThoiGian = a;
                        tblTinNhan.NoiDung = f;
                        tblTinNhan.LoaiTinNhan = 2;
                        db.tblTinNhans.Add(tblTinNhan);
                        tblTinNhans.Add(tblTinNhan);
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                        string f = "/File/TinNhan/" + fileName;
                        fileName = Path.Combine(Server.MapPath("/File/TinNhan/"), fileName);
                        file.SaveAs(fileName);
                        tblTinNhan tblTinNhan = new tblTinNhan();
                        tblTinNhan.NguoiGui = user.Id;
                        tblTinNhan.NguoiNhan = toUserId;
                        tblTinNhan.Nhom = null;
                        DateTime a = DateTime.Now;
                        tblTinNhan.ThoiGian = a;
                        tblTinNhan.NoiDung = f;
                        tblTinNhan.LoaiTinNhan = 3;
                        db.tblTinNhans.Add(tblTinNhan);
                        tblTinNhans.Add(tblTinNhan);
                    }
                }
                db.SaveChanges();
                tblFriend tblFriend = db.tblFriends.Where(m => (m.NguoiGui == user.Id && m.NguoiNhan == toUserId) || (m.NguoiGui == toUserId && m.NguoiNhan == user.Id)).FirstOrDefault();
                if (tblFriend == null)
                {
                    db.tblFriends.Add(new tblFriend
                    {
                        NguoiGui = user.Id,
                        NguoiNhan = toUserId,
                        LastMsg = tblTinNhans.LastOrDefault().Id,
                        Update_At = DateTime.Now
                    });
                }
                else
                {
                    tblFriend.LastMsg = tblTinNhans.LastOrDefault().Id;
                    tblFriend.Update_At = DateTime.Now;
                }
                db.SaveChanges();
                ChatHub.UploadFiles(tblTinNhans);
                return PartialView(tblTinNhans);
            }
            else
            {
                tblUser user = db.tblUsers.Find(long.Parse(User.Identity.Name));
                List<tblTinNhan> tblTinNhans = new List<tblTinNhan>();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToUpperInvariant()))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                        string f = "/Images/TinNhan/" + fileName;
                        fileName = Path.Combine(Server.MapPath("/Images/TinNhan/"), fileName);
                        file.SaveAs(fileName);
                        tblTinNhan tblTinNhan = new tblTinNhan();
                        tblTinNhan.NguoiGui = user.Id;
                        tblTinNhan.NguoiNhan = null;
                        tblTinNhan.Nhom = toUserId;
                        DateTime a = DateTime.Now;
                        tblTinNhan.ThoiGian = a;
                        tblTinNhan.NoiDung = f;
                        tblTinNhan.LoaiTinNhan = 2;
                        db.tblTinNhans.Add(tblTinNhan);
                        tblTinNhans.Add(tblTinNhan);
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                        string f = "/File/TinNhan/" + fileName;
                        fileName = Path.Combine(Server.MapPath("/File/TinNhan/"), fileName);
                        file.SaveAs(fileName);
                        tblTinNhan tblTinNhan = new tblTinNhan();
                        tblTinNhan.NguoiGui = user.Id;
                        tblTinNhan.NguoiNhan = null;
                        tblTinNhan.Nhom = toUserId;
                        DateTime a = DateTime.Now;
                        tblTinNhan.ThoiGian = a;
                        tblTinNhan.NoiDung = f;
                        tblTinNhan.LoaiTinNhan = 3;
                        db.tblTinNhans.Add(tblTinNhan);
                        tblTinNhans.Add(tblTinNhan);
                    }
                }
                db.SaveChanges();
                tblNhomChat tblNhomChat = db.tblNhomChats.Find(toUserId);
                tblNhomChat.LastMsg = tblTinNhans.LastOrDefault().Id;
                tblNhomChat.Update_At = DateTime.Now;
                db.SaveChanges();
                ChatHub.UploadFilesGroup(db.tblChiTietNhomChats.Where(m => m.MaNhom == toUserId && m.User != user.Id).Select(m => m.tblUser.ConnectionId).ToList(), tblTinNhans);
                return PartialView(tblTinNhans);
            }
        }

        [HttpPost]
        public PartialViewResult SendMessage(long toUserId, string message, bool isChatNhom, List<HttpPostedFileBase> files)
        {
            if (isChatNhom == false)
            {
                tblUser user = db.tblUsers.Find(long.Parse(User.Identity.Name));
                DateTime a = DateTime.Now;
                db.tblTinNhans.Add(new tblTinNhan
                {
                    NguoiGui = user.Id,
                    NguoiNhan = toUserId,
                    Nhom = null,
                    ThoiGian = a,
                    NoiDung = message,
                    LoaiTinNhan = 1
                });
                db.SaveChanges();
                var tblTinNhan = db.tblTinNhans.SqlQuery("select * from tblTinNhan where FORMAT(ThoiGian , 'yyyy-MM-dd HH:mm:ss') ='" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", a) + "' and NguoiGui = " + user.Id + " and NguoiNhan = " + toUserId).FirstOrDefault();
                tblFriend tblFriend = db.tblFriends.Where(m => (m.NguoiGui == user.Id && m.NguoiNhan == toUserId) || (m.NguoiGui == toUserId && m.NguoiNhan == user.Id)).FirstOrDefault();
                if (tblFriend == null)
                {
                    db.tblFriends.Add(new tblFriend
                    {
                        NguoiGui = user.Id,
                        NguoiNhan = toUserId,
                        LastMsg = tblTinNhan.Id,
                        Update_At = a
                    });
                }
                else
                {
                    tblFriend.LastMsg = tblTinNhan.Id;
                    tblFriend.Update_At = a;
                }
                db.SaveChanges();
                ChatHub.RecieveMessage(tblTinNhan);
                return PartialView(tblTinNhan);

            }
            else
            {
                tblUser user = db.tblUsers.Find(long.Parse(User.Identity.Name));
                DateTime a = DateTime.Now;
                db.tblTinNhans.Add(new tblTinNhan
                {
                    NguoiGui = user.Id,
                    NguoiNhan = null,
                    Nhom = toUserId,
                    ThoiGian = a,
                    NoiDung = message
                });
                db.SaveChanges();
                var tblTinNhan = db.tblTinNhans.SqlQuery("select * from tblTinNhan where FORMAT(ThoiGian , 'yyyy-MM-dd HH:mm:ss') ='" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", a) + "' and NguoiGui = " + user.Id + " and Nhom = " + toUserId).FirstOrDefault();
                tblNhomChat tblNhomChat = db.tblNhomChats.Find(toUserId);
                tblNhomChat.LastMsg = tblTinNhan.Id;
                tblNhomChat.Update_At = a;
                db.SaveChanges();
                ChatHub.RecieveMessageGroup(db.tblChiTietNhomChats.Where(m => m.MaNhom == toUserId && m.User != user.Id).Select(m => m.tblUser.ConnectionId).ToList(), tblTinNhan);
                return PartialView(tblTinNhan);
            }
        }

        [HttpPost]
        public JsonResult AddUserToGroup(long Id, List<long> UserId)
        {
            foreach (var item in UserId)
            {
                tblChiTietNhomChat tblChiTietNhomChat = new tblChiTietNhomChat()
                {
                    User = item,
                    MaNhom = Id,
                    IsAdmin = false,
                };
                db.tblChiTietNhomChats.Add(tblChiTietNhomChat);
            }
            tblNhomChat tblNhomChat = db.tblNhomChats.Find(Id);
            tblNhomChat.Update_At = DateTime.Now;
            db.SaveChanges();
            ChatHub.AddUserToGroup(UserId);
            return Json(new { status = true });
        }

        public PartialViewResult LoadUserInGroup(long Id, string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return PartialView(db.tblChiTietNhomChats.Where(m => m.MaNhom == Id).OrderByDescending(m => m.IsAdmin).ToList());
            }
            else
            {
                string sql = "SELECT *, CONVERT(varchar,CONCAT(Ho, ' ', Ten)) as firstlast FROM tblUser join tblChiTietNhomChat on tblUser.Id = tblChiTietNhomChat.[User] WHERE CONVERT(varchar,CONCAT(Ho, ' ', Ten)) like '%" + s + "%' and MaNhom = " + Id;
                List<tblChiTietNhomChat> tblChiTietNhomChats = db.tblChiTietNhomChats.SqlQuery(sql).ToList();
                return PartialView(tblChiTietNhomChats.OrderByDescending(m => m.IsAdmin));
            }
        }

        public JsonResult DeleteUserFromGroup(long Id, List<long> UserId)
        {
            foreach (var item in UserId)
            {
                var tblChiTietNhomChat = db.tblChiTietNhomChats.Where(m => UserId.Contains(m.User) && m.MaNhom == Id);
                db.tblChiTietNhomChats.RemoveRange(tblChiTietNhomChat);
            }
            tblNhomChat tblNhomChat = db.tblNhomChats.Find(Id);
            tblNhomChat.Update_At = DateTime.Now;
            db.SaveChanges();
            ChatHub.DeleteUserFromGroup(Id, UserId);
            return Json(new { status = true });
        }

        [HttpGet]
        public JsonResult LoadInfoNhom(long Id)
        {
            var tblNhomChat = from x in db.tblNhomChats where x.Id == Id select new { Id = x.Id, TenNhom = x.TenNhom, Avatar = x.Avatar };
            return Json(new { data = tblNhomChat }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateThongTinNhom(tblNhomChat tblNhomChat)
        {
            long Id = long.Parse(User.Identity.Name);
            var tblNhom = db.tblNhomChats.Find(tblNhomChat.Id);
            if (tblNhomChat.ImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(tblNhomChat.ImageFile.FileName);
                string extension = Path.GetExtension(tblNhomChat.ImageFile.FileName);
                fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                tblNhom.Avatar = "/Images/NhomChat/" + fileName;
                fileName = Path.Combine(Server.MapPath("/Images/NhomChat/"), fileName);
                tblNhomChat.ImageFile.SaveAs(fileName);
            }
            tblNhom.TenNhom = tblNhomChat.TenNhom;
            db.SaveChanges();
            ChatHub.UpdateThongTinNhom(db.tblChiTietNhomChats.Where(m => m.MaNhom == tblNhomChat.Id && m.User != Id).Select(m => m.tblUser.ConnectionId).ToList(), tblNhomChat.Id);
            return Json(new { status = true });
        }

        [HttpPost]
        public JsonResult DeleteGroupChat(long Id)
        {
            long userId = long.Parse(User.Identity.Name);
            tblNhomChat tblNhomChat = db.tblNhomChats.Find(Id);
            tblNhomChat.LastMsg = null;
            db.SaveChanges();
            List<string> connectionIds = db.tblChiTietNhomChats.Where(m => m.MaNhom == Id && m.User != userId).Select(m => m.tblUser.ConnectionId).ToList();
            db.tblChiTietNhomChats.RemoveRange(db.tblChiTietNhomChats.Where(m => m.MaNhom == Id));
            db.tblTinNhans.RemoveRange(db.tblTinNhans.Where(m => m.Nhom == Id));
            db.tblNhomChats.Remove(tblNhomChat);
            db.SaveChanges();
            ChatHub.DeleteGroupChat(connectionIds, Id);
            return Json(new { status = true });
        }

        [HttpPost]
        public JsonResult LeaveGroupChat(long Id)
        {
            db.tblChiTietNhomChats.Remove(db.tblChiTietNhomChats.Find(long.Parse(User.Identity.Name), Id));
            db.SaveChanges();
            return Json(new { status = true });
        }

        [HttpPost]
        public JsonResult DeleteMsg(long Id)
        {
            long userId = long.Parse(User.Identity.Name);
            tblFriend tblFriend = db.tblFriends.Where(m => (m.NguoiGui == userId && m.NguoiNhan == Id) || (m.NguoiGui == Id && m.NguoiNhan == userId)).FirstOrDefault();
            tblFriend.LastMsg = null;
            db.SaveChanges();
            db.tblTinNhans.RemoveRange(db.tblTinNhans.Where(m => (m.NguoiGui == userId && m.NguoiNhan == Id) || (m.NguoiGui == Id && m.NguoiNhan == userId)));
            db.tblFriends.Remove(tblFriend);
            db.SaveChanges();
            ChatHub.DeleteMsg(Id, userId);
            return Json(new { status = true });
        }

        public FileResult Download(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/File/TinNhan/"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }
    }
}