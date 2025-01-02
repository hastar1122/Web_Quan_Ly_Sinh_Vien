using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataTypeAttribute(typeof(tblNganhHocMetadata))]
    public partial class tblNganhHoc
    {
        internal sealed class tblNganhHocMetadata
        {
            [Required(ErrorMessage ="Bạn chưa nhập tên ngành")]
            [Display(Name ="Tên ngành")]
            public string TenNganh { get; set; }
        }
    }
}