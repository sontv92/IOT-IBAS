using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Related
    {
        public int RelatedId { get; set; }
        public int? TargetId { get; set; }
        public int? TargetRelatedId { get; set; }
        public byte? TargetType { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
