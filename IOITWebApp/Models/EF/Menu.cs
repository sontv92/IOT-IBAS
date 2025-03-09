using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Menu
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
