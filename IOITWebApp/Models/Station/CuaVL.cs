using System;

namespace IOITWebApp.Models.Station
{
    public class CuaVL
    {
        public Guid Id { get; set; }
        public string MACUAVL { get; set; }
        public string TENCUAVL { get; set; }
        public string TENLOAIVL { get; set; }
        public int TRANGTHAI { get; set; }
        public int STTCUAVL { get; set; }
        public int MATRAM {  get; set; }
        public int MALOAIVL { get; set; }
    }
}
