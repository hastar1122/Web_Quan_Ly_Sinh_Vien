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
    
    public partial class tblHocPhan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblHocPhan()
        {
            this.tblLopHocPhans = new HashSet<tblLopHocPhan>();
        }
    
        public int MaHP { get; set; }
        public string TenHP { get; set; }
        public Nullable<int> SoTinChi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLopHocPhan> tblLopHocPhans { get; set; }
    }
}