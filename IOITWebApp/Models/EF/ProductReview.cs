using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ProductReview
    {
        public int ProductReviewId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public string Contents { get; set; }
        public int? NumberStar { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
