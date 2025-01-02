using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.ViewModels
{
    public class BangDiemViewModel
    {
        public BangDiemViewModel(long id, string ho, string ten, string maSV, string lopHoc, List<Nullable<double>> diemBT, double? diemDD, double? diemBTTB, double? dIemQT, string ghiChu)
        {
            Id = id;
            Ho = ho;
            Ten = ten;
            MaSV = maSV;
            LopHoc = lopHoc;
            DiemBT = diemBT;
            DiemDD = diemDD;
            DiemBTTB = diemBTTB;
            DIemQT = dIemQT;
            GhiChu = ghiChu;
        }

        public BangDiemViewModel(long id, string ho, string ten, string maSV, string lopHoc, List<double?> diemBT, double? diemBTTB)
        {
            Id = id;
            Ho = ho;
            Ten = ten;
            MaSV = maSV;
            LopHoc = lopHoc;
            DiemBT = diemBT;
            DiemBTTB = diemBTTB;
        }

        public BangDiemViewModel() { }
        public long Id { get; set; }

        [Required]
        [Display(Name = "Họ đệm")]
        public string Ho { get; set; }

        [Required]
        [Display(Name = "Tên")]
        public string Ten { get; set; }

        [Display(Name = "Mã sinh viên")]
        public string MaSV { get; set; }

        [Display(Name = "Lớp học")]
        public string LopHoc { get; set; }

        public List<Nullable<double>> DiemBT { get; set; }

        [Display(Name = "Điểm điểm danh")]
        public Nullable<double> DiemDD { get; set; }

        [Display(Name = "Điểm bài tập trung bình")]
        public Nullable<double> DiemBTTB { get; set; }

        [Display(Name = "Điểm quá trình")]
        public Nullable<double> DIemQT { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}