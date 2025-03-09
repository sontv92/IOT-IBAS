using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class TypeAttribute
    {
        public int TypeAttributeId { get; set; }
        public string Name { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public int? TypeAttribuiteParentId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
