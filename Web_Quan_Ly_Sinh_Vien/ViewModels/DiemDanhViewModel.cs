using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Web_Quan_Ly_Sinh_Vien.Models.DataBase;

namespace Web_Quan_Ly_Sinh_Vien.ViewModels
{
    //public class DiemDanhViewModel
    //{
    //    public List<tblBuoiDiemDanh> tblBuoiDiemDanhs { get; set; }
    //    public List<DanhSachSvDiemDanh> danhSachSvDiemDanhs { get; set; }
    //}

    //public class DanhSachSvDiemDanh
    //{

    //    public string MaSV { get; set; }
    //    public string HoSV { get; set; }
    //    public string TenSV { get; set; }
    //    public string TenLop { get; set; }

    //    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
    //    public Nullable<DateTime> NgaySinh { get; set; }

    //    public List<bool> DiemDanh = new List<bool>();

    //    public DanhSachSvDiemDanh(string maSV, string hoSV, string tenSV, string tenLop, DateTime? ngaySinh, bool diemDanh)
    //    {
    //        MaSV = maSV;
    //        HoSV = hoSV;
    //        TenSV = tenSV;
    //        TenLop = tenLop;
    //        NgaySinh = ngaySinh;
    //        if (diemDanh == false)
    //        {
    //            DiemDanh.Add(false);
    //        }
    //        else
    //        {
    //            DiemDanh.Add(true);
    //        }
    //    }
    //}

    public class DiemDanhViewModel {

        public DiemDanhViewModel() { }

        public DiemDanhViewModel(long id, string maSV, string hoSV, string tenSV, string tenLop, DateTime? ngaySinh, List<bool> diemDanh)
        {
            Id = id;
            MaSV = maSV;
            HoSV = hoSV;
            TenSV = tenSV;
            TenLop = tenLop;
            NgaySinh = ngaySinh;
            DiemDanh = diemDanh;
        }

        public long Id { get; set; }
        public string MaSV { get; set; }
        public string HoSV { get; set; }
        public string TenSV { get; set; }
        public string TenLop { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public Nullable<DateTime> NgaySinh { get; set; }

        public List<bool> DiemDanh = new List<bool>();

    }
}