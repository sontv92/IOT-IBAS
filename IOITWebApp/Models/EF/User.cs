using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class User
    {
        public User()
        {
            UserRole = new HashSet<UserRole>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public int? UnitId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string KeyLock { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? TokenSince { get; set; }
        public string RegEmail { get; set; }
        public int? RoleMax { get; set; }
        public byte? RoleLevel { get; set; }
        public bool? IsRoleGroup { get; set; }
        public int? UserCreateId { get; set; }
        public int? UserEditId { get; set; }
        public byte? Status { get; set; }
        public string BranchId { get; set; }

        public ICollection<UserRole> UserRole { get; set; }
    }
}
