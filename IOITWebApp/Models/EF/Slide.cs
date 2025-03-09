using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Slide
    {
        public int SlideId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TargetId { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int? TypeSlideId { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public bool? IsLinkNewTab { get; set; }
        public int? LanguageId { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
