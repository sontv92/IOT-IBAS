using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Models.Bravo
{
    public class DanhMucCapPhoiBeTongRequestModel
    {
        public int BranchId { get; set; }
        public string MaMac { get; set; }
        public int MaCuaVL { get; set; }
        public string Ma { get; set; }
        public double SoLuong { get; set; }
        public string MaVatLieu { get; set; }
        public string TenVatLieu { get; set; }
    }
}
