//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Capstone.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tc_DecuongGV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tc_DecuongGV()
        {
            this.sf_Notification = new HashSet<sf_Notification>();
        }
    
        public int MaQL { get; set; }
        public int MaKHDT { get; set; }
        public string NguoiST { get; set; }
        public System.DateTime NgayPC { get; set; }
        public string NguoiPC { get; set; }
        public Nullable<bool> Trangthai { get; set; }
        public string Ghichu { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sf_Notification> sf_Notification { get; set; }
        public virtual tc_KHDaotao tc_KHDaotao { get; set; }
    }
}