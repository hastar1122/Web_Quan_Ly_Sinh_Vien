using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;
using Web_Quan_Ly_Sinh_Vien.Services;

namespace Web_Quan_Ly_Sinh_Vien.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SinhVienController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();
        public ActionResult Index()
        {
            // Dropdown list cho modal add
            ViewBag.MaLop = new SelectList(db.tblLops, "ID", "TenLop");

            // Dropdown list cho modal update
            ViewBag.MaLop2 = new SelectList(db.tblLops, "ID", "TenLop");

            // Dropdown list để lọc sinh viên
            ViewBag.NganhHoc = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.Khoa = new SelectList(db.tblLops.GroupBy(m => m.Khoa).Select(x => new { Khoa = x.Key }), "Khoa", "Khoa");
            ViewBag.LopHoc = new SelectList(db.tblLops, "ID", "TenLop");

            return View();
        }

        public PartialViewResult LoadAllSinhVien(int? NganhHoc, string Khoa, int? LopHoc)
        {
            string sql = "Select * from tbluser full join tblLop on tbluser.MaLop = tblLop.ID where Quyen ='Sinh Viên' and 1=1 ";
            if (NganhHoc != null)
            {
                sql += " and tblLop.MaNganh = " + NganhHoc;
            }
            if (!string.IsNullOrWhiteSpace(Khoa))
            {
                sql += " and Khoa = '" + Khoa + "' ";
            }
            if (LopHoc != null)
            {
                sql += " and MaLop = " + LopHoc;
            }
            var tblUsers = db.tblUsers.SqlQuery(sql);
            return PartialView(tblUsers.ToList()); ;
        }

        [HttpPost]
        public JsonResult LoadSinhVien(string MaSV)
        {
            long Id = long.Parse(MaSV);
            var tblGiangVien = from x in db.tblUsers
                               where x.Id == Id
                               select new
                               {
                                   Id = x.Id,
                                   Ho = x.Ho,
                                   Ten = x.Ten,
                                   MaSV = x.MaSV,
                                   MaLop = x.MaLop,
                                   NgaySinh = x.NgaySinh.ToString(),
                                   GioiTinh = x.GioiTinh,
                                   Email = x.Email,
                                   CCCD = x.CCCD,
                                   SDT = x.SDT,
                                   MatKhau = x.MatKhau,
                                   GhiChu = x.GhiChu,
                                   TrangThai = x.TrangThai,
                                   Anh = x.Anh.ToString(),
                               };
            return Json(new { data = tblGiangVien });
        }

        [HttpPost]
        public JsonResult Create(tblUser tblSinhVien)
        {
            if (ModelState.IsValid)
            {
                tblUser test = db.tblUsers.FirstOrDefault(m => m.Email == tblSinhVien.Email);
                if (test != null)
                {
                    return Json(new { status = "Email này đã tồn tại" });
                }
                if (tblSinhVien.MaSV != null)
                {
                    test = db.tblUsers.FirstOrDefault(m => m.MaSV == tblSinhVien.MaSV);
                    if (test != null)
                    {
                        return Json(new { status = "Mã sinh viên này đã tồn tại" });
                    }
                }
                if (tblSinhVien.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblSinhVien.ImageFile.FileName);
                    string extension = Path.GetExtension(tblSinhVien.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblSinhVien.Anh = "/Images/SinhVien/" + fileName;
                    fileName = Path.Combine(Server.MapPath("/Images/SinhVien/"), fileName);
                    tblSinhVien.ImageFile.SaveAs(fileName);
                }
                else
                {
                    tblSinhVien.Anh = "/Images/SinhVien/avatar_2x.png";
                }
                if (tblSinhVien.MatKhau != null)
                {
                    tblSinhVien.MatKhau = Functions.Encrypt(tblSinhVien.MatKhau.ToString());
                }
                else
                {
                    tblSinhVien.MatKhau = Functions.Encrypt("1");
                }
                tblSinhVien.Quyen = "Sinh Viên";
                db.tblUsers.Add(tblSinhVien);
                db.SaveChanges();
                return Json(new { status = true });
            }

            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult Edit(tblUser tblSinhVien)
        {
            if (ModelState.IsValid)
            {
                tblUser test = db.tblUsers.FirstOrDefault(m => m.Email == tblSinhVien.Email && m.Id != tblSinhVien.Id);
                if (test != null)
                {
                    return Json(new { status = "Email này đã tồn tại" });
                }
                if (tblSinhVien.MaSV != null)
                {
                    test = db.tblUsers.FirstOrDefault(m => m.MaSV == tblSinhVien.MaSV && m.Id != tblSinhVien.Id);
                    if (test != null)
                    {
                        return Json(new { status = "Mã sinh viên này đã tồn tại" });
                    }
                }
                if (tblSinhVien.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblSinhVien.ImageFile.FileName);
                    string extension = Path.GetExtension(tblSinhVien.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblSinhVien.Anh = "/Images/SinhVien/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images/SinhVien/"), fileName);
                    tblSinhVien.ImageFile.SaveAs(fileName);
                }
                tblSinhVien.Quyen = "Sinh Viên";
                db.Entry(tblSinhVien).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult ResetPassword(string MaSV)
        {
            string pass = Functions.Encrypt("1");
            tblUser tblSinhVien = db.tblUsers.Find(long.Parse(MaSV));
            tblSinhVien.MatKhau = pass;
            db.Entry(tblSinhVien).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { status = true });
        }


        public JsonResult Delete(string id)
        {
            try
            {
                tblUser tblSinhVien = db.tblUsers.Find(long.Parse(id));
                db.tblUsers.Remove(tblSinhVien);
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

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var tblSinhVien = db.tblUsers.Find(long.Parse(id));
            if (tblSinhVien.TrangThai == true)
            {
                tblSinhVien.TrangThai = false;
            }
            else
            {
                tblSinhVien.TrangThai = true;
            }
            db.SaveChanges();
            return Json(new
            {
                status = tblSinhVien.TrangThai
            });
        }

        public void ExportToExcel(int? NganhHoc2, string Khoa2, int? LopHoc2)
        {
            string sql = "Select * from tbluser join tblLop on tbluser.MaLop = tblLop.ID where Quyen ='Sinh Viên' and 1=1 ";
            if (NganhHoc2 != null)
            {
                sql += " and tblMaLop.MaNganh = " + NganhHoc2;
            }
            if (!string.IsNullOrWhiteSpace(Khoa2))
            {
                sql += " and Khoa = '" + Khoa2 + "' ";
            }
            if (LopHoc2 != null)
            {
                sql += " and MaLop = " + LopHoc2;
            }
            var listSinhVien = db.tblUsers.SqlQuery(sql);

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Danh sách sinh viên");
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

            ws.Cells["A6"].Value = "Ngành học: ";
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "Khóa: ";
            ws.Cells["A7"].Style.Font.Bold = true;
            ws.Cells["A8"].Value = "Lớp học: ";
            ws.Cells["A8"].Style.Font.Bold = true;
            if (NganhHoc2 != null)
            {
                string TenNganh = db.tblNganhHocs.Where(m => m.Id == NganhHoc2).SingleOrDefault().TenNganh.ToString();
                ws.Cells["B6"].Value = TenNganh;
            }
            else
            {
                ws.Cells["B6"].Value = "Tất cả";
            }

            if (!string.IsNullOrWhiteSpace(Khoa2))
            {
                ws.Cells["B7"].Value = Khoa2;
            }
            else
            {
                ws.Cells["B7"].Value = "Tất cả";
            }
            if (LopHoc2 != null)
            {
                var tblLop = db.tblLops.Where(m => m.ID == LopHoc2).SingleOrDefault();
                ws.Cells["B6"].Value = tblLop.tblNganhHoc.TenNganh;
                ws.Cells["B7"].Value = tblLop.Khoa;
                ws.Cells["B8"].Value = tblLop.TenLop;
            }
            else
            {
                ws.Cells["B8"].Value = "Tất cả";
            }

            ws.Cells["A9"].Value = "Họ đệm";
            ws.Cells["B9"].Value = "Tên";
            ws.Cells["C9"].Value = "Mã sinh viên";
            ws.Cells["D9"].Value = "Lớp học";
            ws.Cells["E9"].Value = "Giới tính";
            ws.Cells["F9"].Value = "Ngày sinh";
            ws.Cells["G9"].Value = "Email";
            ws.Cells["H9"].Value = "CCCD";
            ws.Cells["I9"].Value = "Số điện thoại";
            ws.Cells["J9"].Value = "Ghi chú";

            ws.Cells["A9:J9"].Style.Font.Bold = true;
            ws.Cells["A9:J9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["A9:J9"].Style.Font.Name = "Times new roman";

            int rowStart = 10;
            foreach (var item in listSinhVien)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Ho;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Ten;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.MaSV;
                if (item.MaLop != null)
                {
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.tblLop.TenLop;
                }
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.GioiTinh;
                ws.Cells[string.Format("F{0}", rowStart)].Value = string.Format("{0:dd/MM/yyyy}", item.NgaySinh);
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.Email;
                ws.Cells[string.Format("H{0}", rowStart)].Value = item.CCCD;
                ws.Cells[string.Format("I{0}", rowStart)].Value = item.SDT;
                ws.Cells[string.Format("J{0}", rowStart)].Value = item.GhiChu;
                rowStart++;
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

        [HttpPost]
        public JsonResult ImportFromExcel()
        {
            int id = int.Parse(Request.Form["id"].ToString());
            HttpPostedFileBase excelFile = Request.Files["UploadedFile"];
            if (excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx"))
            {
                ExcelPackage pck = new ExcelPackage(excelFile.InputStream);
                ExcelWorksheet ws = pck.Workbook.Worksheets[0];
                List<tblUser> listSinhVien = new List<tblUser>();
                int r = 10;
                try
                {
                    for (r = 10; r <= ws.Dimension.End.Row; r++)
                    {
                        tblUser tblSinhVien = new tblUser();
                        if (ws.Cells[r, 1].Value != null)
                        {
                            tblSinhVien.Ho = ws.Cells[r, 1].Value.ToString();
                        }
                        if (ws.Cells[r, 2].Value != null)
                        {
                            tblSinhVien.Ten = ws.Cells[r, 2].Value.ToString();
                        }
                        if (ws.Cells[r, 3].Value != null)
                        {
                            tblSinhVien.MaSV = ws.Cells[r, 3].Value.ToString();
                        }
                        tblSinhVien.MaLop = id;
                        if (ws.Cells[r, 4].Value != null)
                        {
                            tblSinhVien.GioiTinh = ws.Cells[r, 4].Value.ToString();
                        }
                        if (ws.Cells[r, 5].Value != null)
                        {
                            try
                            {
                                DateTime dt;
                                string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                                                     "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy",
                                                     "dd/MM/yyyy hh:mm:ss tt","d/M/yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt","d/MM/yyyy hh:mm:ss tt",
                                                     "dd/MM/yy hh:mm:ss tt","dd/M/yy hh:mm:ss tt", "d/MM/yy hh:mm:ss tt", "d/M/yy hh:mm:ss tt"};
                                if (DateTime.TryParseExact(ws.Cells[r, 5].Value.ToString(), formats, null, System.Globalization.DateTimeStyles.None, out dt))
                                {
                                    tblSinhVien.NgaySinh = dt;
                                }
                                else
                                {
                                    return Json(new { status = "Ngày sinh nhập không đúng định dạng tại dòng thứ " + r + " cột " + 6 + " (ngày/tháng/năm) VD: 19/1/2000" });
                                }
                            }
                            catch (Exception ex)
                            {
                                string s = ex.Message;
                            }
                        }
                        if (ws.Cells[r, 6].Value != null)
                        {
                            string email = ws.Cells[r, 6].Value.ToString();
                            if (db.tblUsers.FirstOrDefault(m => m.Email == email) != null)
                            {
                                return Json(new { status = "Email của sinh viên tại dòng thứ " + r + " cột " + 6 + " đã được sử dụng vui lòng kiểm tra lại file" });
                            }
                            else
                            {
                                tblSinhVien.Email = ws.Cells[r, 6].Value.ToString();
                            }
                        }
                        else
                        {
                            return Json(new { status = "Email không được bỏ trống" });
                        }
                        if (ws.Cells[r, 7].Value != null)
                        {
                            tblSinhVien.CCCD = ws.Cells[r, 7].Value.ToString();
                        }
                        if (ws.Cells[r, 8].Value != null)
                        {
                            tblSinhVien.SDT = ws.Cells[r, 8].Value.ToString();
                        }
                        tblSinhVien.MatKhau = Functions.Encrypt("1");
                        if (ws.Cells[r, 9].Value != null)
                        {
                            tblSinhVien.GhiChu = ws.Cells[r, 9].Value.ToString();
                        }
                        tblSinhVien.Anh = "/Images/SinhVien/avatar_2x.png";
                        tblSinhVien.Quyen = "Sinh Viên";
                        tblSinhVien.TrangThai = true;
                        listSinhVien.Add(tblSinhVien);
                    }
                    if (listSinhVien.Count() == 0)
                    {
                        return Json(new { status = "Không có dữ liệu vui lòng nhập dữ liệu vào file" });
                    }
                    db.tblUsers.AddRange(listSinhVien);
                    db.SaveChanges();
                    return Json(new { status = true });
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    return Json(new { status = "Đã có lỗi xảy ra vui lòng kiểm tra lại file" });
                }

            }
            else
            {
                return Json(new { status = "File không đúng định dạng! Hãy chọn file excel" });
            }
        }

        public void ExportFileMau()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Danh sách sinh viên");
            ws.Cells["A:AZ"].Style.Font.Name = "Times new roman";
            ws.Cells["A:AZ"].Style.Font.Size = 12;

            ws.Cells[1, 1, 1, 3].Value = "BỘ GIÁO DỤC VÀ ĐÀO TẠO";
            ws.Cells[1, 1, 1, 3].Merge = true;
            ws.Cells[1, 1, 1, 3].Style.Font.Size = 14;
            ws.Cells[1, 6, 1, 10].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
            ws.Cells[1, 6, 1, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 6, 1, 10].Merge = true;
            ws.Cells[1, 6, 1, 10].Style.Font.Size = 14;

            ws.Cells["A2:C2"].Value = "Trường đại học giao thông vận tải";
            ws.Cells["A2:C2"].Merge = true;
            ws.Cells["A2:C2"].Style.Font.Size = 14;
            ws.Cells["A2:C2"].Style.Font.Bold = true;
            ws.Cells["A2:C2"].Style.Font.UnderLine = true;
            ws.Cells[2, 6, 2, 10].Value = "Độc lập - Tự do - Hạnh phúc";
            ws.Cells[2, 6, 2, 10].Merge = true;
            ws.Cells[2, 6, 2, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[2, 6, 2, 10].Style.Font.Size = 14;
            ws.Cells[2, 6, 2, 10].Style.Font.UnderLine = true;

            ws.Cells["B4:F4"].Value = "THÊM MỚI SINH VIÊN";
            ws.Cells["B4:F4"].Merge = true;
            ws.Cells["B4:F4"].Style.Font.Size = 16;
            ws.Cells["B4:F4"].Style.Font.Bold = true;
            ws.Cells["B4:F4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["A6"].Value = "Lưu ý: ";
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells[6, 2, 6, 4].Merge = true;
            ws.Cells[6, 2, 6, 4].Value = "Điền thông tin theo cột dưới đây";
            ws.Cells[7, 2, 7, 5].Merge = true;
            ws.Cells[7, 2, 7, 5].Value = "Vui lòng điền đầy thông tin theo mẫu!";

            ws.Cells["A9"].Value = "Họ đệm";
            ws.Cells["B9"].Value = "Tên";
            ws.Cells["C9"].Value = "Mã sinh viên";
            ws.Cells["D9"].Value = "Giới tính";
            ws.Cells["E9"].Value = "Ngày sinh";
            ws.Cells["F9"].Value = "Email";
            ws.Cells["G9"].Value = "CCCD";
            ws.Cells["H9"].Value = "Số điện thoại";
            ws.Cells["I9"].Value = "Ghi chú";

            ws.Cells["A9:K9"].Style.Font.Bold = true;
            ws.Cells["A9:K9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells["A9:K9"].Style.Font.Name = "Times new roman";

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename = " + "FileMau.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}