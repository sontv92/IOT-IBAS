using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Wards
    {
        public int WardId { get; set; }
        public string Name { get; set; }
        public int? DistinctId { get; set; }
        public string Code { get; set; }
    }
}
