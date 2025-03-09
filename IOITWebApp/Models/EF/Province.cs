using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Province
    {
        public int ProvinceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public byte? Priority { get; set; }
        public string Lang { get; set; }
        public byte? Region { get; set; }
    }
}
