using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.Services;

namespace Web_Quan_Ly_Sinh_Vien.Areas.GiangVien.Controllers
{
    [Authorize(Roles = "Giảng Viên")]
    public class HomeController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        // GET: GiangVien/Home
        public ActionResult Index()
        {
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
            var tblGiangVien = from x in db.tblUsers
                               where x.Id == Id
                               select new
                               {
                                   Id = x.Id,
                                   Ho = x.Ho,
                                   Ten = x.Ten,
                                   MaNganh = x.tblNganhHoc.TenNganh,
                                   TrinhDo = x.TrinhDo,
                                   GioiTinh = x.GioiTinh,
                                   NgaySinh = x.NgaySinh.ToString(),
                                   Email = x.Email,
                                   CCCD = x.CCCD,
                                   SDT = x.SDT,
                                   Anh = x.Anh.ToString(),
                               };
            return Json(new { data = tblGiangVien }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(tblUser tblGiangVien)
        {

            tblUser tblUser = db.tblUsers.Find(tblGiangVien.Id);
            if (tblGiangVien.ImageFile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(tblGiangVien.ImageFile.FileName);
                string extension = Path.GetExtension(tblGiangVien.ImageFile.FileName);
                fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                tblUser.Anh = "/Images/GiangVien/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images/GiangVien/"), fileName);
                tblGiangVien.ImageFile.SaveAs(fileName);
            }
            tblUser.SDT = tblGiangVien.SDT;
            db.SaveChanges();
            return Json(new { status = true });
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