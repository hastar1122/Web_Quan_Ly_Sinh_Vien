﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.ViewModels;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class LopHocPhanController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            // Dropdown list cho modal add
            ViewBag.MaGV = new SelectList(db.tblUsers.Where(m => m.Quyen == "Giảng Viên").Select(m => new { Id =m.Id, TenGV = m.Ho + " " +m.Ten}), "Id", "TenGV");
            ViewBag.MaHP = new SelectList(db.tblHocPhans, "MaHP", "TenHP");
            ViewBag.MaNganh = new SelectList(db.tblNganhHocs, "Id", "TenNganh");

            // Dropdown list cho modal update
            ViewBag.MaGV2 = new SelectList(db.tblUsers.Where(m => m.Quyen == "Giảng Viên").Select(m => new { Id = m.Id, TenGV = m.Ho + " " + m.Ten }), "Id", "TenGV");
            ViewBag.MaHP2 = new SelectList(db.tblHocPhans, "MaHP", "TenHP");
            ViewBag.MaNganh2 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");

            // Dropdown list để lọc lớp học phần
            ViewBag.MaNganh3 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.HocPhan3 = new SelectList(db.tblHocPhans, "MaHP", "TenHP");
            ViewBag.NamHoc3 = new SelectList(db.tblLopHocPhans.GroupBy(t => t.NamHoc).Select(x => new { NamHoc = x.Key}), "NamHoc", "NamHoc");
            ViewBag.HocKy3 = new SelectList(db.tblLopHocPhans.GroupBy(t => t.HocKy).Select(x => new { HocKy = x.Key }), "HocKy", "HocKy");
            return View();
        }

        public PartialViewResult LoadAllLopHocPhan1(string MaNganh, string HocPhan, string NamHoc, string HocKy)
        {
            string sql = "Select * from tblLopHocPhan where 1=1 ";
            if (!string.IsNullOrWhiteSpace(MaNganh))
            {
                sql +=  " and MaNganh = " + MaNganh;
            }
            if (!string.IsNullOrWhiteSpace(HocPhan))
            {
                sql +=  " and MaHP = " + HocPhan;
            }
            if (!string.IsNullOrWhiteSpace(NamHoc))
            {
                sql +=  " and NamHoc = '" + NamHoc + "' ";
            }
            if (!string.IsNullOrWhiteSpace(HocKy))
            {
                sql +=  " and HocKy = " + HocKy;
            }
            var tblLopHocPhans = db.tblLopHocPhans.SqlQuery(sql);
            return PartialView(tblLopHocPhans.ToList());
        }

        public PartialViewResult LoadAllLopHocPhan2(string MaNganh, string HocPhan, string NamHoc, string HocKy)
        {
            string sql = "Select * from tblLopHocPhan where 1=1 ";
            if (!string.IsNullOrWhiteSpace(MaNganh))
            {
                sql += " and MaNganh = " + MaNganh;
            }
            if (!string.IsNullOrWhiteSpace(HocPhan))
            {
                sql += " and MaHP = " + HocPhan;
            }
            if (!string.IsNullOrWhiteSpace(NamHoc))
            {
                sql += " and NamHoc = '" + NamHoc + "' ";
            }
            if (!string.IsNullOrWhiteSpace(HocKy))
            {
                sql += " and HocKy = " + HocKy;
            }
            var tblLopHocPhans = db.tblLopHocPhans.SqlQuery(sql);
            return PartialView(tblLopHocPhans.ToList());
        }

        [HttpPost]
        public JsonResult LoadLopHocPhan(int Id)
        {
            var tblLopHocPhan = from x in db.tblLopHocPhans
                                where x.Id == Id
                                select new
                                {
                                    Id = x.Id,
                                    MaNganh = x.MaNganh,
                                    MaHP = x.MaHP,
                                    TenLHP = x.TenLHP,
                                    MaGV = x.MaGV,
                                    TGBatDau = x.TGBatDau.ToString(),
                                    TGKetThuc = x.TGKetThuc.ToString(),
                                    NamHoc = x.NamHoc,
                                    HocKy = x.HocKy
                                };
            return Json(new { data = tblLopHocPhan });
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

        public PartialViewResult LoadItem (string id)
        {
            return PartialView(db.tblLopHocPhans.Find(int.Parse(id)));
        }

        [HttpPost]
        public JsonResult Create( tblLopHocPhan tblLopHocPhan)
        {
            if (tblLopHocPhan.TGBatDau >= tblLopHocPhan.TGKetThuc)
            {
                return Json(new { status = "Thời gian kết thúc không hợp lệ" });
            }
            if (ModelState.IsValid)
            {
                db.tblLopHocPhans.Add(tblLopHocPhan);
                db.SaveChanges();
                return Json(new { status = true });
            }

            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult Edit(tblLopHocPhan tblLopHocPhan)
        {
            if (tblLopHocPhan.TGBatDau >= tblLopHocPhan.TGKetThuc)
            {
                return Json(new { status = "Thời gian kết thúc không hợp lệ" });
            }
            if (ModelState.IsValid)
            {
                db.Entry(tblLopHocPhan).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        public JsonResult Delete(string id)
        {
            try
            {
                tblLopHocPhan tblLopHocPhan = db.tblLopHocPhans.Find(int.Parse(id));
                db.tblLopHocPhans.Remove(tblLopHocPhan);
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

        public ActionResult DanhSachSinhVienTrongLopHocPhan(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            int Id = int.Parse(id);
            ViewBag.MaLHP = Id;
            ViewBag.NganhHoc = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.LopHoc = new SelectList(db.tblLops, "ID", "TenLop");
            ViewBag.Khoa = new SelectList(db.tblLops.GroupBy(m => m.Khoa).Select(x => new { Khoa = x.Key }), "Khoa", "Khoa");
            ViewBag.NganhHoc2 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.LopHoc2 = new SelectList(db.tblLops, "ID", "TenLop");
            ViewBag.Khoa2 = new SelectList(db.tblLops.GroupBy(m => m.Khoa).Select(x => new { Khoa = x.Key }), "Khoa", "Khoa");
            return View(db.tblDiems.Where(m => m.MaLHP == Id));
        }

        public PartialViewResult DanhSachSinhVien(string MaLHP, string MaNganh, string Khoa, string MaLop)
        {
            int n = int.Parse(MaLHP);
            var tblSinhVien = db.tblDiems.Where(m => m.MaLHP == n).Select(m => m.MaSV).ToArray();
            if (!string.IsNullOrWhiteSpace(MaNganh) && !string.IsNullOrWhiteSpace(Khoa) && !string.IsNullOrWhiteSpace(MaLop))
            {
                int maNganh = int.Parse(MaNganh);
                int maLop = int.Parse(MaLop);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.MaNganh == maNganh && p.tblLop.Khoa == Khoa && p.MaLop == maLop).ToList());
            }
            else if (!string.IsNullOrWhiteSpace(MaNganh) && !string.IsNullOrWhiteSpace(Khoa) )
            {
                int maNganh = int.Parse(MaNganh);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.MaNganh == maNganh && p.tblLop.Khoa == Khoa).ToList());
            }
            else if (!string.IsNullOrWhiteSpace(MaNganh) && !string.IsNullOrWhiteSpace(MaLop))
            {
                int maNganh = int.Parse(MaNganh);
                int maLop = int.Parse(MaLop);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.MaNganh == maNganh && p.MaLop == maLop).ToList());
            }   
            else if (!string.IsNullOrWhiteSpace(Khoa) && !string.IsNullOrWhiteSpace(MaLop))
            {
                int maLop = int.Parse(MaLop);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.Khoa == Khoa && p.MaLop == maLop).ToList());
            }
            else if (!string.IsNullOrWhiteSpace(MaNganh))
            {
                int maNganh = int.Parse(MaNganh);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.MaNganh == maNganh).ToList());
            }
            else if (!string.IsNullOrWhiteSpace(Khoa))
            {
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.tblLop.Khoa == Khoa).ToList());
            }
            else if (!string.IsNullOrWhiteSpace(MaLop))
            {
                int maLop = int.Parse(MaLop);
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên" && p.MaLop == maLop).ToList());
            }
            else
            {
                return PartialView(db.tblUsers.Where(p => !tblSinhVien.Contains(p.Id) && p.Quyen == "Sinh Viên").ToList());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSinhVienOfClass(FormCollection f)
        {
            int MaLHP = int.Parse(f["MaLHP"].ToString());
            int MaLop = int.Parse(f["LopHoc"].ToString());
            List<tblUser> sinhViens = db.tblUsers.Where(m => m.MaLop == MaLop).ToList();
            try
            {
                foreach (var item in sinhViens)
                {
                    tblDiem tblDiem = new tblDiem();
                    tblDiem.MaLHP = MaLHP;
                    tblDiem.MaSV = item.Id;
                    db.tblDiems.Add(tblDiem);
                }
                db.SaveChanges();
                TempData["success"] = "Thêm sinh viên thành công";
            }
            catch
            {
                TempData["warning"] = "Thêm sinh viên không thành công";
                return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
            }
            return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSinhVien(int MaLHP, List<int> MaSV)
        {
            if (MaSV != null)
            {
                try
                {
                    foreach (var item in MaSV)
                    {
                        tblDiem tblDiem = new tblDiem();
                        tblDiem.MaLHP = MaLHP;
                        tblDiem.MaSV = item;
                        db.tblDiems.Add(tblDiem);
                    }
                    db.SaveChanges();
                    TempData["success"] = "Thêm sinh viên thành công";
                }
                catch
                {
                    TempData["warning"] = "Thêm sinh viên không thành công";
                    return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
                }
            }
            return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
        }

        public JsonResult LoadGiangVien(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                int maNganh = int.Parse(id);
                db.Configuration.ProxyCreationEnabled = false;
                var ListGV = db.tblUsers.Where(m => (m.MaNganh == maNganh || m.MaNganh == null) && m.Quyen == "Giảng Viên").Select(m => new { Id = m.Id, TenGV = m.Ho + " " + m.Ten }).ToList();
                return Json(new { list = ListGV.ToList() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Configuration.ProxyCreationEnabled = false;
                var ListGV = db.tblUsers.Where(m => m.Quyen == "Giảng Viên").Select(m => new { Id = m.Id, TenGV = m.Ho + " " + m.Ten }).ToList();
                return Json(new { list = ListGV.ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadLopHoc1(string MaNganh, string Khoa)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<tblLop> listLH = null;
            if (!string.IsNullOrWhiteSpace(MaNganh.ToString()) && !string.IsNullOrWhiteSpace(Khoa))
            {
                int maNganh = int.Parse(MaNganh);
                listLH = db.tblLops.Where(m => m.MaNganh == maNganh && m.Khoa == Khoa).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(MaNganh.ToString()))
            {
                int maNganh = int.Parse(MaNganh);
                listLH = db.tblLops.Where(m => m.MaNganh == maNganh).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(Khoa))
            {
                listLH = db.tblLops.Where(m => m.Khoa == Khoa).ToList();
            }
            else
            {
                listLH = db.tblLops.ToList();
            }

            return Json(new { list = listLH.ToList() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAllSinhVienFromLopHocPhan(int? MaLHP)
        {
            if (MaLHP == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                var tblDiem = db.tblDiems.Where(m => m.MaLHP == MaLHP).ToList();
                foreach (var item in tblDiem)
                {
                    db.tblDiems.Remove(item);
                }
                db.SaveChanges();
                TempData["success"] = "Xóa thành công";
                return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
            }
            catch
            {
                TempData["warning"] = "Xóa không thành công";
                return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSinhVienFromLopHocPhan(int? MaLHP, long MaSV)
        {
            if (MaLHP == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                tblDiem tblDiem = db.tblDiems.Find(MaSV, MaLHP);
                db.tblDiems.Remove(tblDiem);
                db.SaveChanges();
                TempData["success"] = "Xóa thành công";
                return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
            }
            catch
            {
                TempData["warning"] = "Xóa không thành công";
                return RedirectToAction("DanhSachSinhVienTrongLopHocPhan", "LopHocPhan", new { id = MaLHP });
            }
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
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == id).ToList();
            //List<BangDiemViewModel> bangDiemViewModels = new List<BangDiemViewModel>();
            //foreach (var item in tblDiems)
            //{
            //    BangDiemViewModel bangDiemViewModel = new BangDiemViewModel(
            //        item.MaSV,
            //        item.tblUser.Ho,
            //        item.tblUser.Ten,
            //        item.tblUser.MaSV,
            //        item.tblUser.tblLop.TenLop,
            //        null,
            //        item.DiemDD,
            //        item.DIemQT,
            //        item.GhiChu);
            //    bangDiemViewModels.Add(bangDiemViewModel);
            //}
            //foreach (var item in bangDiemViewModels)
            //{
            //    item.DiemBT = db.tblNopBaiTaps.Where(m => m.tblBaiTap.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBaiTap.NgayGiao).Select(m => m.Diem).ToList();
            //}
            return PartialView(tblDiems.ToList());
        }

        public void ExportToExcel(int? id)
        {
            var tblBaiTaps = db.tblBaiTaps.Where(m => m.MaLHP == id).OrderBy(m => m.NgayGiao).ToList();
            var tblLopHocPhan = db.tblLopHocPhans.Find(id);
            List<tblDiem> tblDiems = db.tblDiems.Where(m => m.MaLHP == id).ToList();
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
                    item.DiemDD,
                    item.DiemBT,
                    item.DIemQT,
                    item.GhiChu);
                bangDiemViewModels.Add(bangDiemViewModel);
            }
            foreach (var item in bangDiemViewModels)
            {
                item.DiemBT = db.tblNopBaiTaps.Where(m => m.tblBaiTap.MaLHP == id && m.MaSV == item.Id).OrderBy(m => m.tblBaiTap.NgayGiao).Select(m => m.Diem).ToList();
            }

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Bảng điểm quá trình");
            ws.Cells["A:AZ"].Style.Font.Name = "Times new roman";
            ws.Cells["A:AZ"].Style.Font.Size = 12;

            ws.Cells["A1:B1"].Value = "BỘ GIÁO DỤC VÀ ĐÀO TẠO";
            ws.Cells["A1:B1"].Merge = true;
            ws.Cells["A1:B1"].Style.Font.Size = 14;
            ws.Cells["F1:H1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
            ws.Cells["F1:H1"].Merge = true;
            ws.Cells["F1:H1"].Style.Font.Size = 14;

            ws.Cells["A2:C2"].Value = "Trường đại học giao thông vận tải";
            ws.Cells["A2:C2"].Merge = true;
            ws.Cells["A2:C2"].Style.Font.Size = 14;
            ws.Cells["A2:C2"].Style.Font.Bold = true;
            ws.Cells["A2:C2"].Style.Font.UnderLine = true;
            ws.Cells["F2:G2"].Value = "Độc lập - Tự do - Hạnh phúc";
            ws.Cells["F2:G2"].Merge = true;
            ws.Cells["F2:G2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["F2:G2"].Style.Font.Size = 14;
            ws.Cells["F2:G2"].Style.Font.UnderLine = true;

            ws.Cells["B4:F4"].Value = "DANH SÁCH SINH VIÊN";
            ws.Cells["B4:F4"].Merge = true;
            ws.Cells["B4:F4"].Style.Font.Size = 16;
            ws.Cells["B4:F4"].Style.Font.Bold = true;
            ws.Cells["B4:F4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["A6"].Value = "Học phần: ";
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "Tên lớp học phần: ";
            ws.Cells["A7"].Style.Font.Bold = true;
            ws.Cells["A8"].Value = "Thời gian: ";
            ws.Cells["A8"].Style.Font.Bold = true;
            ws.Cells["A9"].Value = "Giảng viên: ";
            ws.Cells["A9"].Style.Font.Bold = true;

            ws.Cells["B6:D6"].Merge = true;
            ws.Cells["B6:D6"].Value = tblLopHocPhan.tblHocPhan.TenHP;
            ws.Cells["B7:D7"].Merge = true;
            ws.Cells["B7:D7"].Value = tblLopHocPhan.TenLHP;
            ws.Cells["B8:D8"].Merge = true;
            ws.Cells["B8:D8"].Value = string.Format("{0:dd/MM/yyyy}", tblLopHocPhan.TGBatDau) + " đến " + string.Format("{0:dd/MM/yyyy}", tblLopHocPhan.TGKetThuc);
            ws.Cells["B9:D9"].Merge = true;
            ws.Cells["B9:D9"].Value = tblLopHocPhan.tblUser.Ho + " " + tblLopHocPhan.tblUser.Ten;

            ws.Cells[10, 1].Value = "STT";
            ws.Cells[10, 2].Value = "Họ đệm";
            ws.Cells[10, 3].Value = "Tên";
            ws.Cells[10, 4].Value = "Mã sinh viên";
            ws.Cells[10, 5].Value = "Lớp học";
            int colStart = 6;
            foreach(var item in tblBaiTaps)
            {
                ws.Cells[10, colStart].Value = item.TenBT;
                colStart++;
            }
            ws.Cells[10, colStart++].Value = "Điểm điểm danh";
            ws.Cells[10, colStart++].Value = "Điểm bài tập trung bình";
            ws.Cells[10, colStart++].Value = "Điểm quá trình";
            ws.Cells[10, colStart++].Value = "Ghi chú";

            ws.Cells["A10:W10"].Style.Font.Bold = true;
            ws.Cells["A10:W10"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["A10:W10"].Style.Font.Name = "Times new roman";

            int rowStart = 11;
            int i = 1;
            foreach (var item in bangDiemViewModels)
            {
                colStart = 6;
                ws.Cells[rowStart, 1].Value = i;
                ws.Cells[rowStart, 2].Value = item.Ho;
                ws.Cells[rowStart, 3].Value = item.Ten;
                ws.Cells[rowStart, 4].Value = item.MaSV;
                if (item.LopHoc != null)
                {
                    ws.Cells[rowStart, 5].Value = item.LopHoc;
                }
                foreach (var a in item.DiemBT)
                {
                    ws.Cells[rowStart, colStart].Value = a;
                    colStart++;
                }
                ws.Cells[rowStart, colStart++].Value = item.DiemDD;
                ws.Cells[rowStart, colStart++].Value = item.DiemBTTB;
                ws.Cells[rowStart, colStart++].Value = item.DIemQT;
                ws.Cells[rowStart, colStart++].Value = item.GhiChu;
                rowStart++;
                i++;
            }

            ws.Cells[rowStart + 1, 6, rowStart + 1, 7].Merge = true;
            ws.Cells[rowStart + 1, 6, rowStart + 1, 7].Style.Font.Italic = true;
            ws.Cells[rowStart + 1, 6, rowStart + 1, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            DateTime d = DateTime.Now;
            ws.Cells[rowStart + 1, 6, rowStart + 1, 7].Value = "Ngày " + d.Day + " Tháng " + d.Month + " Năm " + d.Year;

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename = " + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}