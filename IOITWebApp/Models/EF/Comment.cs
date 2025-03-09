using System;
using System.Collections.Generic;

namespace IOITWebApp.Models.EF
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int? CustomerId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public string Contents { get; set; }
        public int? CommentParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public byte? Status { get; set; }
    }
}
