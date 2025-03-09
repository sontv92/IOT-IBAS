using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class CategoryRank
    {
        public int CategoryRankId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int? RankStart { get; set; }
        public int? RankEnd { get; set; }
        public int? TypeRankId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
