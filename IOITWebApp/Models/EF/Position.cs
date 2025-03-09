using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Position
    {
        public int PositionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? LevelId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
