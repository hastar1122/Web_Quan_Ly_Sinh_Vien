//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_Quan_Ly_Sinh_Vien.Models.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblNopBaiTap
    {
        public int MaBT { get; set; }
        public long MaSV { get; set; }
        public string BaiLam { get; set; }
        public Nullable<System.DateTime> NgayNop { get; set; }
        public Nullable<double> Diem { get; set; }
        public string NhanXet { get; set; }
    
        public virtual tblBaiTap tblBaiTap { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}