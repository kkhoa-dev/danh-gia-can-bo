using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanhGiaCanBo.Models.DTO
{
    public class DanhGiaDTO
    {
        public Nullable<int> CanBo_ID { get; set; }
        public int ChuaHaiLong { get; set; }
        public int HaiLong { get; set; }
        public int RatHaiLong { get; set; }
        public string HoTen { get; set; }

        public int thang { get; set; }

        public int tong { get; set; }
        public string danhgia { get; set; }

        public string Quay { get; set; }

        public string GhiChu { get; set; }

    }
}