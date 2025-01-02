using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LopController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            ViewBag.MaNganh = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.MaNganh2 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.NganhHoc = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.KhoaHoc = new SelectList(db.tblLops.GroupBy(m => m.Khoa).Select(x => new { Khoa = x.Key }), "Khoa", "Khoa");
            return View();
        }

        public PartialViewResult LoadAllLop(int? MaNganh, string Khoa)
        {
            string sql = "Select * from tblLop where 1=1 ";
            if (MaNganh != null)
            {
                sql += " and MaNganh = " + MaNganh;
            }
            if (!string.IsNullOrWhiteSpace(Khoa))
            {
                sql += " and Khoa = '" + Khoa + "' ";
            }
            var tblLops = db.tblLops.SqlQuery(sql);
            return PartialView(tblLops.ToList());
        }

        [HttpPost]
        public JsonResult LoadLop(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.tblLops.Find(id));
        }

        public PartialViewResult Details(int? id)
        {
            if (id == null)
            {
                return null;
            }
            tblLop tblLop = db.tblLops.Find(id);
            return PartialView(tblLop);
        }

        public ActionResult DanhSachSinhVien(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.MaNganh2 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.MaLop = id;
            return View();
        }

        [HttpPost]
        public JsonResult Create([Bind(Include = "ID,MaNganh,TenLop,Khoa,NamHoc")] tblLop tblLop)
        {
            if (ModelState.IsValid)
            {
                db.tblLops.Add(tblLop);
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult Edit([Bind(Include = "ID,MaNganh,TenLop,Khoa,NamHoc")] tblLop tblLop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblLop).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                tblLop tblLop = db.tblLops.Find(id);
                db.tblLops.Remove(tblLop);
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