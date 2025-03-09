using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Action
    {
        public long ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }
        public int? TargetId { get; set; }
        public string TargetType { get; set; }
        public string Logs { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Ipaddress { get; set; }
        public int? Time { get; set; }
        public byte? Type { get; set; }
        public int? CompanyId { get; set; }
        public int? UserPushId { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
