using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    [Authorize(Roles = "Sinh Viên")]
    public class BaiTapController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index(int? id)
        {
            ViewBag.MaLHP = id;
            long MaSV = long.Parse(User.Identity.Name);
            var tblBaiTaps = from x in db.tblBaiTaps join y in db.tblNopBaiTaps on x.Id equals y.MaBT where x.MaLHP == id && y.MaSV == MaSV select y;
            return View(tblBaiTaps.ToList());
        }

        public FileResult Download1(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/File/BaiTap/"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public FileResult Download2(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/File/NopBaiTap/"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpPost]
        public ActionResult NopBaiTap(HttpPostedFileBase file, int MaBT, long? MaSV)
        {
            var tblBaiTap = db.tblBaiTaps.SingleOrDefault(m => m.Id == MaBT);
            var tblNopBaiTap = db.tblNopBaiTaps.SingleOrDefault(m => m.MaBT == MaBT && m.MaSV == MaSV);
            if (file.ContentLength > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                tblNopBaiTap.BaiLam = /*"~/File/BaiTap/" +*/ fileName;
                fileName = Path.Combine(Server.MapPath("~/File/NopBaiTap/"), fileName);
                file.SaveAs(fileName);
            }
            tblNopBaiTap.NgayNop = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = tblBaiTap.MaLHP });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}