using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class UserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }

        public User User { get; set; }
    }
}
