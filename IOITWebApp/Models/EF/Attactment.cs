using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Attactment
    {
        public int AttactmentId { get; set; }
        public string Name { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Url { get; set; }
        public string Thumb { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
        public bool? IsImageMain { get; set; }
    }
}
