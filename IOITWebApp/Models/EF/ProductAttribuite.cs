using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ProductAttribuite
    {
        public int ProductAttributesId { get; set; }
        public int? ProductId { get; set; }
        public int? AttribuiteId { get; set; }
        public string Value { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
