using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblUserMetadata))]
    public partial class tblUser
    {
        internal sealed class tblUserMetadata
        {
            public long Id { get; set; }

            [Required]
            [Display(Name ="Họ đệm")]
            public string Ho { get; set; }

            [Required]
            [Display(Name = "Tên")]
            public string Ten { get; set; }

            [Display(Name = "Mã sinh viên")]
            public string MaSV { get; set; }

            [Display(Name ="Lớp học")]
            public Nullable<int> MaLop { get; set; }

            [Display(Name ="Ngành giảng dạy")]
            public Nullable<int> MaNganh { get; set; }

            [Display(Name ="Trình độ")]
            public string TrinhDo { get; set; }

            [Display(Name ="Giới tính")]
            public string GioiTinh { get; set; }

            [Display(Name ="Ngày sinh")]
            [DataType(DataType.Date, ErrorMessage = "Date only")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
            public Nullable<System.DateTime> NgaySinh { get; set; }

            [Required]
            public string Email { get; set; }

            public string CCCD { get; set; }

            [Display(Name = "Số điện thoại")]
            public string SDT { get; set; }

            [Display(Name = "Mật khẩu")]
            [DataType(DataType.Password)]
            public string MatKhau { get; set; }

            [Display(Name = "Ghi chú")]
            public string GhiChu { get; set; }

            [Display(Name = "Hình ảnh")]
            public string Anh { get; set; }

            [Display(Name = "Trạng thái")]
            public bool TrangThai { get; set; }

            public string Quyen { get; set; }

            public HttpPostedFileBase ImageFile { get; set; }
        }
    }
}