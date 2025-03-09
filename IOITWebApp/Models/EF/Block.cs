using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Block
    {
        public int BlockId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Icon { get; set; }
        public string IconFa { get; set; }
        public bool? IconText { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
