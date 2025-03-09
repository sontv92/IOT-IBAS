using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CategoryParentId { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public string IconFa { get; set; }
        public bool? IconText { get; set; }
        public int? Location { get; set; }
        public int? TypeCategoryId { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public int? NumberDisplayMobile { get; set; }
    }
}
