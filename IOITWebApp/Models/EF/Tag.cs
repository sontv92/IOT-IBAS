using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
