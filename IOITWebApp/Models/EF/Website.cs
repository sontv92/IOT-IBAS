using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Website
    {
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? LanguageId { get; set; }
        public int? CompanyId { get; set; }
        public int? WebsiteParentId { get; set; }
        public string LogoHeader { get; set; }
        public string LogoFooter { get; set; }
        public string Banner { get; set; }
        public string Hotline { get; set; }
        public string Hotmail { get; set; }
        public string Address { get; set; }
        public string LinkGooglePlus { get; set; }
        public string LinkFacebookPage { get; set; }
        public string LinkTwitter { get; set; }
        public string LinkYoutube { get; set; }
        public string LinkInstagram { get; set; }
        public string LinkLinkedIn { get; set; }
        public string LinkOther1 { get; set; }
        public string LinkOther2 { get; set; }
        public string LinkOther3 { get; set; }
        public string GoogleAnalitics { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte? Status { get; set; }
        public int? HighlightsNewsId { get; set; }
        public string TechNiQuePhone { get; set; }
        public string GuaRanTeePhone { get; set; }
    }
}
