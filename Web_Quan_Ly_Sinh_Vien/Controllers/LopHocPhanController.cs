using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.ViewModels;

namespace Web_Quan_Ly_Sinh_Vien.Controllers
{
    [Authorize(Roles = "Sinh Viên")]
    public class LopHocPhanController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            ViewBag.HocPhan = new SelectList(db.tblHocPhans, "MaHP", "TenHP");
            ViewBag.NamHoc = new SelectList(db.tblLopHocPhans, "NamHoc", "NamHoc");
            ViewBag.HocKy = new SelectList(db.tblLopHocPhans.GroupBy(t => t.HocKy).Select(x => new { HocKy = x.Key }), "HocKy", "HocKy");
            return View();
        }

        public PartialViewResult LoadAllLopHocPhan1(string HocPhan, string NamHoc, string HocKy, string TrangThai)
        {
            string sql = "Select * from tblLopHocPhan join tblDiem on tblLopHocPhan.Id = tblDiem.MaLHP where MaSV = " + User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(HocKy))
            {
                sql += " and HocKy = " + HocKy;
            }
            if (!string.IsNullOrWhiteSpace(HocPhan))
            {
                sql += " and MaHP = " + HocPhan;
            }
            if (!string.IsNullOrWhiteSpace(NamHoc))
            {
                sql += " and NamHoc = '" + NamHoc + "' ";
            }
            if (!string.IsNullOrWhiteSpace(TrangThai))
            {
                if (TrangThai.Equals("Đã hoàn thành"))
                {
                    sql += " and TGKetThuc <= '" + DateTime.Now + "'";
                }
                if (TrangThai.Equals("Đang tham gia"))
                {
                    sql += " and TGKetThuc > '" + DateTime.Now + "'";
                }
            }
            var tblLopHocPhans = db.tblLopHocPhans.SqlQuery(sql);
            return PartialView(tblLopHocPhans.ToList());
        }

        public PartialViewResult LoadAllLopHocPhan2(string HocPhan, string NamHoc, string HocKy, string TrangThai)
        {
            string sql = "Select * from tblLopHocPhan join tblDiem on tblLopHocPhan.Id = tblDiem.MaLHP where MaSV = " + User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(HocKy))
            {
                sql += " and HocKy = " + HocKy;
            }
            if (!string.IsNullOrWhiteSpace(HocPhan))
            {
                sql += " and MaHP = " + HocPhan;
            }
            if (!string.IsNullOrWhiteSpace(NamHoc))
            {
                sql += " and NamHoc = '" + NamHoc + "' ";
            }
            if (!string.IsNullOrWhiteSpace(TrangThai))
            {
                if (TrangThai.Equals("Đã hoàn thành"))
                {
                    sql += " and TGKetThuc <= '" + DateTime.Now + "'";
                }
                if (TrangThai.Equals("Đang tham gia"))
                {
                    sql += " and TGKetThuc > '" + DateTime.Now + "'";
                }
            }
            var tblLopHocPhans = db.tblLopHocPhans.SqlQuery(sql);
            return PartialView(tblLopHocPhans.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            tblLopHocPhan tblLopHocPhan = db.tblLopHocPhans.Find(int.Parse(id));
            if (tblLopHocPhan == null)
            {
                return HttpNotFound();
            }
            return View(tblLopHocPhan);
        }

        public PartialViewResult LoadItem(string id)
        {
            return PartialView(db.tblLopHocPhans.Find(int.Parse(id)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult BangDiem(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.MaLHP = id;
            //var tblBaiTaps = db.tblBaiTaps.Where(m => m.MaLHP == id).OrderBy(m => m.NgayGiao).ToList();
            return View(db.tblLopHocPhans.Find(id));
        }

        public PartialViewResult LoadBangDiem(int? id)
        {
            return PartialView(db.tblDiems.Find(long.Parse(User.Identity.Name), id));
        }
    }
}