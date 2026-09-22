using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using IOITWebApp.Models;
using IOITWebApp.Models.EF;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace IOITWebApp.Helper
{
    /// <summary>
    /// Các loại thao tác được ghi vào AuditLog.Action
    /// </summary>
    public static class AuditAction
    {
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
    }

    /// <summary>
    /// Các loại đối tượng được ghi vào AuditLog.EntityType
    /// </summary>
    public static class AuditEntity
    {
        public const string MacBeTong = "MacBeTong";
        public const string HopDong = "HopDong";
        public const string KhachHang = "KhachHang";
        public const string DuAn = "DuAn";
        public const string NhanVien = "NhanVien";
        public const string Xe = "Xe";
    }

    /// <summary>
    /// Ghi log thao tác người dùng vào bảng [AuditLog] (DB chính - DefaultConnection).
    /// </summary>
    public class AuditLogService
    {
        private static readonly ILog log = LogMaster.GetLogger("AuditLog", "AuditLog");

        private static readonly JsonSerializerSettings JsonOptions = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string Serialize(object value)
        {
            return value == null ? null : JsonConvert.SerializeObject(value, JsonOptions);
        }

        /// <summary>
        /// Ghi log trong một transaction có sẵn.
        /// </summary>
        public async Task WriteAsync(
            SqlConnection connection,
            SqlTransaction transaction,
            int? userId,
            string userName,
            string action,
            string entityType,
            string entityId,
            object oldValues,
            object newValues,
            bool success,
            string ipAddress,
            string traceId,
            string description = null,
            int? companyId = null,
            int? branchId = null)
        {
            string oldValuesJson = Serialize(oldValues);
            string newValuesJson = Serialize(newValues);

            const string sql = @"
            INSERT INTO [AuditLog]
            (
                [Id],
                [OccurredAt],
                [UserId],
                [UserName],
                [Action],
                [EntityType],
                [EntityId],
                [OldValues],
                [NewValues],
                [Success],
                [IpAddress],
                [TraceId],
                [Description],
                [CompanyId],
                [BranchId]
            )
            VALUES
            (
                @Id,
                GETDATE(),
                @UserId,
                @UserName,
                @Action,
                @EntityType,
                @EntityId,
                @OldValues,
                @NewValues,
                @Success,
                @IpAddress,
                @TraceId,
                @Description,
                @CompanyId,
                @BranchId
            );";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                command.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserName", string.IsNullOrWhiteSpace(userName) ? (object)DBNull.Value : Truncate(userName, 100));
                command.Parameters.AddWithValue("@Action", Truncate(action, 30));
                command.Parameters.AddWithValue("@EntityType", Truncate(entityType, 100));
                command.Parameters.AddWithValue("@EntityId", string.IsNullOrWhiteSpace(entityId) ? (object)DBNull.Value : Truncate(entityId, 100));
                command.Parameters.AddWithValue("@OldValues", oldValuesJson == null ? (object)DBNull.Value : oldValuesJson);
                command.Parameters.AddWithValue("@NewValues", newValuesJson == null ? (object)DBNull.Value : newValuesJson);
                command.Parameters.AddWithValue("@Success", success);
                command.Parameters.AddWithValue("@IpAddress", string.IsNullOrWhiteSpace(ipAddress) ? (object)DBNull.Value : Truncate(ipAddress, 50));
                command.Parameters.AddWithValue("@TraceId", string.IsNullOrWhiteSpace(traceId) ? (object)DBNull.Value : Truncate(traceId, 100));
                command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : Truncate(description, 500));
                command.Parameters.AddWithValue("@CompanyId", (object)companyId ?? DBNull.Value);
                command.Parameters.AddWithValue("@BranchId", (object)branchId ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Ghi log từ controller: tự lấy user / IP / TraceId từ HttpContext, tự mở kết nối tới DB chính.
        /// Không bao giờ ném exception (lỗi ghi log không được làm hỏng nghiệp vụ).
        /// </summary>
        public static void Write(
            HttpContext httpContext,
            string action,
            string entityType,
            string entityId,
            object oldValues,
            object newValues,
            bool success,
            string description = null,
            Branch branch = null,
            int? branchId = null)
        {
            try
            {
                // Sửa thành công nhưng dữ liệu không đổi (chỉ khác LASTUPDATED) thì không ghi để tránh rác
                if (action == AuditAction.UPDATE && success && !HasChanges(oldValues, newValues))
                    return;

                int? userId = null;
                int? companyId = null;
                if (branch != null)
                {
                    branchId = branch.BranchId;
                    companyId = branch.CompanyId;
                }
                else if (branchId.HasValue && branchId.Value > 0)
                {
                    companyId = LookupCompanyId(branchId.Value);
                }
                else
                {
                    branchId = null;
                }
                string userName = null;
                string ipAddress = null;
                string traceId = null;

                if (httpContext != null)
                {
                    var identity = httpContext.User != null ? httpContext.User.Identity as ClaimsIdentity : null;
                    if (identity != null)
                    {
                        string sUserId = GetClaim(identity, "UserId");
                        int parsed;
                        if (int.TryParse(sUserId, out parsed)) userId = parsed;

                        string fullName = GetClaim(identity, ClaimTypes.Name) ?? GetClaim(identity, "unique_name") ?? GetClaim(identity, "name");
                        string email = GetClaim(identity, "email");
                        if (!string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(email))
                            userName = fullName + " (" + email + ")";
                        else
                            userName = fullName ?? email;
                    }

                    var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(forwarded))
                        ipAddress = forwarded.Split(',')[0].Trim();
                    else if (httpContext.Connection.RemoteIpAddress != null)
                        ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

                    traceId = httpContext.TraceIdentifier;
                }

                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    connection.Open();
                    new AuditLogService()
                        .WriteAsync(connection, null, userId, userName, action, entityType, entityId,
                                    oldValues, newValues, success, ipAddress, traceId, description, companyId, branchId)
                        .GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                log.Error("Khong ghi duoc AuditLog: " + ex);
            }
        }

        /// <summary>Các cột bỏ qua khi so sánh cũ / mới (thay đổi tự động, không phải do người dùng)</summary>
        private static readonly HashSet<string> IgnoredCompareFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LASTUPDATED"
        };

        /// <summary>
        /// So sánh giá trị cũ / mới (bỏ qua các cột trong IgnoredCompareFields).
        /// Trả về true nếu có thay đổi thực sự hoặc không so sánh được (để không bỏ sót log).
        /// </summary>
        public static bool HasChanges(object oldValues, object newValues)
        {
            if (oldValues == null || newValues == null) return true;
            try
            {
                return Serialize(StripIgnored(oldValues)) != Serialize(StripIgnored(newValues));
            }
            catch (Exception ex)
            {
                log.Error("AuditLog so sanh cu/moi loi: " + ex);
                return true;
            }
        }

        private static object StripIgnored(object value)
        {
            var dict = value as IDictionary<string, object>;
            if (dict == null) return value;
            var copy = new Dictionary<string, object>();
            foreach (var kv in dict)
            {
                if (IgnoredCompareFields.Contains(kv.Key)) continue;
                var list = kv.Value as IEnumerable<Dictionary<string, object>>;
                copy[kv.Key] = list != null ? (object)list.Select(StripIgnored).ToList() : kv.Value;
            }
            return copy;
        }

        /// <summary>
        /// Đọc 1 bản ghi (dạng dictionary cột -> giá trị) để làm snapshot OldValues / NewValues.
        /// Trả về null nếu không có bản ghi.
        /// </summary>
        public static Dictionary<string, object> Snapshot(string sql, object param = null)
        {
            try
            {
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    var row = connection.Query(sql, param).FirstOrDefault();
                    if (row == null) return null;
                    return ToDictionary(row);
                }
            }
            catch (Exception ex)
            {
                log.Error("AuditLog snapshot loi: " + sql + "\t" + ex);
                return null;
            }
        }

        /// <summary>
        /// Đọc nhiều bản ghi (dạng list dictionary) để làm snapshot chi tiết.
        /// </summary>
        public static List<Dictionary<string, object>> SnapshotList(string sql, object param = null)
        {
            try
            {
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    return connection.Query(sql, param).Select(r => ToDictionary((object)r)).ToList<Dictionary<string, object>>();
                }
            }
            catch (Exception ex)
            {
                log.Error("AuditLog snapshot loi: " + sql + "\t" + ex);
                return new List<Dictionary<string, object>>();
            }
        }

        private static Dictionary<string, object> ToDictionary(object row)
        {
            var dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kv in (IDictionary<string, object>)row)
            {
                dict[kv.Key] = kv.Value;
            }
            return dict;
        }

        private static int? LookupCompanyId(int branchId)
        {
            try
            {
                using (var connection = new SqlConnection(LocalSettings.ConnectString))
                {
                    return connection.Query<int?>("SELECT CompanyId FROM [Branch] WHERE BranchId = @id", new { id = branchId }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error("AuditLog lookup CompanyId loi: " + ex);
                return null;
            }
        }

        private static string GetClaim(ClaimsIdentity identity, string type)
        {
            var value = identity.Claims.Where(c => c.Type == type).Select(c => c.Value).FirstOrDefault();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static string Truncate(string value, int max)
        {
            if (value == null) return null;
            return value.Length <= max ? value : value.Substring(0, max);
        }
    }
}
