using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblBaiTapMetadata))]
    public partial class tblBaiTap
    {
        internal sealed class tblBaiTapMetadata
        {
            public int Id { get; set; }

            public Nullable<int> MaLHP { get; set; }

            [Display(Name = "Tên bài tập")]
            [Required(ErrorMessage = "Bạn chưa nhập tên bài tập")]
            public string TenBT { get; set; }

            [Display(Name = "Đề bài")]
            public string DeBai { get; set; }

            public string Link { get; set; }

            [Display(Name = "Ngày giao")]
            [DataType(DataType.DateTime, ErrorMessage = "Date only")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
            public Nullable<System.DateTime> NgayGiao { get; set; }

            [Display(Name = "Hạn nộp")]
            [Required(ErrorMessage = "Bạn chưa nhập hạn nộp bài tập")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
            [DataType(DataType.DateTime, ErrorMessage = "Date only")]
            public Nullable<System.DateTime> HanNop { get; set; }

            public HttpPostedFileBase File { get; set; }
        }
    }
}