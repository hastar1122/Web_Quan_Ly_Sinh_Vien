using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.Services;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        // GET: Admin/Home
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.TongSoNganhHoc = db.tblNganhHocs.Count();
            ViewBag.TongSoHocPhan = db.tblHocPhans.Count();
            ViewBag.TongSoGiangVien = db.tblUsers.Where(m => m.Quyen == "Giảng Viên").Count();
            ViewBag.TongSoLopHocPhan = db.tblLopHocPhans.Count();
            ViewBag.TongSoLop = db.tblLops.Count();
            ViewBag.TongSoSinhVien = db.tblUsers.Where(m => m.Quyen == "Sinh Viên").Count();
            return View();
        }

        public PartialViewResult _InfoUser()
        {
            long Id = long.Parse(User.Identity.Name);
            return PartialView(db.tblUsers.Find(Id));
        }

        public JsonResult ThongTinTaiKhoan()
        {
            db.Configuration.ProxyCreationEnabled = false;
            long Id = long.Parse(User.Identity.Name);
            var tblUser = from x in db.tblUsers
                               where x.Id == Id
                               select new
                               {
                                   Id = x.Id,
                                   Ho = x.Ho,
                                   Ten = x.Ten,
                                   GioiTinh = x.GioiTinh,
                                   NgaySinh = x.NgaySinh.ToString(),
                                   Email = x.Email,
                                   CCCD = x.CCCD,
                                   SDT = x.SDT,
                                   Anh = x.Anh.ToString(),
                               };
            return Json(new { data = tblUser }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(tblUser tblAdmin)
        {
            if (ModelState.IsValid)
            {
                tblUser test = db.tblUsers.FirstOrDefault(m => m.Email == tblAdmin.Email && m.Id != tblAdmin.Id);
                if (test != null)
                {
                    return Json(new { status = "Email này đã tồn tại" });
                }
                tblUser tblUser = db.tblUsers.Find(tblAdmin.Id);
                if (tblAdmin.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblAdmin.ImageFile.FileName);
                    string extension = Path.GetExtension(tblAdmin.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblUser.Anh = "/Images/Admin/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images/Admin/"), fileName);
                    tblAdmin.ImageFile.SaveAs(fileName);
                }
                tblUser.Ho = tblAdmin.Ho;
                tblUser.Ten = tblAdmin.Ten;
                tblUser.GioiTinh = tblAdmin.GioiTinh;
                tblUser.NgaySinh = tblAdmin.NgaySinh;
                tblUser.Email = tblAdmin.Email;
                tblUser.CCCD = tblAdmin.CCCD;
                tblUser.SDT = tblAdmin.SDT;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

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
    }
}