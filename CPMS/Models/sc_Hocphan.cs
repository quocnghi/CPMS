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
    
    public partial class sc_Hocphan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sc_Hocphan()
        {
            this.sc_Hocphan1 = new HashSet<sc_Hocphan>();
            this.tc_KHDaotao = new HashSet<tc_KHDaotao>();
        }
    
        public int MaMH { get; set; }
        public string MaHT { get; set; }
        public string TenMH { get; set; }
        public Nullable<int> MaQH { get; set; }
        public string Hieuluc { get; set; }
        public string Ghichu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sc_Hocphan> sc_Hocphan1 { get; set; }
        public virtual sc_Hocphan sc_Hocphan2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tc_KHDaotao> tc_KHDaotao { get; set; }
    }
}
