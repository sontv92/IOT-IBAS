using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Customer
    {
        public int CustomerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avata { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string KeyRandom { get; set; }
        public bool? IsEmailConfirm { get; set; }
        public bool? IsSentEmailConfirm { get; set; }
        public bool? IsPhoneConfirm { get; set; }
        public int? Type { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? TypeThirdId { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public int? CountryId { get; set; }
    }
}
