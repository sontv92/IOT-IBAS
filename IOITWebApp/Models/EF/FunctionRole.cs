using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class FunctionRole
    {
        public int FunctionRoleId { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
        public byte? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }

        public Function Function { get; set; }
    }
}
