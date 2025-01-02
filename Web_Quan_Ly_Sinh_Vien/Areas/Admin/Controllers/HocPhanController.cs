using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HocPhanController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult LoadAllHocPhan()
        {
            return PartialView(db.tblHocPhans.ToList());
        }

        [HttpPost]
        public JsonResult LoadHocPhan(string MaHP)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.tblHocPhans.Find(int.Parse(MaHP)));
        }

        [HttpPost]
        public JsonResult CapNhatHocPhan(tblHocPhan tblHocPhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblHocPhan).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });

        }

        [HttpPost]
        public JsonResult Create(tblHocPhan tblHocPhan)
        {
            tblHocPhan test = db.tblHocPhans.Find(tblHocPhan.MaHP);
            if (test != null)
            {
                return Json(new { status = "Mã học phần này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                db.tblHocPhans.Add(tblHocPhan);
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        public JsonResult Delete(string id)
        {
            try
            {
                tblHocPhan tblHocPhan = db.tblHocPhans.Find(int.Parse(id));
                db.tblHocPhans.Remove(tblHocPhan);
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }
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