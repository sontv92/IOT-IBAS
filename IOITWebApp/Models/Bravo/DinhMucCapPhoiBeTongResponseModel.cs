using System.Collections.Generic;

namespace IOITWebApp.Models.Bravo
{
    public class DinhMucCapPhoiBeTongResponseModel
    {
        public string MaCapPhoi {  get; set; }
        public string TenCapPhoi {  set; get; }
        public string DoSut {  set; get; }
        public List<Detail> Details { get; set; }
    }
    public class Detail
    {
        public int MaVatTu { get; set; }
        public decimal KhoiLuong { get; set; }
    }
}
