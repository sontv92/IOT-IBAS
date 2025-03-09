using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class ProductCustomer
    {
        public int ProductCustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? CustomerId { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
