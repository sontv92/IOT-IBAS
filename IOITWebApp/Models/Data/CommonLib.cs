using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Sockets;
using System.Linq;

namespace IOITWebApp.Models.Data
{
    public class CommonLibData
    {
        public static DataTable GetDataBySql(string selectSQL, string connectStringPrivate)
        {
            SqlConnection con = new System.Data.SqlClient.SqlConnection(connectStringPrivate);
            try
            {
                con.Open();

                SqlCommand cmd = con.CreateCommand();
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
    }
}
