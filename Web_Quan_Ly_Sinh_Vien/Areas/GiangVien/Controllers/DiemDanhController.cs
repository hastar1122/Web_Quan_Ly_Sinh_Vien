using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Areas.GiangVien.Controllers
{
    [Authorize(Roles = "Giảng Viên")]
    public class DiemDanhController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            var tblDiemDanhs = db.tblDiemDanhs.Where(m => m.MaBuoiDD == id);
            ViewBag.MaBuoiDD = id;
            return View(tblDiemDanhs.ToList());
        }

        [HttpPost]
        public JsonResult CapNhatDiemDanh(int MaBuoiDD, long MaSV)
        {
            try
            {
                var tblDiemDanh = db.tblDiemDanhs.Find(MaBuoiDD, MaSV);
                if (tblDiemDanh.DiemDanh == false)
                {
                    tblDiemDanh.DiemDanh = true;
                }
                else
                {
                    tblDiemDanh.DiemDanh = false;
                }
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult CapNhatDiemDiemDanh(int MaBuoiDD, long MaSV, double Diem)
        {
            try
            {
                var tblDiemDanh = db.tblDiemDanhs.Find(MaBuoiDD, MaSV);
                tblDiemDanh.Diem = Diem;
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult CapNhatGhiChuDiemDanh(int MaBuoiDD, string MaSV, string GhiChu)
        {
            try
            {
                var tblDiemDanh = db.tblDiemDanhs.Find(MaBuoiDD, MaSV);
                tblDiemDanh.GhiChu = GhiChu;
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