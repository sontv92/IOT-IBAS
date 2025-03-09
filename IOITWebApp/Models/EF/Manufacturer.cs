using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Manufacturer
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public byte? TypeOriginId { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Url { get; set; }
        public int? Location { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Owner { get; set; }
        public string AvatarOwner { get; set; }
        public string NickName { get; set; }
        public string Country { get; set; }
        public string Contents { get; set; }
    }
}
