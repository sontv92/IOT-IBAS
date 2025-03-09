using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Bank
    {
        public int BankId { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string BranchName { get; set; }
        public string Note { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
