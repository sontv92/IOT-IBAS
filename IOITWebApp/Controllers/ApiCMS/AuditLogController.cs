using Dapper;
using IOITWebApp.Helper;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace IOITWebApp.Controllers.ApiCMS
{
    /// <summary>
    /// Tra cứu và theo dõi nhật ký thao tác người dùng (bảng AuditLog - DB chính)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("AuditLog", "AuditLog");
        private static string functionCode = "NKND";

        private static readonly HashSet<string> AllowedOrderBy = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "OccurredAt", "UserName", "Action", "EntityType", "EntityId", "Success", "IpAddress"
        };

        private bool HasPermission(int action)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            return !string.IsNullOrEmpty(access_key) && CheckRole.CheckRoleByCode(access_key, functionCode, action);
        }

        /// <summary>
        /// Xây dựng mệnh đề WHERE + tham số từ bộ lọc
        /// </summary>
        private static string BuildWhere(AuditLogFilter f, DynamicParameters p)
        {
            var where = new StringBuilder("WHERE 1 = 1");

            if (f.tungay.HasValue)
            {
                where.Append(" AND OccurredAt >= @tungay");
                p.Add("@tungay", f.tungay.Value.Date);
            }
            if (f.denngay.HasValue)
            {
                where.Append(" AND OccurredAt < @denngay");
                p.Add("@denngay", f.denngay.Value.Date.AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(f.UserName))
            {
                where.Append(" AND UserName LIKE @UserName");
                p.Add("@UserName", "%" + f.UserName.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(f.Action))
            {
                where.Append(" AND [Action] = @Action");
                p.Add("@Action", f.Action.Trim());
            }
            if (!string.IsNullOrWhiteSpace(f.EntityType))
            {
                where.Append(" AND EntityType = @EntityType");
                p.Add("@EntityType", f.EntityType.Trim());
            }
            if (!string.IsNullOrWhiteSpace(f.EntityId))
            {
                where.Append(" AND EntityId LIKE @EntityId");
                p.Add("@EntityId", "%" + f.EntityId.Trim() + "%");
            }
            if (f.Success == 0 || f.Success == 1)
            {
                where.Append(" AND Success = @Success");
                p.Add("@Success", f.Success == 1);
            }
            if (!string.IsNullOrWhiteSpace(f.search))
            {
                where.Append(" AND (UserName LIKE @search OR EntityId LIKE @search OR [Description] LIKE @search OR IpAddress LIKE @search OR OldValues LIKE @search OR NewValues LIKE @search)");
                p.Add("@search", "%" + f.search.Trim() + "%");
            }

            return where.ToString();
        }

        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] AuditLogFilter paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (!HasPermission((int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                if (paging.page <= 0) paging.page = 1;
                if (paging.page_size <= 0) paging.page_size = 20;

                // order_by dạng "OccurredAt Desc" - chỉ cho phép các cột trong danh sách
                string orderBy = "OccurredAt DESC";
                if (!string.IsNullOrWhiteSpace(paging.order_by))
                {
                    var parts = paging.order_by.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0 && AllowedOrderBy.Contains(parts[0]))
                    {
                        string dir = parts.Length > 1 && parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
                        orderBy = "[" + parts[0] + "] " + dir;
                        // Cột phụ để thứ tự ổn định - không được trùng với cột chính (SQL Server báo lỗi)
                        if (!parts[0].Equals("OccurredAt", StringComparison.OrdinalIgnoreCase))
                            orderBy += ", OccurredAt DESC";
                    }
                }

                var p = new DynamicParameters();
                string where = BuildWhere(paging, p);
                p.Add("@offset", (paging.page - 1) * paging.page_size);
                p.Add("@fetch", paging.page_size);

                string sqlCount = "SELECT COUNT(*) FROM [AuditLog] " + where;
                // Không trả OldValues/NewValues ở danh sách để nhẹ dữ liệu, chỉ trả cờ có/không
                string sqlData = @"SELECT Id, OccurredAt, UserId, UserName, [Action], EntityType, EntityId, Success, IpAddress, TraceId, [Description],
                                          CASE WHEN OldValues IS NULL THEN NULL ELSE '1' END AS OldValues,
                                          CASE WHEN NewValues IS NULL THEN NULL ELSE '1' END AS NewValues
                                   FROM [AuditLog] " + where +
                                 " ORDER BY " + orderBy +
                                 " OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY";

                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    int count = connection.ExecuteScalar<int>(sqlCount, p);
                    var list = connection.Query<AuditLogDTO>(sqlData, p).ToList();
                    foreach (var item in list)
                    {
                        item.OccurredAtTitle = item.OccurredAt.ToString("dd/MM/yyyy HH:mm:ss");
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = list;
                    def.metadata = count;
                }
                return Ok(def);
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(Guid id)
        {
            DefaultResponse def = new DefaultResponse();
            if (!HasPermission((int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    var item = connection.Query<AuditLogDTO>("SELECT * FROM [AuditLog] WHERE Id = @id", new { id }).FirstOrDefault();
                    if (item == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy bản ghi");
                        return Ok(def);
                    }
                    item.OccurredAtTitle = item.OccurredAt.ToString("dd/MM/yyyy HH:mm:ss");
                    def.meta = new Meta(200, "Success");
                    def.data = item;
                }
                return Ok(def);
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        /// <summary>
        /// Danh sách người dùng đã có log (để đổ dropdown lọc)
        /// </summary>
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            DefaultResponse def = new DefaultResponse();
            if (!HasPermission((int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    var list = connection.Query<string>("SELECT DISTINCT UserName FROM [AuditLog] WHERE UserName IS NOT NULL ORDER BY UserName").ToList();
                    def.meta = new Meta(200, "Success");
                    def.data = list.Select(u => new { UserName = u }).ToList();
                }
                return Ok(def);
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        /// <summary>
        /// Xuất Excel theo bộ lọc hiện tại
        /// </summary>
        [HttpGet("Export")]
        public IActionResult Export([FromQuery] AuditLogFilter paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (!HasPermission((int)Const.Action.EXPORT))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            try
            {
                var p = new DynamicParameters();
                string where = BuildWhere(paging, p);
                string sql = "SELECT TOP 50000 * FROM [AuditLog] " + where + " ORDER BY OccurredAt DESC";

                List<AuditLogDTO> list;
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    list = connection.Query<AuditLogDTO>(sql, p).ToList();
                }

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("NhatKyNguoiDung");
                    string[] headers = { "STT", "Thời gian", "Người dùng", "Thao tác", "Đối tượng", "Kết quả", "IP", "Mô tả", "Giá trị cũ", "Giá trị mới", "TraceId" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cell(1, i + 1).Value = headers[i];
                        ws.Cell(1, i + 1).Style.Font.Bold = true;
                        ws.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                    }
                    int row = 2;
                    int stt = 1;
                    foreach (var item in list)
                    {
                        ws.Cell(row, 1).Value = stt++;
                        ws.Cell(row, 2).Value = item.OccurredAt.ToString("dd/MM/yyyy HH:mm:ss");
                        ws.Cell(row, 3).Value = item.UserName;
                        ws.Cell(row, 4).Value = item.Action;
                        ws.Cell(row, 5).Value = item.EntityType;
                        ws.Cell(row, 6).Value = item.Success ? "Thành công" : "Thất bại";
                        ws.Cell(row, 7).Value = item.IpAddress;
                        ws.Cell(row, 8).Value = item.Description;
                        ws.Cell(row, 9).Value = item.OldValues;
                        ws.Cell(row, 10).Value = item.NewValues;
                        ws.Cell(row, 11).Value = item.TraceId;
                        row++;
                    }
                    ws.Columns(1, 8).AdjustToContents();

                    using (var stream = new System.IO.MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "nhat_ky_nguoi_dung_" + DateTime.Now.ToString("ddMMyyyy_HHmm") + ".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }
    }
}
