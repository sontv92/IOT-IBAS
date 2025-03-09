using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Company
    {
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Fax { get; set; }
        public string Representative { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int CountUser { get; set; }
        public int Active { get; set; }
       
        public string Note { get; set; }
        public string Logo { get; set; }
    }
}
