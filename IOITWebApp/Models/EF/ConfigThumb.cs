using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ConfigThumb
    {
        public int ConfigThumbId { get; set; }
        public int Width { get; set; }
        public int? Height { get; set; }
        public byte? Type { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
