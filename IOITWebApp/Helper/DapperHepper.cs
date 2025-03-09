using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System;
using Dapper;
using log4net;

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
    }
}
