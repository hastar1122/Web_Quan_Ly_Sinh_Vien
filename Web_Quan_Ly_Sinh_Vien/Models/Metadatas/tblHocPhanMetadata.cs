using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblHocPhanMetadata))]
    public partial class tblHocPhan
    {
        internal sealed class tblHocPhanMetadata
        {
            [Display(Name ="Mã học phần")]
            [Required(ErrorMessage ="Bạn chưa nhập mã học phần")]
            public int MaHP { get; set; }

            [Display(Name ="Tên học phần")]
            [Required(ErrorMessage ="Bạn chưa nhập tên học phần")]
            public string TenHP { get; set; }

            [Display(Name ="Số tín chỉ")]
            [Required(ErrorMessage ="Bạn chưa nhập số tín chỉ")]
            public Nullable<int> SoTinChi { get; set; }
        }
    }
}