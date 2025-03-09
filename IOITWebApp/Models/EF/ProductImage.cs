using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int? ProductId { get; set; }
        public bool? IsImageMain { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
