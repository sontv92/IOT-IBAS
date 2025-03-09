using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class MenuItem
    {
        public int MenuItemId { get; set; }
        public int? CategoryId { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
