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
    public class NganhHocController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult LoadAllNganhHoc()
        {
            return PartialView(db.tblNganhHocs.ToList());
        }

        [HttpPost]
        public JsonResult LoadNganhHoc(string MaNganh)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.tblNganhHocs.Find(long.Parse(MaNganh)));
        }

        [HttpPost]
        public JsonResult CapNhatNganhHoc(tblNganhHoc tblNganhHoc)
        {
            if (ModelState.IsValid)
            {

                db.Entry(tblNganhHoc).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });

        }

        // POST: Admin/NganhHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(tblNganhHoc tblNganhHoc)
        {
            tblNganhHoc test = db.tblNganhHocs.Find(tblNganhHoc.Id);
            if (test != null)
            {
                return Json(new { status = "Mã ngành học này đã tồn tại" });
            }
            if (ModelState.IsValid)
            {
                db.tblNganhHocs.Add(tblNganhHoc);
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        public JsonResult Delete(string id)
        {
            try
            {
                tblNganhHoc tblNganhHoc = db.tblNganhHocs.Find(long.Parse(id));
                db.tblNganhHocs.Remove(tblNganhHoc);
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