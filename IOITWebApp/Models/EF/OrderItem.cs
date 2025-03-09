using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceTax { get; set; }
        public decimal? PriceDiscount { get; set; }
        public decimal? PriceTotal { get; set; }
        public byte? Status { get; set; }
    }
}
