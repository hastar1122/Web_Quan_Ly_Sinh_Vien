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
    [Authorize(Roles ="Admin")]
    public class GiangVienController : Controller
    {
        Web_Quan_Ly_Sinh_VienEntities db = new Web_Quan_Ly_Sinh_VienEntities();

        public ActionResult Index()
        {
            ViewBag.MaNganh = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.MaNganh2 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.MaNganh3 = new SelectList(db.tblNganhHocs, "Id", "TenNganh");
            ViewBag.TrinhDo3 = new SelectList(db.tblUsers.GroupBy(m => m.TrinhDo).Where(m => m.Key != null).Select(x => new { TrinhDo = x.Key }), "TrinhDo", "TrinhDo");
            return View();
        }

        public PartialViewResult LoadAllGiangVien()
        {
            return PartialView(db.tblUsers.Where(m => m.Quyen == "Giảng Viên").ToList());
        }

        public PartialViewResult LoadAllGiangVien2(string MaNganh, string TrinhDo)
        {
            if (!String.IsNullOrWhiteSpace(MaNganh) && !String.IsNullOrWhiteSpace(TrinhDo))
            {
                int maNganh = int.Parse(MaNganh);
                return PartialView("LoadAllGiangVien", db.tblUsers.Where(m => m.MaNganh == maNganh && m.TrinhDo == TrinhDo && m.Quyen == "Giảng Viên").ToList());
            }
            else if (!String.IsNullOrWhiteSpace(MaNganh))
            {
                int maNganh = int.Parse(MaNganh);
                return PartialView("LoadAllGiangVien", db.tblUsers.Where(m => m.MaNganh == maNganh && m.Quyen == "Giảng Viên").ToList());
            }
            else if (!String.IsNullOrWhiteSpace(TrinhDo))
            {
                return PartialView("LoadAllGiangVien", db.tblUsers.Where(m => m.TrinhDo == TrinhDo && m.Quyen == "Giảng Viên").ToList());
            }
            else
            {
                return PartialView("LoadAllGiangVien", db.tblUsers.Where(m => m.Quyen == "Giảng Viên").ToList());
            }
        }

        [HttpPost]
        public JsonResult LoadGiangVien(string MaGV)
        {
            long Id = long.Parse(MaGV);
            var tblGiangVien = from x in db.tblUsers
                               where x.Id == Id
                               select new
                               {
                                   Id = x.Id,
                                   Ho = x.Ho,
                                   Ten = x.Ten,
                                   MaNganh = x.MaNganh,
                                   TrinhDo = x.TrinhDo,
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
        public JsonResult Create(tblUser tblGiangVien)
        {
            if (ModelState.IsValid)
            {
                tblUser test = db.tblUsers.FirstOrDefault(m => m.Email == tblGiangVien.Email);
                if (test != null)
                {
                    return Json(new { status = "Email này đã tồn tại" });
                }
                if (tblGiangVien.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblGiangVien.ImageFile.FileName);
                    string extension = Path.GetExtension(tblGiangVien.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblGiangVien.Anh = "/Images/GiangVien/" + fileName;
                    fileName = Path.Combine(Server.MapPath("/Images/GiangVien/"), fileName);
                    tblGiangVien.ImageFile.SaveAs(fileName);
                }
                else
                {
                    tblGiangVien.Anh = "/Images/GiangVien/avatar_2x.png";
                }
                if (tblGiangVien.MatKhau != null)
                {
                    tblGiangVien.MatKhau = Functions.Encrypt(tblGiangVien.MatKhau.ToString());
                }
                else
                {
                    tblGiangVien.MatKhau = Functions.Encrypt("1");
                }
                tblGiangVien.Quyen = "Giảng Viên";
                db.tblUsers.Add(tblGiangVien);
                db.SaveChanges();
                return Json(new { status = true });
            }

            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,Ho,Ten,MaNganh,TrinhDo,GioiTinh,NgaySinh,Email,CCCD,SDT,MatKhau,GhiChu,TrangThai,Anh,ImageFile")] tblUser tblGiangVien)
        {
            if (ModelState.IsValid)
            {
                tblUser test = db.tblUsers.FirstOrDefault(m => m.Email == tblGiangVien.Email && m.Id != tblGiangVien.Id );
                if (test != null)
                {
                    return Json(new { status = "Email này đã tồn tại" });
                }
                if (tblGiangVien.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tblGiangVien.ImageFile.FileName);
                    string extension = Path.GetExtension(tblGiangVien.ImageFile.FileName);
                    fileName += DateTime.Now.ToString("ddMMyyyy") + extension;
                    tblGiangVien.Anh = "/Images/GiangVien/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images/GiangVien/"), fileName);
                    tblGiangVien.ImageFile.SaveAs(fileName);
                }
                tblGiangVien.Quyen = "Giảng Viên";
                db.Entry(tblGiangVien).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = "Vui lòng nhập đầy đủ thông tin" });
        }

        [HttpPost]
        public JsonResult ResetPassword(string MaGV)
        {
            string pass = Functions.Encrypt("1");
            tblUser tblGiangVien = db.tblUsers.Find(long.Parse(MaGV));
            tblGiangVien.MatKhau = pass;
            db.Entry(tblGiangVien).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { status = true });
        }


        public JsonResult Delete(string id)
        {
            try
            {
                tblUser tblGiangVien = db.tblUsers.Find(long.Parse(id));
                db.tblUsers.Remove(tblGiangVien);
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
            var giangVien = db.tblUsers.Find(long.Parse(id));
            if (giangVien.TrangThai == true)
            {
                giangVien.TrangThai = false;
            }
            else
            {
                giangVien.TrangThai = true;
            }
            db.SaveChanges();
            return Json(new
            {
                status = giangVien.TrangThai
            });
        }

        public void ExportToExcel(string MaNganh4, string TrinhDo4)
        {
            var listGiangVien = db.tblUsers.Where(m => m.Quyen == "Giảng Viên").ToList();
            if (!String.IsNullOrWhiteSpace(MaNganh4) && !String.IsNullOrWhiteSpace(TrinhDo4))
            {
                int maNganh = int.Parse(MaNganh4);
                listGiangVien = db.tblUsers.Where(m => m.MaNganh == maNganh && m.TrinhDo == TrinhDo4 && m.Quyen == "Giảng Viên").ToList();
            }
            else if (!String.IsNullOrWhiteSpace(MaNganh4))
            {
                int maNganh = int.Parse(MaNganh4);
                listGiangVien = db.tblUsers.Where(m => m.MaNganh == maNganh && m.Quyen == "Giảng Viên").ToList();
            }
            else if (!String.IsNullOrWhiteSpace(TrinhDo4))
            {
                listGiangVien = db.tblUsers.Where(m => m.TrinhDo == TrinhDo4 && m.Quyen == "Giảng Viên").ToList();
            }

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Danh sách giảng viên");
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

            ws.Cells["B4:F4"].Value = "DANH SÁCH GIẢNG VIÊN";
            ws.Cells["B4:F4"].Merge = true;
            ws.Cells["B4:F4"].Style.Font.Size = 16;
            ws.Cells["B4:F4"].Style.Font.Bold = true;
            ws.Cells["B4:F4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            ws.Cells["A6"].Value = "Ngành giảng dạy: ";
            ws.Cells["A6"].Style.Font.Bold = true;
            ws.Cells["A7"].Value = "Trình độ: ";
            ws.Cells["A7"].Style.Font.Bold = true;
            if (!string.IsNullOrWhiteSpace(MaNganh4))
            {
                int maNganh = int.Parse(MaNganh4);
                string TenNganh = db.tblNganhHocs.Where(m => m.Id == maNganh).SingleOrDefault().TenNganh.ToString();
                ws.Cells["B6"].Value = TenNganh;
            }
            else
            {
                ws.Cells["B6"].Value = "Tất cả";
            }

            if (!string.IsNullOrWhiteSpace(TrinhDo4))
            {
                ws.Cells["B7"].Value = TrinhDo4;
            }
            else
            {
                ws.Cells["B7"].Value = "Tất cả";
            }

            ws.Cells["A9"].Value = "Họ đệm";
            ws.Cells["B9"].Value = "Tên";
            ws.Cells["C9"].Value = "Ngành giảng dạy";
            ws.Cells["D9"].Value = "Trình độ";
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
            foreach (var item in listGiangVien)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Ho;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Ten;
                if (item.MaNganh != null)
                {
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.tblNganhHoc.TenNganh;
                }
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.TrinhDo;
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
            HttpPostedFileBase excelFile = Request.Files["UploadedFile"];
            if (excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx"))
            {
                ExcelPackage pck = new ExcelPackage(excelFile.InputStream);
                ExcelWorksheet ws = pck.Workbook.Worksheets[0];
                List<tblUser> listGiangVien = new List<tblUser>();
                int r = 10;
                try
                {
                    for (r = 10; r <= ws.Dimension.End.Row; r++)
                    {
                        tblUser tblGiangVien = new tblUser();
                        if (ws.Cells[r, 1].Value != null)
                        {
                            tblGiangVien.Ho = ws.Cells[r, 1].Value.ToString();
                        }
                        if (ws.Cells[r, 2].Value != null)
                        {
                            tblGiangVien.Ten = ws.Cells[r, 2].Value.ToString();
                        }
                        if (ws.Cells[r, 3].Value != null)
                        {
                            int maNganh;
                            if (int.TryParse(ws.Cells[r, 3].Value.ToString(), out maNganh))
                            {
                                tblGiangVien.MaNganh = db.tblNganhHocs.FirstOrDefault(m => m.Id == maNganh).Id;
                            }
                            else
                            {
                                string tenNganh = ws.Cells[r, 3].Value.ToString();
                                tblGiangVien.MaNganh = db.tblNganhHocs.FirstOrDefault(m => m.TenNganh == tenNganh).Id;
                            }
                        }
                        if (ws.Cells[r, 4].Value != null)
                        {
                            tblGiangVien.TrinhDo = ws.Cells[r, 4].Value.ToString();
                        }
                        if (ws.Cells[r, 5].Value != null)
                        {
                            tblGiangVien.GioiTinh = ws.Cells[r, 5].Value.ToString();
                        }
                        if (ws.Cells[r, 6].Value != null)
                        {
                            try
                            {
                                DateTime dt;
                                string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                                                     "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", 
                                                     "dd/MM/yyyy hh:mm:ss tt","d/M/yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt","d/MM/yyyy hh:mm:ss tt",
                                                     "dd/MM/yy hh:mm:ss tt","dd/M/yy hh:mm:ss tt", "d/MM/yy hh:mm:ss tt", "d/M/yy hh:mm:ss tt"};
                                if (DateTime.TryParseExact(ws.Cells[r, 6].Value.ToString(), formats, null, System.Globalization.DateTimeStyles.None, out dt))
                                {
                                    tblGiangVien.NgaySinh = dt;
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
                        if (ws.Cells[r, 7].Value != null)
                        {
                            string email = ws.Cells[r, 7].Value.ToString();
                            if (db.tblUsers.FirstOrDefault(m => m.Email == email) != null)
                            {
                                return Json(new { status = "Email của giảng viên tại dòng thứ " + r + " cột " + 7 + " đã được sử dụng vui lòng kiểm tra lại file" });
                            }
                            else
                            {
                                tblGiangVien.Email = ws.Cells[r, 7].Value.ToString();
                            }
                        }
                        else
                        {
                            return Json(new { status = "Email không được bỏ trống" });
                        }
                        if (ws.Cells[r, 8].Value != null)
                        {
                            tblGiangVien.CCCD = ws.Cells[r, 8].Value.ToString();
                        }
                        if (ws.Cells[r, 9].Value != null)
                        {
                            tblGiangVien.SDT = ws.Cells[r, 9].Value.ToString();
                        }
                        tblGiangVien.MatKhau = Functions.Encrypt("1");
                        if (ws.Cells[r, 10].Value != null)
                        {
                            tblGiangVien.GhiChu = ws.Cells[r, 10].Value.ToString();
                        }
                        tblGiangVien.Anh = "/Images/GiangVien/avatar_2x.png";
                        tblGiangVien.Quyen = "Giảng Viên";
                        tblGiangVien.TrangThai = true;
                        listGiangVien.Add(tblGiangVien);
                    }
                    if (listGiangVien.Count() == 0)
                    {
                        return Json(new { status = "Không có dữ liệu vui lòng nhập dữ liệu vào file" });
                    }
                    db.tblUsers.AddRange(listGiangVien);
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
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Danh sách giảng viên");
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

            ws.Cells["B4:F4"].Value = "THÊM MỚI GIẢNG VIÊN";
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
            ws.Cells["C9"].Value = "Ngành giảng dạy";
            ws.Cells["D9"].Value = "Trình độ";
            ws.Cells["E9"].Value = "Giới tính";
            ws.Cells["F9"].Value = "Ngày sinh";
            ws.Cells["G9"].Value = "Email";
            ws.Cells["H9"].Value = "CCCD";
            ws.Cells["I9"].Value = "Số điện thoại";
            ws.Cells["J9"].Value = "Ghi chú";

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