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
    
    public partial class tblDiem
    {
        public long MaSV { get; set; }
        public int MaLHP { get; set; }
        public Nullable<double> DiemDD { get; set; }
        public Nullable<double> DIemQT { get; set; }
        public string GhiChu { get; set; }
        public Nullable<double> DiemBT { get; set; }
    
        public virtual tblLopHocPhan tblLopHocPhan { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}