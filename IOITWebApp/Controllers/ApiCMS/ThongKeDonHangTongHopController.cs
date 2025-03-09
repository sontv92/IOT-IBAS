using ClosedXML.Excel;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace IOITWebApp.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeDonHangTongHopController : Controller
    {
        private static readonly ILog log = LogMaster.GetLogger("thongkedonhangtonghop", "thongkedonhangtonghop");
        private static string functionCode = "TKDHTH";
        private IHostingEnvironment _hostingEnvironment;
        public ThongKeDonHangTongHopController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            try
            {
                DefaultResponse def = new DefaultResponse();
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                {
                    def.meta = new Models.Meta(222, "No permission");
                    return Ok(def);
                }
                if (paging != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        List<ThongKeDonHangTongHopDTO> lstTong = new List<ThongKeDonHangTongHopDTO>();
                        List<ThongKeDonHangTongHopDTO> lstxe = new List<ThongKeDonHangTongHopDTO>();
                        if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined")
                        {
                            paging.TENKHACHHANG = "";
                        }
                        if (paging.BIENSO is null || paging.BIENSO == "undefined")
                        {
                            paging.BIENSO = "";
                        }
                        if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined")
                        {
                            paging.TENMACBETONG = "";
                        }
                        if (paging.Branchlist is null || paging.Branchlist == "undefined")
                        {
                            paging.Branchlist = "";
                        }
                        if (paging.TENNV is null || paging.TENNV == "undefined")
                        {
                            paging.TENNV = "";
                        }
                        if (paging.TENHANGMUC is null || paging.TENHANGMUC == "undefined")
                        {
                            paging.TENHANGMUC = "";
                        }

                        if (paging.CHEDO is null || paging.CHEDO == "undefined")
                        {
                            paging.CHEDO = "";
                        }

                        int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                        int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                        int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                        int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                        DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                        DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);

                        DataTable _TableCuaVL = null;

                        if (paging.Branchlist != "" && paging.Branchlist != null)
                        {
                            var arrListStr = paging.Branchlist.Split(',');
                            int i = 0;
                            string cuaVLSOLUONG1 = "";
                            string cuaVLSOLUONG2 = "";
                            string cuaVLSOLUONGT1 = "";
                            string cuaVLSOLUONGT2 = "";
                            string cuaVLSOLUONGCP1 = "";
                            string cuaVLSOLUONGCP2 = "";
                            string selectByCuaVL = "";
                            foreach (var item in arrListStr)
                            {
                                if (item != "")
                                {
                                    Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (branch != null)
                                    {
                                        _TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL  FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");

                                        if (_TableCuaVL != null && _TableCuaVL.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in _TableCuaVL.Rows)
                                            {
                                                string tenCuaVL = CommonLib.ConvertToString(row["TENCUAVL"].ToString());
                                                string maCuaVL = CommonLib.ConvertToString(row["STTCUAVL"].ToString());
                                                bool cophaiPhuGia = CommonLib.ConvertToBool(row["COPHAIPHUGIA"].ToString());
                                                if (maCuaVL.Trim() != "")
                                                {
                                                    cuaVLSOLUONG1 += string.Format("ISNULL(p.[{0}],0) [{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONG2 += string.Format("[{0}], ", maCuaVL);

                                                    // dạng T.[Sand 1], ..
                                                    cuaVLSOLUONGT1 += string.Format("ISNULL(p.[T_{0}],0) [T_{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONGT2 += string.Format("[T_{0}], ", maCuaVL);

                                                    // dạng CP.[Sand 1], ..
                                                    cuaVLSOLUONGCP1 += string.Format("ISNULL(p.[CP_{0}],0) [CP_{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONGCP2 += string.Format("[CP_{0}], ", maCuaVL);

                                                    //dạng , SUM(ISNULL(D.[CP_Cát 1],0)) N'CP_Cát 1', SUM(ISNULL(B.[Cát 1],0)) N'Cát 1', SUM(ISNULL(C.[T_Cát 1],0)) N'T_Cát 1', (SUM(ISNULL(B.[Cát 1],0)) + SUM(ISNULL(C.[T_Cát 1],0)) - SUM(ISNULL(D.[CP_Cát 1],0))) N'Sai số_Cát 1', ISNULL(abs(SUM(ISNULL(B.[Cát 1],0)) + SUM(ISNULL(C.[T_Cát 1],0)) - SUM(ISNULL(D.[CP_Cát 1],0))) / SUM(nullif(D.[CP_Cát 1], 0)) * 100,0) '%_Cát 1' 
                                                    selectByCuaVL += string.Format(", ROUND(SUM(ISNULL(D.[CP_{0}],0)),2) N'CP_{1}', ROUND(SUM(ISNULL(B.[{0}],0)),2) N'{1}', ROUND(SUM(ISNULL(C.[T_{0}],0)),2) N'T_{1}', ROUND((SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))),2) N'Sai số_{1}', ROUND(ISNULL(abs(SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))) / SUM(nullif(D.[CP_{0}], 0)) * 100,0),2) '%_{1}' \n", maCuaVL, tenCuaVL);
                                                }
                                            }
                                        }
                                        if (cuaVLSOLUONG1.EndsWith(", ")) cuaVLSOLUONG1 = cuaVLSOLUONG1.Substring(0, cuaVLSOLUONG1.Length - 2);
                                        if (cuaVLSOLUONG2.EndsWith(", ")) cuaVLSOLUONG2 = cuaVLSOLUONG2.Substring(0, cuaVLSOLUONG2.Length - 2);
                                        if (cuaVLSOLUONGT1.EndsWith(", ")) cuaVLSOLUONGT1 = cuaVLSOLUONGT1.Substring(0, cuaVLSOLUONGT1.Length - 2);
                                        if (cuaVLSOLUONGT2.EndsWith(", ")) cuaVLSOLUONGT2 = cuaVLSOLUONGT2.Substring(0, cuaVLSOLUONGT2.Length - 2);
                                        if (cuaVLSOLUONGCP1.EndsWith(", ")) cuaVLSOLUONGCP1 = cuaVLSOLUONGCP1.Substring(0, cuaVLSOLUONGCP1.Length - 2);
                                        if (cuaVLSOLUONGCP2.EndsWith(", ")) cuaVLSOLUONGCP2 = cuaVLSOLUONGCP2.Substring(0, cuaVLSOLUONGCP2.Length - 2);

                                        string tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";

                                        if (!paging.TENKHACHHANG.Equals("")) tenKHCond = string.Format("H.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG.ToString());
                                        if (!paging.BIENSO.Equals("")) xeCond = string.Format("A.BIENSO = N'{0}'", paging.BIENSO.ToString());
                                        if (!paging.TENMACBETONG.Equals("")) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG.ToString());
                                        if (!paging.TENHANGMUC.Equals("")) tenHangMucCond = string.Format("H.TENHANGMUC = N'{0}'", paging.TENHANGMUC.ToString());
                                        if (!paging.TENNV.Equals("")) nvkdCond = string.Format("H.TENNV = N'{0}'", paging.TENNV.ToString());
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");

                                       
                                        String subQuerySumSOLUONG = string.Format("	SELECT [MACHITIETMETRON], {8} \n" +
                                                        "		FROM ( \n" +
                                                        "		SELECT DISTINCT B.MACHITIETMETRON, D.STTCUAVL STTCUAVL\n" +
                                                        "			, (ISNULL(D.SOLUONGTD,0)) SUMSOLUONG \n" +
                                                        "		FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                        "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID   \n" +
                                                        "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT   \n" +
                                                        "			LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                                        "		WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                        "           AND {2} AND {3} AND {4} AND {5} AND {6} AND {7} \n" +
                                                        "	) AS j  \n" +
                                                        "	PIVOT (SUM(SUMSOLUONG) FOR [STTCUAVL] in ({9})) AS p \n",
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                                        cuaVLSOLUONG1, cuaVLSOLUONG2);


                                        String subQuerySumSOLUONGT = subQuerySumSOLUONG.Replace("(ISNULL(D.SOLUONGTD,0))", "(ISNULL(D.SOLUONGTAY,0))");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace("D.STTCUAVL", "N'T_' + CAST(D.STTCUAVL as varchar(10))");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace("D@STTCUAVL", "D.STTCUAVL");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace(cuaVLSOLUONG1, cuaVLSOLUONGT1);
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace(cuaVLSOLUONG2, cuaVLSOLUONGT2);

                                        String subQuerySumSOLUONGCP = subQuerySumSOLUONG.Replace("(ISNULL(D.SOLUONGTD,0))", "(ISNULL(D.SOLUONGCP,0))");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace("D.STTCUAVL", "N'CP_' + CAST(D.STTCUAVL as varchar(10))");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace("D@STTCUAVL", "D.STTCUAVL");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace(cuaVLSOLUONG1, cuaVLSOLUONGCP1);
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace(cuaVLSOLUONG2, cuaVLSOLUONGCP2);

                                        subQuerySumSOLUONG = subQuerySumSOLUONG.Replace("D@STTCUAVL", "D.STTCUAVL");



                                        command.CommandText = "select * INTO #Result FROM (";

                                        

                                        String sql = string.Format("SELECT A.[Mã phiếu], A.[Ngày trộn], MIN(A.[Giờ bắt đầu]) [Giờ bắt đầu], MAX(A.[Giờ kết thúc])[Giờ kết thúc], A.[Khách hàng], MAX(A.[Dự án]) [Dự án], A.[Biển số], A.[Nhân viên KD], A.[Tên mác BT],A.[Chế độ], ROUND(SUM(A.[Thể tích]),2) [Thể tích] \n" +
                                        " {11} \n" +
                                        "FROM ( \n" +
                                        "	SELECT DISTINCT B.MACHITIETMETRON MACHITIETMETRON_MAIN, B.MALSTRON N'Mã phiếu', B.SOTTMETRON N'STT mẻ trộn'  \n" +
                                        "		, FORMAT(A.NGAYTRON, 'dd/MM/yyy') N'Ngày trộn', FORMAT(A.GIOBATDAU, 'hh:mm tt') N'Giờ bắt đầu', FORMAT(A.GIOXONG, 'hh:mm tt') N'Giờ kết thúc' \n" +
                                        "		, H.TENKHACHHANG N'Khách hàng', H.TENDUAN N'Dự án', A.BIENSO N'Biển số', H.TENNV N'Nhân viên KD', A.TENMACBETONG N'Tên mác BT', A.CHEDO N'Chế độ', B.M3METRON N'Thể tích' \n" +
                                        "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                        "		LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                        "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                        "       AND {2} AND {3} AND {4} AND {5} AND {6} AND {7}\n" +
                                        ") AS A LEFT JOIN ( \n" +
                                        "	 {8}\n" +
                                        ") AS B ON A.MACHITIETMETRON_MAIN = B.MACHITIETMETRON LEFT JOIN ( \n" +
                                        "	 {9}\n" +
                                        ") AS C ON A.MACHITIETMETRON_MAIN = C.MACHITIETMETRON LEFT JOIN ( \n" +
                                        "	 {10}\n" +
                                        ") AS D ON A.MACHITIETMETRON_MAIN = D.MACHITIETMETRON \n" +
                                        "GROUP BY A.[Mã phiếu], A.[Ngày trộn], A.[Khách hàng], A.[Biển số], A.[Nhân viên KD], A.[Tên mác BT], A.[Chế độ]  \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                        subQuerySumSOLUONG, subQuerySumSOLUONGT, subQuerySumSOLUONGCP,
                                        selectByCuaVL);

                                        command.CommandText += sql.ToString();

                                        command.CommandText += ") as TongHop; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT *  FROM #Result ORDER BY N'Mã phiếu'  OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);

                                        context.Database.OpenConnection();
                                        using (var result = command.ExecuteReader())
                                        {
                                            result.Read();

                                            def.metadata = result[0];
                                            result.NextResult();

                                            DataTable dtresult = new DataTable();


                                            dtresult.Load(result);

                                            DataTable newTable = dtresult.Clone();

                                            string list = "";
                                            DataRow rowTong = newTable.NewRow();
                                            rowTong["Chế độ"] = "TỔNG";

                                        //B1: Tổng quá cho tất cả các cột kiểu Int, Double, Float, Decimal SUM()
                                            foreach (DataColumn col in newTable.Columns)
                                            {
                                                string type = col.DataType.Name.ToString().ToUpper();
                                                string colName = col.ColumnName;
                                                if (colName != "STT" && colName != "Mã phiếu" && colName != "STT mẻ trộn")
                                                {
                                                    list += col.DataType.Name.ToString().ToUpper() + ", ";   //Để xem có các kiểu dữ liệu gì dạng số

                                                    switch (col.DataType.Name.ToString().ToUpper())
                                                    {
                                                        case "INT32":
                                                        case "INT64":
                                                            try
                                                            {
                                                                rowTong[colName] = (int)dtSource.Compute(string.Format("SUM({0})", colName), "");
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                col.AllowDBNull = true;
                                                            }
                                                            break;
                                                        case "DOUBLE":
                                                        case "FLOAT":
                                                        case "DECIMAL":
                                                            try
                                                            {
                                                                rowTong[colName] = Math.Round((double)dtSource.Compute(string.Format("SUM([{0}])", colName), ""),2);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                col.AllowDBNull = true;
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    col.AllowDBNull = true;
                                                }
                                            }

                                            //B2: Một số cột sẽ công thức khác sẽ tính lại ở đây
                                            foreach (DataColumn col in dtSource.Columns)
                                            {
                                                string colName = col.ColumnName;
                                                if (colName.ToUpper().StartsWith("%"))
                                                {
                                                    string tenCUAVL = colName.Replace("%_", ""); //Chỉ lấy tên cửa VL, VD từu '%_Cát 1' --> 'Cát 1'
                                                    double sumSaiSo = (rowTong["Sai số_" + tenCUAVL] is DBNull) ? 0 : Math.Round((double)rowTong["Sai số_" + tenCUAVL], 2);
                                                    double sumCP = (rowTong["CP_" + tenCUAVL] is DBNull) ? 0 : Math.Round((double)rowTong["CP_" + tenCUAVL], 2);

                                                    if (sumCP != 0) rowTong[colName] = Math.Round((double)(Math.Abs(sumSaiSo) / sumCP) * 100, 2);
                                                }
                                            }
                                            newTable.Rows.Add(rowTong);


                                            foreach (DataColumn col in dtresult.Columns)
                                            {
                                                ThongKeDonHangTongHopDTO items = new ThongKeDonHangTongHopDTO();
                                                string colName = col.ColumnName;


                                                if (colName != "ID")
                                                {
                                                    items.header = colName;
                                                    items.rows = new List<string>();
                                                    for (int m = 0; m < dtresult.Rows.Count; m++)
                                                    {
                                                        var myValue = dtresult.Rows[m][colName];
                                                        if (myValue.ToString().Length>0)
                                                        {
                                                            
                                                            items.rows.Add(myValue.ToString());
                                                        }
                                                        else
                                                        {
                                                            items.rows.Add("NULL");
                                                        }
                                                        
                                                        
                                                    }
                                                    lstxe.Add(items);
                                                }

                                            }

                                            def.data = lstxe;
                                            foreach (DataColumn col in newTable.Columns)
                                            {
                                                ThongKeDonHangTongHopDTO items = new ThongKeDonHangTongHopDTO();
                                                string colName = col.ColumnName;


                                                if (colName != "ID")
                                                {
                                                    items.header = colName;
                                                    items.rows = new List<string>();
                                                    for (int m = 0; m < newTable.Rows.Count; m++)
                                                    {
                                                        var myValue = newTable.Rows[m][colName];
                                                        if (myValue.ToString().Length > 0)
                                                        {

                                                            items.rows.Add(myValue.ToString());
                                                        }
                                                        else
                                                        {
                                                            items.rows.Add("0");
                                                        }


                                                    }
                                                    lstTong.Add(items);
                                                }

                                            }
                                            def.data1 = lstTong;

                                        }
                                    }

                                }

                            }
                        }

                        def.meta = new Models.Meta(200, "Success");
                        return Ok(def);
                    }
                }
                else
                {
                    def.meta = new Models.Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            catch (Exception ex)
            {
                DefaultResponse def = new DefaultResponse();
                def.data = null;
                def.meta = new Models.Meta(400, "Bad Request");
                return Ok(def);
                throw;
            }

            

        }
        [HttpGet("GetReportTongHop")]
        public HttpResponseMessage GetReportTongHop([FromQuery] FilteredPagination paging)
        {
            try
            {
                DefaultResponse def = new DefaultResponse();

                if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined")
                {
                    paging.TENKHACHHANG = "";
                }
                if (paging.BIENSO is null || paging.BIENSO == "undefined")
                {
                    paging.BIENSO = "";
                }
                if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined")
                {
                    paging.TENMACBETONG = "";
                }
                if (paging.Branchlist is null || paging.Branchlist == "undefined")
                {
                    paging.Branchlist = "";
                }
                if (paging.TENNV is null || paging.TENNV == "undefined")
                {
                    paging.TENNV = "";
                }
                if (paging.TENHANGMUC is null || paging.TENHANGMUC == "undefined")
                {
                    paging.TENHANGMUC = "";
                }

                if (paging.CHEDO is null || paging.CHEDO == "undefined")
                {
                    paging.CHEDO = "";
                }


                int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);

                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                {
                    return null;
                }
                DataTable _TableCuaVL = null;

                if (paging != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandTimeout = 300;
                        List<rpthongkeDTO> rpdonhang = new List<rpthongkeDTO>();
                        command.CommandText = "";



                        if (paging.Branchlist != "" && paging.Branchlist != null)
                        {
                            var arrListStr = paging.Branchlist.Split(',');
                            int ii = 0;
                            string cuaVLSOLUONG1 = "";
                            string cuaVLSOLUONG2 = "";
                            string cuaVLSOLUONGT1 = "";
                            string cuaVLSOLUONGT2 = "";
                            string cuaVLSOLUONGCP1 = "";
                            string cuaVLSOLUONGCP2 = "";
                            string selectByCuaVL = "";
                            foreach (var item in arrListStr)
                            {
                                if (item != "")
                                {
                                    Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (branch != null)
                                    {

                                        
                                        _TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");

                                        if (_TableCuaVL != null && _TableCuaVL.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in _TableCuaVL.Rows)
                                            {
                                                string tenCuaVL = CommonLib.ConvertToString(row["TENCUAVL"].ToString());
                                                string maCuaVL = CommonLib.ConvertToString(row["STTCUAVL"].ToString());
                                                bool cophaiPhuGia = CommonLib.ConvertToBool(row["COPHAIPHUGIA"].ToString());
                                                if (tenCuaVL.Trim() != "")
                                                {
                                                    cuaVLSOLUONG1 += string.Format("ISNULL(p.[{0}],0) [{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONG2 += string.Format("[{0}], ", maCuaVL);

                                                    // dạng T.[Sand 1], ..
                                                    cuaVLSOLUONGT1 += string.Format("ISNULL(p.[T_{0}],0) [T_{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONGT2 += string.Format("[T_{0}], ", maCuaVL);

                                                    // dạng CP.[Sand 1], ..
                                                    cuaVLSOLUONGCP1 += string.Format("ISNULL(p.[CP_{0}],0) [CP_{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONGCP2 += string.Format("[CP_{0}], ", maCuaVL);

                                                    //dạng , SUM(ISNULL(D.[CP_Cát 1],0)) N'CP_Cát 1', SUM(ISNULL(B.[Cát 1],0)) N'Cát 1', SUM(ISNULL(C.[T_Cát 1],0)) N'T_Cát 1', (SUM(ISNULL(B.[Cát 1],0)) + SUM(ISNULL(C.[T_Cát 1],0)) - SUM(ISNULL(D.[CP_Cát 1],0))) N'Sai số_Cát 1', ISNULL(abs(SUM(ISNULL(B.[Cát 1],0)) + SUM(ISNULL(C.[T_Cát 1],0)) - SUM(ISNULL(D.[CP_Cát 1],0))) / SUM(nullif(D.[CP_Cát 1], 0)) * 100,0) '%_Cát 1' 
                                                    //selectByCuaVL += string.Format(", ROUND(SUM(ISNULL(D.[CP_{0}],0)),2) N'CP_{1}', ROUND(SUM(ISNULL(B.[{0}],0)),2) N'{1}', ROUND(SUM(ISNULL(C.[T_{0}],0)),2) N'T_{1}', ROUND((SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))),2) N'Sai số_{1}', ROUND(ISNULL(abs(SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))) / SUM(nullif(D.[CP_{0}], 0)) * 100,0),2) '%_{1}' \n", maCuaVL, tenCuaVL);
                                                    selectByCuaVL += string.Format(", ROUND(SUM(ISNULL(D.[CP_{0}],0)),2) N'CP_{1}', ROUND(SUM(ISNULL(B.[{0}],0)),2) N'{1}', ROUND(SUM(ISNULL(C.[T_{0}],0)),2) N'T_{1}', ROUND((SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))),2) N'Sai số_{1}', ROUND(ISNULL(abs(SUM(ISNULL(B.[{0}],0)) + SUM(ISNULL(C.[T_{0}],0)) - SUM(ISNULL(D.[CP_{0}],0))) / SUM(nullif(D.[CP_{0}], 0)) * 100,0),2) '%_{1}' \n", maCuaVL, tenCuaVL);


                                                }
                                            }
                                        }

                                        if (cuaVLSOLUONG1.EndsWith(", ")) cuaVLSOLUONG1 = cuaVLSOLUONG1.Substring(0, cuaVLSOLUONG1.Length - 2);
                                        if (cuaVLSOLUONG2.EndsWith(", ")) cuaVLSOLUONG2 = cuaVLSOLUONG2.Substring(0, cuaVLSOLUONG2.Length - 2);
                                        if (cuaVLSOLUONGT1.EndsWith(", ")) cuaVLSOLUONGT1 = cuaVLSOLUONGT1.Substring(0, cuaVLSOLUONGT1.Length - 2);
                                        if (cuaVLSOLUONGT2.EndsWith(", ")) cuaVLSOLUONGT2 = cuaVLSOLUONGT2.Substring(0, cuaVLSOLUONGT2.Length - 2);
                                        if (cuaVLSOLUONGCP1.EndsWith(", ")) cuaVLSOLUONGCP1 = cuaVLSOLUONGCP1.Substring(0, cuaVLSOLUONGCP1.Length - 2);
                                        if (cuaVLSOLUONGCP2.EndsWith(", ")) cuaVLSOLUONGCP2 = cuaVLSOLUONGCP2.Substring(0, cuaVLSOLUONGCP2.Length - 2);

                                        string tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";

                                        if (!paging.TENKHACHHANG.Equals("")) tenKHCond = string.Format("H.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG.ToString());
                                        if (!paging.BIENSO.Equals("")) xeCond = string.Format("A.BIENSO = N'{0}'", paging.BIENSO.ToString());
                                        if (!paging.TENMACBETONG.Equals("")) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG.ToString());
                                        if (!paging.TENHANGMUC.Equals("")) tenHangMucCond = string.Format("H.TENHANGMUC = N'{0}'", paging.TENHANGMUC.ToString());
                                        if (!paging.TENNV.Equals("")) nvkdCond = string.Format("H.TENNV = N'{0}'", paging.TENNV.ToString());
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");

                                        String subQuerySumSOLUONG = string.Format("	SELECT [MACHITIETMETRON], {8} \n" +
                                                        "		FROM ( \n" +
                                                        "		SELECT DISTINCT B.MACHITIETMETRON, D.STTCUAVL STTCUAVL\n" +
                                                        "			, (ISNULL(D.SOLUONGTD,0)) SUMSOLUONG \n" +
                                                        "		FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                        "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID   \n" +
                                                        "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT   \n" +
                                                        "			LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                                        "		WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                        "           AND {2} AND {3} AND {4} AND {5} AND {6} AND {7} \n" +
                                                        "	) AS j  \n" +
                                                        "	PIVOT (SUM(SUMSOLUONG) FOR [STTCUAVL] in ({9})) AS p \n",
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                                        cuaVLSOLUONG1, cuaVLSOLUONG2);


                                        String subQuerySumSOLUONGT = subQuerySumSOLUONG.Replace("(ISNULL(D.SOLUONGTD,0))", "(ISNULL(D.SOLUONGTAY,0))");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace("D.STTCUAVL", "N'T_' + CAST(D.STTCUAVL as varchar(10))");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace("D@STTCUAVL", "D.STTCUAVL");
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace(cuaVLSOLUONG1, cuaVLSOLUONGT1);
                                        subQuerySumSOLUONGT = subQuerySumSOLUONGT.Replace(cuaVLSOLUONG2, cuaVLSOLUONGT2);

                                        String subQuerySumSOLUONGCP = subQuerySumSOLUONG.Replace("(ISNULL(D.SOLUONGTD,0))", "(ISNULL(D.SOLUONGCP,0))");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace("D.STTCUAVL", "N'CP_' + CAST(D.STTCUAVL as varchar(10))");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace("D@STTCUAVL", "D.STTCUAVL");
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace(cuaVLSOLUONG1, cuaVLSOLUONGCP1);
                                        subQuerySumSOLUONGCP = subQuerySumSOLUONGCP.Replace(cuaVLSOLUONG2, cuaVLSOLUONGCP2);

                                        subQuerySumSOLUONG = subQuerySumSOLUONG.Replace("D@STTCUAVL", "D.STTCUAVL");


                                        String sql = string.Format("SELECT A.[Mã phiếu], A.[Ngày trộn], MIN(A.[Giờ bắt đầu]) [Giờ bắt đầu], MAX(A.[Giờ kết thúc])[Giờ kết thúc], A.[Khách hàng], MAX(A.[Dự án]) [Dự án], A.[Nhân viên KD], A.[Tên mác BT],A.[Chế độ], ROUND(SUM(A.[Thể tích]),2) [Thể tích] \n" +
                                        " {11} \n" +
                                        "FROM ( \n" +
                                        "	SELECT DISTINCT B.MACHITIETMETRON MACHITIETMETRON_MAIN, B.MALSTRON N'Mã phiếu', B.SOTTMETRON N'STT mẻ trộn'  \n" +
                                        "		, FORMAT(A.NGAYTRON, 'dd/MM/yyy') N'Ngày trộn', FORMAT(A.GIOBATDAU, 'HH:mm') N'Giờ bắt đầu', FORMAT(A.GIOXONG, 'HH:mm') N'Giờ kết thúc' \n" +
                                        "		, H.TENKHACHHANG N'Khách hàng', H.TENDUAN N'Dự án', A.BIENSO N'Biển số', H.TENNV N'Nhân viên KD', A.TENMACBETONG N'Tên mác BT', A.CHEDO N'Chế độ', B.M3METRON N'Thể tích' \n" +
                                        "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                        "		LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                        "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                        "       AND {2} AND {3} AND {4} AND {5} AND {6} AND {7}\n" +
                                        ") AS A LEFT JOIN ( \n" +
                                        "	 {8}\n" +
                                        ") AS B ON A.MACHITIETMETRON_MAIN = B.MACHITIETMETRON LEFT JOIN ( \n" +
                                        "	 {9}\n" +
                                        ") AS C ON A.MACHITIETMETRON_MAIN = C.MACHITIETMETRON LEFT JOIN ( \n" +
                                        "	 {10}\n" +
                                        ") AS D ON A.MACHITIETMETRON_MAIN = D.MACHITIETMETRON \n" +
                                        "GROUP BY A.[Mã phiếu], A.[Ngày trộn], A.[Khách hàng], A.[Nhân viên KD], A.[Tên mác BT], A.[Chế độ]  \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                        subQuerySumSOLUONG, subQuerySumSOLUONGT, subQuerySumSOLUONGCP,
                                        selectByCuaVL);

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);

                                        

                                        //Tính tổng dòng cuối, chỉ web mới dùng
                                        string list = "";
                                        DataRow rowTong = dtSource.NewRow();
                                        rowTong["Khách hàng"] = "TỔNG";

                                        //B1: Tổng quá cho tất cả các cột kiểu Int, Double, Float, Decimal SUM()
                                        foreach (DataColumn col in dtSource.Columns)
                                        {
                                            string type = col.DataType.Name.ToString().ToUpper();
                                            string colName = col.ColumnName;
                                            if (colName != "STT" && colName != "Mã phiếu" && colName != "STT mẻ trộn")
                                            {
                                                list += col.DataType.Name.ToString().ToUpper() + ", ";   //Để xem có các kiểu dữ liệu gì dạng số

                                                switch (col.DataType.Name.ToString().ToUpper())
                                                {
                                                    case "INT32":
                                                    case "INT64":
                                                        try
                                                        {
                                                            rowTong[colName] = (int)dtSource.Compute(string.Format("SUM({0})", colName), "");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            col.AllowDBNull = true;
                                                        }
                                                        break;
                                                    case "SINGLE":
                                                        try
                                                        {
                                                            rowTong[colName] = Math.Round((Single)dtSource.Compute(string.Format("SUM([{0}])", colName), ""), 2);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            col.AllowDBNull = true;
                                                        }
                                                        break;
                                                    case "DOUBLE":
                                                        try
                                                        {
                                                            rowTong[colName] = Math.Round((double)dtSource.Compute(string.Format("SUM([{0}])", colName), ""),2);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            col.AllowDBNull = true;
                                                        }
                                                        break;
                                                    case "FLOAT":
                                                    case "DECIMAL":
                                                        try
                                                        {
                                                            rowTong[colName] = Math.Round((double)dtSource.Compute(string.Format("SUM([{0}])", colName), ""), 2);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            col.AllowDBNull = true;
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }

                                        //B2: Một số cột sẽ công thức khác sẽ tính lại ở đây
                                        foreach (DataColumn col in dtSource.Columns)
                                        {
                                            string colName = col.ColumnName;
                                            if (colName.ToUpper().StartsWith("%"))
                                            {
                                                string tenCUAVL = colName.Replace("%_", ""); //Chỉ lấy tên cửa VL, VD từu '%_Cát 1' --> 'Cát 1'
                                               
                                                double sumSaiSo = (rowTong["Sai số_" + tenCUAVL] is DBNull) ? 0 : Math.Round((double)rowTong["Sai số_" + tenCUAVL], 2);
                                                double sumCP = (rowTong["CP_" + tenCUAVL] is DBNull) ? 0 : Math.Round((double)rowTong["CP_" + tenCUAVL], 2);

                                                if (sumCP != 0) rowTong[colName] = Math.Round((double)(Math.Abs(sumSaiSo) / sumCP) * 100,2);
                                            }
                                        }
                                        dtSource.Rows.Add(rowTong);


                                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                        using (var package = new ExcelPackage())
                                        {
                                            var alphabet = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,AA,AB,AC,AD,AE,AF,AG,AH,AI,AJ,AK,AL,AM,AN,AO,AP,AQ,AR,AS,AT,AU,AV,AW,AX,AY,AZ,"
                                                            + "BA,BB,BC,BD,BE,BF,BG,BH,BI,BJ,BK,BL,BM,BN,BO,BP,BQ,BR,BS,BT,BU,BV,BW,BX,BY,BZ,"
                                                            + "CA,CB,CC,CD,CE,CF,CG,CH,CI,CJ,CK,CL,CM,CN,CO,CP,CQ,CR,CS,CT,CU,CV,CW,CX,CY,CZ,"
                                                            + "DA,DB,DC,DD,DE,DF,DG,DH,DI,DJ,DK,DL,DM,DN,DO,DP,DQ,DR,DS,DT,DU,DV,DW,DX,DY,DZ";
                                            var arrAlphabet = alphabet.Split(",");
                                            var lastAlphabet = string.Empty;
                                            lastAlphabet = arrAlphabet[dtSource.Columns.Count - 1];

                                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê tổng hợp đồng");
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Merge = true;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Value = "THỐNG KÊ TỔNG HỢP ĐỒNG";
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Style.Font.Bold = true;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Style.Font.Size = 16;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A1:Y1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Merge = true;
                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Value = "Báo cáo được tạo vào ngày " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Style.Font.Italic = true;
                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A2:" + lastAlphabet + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            //Điều kiện lọc
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Merge = true;
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Value = "* Điều kiện:";
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Style.Font.Italic = true;
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Style.Font.Bold = true;
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A3:" + lastAlphabet + "3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            #region Điều kiện

                                            // Khách hàng
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Merge = true;
                                            var valueFilterKhachhang = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.TENKHACHHANG))
                                            {
                                                valueFilterKhachhang = paging.TENKHACHHANG;
                                            }
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Value = "- Khách hàng: " + valueFilterKhachhang;
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Tên mác be tông
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Merge = true;
                                            var valueFilterMac = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.TENMACBETONG))
                                            {
                                                valueFilterMac = paging.TENMACBETONG;
                                            }
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Value = "- Tên mác bê tông: " + valueFilterMac;
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Biển số
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Merge = true;
                                            var valueFilterBienSo = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.BIENSO))
                                            {
                                                valueFilterBienSo = paging.BIENSO;
                                            }
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Value = "- Biển số: " + valueFilterBienSo;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Nhân viên
                                            worksheet.Cells["A7:" + lastAlphabet + "7"].Merge = true;
                                            var valueFilterNV = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.TENNV))
                                            {
                                                valueFilterNV = paging.TENNV;
                                            }
                                            worksheet.Cells["A7:" + lastAlphabet + "7"].Value = "- Nhân viên: " + valueFilterNV;
                                            worksheet.Cells["A7:" + lastAlphabet + "7"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A7:" + lastAlphabet + "7"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A7:" + lastAlphabet + "7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Hạng mục
                                            worksheet.Cells["A8:" + lastAlphabet + "8"].Merge = true;
                                            var valueFilterHM = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.TENHANGMUC))
                                            {
                                                valueFilterHM = paging.TENHANGMUC;
                                            }
                                            worksheet.Cells["A8:" + lastAlphabet + "8"].Value = "- Hạng mục: " + valueFilterHM;
                                            worksheet.Cells["A8:" + lastAlphabet + "8"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A8:" + lastAlphabet + "8"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A8:" + lastAlphabet + "8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Chế độ
                                            worksheet.Cells["A9:" + lastAlphabet + "9"].Merge = true;
                                            var valueFilterCD = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.CHEDO))
                                            {
                                                valueFilterCD = paging.CHEDO;
                                            }
                                            worksheet.Cells["A9:" + lastAlphabet + "9"].Value = "- Chế độ: " + valueFilterCD;
                                            worksheet.Cells["A9:" + lastAlphabet + "9"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A9:" + lastAlphabet + "9"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A9:" + lastAlphabet + "9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                            #endregion


                                            var index = 11;
                                            var cell = string.Empty;

                                            var count = 0;
                                            foreach (DataColumn col in dtSource.Columns)
                                            {
                                                cell = arrAlphabet[count] + "11";
                                                worksheet.Cells[cell].Value = col.ColumnName;
                                                worksheet.Cells[cell].Style.Font.Bold = true;
                                                count++;

                                            }
                                            cell = "A" + index + ":" + lastAlphabet + index;
                                            worksheet.Cells[cell].Style.Font.Color.SetColor(Color.White);
                                            worksheet.Cells[cell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[cell].Style.Fill.BackgroundColor.SetColor(Color.Green);

                                            if (dtSource.Rows.Count > 0)
                                            {
                                                int rowFirts = 12;
                                                for (int i = 0; i < dtSource.Rows.Count; i++)
                                                {
                                                    int row = rowFirts + i;
                                                    var element = dtSource.Rows[i];

                                                    int column = 1;

                                                    foreach (var rowItem in element.ItemArray)
                                                    {
                                                        worksheet.Cells[row, column].Value = rowItem;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        if (i == dtSource.Rows.Count - 1)
                                                        {
                                                            worksheet.Cells[row, column].Style.Font.Bold = true;
                                                        }

                                                        column++;
                                                    }

                                                }
                                                //Row merge total
                                                var regionTotal = dtSource.Rows.Count + 11;
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Value = "Tổng";
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Merge = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Style.Font.Italic = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Style.Font.Bold = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Center;
                                                worksheet.Cells["A" + regionTotal + ":" + "H" + regionTotal].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



                                                string modelRange = "A12:" + lastAlphabet + (dtSource.Rows.Count + 11);
                                                var modelTable = worksheet.Cells[modelRange];
                                                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                //modelCells.LoadFromCollection(Collection: model, PrintHeaders: true);
                                                worksheet.Cells["A:DZ"].AutoFitColumns();

                                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                                {
                                                    Content = new ByteArrayContent(package.GetAsByteArray())
                                                };
                                                return response;
                                            }
                                        }
                                    }

                                }
                                ++ii;
                            }
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}