using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Bravo
{
    public class DanhMucVatTuRequestModel
    {
        public int BranchId { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string NhaCungCap { get; set; }
        public int MaLoaiVL { get; set; }
        public string TenLoaiVL { get; set; }
        public float HeSoQuyDoi { get; set; }
        public string DonViQuyDoi { get; set; }
        
    }
}
