//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookingCinema.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CT_DichVu
    {
        public int id { get; set; }
        public Nullable<int> so_luong_dv { get; set; }
        public Nullable<int> dich_vu_id { get; set; }
        public Nullable<int> orders_id { get; set; }
    
        public virtual DichVu DichVu { get; set; }
        public virtual Order Order { get; set; }
    }
}
