using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Role
    {
        public int RoleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserEditId { get; set; }
        public int? UserId { get; set; }
        public byte? LevelRole { get; set; }
        public byte? Status { get; set; }
    }
}
