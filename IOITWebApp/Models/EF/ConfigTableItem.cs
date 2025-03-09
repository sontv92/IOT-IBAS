using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ConfigTableItem
    {
        public int ConfigTableItemId { get; set; }
        public int? ConfigTableId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool? IsNull { get; set; }
        public int? RankMin { get; set; }
        public int? RankMax { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
