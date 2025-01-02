using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblBuoiDiemDanhMetadata))]
    public partial class tblBuoiDiemDanh
    {
        sealed internal class tblBuoiDiemDanhMetadata
        {
            public int Id { get; set; }

            public Nullable<int> MaLHP { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
            [Display(Name = "Ngày điểm danh")]
            public Nullable<System.DateTime> NgayDD { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
            [Display(Name = "Hạn điểm danh")]
            public Nullable<System.DateTime> HanDD { get; set; }
        }
    }
}