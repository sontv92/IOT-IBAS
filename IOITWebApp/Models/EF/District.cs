using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class District
    {
        public int DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Priority { get; set; }
    }
}
