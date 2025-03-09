using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace IOITWebApp.Models.Common
{
    public class CommonLib
    {

        public static string ExtractToArrayNumber(string stringSource)
        {
            Regex myRegex = new Regex(@"\D");
            string chuoiSo = myRegex.Replace(stringSource, "");

            return chuoiSo;
        }
        public static bool StartWithSoMay(string stringSource)
        {

            SqlConnection con = new SqlConnection(LocalSettings.ConnectString);

            try
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                SqlDataReader rd = null;
                string maybeSoMay = "";

                //TH1: K convert được thành kiểu int --> có chữ cái
                if (stringSource.Length >= 2) maybeSoMay = stringSource.Substring(0, 2);
                try
                {
                    int test = Convert.ToInt32(maybeSoMay.ToString());
                }
                catch (Exception ex)
                {
                    return false; //Không phải số máy vì không ép kiểu được
                }
                //Nếu convert được thì xét tiếp
                //Cẩn thận trường hợp ExtractToArrayNumber xong thì H1 -> 1 , tưởng nhầm là số máy
                maybeSoMay = ExtractToArrayNumber(maybeSoMay);  //Loại bỏ các chữ không phải là chữ số. Cho ra chuỗi ký tự chỉ chứa các chữ số (tối đa 2 chữ số)
                if (maybeSoMay != "")
                {
                    cmd.CommandText = string.Format("SELECT TOP 1 * FROM MayTinh WHERE OrderNumber = {0}", maybeSoMay);
                    rd = cmd.ExecuteReader();
                }
                else return false;

                if (rd != null && rd.Read())
                    return true;
                else
                {
                    return false;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(sqlEx.Message);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
            finally
            {
                con.Close();
            }
        }
        public static string CutSoMayWithOutSoChungTu(string soChungTu)
        {
            return soChungTu.Substring(0);
        }
        public static string GetSo(string tableName, string columnName, string prefix, string dataname)
        {
            int noDigits = 4;
            SqlConnection con = new SqlConnection(LocalSettings.ConnectString);
            try
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                SqlDataReader rd = null;

                string pattern = prefix;
                if (prefix.Contains("Ă"))
                    pattern = prefix.Replace("Ă", "%");
                else if (prefix.Contains("Â"))
                    pattern = prefix.Replace("Â", "%");
                else if (prefix.Contains("Đ"))
                    pattern = prefix.Replace("Đ", "%");
                else if (prefix.Contains("Ê"))
                    pattern = prefix.Replace("Ê", "%");
                else if (prefix.Contains("Ô"))
                    pattern = prefix.Replace("Ô", "%");
                else if (prefix.Contains("Ơ"))
                    pattern = prefix.Replace("Ơ", "%");
                else if (prefix.Contains("Ư"))
                    pattern = prefix.Replace("Ư", "%");

                for (int i = 0; i < noDigits; i++)
                    pattern += "[0-9]";

                cmd.CommandText = string.Format("SELECT TOP 1 {1} FROM [" + dataname.ToString() + "].[dbo].[{0}]  \n" +
                                                "WHERE {1} LIKE N'{2}' ORDER BY {1} DESC",
                                                tableName, columnName, pattern);

                Int64 maxOldID = 0;
                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string maxOldSoHieu = (string)rd[columnName];
                    if (StartWithSoMay(maxOldSoHieu)) maxOldSoHieu = CutSoMayWithOutSoChungTu(maxOldSoHieu);
                    if (maxOldSoHieu.StartsWith(prefix)) maxOldSoHieu = maxOldSoHieu.Substring(prefix.Length);
                    if (maxOldSoHieu.Length > noDigits) maxOldSoHieu = maxOldSoHieu.Substring(maxOldSoHieu.Length - noDigits);

                    Regex regex = new Regex(string.Format(@"(?<=.*)(\d)+"));
                    Match match = regex.Match(maxOldSoHieu);
                    try
                    {
                        maxOldID = Int64.Parse(match.Value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                rd.Close();

                while (true)
                {
                    maxOldID++;
                    string strDigits = maxOldID.ToString();
                    int digitsLength = strDigits.Length;
                    for (int i = digitsLength; i < noDigits; i++)
                    {
                        strDigits = "0" + strDigits;
                    }

                    string soChungTu = "";

                    soChungTu = string.Format("{0}{1}", prefix, strDigits);

                    cmd.CommandText = string.Format("SELECT {0} FROM ["+ dataname.ToString() + "].[dbo].[{1}] WHERE {0} = N'{2}' ",
                                                        columnName, tableName, pattern);
                    rd = cmd.ExecuteReader();
                    if (!rd.Read())
                    {
                        string checkSoMay = soChungTu.Substring(0, 0);
                        ////Cần dùng nếu muốn thêm đầu số
                        //if (checkSoMay.Equals(LocalSettings.HeThong.SoMay) && !coKemSoMay)
                        //{
                        //    soChungTu = soChungTu.Substring(LocalSettings.HeThong.SoMay.Length - 1, soChungTu.Length - LocalSettings.HeThong.SoMay.Length);
                        //}
                        //else if (!checkSoMay.Equals(LocalSettings.HeThong.SoMay) && coKemSoMay)
                        //{
                        //    soChungTu = string.Format("{0}{1}", LocalSettings.HeThong.SoMay, soChungTu);
                        //}

                        return soChungTu;
                        rd.Close();
                    }
                    else
                    {
                        rd.Close();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static DataTable GetDataBySql(string selectSQL)
        {
            SqlConnection con = new System.Data.SqlClient.SqlConnection(LocalSettings.ConnectString);
            try
            {
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandTimeout = 10000 * 10;

                SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                DataTable dt = new System.Data.DataTable();

                cmd.CommandText = selectSQL;
                da.SelectCommand = cmd;

                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }
        public static string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        public static string ConvertToString(string str)
        {
            string mybool = System.Convert.ToString(str);
            return mybool;
        }
        public static bool ConvertToBool(string str)
        {
            bool mybool = System.Convert.ToBoolean(str);
            return mybool;
        }

        public static string GetSystemDateTimeDayOfYear(SqlCommand cmd)
        {
            try
            {
                cmd.CommandText = "SELECT GETDATE()";
                DateTime retDate = (DateTime)cmd.ExecuteScalar();

                int dayOfYear = retDate.DayOfYear;
                string result = dayOfYear.ToString();
                int noDigits = 3; //1 năm có 365 --> cần format đến hàng trăm


                for (int i = result.Length; i < noDigits; i++)
                {
                    result = "0" + result;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string DateTimeRealDayForSQLToString(DateTime dateTime)  //2018-12-31 HH:mm:ss
        {
            try
            {
                return dateTime.Year.ToString() + "-" + dateTime.Month.ToString() + "-" + dateTime.Day.ToString() + " " + dateTime.Hour.ToString() + ":" + dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IEnumerable<DataRow> AsEnumerable(DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                yield return table.Rows[i];
            }
        }
    }
}
