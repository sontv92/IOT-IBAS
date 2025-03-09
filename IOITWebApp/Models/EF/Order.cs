using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public string Code { get; set; }
        public int? CustomerId { get; set; }
        public Guid? CustomerAddressId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? ShippingStatusId { get; set; }
        public int? OrderStatusId { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal? OrderDelivery { get; set; }
        public decimal? OrderDiscount { get; set; }
        public decimal? OrderTotal { get; set; }
        public decimal? OrderPaid { get; set; }
        public string CustomerNote { get; set; }
        public byte? StepSent { get; set; }
        public bool? IsSentMail { get; set; }
        public int? WebsiteId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
