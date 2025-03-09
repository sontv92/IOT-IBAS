using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class CustomerAddress
    {
        public Guid CustomerAddressId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public bool? IsMain { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
