using System;

namespace IOITWebApp.Models
{
    /// <summary>
    /// Tham số lọc cho màn hình tra cứu nhật ký người dùng
    /// </summary>
    public class AuditLogFilter : BasePagination
    {
        [System.ComponentModel.DefaultValue(null)]
        public DateTime? tungay { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public DateTime? denngay { get; set; }

        /// <summary>0: tất cả công ty</summary>
        [System.ComponentModel.DefaultValue(0)]
        public int CompanyId { get; set; }

        /// <summary>Danh sách BranchId cách nhau bởi dấu phẩy, rỗng: tất cả trạm</summary>
        [System.ComponentModel.DefaultValue("")]
        public string Branchlist { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string UserName { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string Action { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string EntityType { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string EntityId { get; set; }

        /// <summary>-1: tất cả, 1: thành công, 0: thất bại</summary>
        [System.ComponentModel.DefaultValue(-1)]
        public int Success { get; set; } = -1;

        /// <summary>Tìm trong UserName / EntityId / Description / IpAddress</summary>
        [System.ComponentModel.DefaultValue("")]
        public string search { get; set; }
    }
}
