using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System;
using Dapper;
using log4net;
using System.Linq;
using System.Threading.Tasks;

namespace IOITWebApp.Helper
{
    public class DapperHepper
    {
        private static readonly ILog log = LogMaster.GetLogger("DapperHepper", "DapperHepper");
        public static IEnumerable<T> Query<T>(string connectionStr, string query)
        {
            try
            {
                IEnumerable<T> result = null;
                using (IDbConnection db = new SqlConnection(connectionStr))
                {
                    result = db.Query<T>(query);
                }
                return result;
            }
            catch (Exception e)
            {
                log.Error((new StackFrame(1).GetMethod().Name) + "\t" + query + e.ToString());
                return null;
            }
        }
        public static async Task<IEnumerable<T>> QueryAsync<T>(string connectionStr, string query, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionStr))
                {
                    db.Open(); // Mở kết nối bất đồng bộ
                    return await db.QueryAsync<T>(query, parameters);
                }
            }
            catch (Exception e)
            {
                log.Error($"{new StackFrame(1).GetMethod().Name}\t{query}\t{e}");
                return Enumerable.Empty<T>(); // Trả về danh sách rỗng thay vì null để tránh lỗi NullReferenceException
            }
        }

        public static int Execute(string connectionStr, string query, object p = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionStr))
                {
                    return Convert.ToInt32(db.ExecuteScalar(query, p));
                }
            }
            catch (Exception ex)
            {
                log.Error((new StackFrame(1).GetMethod().Name) + "\t" + query + ex.ToString());
                return -1;
            }
        }

        public static int ExecuteNew(string connectionStr, string query, object p = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionStr))
                {
                    return db.Execute(query, p); // ✅ Sửa thành Execute()
                }
            }
            catch (Exception ex)
            {
                log.Error((new StackFrame(1).GetMethod().Name) + "\t" + query + ex.ToString());
                return -1; // Trả về -1 khi lỗi
            }
        }

    }
}
