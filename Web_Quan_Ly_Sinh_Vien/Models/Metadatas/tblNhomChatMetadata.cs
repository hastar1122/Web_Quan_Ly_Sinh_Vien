using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    [MetadataType(typeof(tblNhomChatMetadata))]
    public partial class tblNhomChat
    {
        internal sealed class tblNhomChatMetadata
        {
            [Display(Name ="Tên nhóm")]
            public string TenNhom { get; set; }
            [Display(Name ="Hình ảnh")]
            public string Avatar { get; set; }
            public HttpPostedFileBase ImageFile { get; set; }
        }
    }
}