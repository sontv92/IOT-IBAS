using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Attribuite
    {
        public int AttribuiteId { get; set; }
        public string Name { get; set; }
        public bool? IsCustom { get; set; }
        public int? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public byte? Status { get; set; }
    }
}
