using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Hubs;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    public class ThaoLuanController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            ViewBag.MaLHP = id;
            return View();
        }

        public PartialViewResult LoadListBuoiThaoLuan(int id)
        {
            var tblNhomChats = from x in db.tblNhomChats where x.MaLHP == id select x;
            return PartialView(tblNhomChats.OrderByDescending(m => m.Update_At).ToList());
        }

        public JsonResult CreateBuoiThaoLuan(tblNhomChat tblNhomChat)
        {
            try
            {
                List<long> UserIds = new List<long>();
                long UserId = long.Parse(User.Identity.Name);
                DateTime a;
                tblNhomChat.Avatar = "/Images/NhomChat/avatar.png";
                tblNhomChat.Create_At = DateTime.Now;
                a = (DateTime)tblNhomChat.Create_At;
                tblNhomChat.Update_At = a;
                db.tblNhomChats.Add(tblNhomChat);
                tblChiTietNhomChat tblChiTietNhomChat = new tblChiTietNhomChat();
                tblChiTietNhomChat.MaNhom = tblNhomChat.Id;
                tblChiTietNhomChat.User = UserId;
                tblChiTietNhomChat.IsAdmin = true;
                db.tblChiTietNhomChats.Add(tblChiTietNhomChat);
                List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == tblNhomChat.MaLHP).ToList();
                foreach (var item in tblDiems)
                {
                    tblChiTietNhomChat tblChiTietNhomChat1 = new tblChiTietNhomChat();
                    tblChiTietNhomChat1.IsAdmin = false;
                    tblChiTietNhomChat1.MaNhom = tblNhomChat.Id;
                    tblChiTietNhomChat1.User = item.MaSV;
                    UserIds.Add(item.MaSV);
                    db.tblChiTietNhomChats.Add(tblChiTietNhomChat1);
                }
                List<tblUser> tblUsers = db.tblUsers.Where(m => m.Quyen == "Admin").ToList();
                foreach (var item in tblUsers)
                {
                    tblChiTietNhomChat tblChiTietNhomChat1 = new tblChiTietNhomChat();
                    tblChiTietNhomChat1.IsAdmin = false;
                    tblChiTietNhomChat1.MaNhom = tblNhomChat.Id;
                    tblChiTietNhomChat1.User = item.Id;
                    UserIds.Add(item.Id);
                    db.tblChiTietNhomChats.Add(tblChiTietNhomChat1);
                }
                db.SaveChanges();
                ChatHub.AddUserToGroup(UserIds);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(new { status = false });
            }
        }

        public PartialViewResult LoadThongTinBuoiThaoLuan (long? id)
        {
            return PartialView(db.tblChiTietNhomChats.Find(long.Parse(User.Identity.Name), id));
        }

        public PartialViewResult LoadDanhSachSinhVien (int? id)
        {
            if (id == null)
            {
                return null;
            }
            return PartialView(db.tblDiems.Where(m => m.MaLHP == id).ToList());
        }
    }
}