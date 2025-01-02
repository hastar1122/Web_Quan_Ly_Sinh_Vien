using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblDiemDanhMetadata))]
    public partial class tblDiemDanh
    {
        sealed internal class tblDiemDanhMetadata
        {
            [Display(Name ="Điểm danh")]
            public bool DiemDanh { get; set; }
            [Display(Name = "Ghi chú")]
            public string GhiChu { get; set; }
            [Display(Name ="Điểm")]
            public Nullable<double> Diem { get; set; }
        }
    }
}