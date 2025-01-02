using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblLopMetadata))]
    public partial class tblLop
    {
        internal sealed class tblLopMetadata
        {
            [Display(Name = "Ngành học")]
            [Required(ErrorMessage ="Vui lòng chọn ngành học")]
            public string MaNganh { get; set; }
            [Display(Name ="Tên lớp")]
            [Required(ErrorMessage ="Vui lòng nhập tên lớp")]
            public string TenLop { get; set; }
            [Display(Name ="Khóa")]
            public string Khoa { get; set; }
            [Display(Name ="Năm học")]
            public string NamHoc { get; set; }
        }
    }
}