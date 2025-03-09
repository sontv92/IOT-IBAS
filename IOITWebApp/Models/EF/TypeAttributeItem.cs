using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class TypeAttributeItem
    {
        public int TypeAttributeItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int TypeAttributeId { get; set; }
        public int? Location { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
