using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblNopBaiTapMetadata))]
    public partial class tblNopBaiTap
    {
        internal sealed class tblNopBaiTapMetadata
        {
            public int MaBT { get; set; }
            [Display(Name = "Mã sinh viên")]
            public long MaSV { get; set; }
            [Display(Name = "Bài làm")]
            public string BaiLam { get; set; }
            [Display(Name ="Thời gian nộp")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
            public Nullable<System.DateTime> NgayNop { get; set; }
            [Display(Name ="Điểm")]
            public Nullable<double> Diem { get; set; }
            [Display(Name ="Nhận xét")]
            public string NhanXet { get; set; }
        }
    }
}