using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblDiemMetadata))]
    public partial class tblDiem
    {
        internal sealed class tblDiemMetadata
        {
            public long MaSV { get; set; }

            public int MaLHP { get; set; }

            [Display(Name = "Điểm điểm danh")]
            public Nullable<double> DiemDD { get; set; }

            [Display(Name = "Điểm quá trình")]
            public Nullable<double> DIemQT { get; set; }

            [Display(Name = "Ghi chú")]
            public string GhiChu { get; set; }
        }
    }
}