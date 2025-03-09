using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class UserMapping
    {
        public int UserMappingId { get; set; }
        public int? UserId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? UserIdCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }
}
