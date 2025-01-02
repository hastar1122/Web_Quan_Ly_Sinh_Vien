using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.ViewModels;

namespace Web_Quan_Ly_Sinh_Vien.Areas.GiangVien.Controllers
{
    [Authorize(Roles = "Giảng Viên")]
    public class BuoiDiemDanhController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            ViewBag.MaLHP = id;
            return View(db.tblBuoiDiemDanhs.Where(m => m.MaLHP == id).OrderBy(m => m.NgayDD));
        }

        public PartialViewResult LoadBuoiDiemDanh(int? id)
        {
            //DiemDanhViewModel diemDanhViewModel = new DiemDanhViewModel();
            //List<DanhSachSvDiemDanh> DsSvDiemDanh = new List<DanhSachSvDiemDanh>();
            //List<tblBuoiDiemDanh> tblBuoiDiemDanhs = db.tblBuoiDiemDanhs.Include(t => t.tblLopHocPhan).Where(m => m.MaLHP == id).ToList();

            //diemDanhViewModel.tblBuoiDiemDanhs = tblBuoiDiemDanhs;

            //if (tblBuoiDiemDanhs.Count() > 0)
            //{
            //    int n = tblBuoiDiemDanhs[0].Id;

            //    List<tblDiemDanh> tblDiemDanh = db.tblDiemDanhs.Where(m => m.MaBuoiDD == n).OrderBy(m => m.MaSV).ToList();
            //    foreach (var item in tblDiemDanh)
            //    {
            //        DsSvDiemDanh.Add(new DanhSachSvDiemDanh(item.tblUser.MaSV, item.tblUser.Ho, item.tblUser.Ten, item.tblUser.tblLop.TenLop, item.tblUser.NgaySinh, item.DiemDanh));
            //    }
            //    for (int i = 1; i < tblBuoiDiemDanhs.Count(); i++)
            //    {
            //        int y = 0;
            //        n = tblBuoiDiemDanhs[i].Id;
            //        tblDiemDanh = db.tblDiemDanhs.Where(m => m.MaBuoiDD == n).OrderBy(m => m.MaSV).ToList();
            //        foreach (var item in tblDiemDanh)
            //        {
            //            DsSvDiemDanh[y].DiemDanh.Add(item.DiemDanh);
            //            y++;
            //        }
            //    }
            //}
            //diemDanhViewModel.danhSachSvDiemDanhs = DsSvDiemDanh;
            //return PartialView(diemDanhViewModel);
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == id).ToList();
            List<tblBuoiDiemDanh> tblBuoiDiemDanhs = db.tblBuoiDiemDanhs.Where(m => m.MaLHP == id).OrderBy(m => m.NgayDD).ToList();
            List<DiemDanhViewModel> diemDanhViewModels = new List<DiemDanhViewModel>();
            foreach (var item in tblDiems)
            {
                DiemDanhViewModel diemDanhViewModel = new DiemDanhViewModel(
                    item.MaSV,
                    item.tblUser.MaSV,
                    item.tblUser.Ho,
                    item.tblUser.Ten,
                    item.tblUser.tblLop.TenLop,
                    item.tblUser.NgaySinh,
                    null
                    );
                diemDanhViewModels.Add(diemDanhViewModel);
            }
            foreach (var item in diemDanhViewModels)
            {
                item.DiemDanh = db.tblDiemDanhs.Where(m => m.tblBuoiDiemDanh.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBuoiDiemDanh.NgayDD).Select(m => m.DiemDanh).ToList();
                if (item.DiemDanh.Count() == 0)
                {
                    for (int i = 0; i < tblBuoiDiemDanhs.Count(); i++)
                    {
                        tblDiemDanh tblDiemDanh = new tblDiemDanh();
                        tblDiemDanh.MaBuoiDD = tblBuoiDiemDanhs[i].Id;
                        tblDiemDanh.MaSV = item.Id;
                        tblDiemDanh.DiemDanh = false;
                        tblDiemDanh.Diem = 0;
                        db.tblDiemDanhs.Add(tblDiemDanh);
                    }
                    db.SaveChanges();
                    item.DiemDanh = db.tblDiemDanhs.Where(m => m.tblBuoiDiemDanh.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBuoiDiemDanh.NgayDD).Select(m => m.DiemDanh).ToList();
                }
            }
            return PartialView(diemDanhViewModels);
        }

        public PartialViewResult Details(int? id)
        {
            tblBuoiDiemDanh tblBuoiDiemDanh = db.tblBuoiDiemDanhs.Find(id);
            return PartialView(tblBuoiDiemDanh);
        }

        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            ViewBag.MaLHP = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblBuoiDiemDanh tblBuoiDiemDanh)
        {
            ViewBag.MaLHP = tblBuoiDiemDanh.MaLHP;
            if (tblBuoiDiemDanh.NgayDD >= tblBuoiDiemDanh.HanDD)
            {
                ModelState.AddModelError("HanDD", "Hạn điểm danh không hợp lệ");
                TempData["warning"] = "Thêm mới không thành công";
            }
            if (ModelState.IsValid)
            {
                db.tblBuoiDiemDanhs.Add(tblBuoiDiemDanh);
                db.SaveChanges();
                TempData["success"] = "Thêm mới thành công";
                return RedirectToAction("Index", new { id = tblBuoiDiemDanh.MaLHP });
            }
            return View("Create", tblBuoiDiemDanh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblBuoiDiemDanh tblBuoiDiemDanh)
        {
            ViewBag.MaLHP = tblBuoiDiemDanh.MaLHP;
            if (tblBuoiDiemDanh.NgayDD >= tblBuoiDiemDanh.HanDD)
            {
                ModelState.AddModelError("HanDD", "Hạn điểm danh không hợp lệ");
                TempData["warning"] = "Chỉnh sửa không thành công";
            }
            if (ModelState.IsValid)
            {
                db.Entry(tblBuoiDiemDanh).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Chỉnh sửa thành công";
                return RedirectToAction("Index", "DiemDanh", new { id = tblBuoiDiemDanh.Id });
            }
            return RedirectToAction("Index", "DiemDanh", new { id = tblBuoiDiemDanh.Id });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblBuoiDiemDanh tblBuoiDiemDanh = db.tblBuoiDiemDanhs.Find(id);
            int MaLHP = (int)tblBuoiDiemDanh.MaLHP;
            List<tblDiemDanh> tblDiemDanh = db.tblDiemDanhs.Where(m => m.MaBuoiDD == id).ToList();
            db.tblDiemDanhs.RemoveRange(tblDiemDanh);
            db.tblBuoiDiemDanhs.Remove(tblBuoiDiemDanh);
            db.SaveChanges();
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == MaLHP).ToList();
            List<tblBuoiDiemDanh> tblBuoiDiemDanhs = db.tblBuoiDiemDanhs.Where(m => m.MaLHP == MaLHP).ToList();
            if (tblBuoiDiemDanhs.Count() == 0)
            {
                foreach (var item in tblDiems)
                {
                    item.DiemDD = 0;
                }
                db.SaveChanges();
            }
            else
            {
                foreach (var item in tblDiems)
                {
                    double tongdiem = 0;
                    for (int i = 0; i < tblBuoiDiemDanhs.Count(); i++)
                    {
                        int n = tblBuoiDiemDanhs[i].Id;
                        tongdiem += double.Parse(db.tblDiemDanhs.Where(m => m.MaBuoiDD == n && m.MaSV == item.MaSV).SingleOrDefault().Diem.ToString());
                    }
                    item.DiemDD = tongdiem / tblBuoiDiemDanhs.Count();

                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = MaLHP });
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