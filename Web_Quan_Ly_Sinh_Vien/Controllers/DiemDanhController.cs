using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    [Authorize(Roles = "Sinh Viên")]
    public class DiemDanhController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index(int? id)
        {
            ViewBag.MaLHP = id;
            return View();
        }

        public PartialViewResult LoadAllBuoiDiemDanh(int? id)
        {
            long MaSV = long.Parse(User.Identity.Name);
            var tblBuoiDiemDanh = from x in db.tblDiemDanhs join y in db.tblBuoiDiemDanhs on x.MaBuoiDD equals y.Id where y.MaLHP == id && x.MaSV == MaSV && x.DiemDanh == false && (DateTime.Now < y.HanDD || y.HanDD == null) select x;
            return PartialView(tblBuoiDiemDanh.ToList());
        }

        [HttpPost]
        public JsonResult DiemDanh(int MaBuoiDD, long MaSV)
        {
            try
            {
                var tblDiemDanh = db.tblDiemDanhs.Find(MaBuoiDD, MaSV);
                tblDiemDanh.DiemDanh = true;
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }
        }
    }
}