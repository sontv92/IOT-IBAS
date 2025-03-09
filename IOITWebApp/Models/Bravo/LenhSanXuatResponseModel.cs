using System;

namespace IOITWebApp.Models.Bravo
{
    public class LenhSanXuatResponseModel
    {
        public string MaLenhSanXuat { get; set; }
        public string TenLenhSanXuat { get; set; }
        public string MaKH {  get; set; }
        public string MaDA { get; set; }
        public string MaCongTrinh { get; set; }
        public string MaSP { get; set; }
        public string MaCapPhoi { get; set; }
        public double KhoiLuong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string MaTram { get; set; }
        public double MetKhoiDaTron { get; set; }
    }
}
