using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.Data;
using Web_Quan_Ly_Sinh_Vien.Services;
using System.Web.Security;
using System.IO;
using Web_Quan_Ly_Sinh_Vien.Hubs;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    [Authorize]
    public class HomeController : Controller 
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        // GET: Home
        [Authorize(Roles ="Sinh Viên")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult DangNhap()
        {
            if (User.Identity.IsAuthenticated)
            {
                long Id = long.Parse(User.Identity.Name);
                tblUser user = db.tblUsers.FirstOrDefault(m => m.Id == Id);
                if (user.Quyen == "Sinh Viên")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Quyen == "Admin")
                {
                    return RedirectToAction("Index", "Admin/Home");
                }
                else if (user.Quyen == "Giảng Viên")
                {
                    return RedirectToAction("Index", "GiangVien/Home");
                }
            }
            return View(new LoginData());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(LoginData loginData)
        {
            if (ModelState.IsValid)
            {
                tblUser user = new AppService().Login(loginData);
                if ( user != null)
                {
                    if (user.Quyen == "Sinh Viên")
                    {
                        FormsAuthentication.SetAuthCookie(user.Id.ToString(), loginData.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (user.Quyen == "Admin")
                    {
                        FormsAuthentication.SetAuthCookie(user.Id.ToString(), loginData.RememberMe);
                        return RedirectToAction("Index", "Admin/Home");
                    }
                    else if (user.Quyen == "Giảng Viên")
                    {
                        FormsAuthentication.SetAuthCookie(user.Id.ToString(), loginData.RememberMe);
                        return RedirectToAction("Index", "GiangVien/Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "Tài khoản hoặc mật khẩu không chính xác");
                }
            }
            
            return View();
        }

        [Authorize(Roles = "Sinh Viên")]
        public PartialViewResult _InfoUser()
        {
            long Id = long.Parse(User.Identity.Name);
            return PartialView(db.tblUsers.Find(Id));
        }

        [Authorize(Roles = "Sinh Viên")]
        public JsonResult ThongTinTaiKhoan()
        {
            db.Configuration.ProxyCreationEnabled = false;
            long Id = long.Parse(User.Identity.Name);
            var tblGiangVien = from x in db.tblUsers
                               where x.Id == Id
                               select new
                               {
                                   Id = x.Id,
                                   Ho = x.Ho,
                                   Ten = x.Ten,
                                   MaSV = x.MaSV,
                                   MaLop = x.tblLop.TenLop,
                                   GioiTinh = x.GioiTinh,
                                   NgaySinh = x.NgaySinh.ToString(),
                                   Email = x.Email,
                                   CCCD = x.CCCD,
                                   SDT = x.SDT,
                                   Anh = x.Anh.ToString(),
                               };
            return Json(new { data = tblGiangVien }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Sinh Viên")]
        [HttpPost]
        public JsonResult Edit(tblUser tblSinhVien)
        {

            tblUser tblUser = db.tblUsers.Find(tblSinhVien.Id);
            if (tblSinhVien.ImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(tblSinhVien.ImageFile.FileName);
                string extension = Path.GetExtension(tblSinhVien.ImageFile.FileName);
                fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                tblUser.Anh = "/Images/SinhVien/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/SinhVien/"), fileName);
                tblSinhVien.ImageFile.SaveAs(fileName);
            }
            tblUser.SDT = tblSinhVien.SDT;
            db.SaveChanges();
            return Json(new { status = true });
        }

        [Authorize(Roles = "Sinh Viên")]
        [HttpPost]
        public JsonResult ChangePassword()
        {
            string MatKhauCu = Request.Form["MatKhauCu"];
            string MatKhauMoi = Request.Form["MatKhauMoi"];
            tblUser tblUser = db.tblUsers.Find(long.Parse(User.Identity.Name));
            if (tblUser.MatKhau != Functions.Encrypt(MatKhauCu))
            {
                return Json(new { status = "Mật khẩu không chính xác" });
            }
            else
            {
                tblUser.MatKhau = Functions.Encrypt(MatKhauMoi);
                db.SaveChanges();
                return Json(new { status = true });
            }
        }

        public ActionResult DangXuat()
        {
            tblUser user = db.tblUsers.Find(long.Parse(User.Identity.Name));
            user.ConnectionId = null;
            db.SaveChanges();
            ChatHub.OfflineUser(user.Id);
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhap", "Home");
        }
    }
}