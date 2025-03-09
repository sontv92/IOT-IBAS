using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Language
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
        public bool? IsMain { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
