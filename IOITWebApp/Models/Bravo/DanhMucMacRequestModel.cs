using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Bravo
{
    public class DanhMucMacRequestModel
    {
        public int BranchId { get; set; }
        public string MaMac { get; set; }
        public string TenMac { get; set; }
        public string CuongDo { get; set; }
        public decimal CotLieuMax { get; set; }
        public string DoSut { get; set; }
        public string DonViTinh{ get; set; }
        public string GhiChu { get; set; }
    }
}
