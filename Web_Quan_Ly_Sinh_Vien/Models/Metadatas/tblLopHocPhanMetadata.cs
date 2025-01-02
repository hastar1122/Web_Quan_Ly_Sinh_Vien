using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblLopHocPhanMetadata))]
    public partial class tblLopHocPhan
    {
        internal sealed class tblLopHocPhanMetadata
        {
            [Display(Name = "Mã lớp")]
            public int Id { get; set; }
            [Required(ErrorMessage ="Vui lòng chọn học phần")]
            [DisplayName("Học phần")]
            public string MaHP { get; set; }
            //[Required(ErrorMessage ="Vui lòng chọn ngành học")]
            [Display(Name ="Ngành học")]
            public string MaNganh { get; set; }
            [Display(Name ="Tên lớp học phần")]
            public string TenLHP { get; set; }
            [Required(ErrorMessage ="Vui lòng chọn giảng viên phụ trách")]
            [Display(Name ="Giảng viên phụ trách")]
            public string MaGV { get; set; }
            [Required(ErrorMessage ="Bạn chưa nhập thời gian bắt đầu")]
            [Display(Name = "Thời gian bắt đầu")]
            [DataType(DataType.Date, ErrorMessage = "Date only")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
            public Nullable<System.DateTime> TGBatDau { get; set; }
            [Required(ErrorMessage = "Bạn chưa nhập thời gian kết thúc")]
            [Display(Name = "Thời gian kết thúc")]
            [DataType(DataType.Date, ErrorMessage = "Date only")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
            public Nullable<System.DateTime> TGKetThuc { get; set; }
            [Display(Name = "Năm học")]
            [Required(ErrorMessage ="Bạn chưa nhập năm học")]
            public Nullable<int> NamHoc { get; set; }
            [Display(Name ="Học kỳ")]
            [Required(ErrorMessage ="Bạn chưa nhập học kỳ")]
            public Nullable<int> HocKy { get; set; }

            [Display(Name ="Tỷ lệ điểm điểm danh")]
            public Nullable<double> TyLeDiemDD { get; set; }

            [Display(Name = "Tỷ lệ điểm bài tập")]
            public Nullable<double> TyLeDiemBT { get; set; }
        }
    }
}