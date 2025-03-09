using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class CategoryMapping
    {
        public int CategoryMappingId { get; set; }
        public int? CategoryId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetType { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
