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
    
    public partial class tp_TailieuHP
    {
        public int MaTL { get; set; }
        public Nullable<int> MaKHDT { get; set; }
        public string LoaiTL { get; set; }
        public string TenTL { get; set; }
        public string Tacgia { get; set; }
        public string NhaXB { get; set; }
        public string NamXB { get; set; }
        public string Kieunhap { get; set; }
    
        public virtual tp_KHDaotao tp_KHDaotao { get; set; }
    }
}
