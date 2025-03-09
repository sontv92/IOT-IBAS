using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Function
    {
        public Function()
        {
            FunctionRole = new HashSet<FunctionRole>();
        }

        public int FunctionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int FunctionParentId { get; set; }
        public string Url { get; set; }
        public string Note { get; set; }
        public int? Location { get; set; }
        public string Icon { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }

        public ICollection<FunctionRole> FunctionRole { get; set; }
    }
}
