using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Branch
    {
        public int BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Contents { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public int? Location { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Dataname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int CompanyId { get; set; }
        public string PMQLXe { get; set; }
        public string QLCamera { get; set; }
    }
}
