using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.ViewModels;

namespace Web_Quan_Ly_Sinh_Vien.Areas.GiangVien.Controllers
{
    [Authorize(Roles = "Giảng Viên")]
    public class BaiTapController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            tblLopHocPhan tblLopHocPhan = db.tblLopHocPhans.Find(id);
            ViewBag.MaLHP = id;
            var tblBaiTaps = db.tblBaiTaps.Include(t => t.tblLopHocPhan).Where(m => m.MaLHP == id);
            return View(tblBaiTaps.ToList());
        }

        // Download file bài tập
        public FileResult Download(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/File/BaiTap/"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public ActionResult BangDiemBaiTap(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            ViewBag.MaLHP = id;
            return View(db.tblBaiTaps.Where(m => m.MaLHP == id).OrderBy(m => m.NgayGiao));
        }

        [HttpPost]
        public JsonResult LoadTyLe(int? id)
        {
            List<Nullable<double>> tyLes = db.tblBaiTaps.Where(m => m.MaLHP == id).Select(m => m.TyLe).ToList();
            return Json(new { data = tyLes});
        }

        [HttpPost]
        public JsonResult UpdateDiem (int? id, List<double> TyLe)
        {
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == id).OrderBy(m => m.MaSV).ToList();
            List<tblBaiTap> tblBaiTaps = db.tblBaiTaps.Where(m => m.MaLHP == id).OrderBy(m => m.NgayGiao).ToList();
            double[] a = new double[tblDiems.Count];
            for (int i = 0; i <tblBaiTaps.Count(); i++)
            {
                tblBaiTaps[i].TyLe = TyLe[i];
                int MabT = tblBaiTaps[i].Id;
                List<Nullable<double>> tblNopBaiTaps = db.tblNopBaiTaps.Where(m => m.MaBT == MabT).OrderBy(m => m.MaSV).Select(m => m.Diem).ToList();
                for (int j = 0; j< tblNopBaiTaps.Count(); j++)
                {
                    if (tblNopBaiTaps[j] != null)
                    {
                        double b = (double)tblNopBaiTaps[j];
                        a[j] += b * TyLe[i];
                    }
                    else
                    {
                        a[j] += 0;
                    }
                }
            }
            for (int i =0; i< tblDiems.Count(); i++)
            {
                tblDiems[i].DiemBT = a[i];
            }
            db.SaveChanges();
            return Json(new { status = true });
        }

        public PartialViewResult LoadBangDiemBaiTap (int? id)
        {
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == id).ToList();
            List<tblBaiTap> tblBaiTaps = db.tblBaiTaps.Where(m => m.MaLHP == id).OrderBy(m => m.NgayGiao).ToList();
            List<BangDiemViewModel> bangDiemViewModels = new List<BangDiemViewModel>();
            foreach (var item in tblDiems)
            {
                BangDiemViewModel bangDiemViewModel = new BangDiemViewModel(
                    item.MaSV,
                    item.tblUser.Ho,
                    item.tblUser.Ten,
                    item.tblUser.MaSV,
                    item.tblUser.tblLop.TenLop,
                    null,
                    item.DiemBT);
                bangDiemViewModels.Add(bangDiemViewModel);
            }
            foreach (var item in bangDiemViewModels)
            {
                item.DiemBT = db.tblNopBaiTaps.Where(m => m.tblBaiTap.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBaiTap.NgayGiao).Select(m => m.Diem).ToList();
                if (item.DiemBT.Count() == 0)
                {
                    List<Nullable<double>> a = new List<double?>();
                    for (int i = 0; i< tblBaiTaps.Count(); i++)
                    {
                        tblNopBaiTap tblNopBaiTap = new tblNopBaiTap();
                        tblNopBaiTap.MaBT = tblBaiTaps[i].Id;
                        tblNopBaiTap.MaSV = item.Id;
                        db.tblNopBaiTaps.Add(tblNopBaiTap);
                    }
                    db.SaveChanges();
                    item.DiemBT = db.tblNopBaiTaps.Where(m => m.tblBaiTap.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBaiTap.NgayGiao).Select(m => m.Diem).ToList();
                }
            }
            return PartialView(bangDiemViewModels);
        }

        public PartialViewResult Details(int? id)
        {
            tblBaiTap tblBaiTap = db.tblBaiTaps.Find(id);
            var tongSV = db.tblNopBaiTaps.Where(m => m.MaBT == id);
            var tongSVDaNop = db.tblNopBaiTaps.Where(m => m.MaBT == id && m.NgayNop != null);
            var tongSvDaCham = tongSVDaNop.Where(m => m.Diem != null).OrderByDescending(m => m.Diem);
            ViewBag.TongSV = tongSV.Count();
            ViewBag.TongSvDaNop = tongSVDaNop.Count();
            ViewBag.TongSvDaCham = tongSvDaCham.Count();
            if (tongSvDaCham.Count() != 0)
            {
                ViewBag.DiemCaoNhat = tongSvDaCham.FirstOrDefault().Diem;
                ViewBag.DiemThapNhat = tongSvDaCham.OrderBy(m => m.Diem).FirstOrDefault().Diem;
                ViewBag.DiemTrungBinh = tongSvDaCham.Sum(m => m.Diem) / tongSvDaCham.Count();
            }
            else
            {
                ViewBag.DiemCaoNhat = 0;
                ViewBag.DiemThapNhat = 0;
                ViewBag.DiemTrungBinh = 0;
            }
            return PartialView(tblBaiTap);
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
        public ActionResult Create(tblBaiTap tblBaiTap)
        {
            if (tblBaiTap.NgayGiao >= tblBaiTap.HanNop)
            {
                ModelState.AddModelError("HanNop", "Thời gian nộp không hợp lệ");
            }
            if (ModelState.IsValid)
            {
                if (tblBaiTap.File != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblBaiTap.File.FileName);
                    string extension = Path.GetExtension(tblBaiTap.File.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblBaiTap.Link = /*"~/File/BaiTap/" +*/ fileName;
                    fileName = Path.Combine(Server.MapPath("~/File/BaiTap/"), fileName);
                    tblBaiTap.File.SaveAs(fileName);
                }
                tblBaiTap.NgayGiao = DateTime.Now;
                db.tblBaiTaps.Add(tblBaiTap);
                db.SaveChanges();
                TempData["success"] = "Thêm mới thành công";
                return RedirectToAction("Index", new { id = tblBaiTap.MaLHP });
            }
            return View(tblBaiTap);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBaiTap tblBaiTap = db.tblBaiTaps.Find(id);
            if (tblBaiTap == null)
            {
                return HttpNotFound();
            }
            return View(tblBaiTap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblBaiTap tblBaiTap)
        {
            if (ModelState.IsValid)
            {
                if (tblBaiTap.File != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblBaiTap.File.FileName);
                    string extension = Path.GetExtension(tblBaiTap.File.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblBaiTap.Link = /*"~/File/BaiTap/" +*/ fileName;
                    fileName = Path.Combine(Server.MapPath("~/File/BaiTap/"), fileName);
                    tblBaiTap.File.SaveAs(fileName);
                }
                db.Entry(tblBaiTap).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Chỉnh sửa thành công";
                return RedirectToAction("Index", new { id = tblBaiTap.MaLHP });
            }
            ViewBag.MaLHP = new SelectList(db.tblLopHocPhans, "MaLHP", "MaHP", tblBaiTap.MaLHP);
            return View(tblBaiTap);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            tblBaiTap tblBaiTap = db.tblBaiTaps.Find(id);
            int n = (int)tblBaiTap.MaLHP;
            var sinhVienNopBT = db.tblNopBaiTaps.Where(m => m.MaBT == id);
            db.tblNopBaiTaps.RemoveRange(sinhVienNopBT);
            db.tblBaiTaps.Remove(tblBaiTap);
            db.SaveChanges();
            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index", new { id = n });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DanhSachSinhVienNopBaiTap(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "LopHocPhan");
            }
            var tblNopBaiTaps = db.tblNopBaiTaps.Where(m => m.MaBT == id);
            ViewBag.MaBT = id;
            return View(tblNopBaiTaps.ToList());
        }

        // Download file nộp bài tập của sinh viên
        public FileResult Download2(string filename)
        {
            string fullPath = Path.Combine(Server.MapPath("~/File/NopBaiTap/"), filename);
            
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpPost]
        public JsonResult CapNhatDiem(int MaBT, long MaSV, double Diem)
        {
            try
            {
                var tblNopBaiTap = db.tblNopBaiTaps.Find(MaBT, MaSV);
                tblNopBaiTap.Diem = Diem;
                db.SaveChanges();
                return Json(new { status = true });
            }
            catch
            {
                return Json(new { status = false });
            }

        }

        [HttpPost]
        public JsonResult CapNhatNhanXet(int MaBT, long MaSV, string NhanXet)
        {
            try
            {
                var tblNopBaiTap = db.tblNopBaiTaps.Find(MaBT, MaSV);
                tblNopBaiTap.NhanXet = NhanXet;
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