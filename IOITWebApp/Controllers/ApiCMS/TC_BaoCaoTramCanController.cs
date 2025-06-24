using ClosedXML.Excel;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
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
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TC_BaoCaoTramCanController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("tc_baocaotramcan", "tc_baocaotramcan");
        private static string functionCode = "BAOCAOTRAMCAN";
        private IHostingEnvironment _hostingEnvironment;
        public TC_BaoCaoTramCanController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    List<ThongKeDonHangChiTietDTO> xe = new List<ThongKeDonHangChiTietDTO>();
                    List<ThongKeDonHangTongHopDTO> lstxe = new List<ThongKeDonHangTongHopDTO>();
                    List<ThongKeDonHangTongHopDTO> lstTong = new List<ThongKeDonHangTongHopDTO>();
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

                    if (paging != null)
                    {
                        List<rpthongkeDTO> rpdonhang = new List<rpthongkeDTO>();
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

                                        command.CommandText = "select * INTO #Result FROM (";


                                        //_TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");
                                        _TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");

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

                                                    //dạng , D.[CP_Cát 1], B.[Cát 1], C.[T_Cát 1], (B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) N'Sai số_Cát 1', abs(B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) / nullif(D.[CP_Cát 1], 0) * 100 '%_Cát 1'
                                                    selectByCuaVL += string.Format(", ISNULL(D.[CP_{0}],0) N'CP_{1}', ISNULL(B.[{0}],0) N'{1}', ISNULL(C.[T_{0}],0) N'T_{1}', (ISNULL(B.[{0}],0) + ISNULL(C.[T_{0}],0) - ISNULL(D.[CP_{0}],0)) N'Sai số_{1}', ISNULL(abs(ISNULL(B.[{0}],0) + ISNULL(C.[T_{0}],0) - ISNULL(D.[CP_{0}],0)) / nullif(D.[CP_{0}], 0) * 100,0) '%_{1}' \n", maCuaVL, tenCuaVL);
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

                                        String sql = string.Format("SELECT ROW_NUMBER() OVER(ORDER BY A.MACHITIETMETRON_MAIN ASC) AS STT, A.* \n" +
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
                                        ") AS D ON A.MACHITIETMETRON_MAIN = D.MACHITIETMETRON \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                        subQuerySumSOLUONG, subQuerySumSOLUONGT, subQuerySumSOLUONGCP,
                                        selectByCuaVL);



                                        command.CommandText += sql.ToString();
                                        command.CommandText += ") as ChiTiet; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT *  FROM #Result ORDER BY MACHITIETMETRON_MAIN  OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";
                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        context.Database.OpenConnection();
                                        using (var result = command.ExecuteReader())
                                        {
                                            result.Read();
                                            def.metadata = result[0];
                                            result.NextResult();

                                            DataTable dtresult = new DataTable();

                                            dtresult.Load(result);


                                            dtresult.Columns.Remove("MACHITIETMETRON_MAIN");

                                            DataTable newTable = dtresult.Clone();

                                            string list = "";
                                            DataRow rowTong = newTable.NewRow();
                                            rowTong["Chế độ"] = "TỔNG";



                                            //B1: Tổng quá cho tất cả các cột kiểu Int, Double, Float, Decimal SUM()
                                            foreach (DataColumn col in newTable.Columns)
                                            {
                                                string type = col.DataType.Name.ToString().ToUpper();
                                                string colName = col.ColumnName;
                                                if (colName != "STT" && colName != "MACHITIETMETRON_MAIN" && colName != "Mã phiếu" && colName != "STT mẻ trộn")
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
                                                            try
                                                            {
                                                                rowTong[colName] = Math.Round((double)dtSource.Compute(string.Format("SUM([{0}])", colName), ""), 2);
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
                                                        items.rows.Add(myValue.ToString());
                                                    }
                                                    lstxe.Add(items);
                                                }

                                            }
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
                                            def.data = lstxe;

                                        }

                                    }

                                }

                            }
                        }
                        def.meta = new Meta(200, "Success");
                        return Ok(def);
                    }
                    else
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

        }

        [HttpGet("GetReportChiTiet")]
        public HttpResponseMessage GetReportChiTiet([FromQuery] FilteredPagination paging)
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
                                                if (maCuaVL.Trim() != "")
                                                {
                                                    // dạng p.[Sand 1], p.[Sand 2], p.[Stone 1], p.[Stone 2], p.[Cement 1], p.[Cement 2], p.[Cement 3], p.[Cement 4], p.[Water], p.[Adm 1], p.[Adm 2]
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

                                                    //dạng , D.[CP_Cát 1], B.[Cát 1], C.[T_Cát 1], (B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) N'Sai số_Cát 1', abs(B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) / nullif(D.[CP_Cát 1], 0) * 100 '%_Cát 1'
                                                    selectByCuaVL += string.Format(", ROUND(ISNULL(D.[CP_{0}],0),2) N'CP_{1}', ROUND(ISNULL(B.[{0}],0),2) N'{1}', ROUND(ISNULL(C.[T_{0}],0),2) N'T_{1}', ROUND((ISNULL(B.[{0}],0) + ISNULL(C.[T_{0}],0) - ISNULL(D.[CP_{0}],0)),2) N'Sai số_{1}', ROUND(ISNULL(abs(ISNULL(B.[{0}],0) + ISNULL(C.[T_{0}],0) - ISNULL(D.[CP_{0}],0)) / nullif(D.[CP_{0}], 0) * 100,0),2) '%_{1}' \n", maCuaVL, tenCuaVL);
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

                                        String sql = string.Format("SELECT ROW_NUMBER() OVER(ORDER BY A.MACHITIETMETRON_MAIN ASC) AS STT, A.* \n" +
                                        " {11} \n" +
                                        "FROM ( \n" +
                                        "	SELECT DISTINCT B.MACHITIETMETRON MACHITIETMETRON_MAIN, B.MALSTRON N'Mã phiếu', B.SOTTMETRON N'STT mẻ trộn'  \n" +
                                        "		, FORMAT(A.NGAYTRON, 'dd/MM/yyy') N'Ngày trộn', FORMAT(A.GIOBATDAU, 'HH:mm') N'Giờ bắt đầu', FORMAT(A.GIOXONG, 'HH:mm') N'Giờ kết thúc' \n" +
                                        "		, H.TENKHACHHANG N'Khách hàng', A.BIENSO N'Biển số', H.TENDUAN N'Dự án', H.TENNV N'Nhân viên KD', A.TENMACBETONG N'Tên mác BT', A.CHEDO N'Chế độ', ROUND(B.M3METRON,2) N'Thể tích' \n" +
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
                                        ") AS D ON A.MACHITIETMETRON_MAIN = D.MACHITIETMETRON \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo,
                                        subQuerySumSOLUONG, subQuerySumSOLUONGT, subQuerySumSOLUONGCP,
                                        selectByCuaVL);



                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        //DataTable dtSource = dt.Copy();
                                        dtSource.Columns.Remove("MACHITIETMETRON_MAIN");



                                        //Tính tổng dòng cuối, chỉ web mới dùng
                                        string list = "";
                                        DataRow rowTong = dtSource.NewRow();
                                        rowTong["Ngày trộn"] = "TỔNG";

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
                                                        { }
                                                        break;
                                                    case "DOUBLE":
                                                    case "FLOAT":
                                                    case "DECIMAL":
                                                        try
                                                        {
                                                            rowTong[colName] = Math.Round((double)dtSource.Compute(string.Format("SUM([{0}])", colName), ""), 2);
                                                        }
                                                        catch (Exception ex)
                                                        { }
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
                                                double sumSaiSo = Math.Round((double)rowTong["Sai số_" + tenCUAVL], 2);
                                                double sumCP = Math.Round((double)rowTong["CP_" + tenCUAVL], 2);

                                                if (sumCP != 0) rowTong[colName] = Math.Round((double)(Math.Abs(sumSaiSo) / sumCP) * 100, 2);
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

                                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê chi tiết hợp đồng");
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Merge = true;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Value = "THỐNG KÊ CHI TIẾT HỢP ĐỒNG";
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
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Value = "Tổng";
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Merge = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Style.Font.Italic = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Style.Font.Bold = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Center;
                                                worksheet.Cells["A" + regionTotal + ":" + "L" + regionTotal].Style.VerticalAlignment = ExcelVerticalAlignment.Center;



                                                string modelRange = "A12:" + lastAlphabet + (dtSource.Rows.Count + 11);
                                                var modelTable = worksheet.Cells[modelRange];
                                                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                //modelCells.LoadFromCollection(Collection: model, PrintHeaders: true);
                                                worksheet.Cells["A:AZ"].AutoFitColumns();

                                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                                {
                                                    Content = new ByteArrayContent(package.GetAsByteArray())
                                                };
                                                return response;
                                            }
                                        }

                                        #region Comment

                                        //dtSource.TableName = "ThongKeDonHangChiTiet";

                                        //if (dtSource.Rows.Count > 0)
                                        //{
                                        //    XSSFWorkbook wb = new XSSFWorkbook();
                                        //    // Tạo ra 1 sheet
                                        //    ISheet sheet = wb.CreateSheet();

                                        //    string fileName = "Bao-cao-ke-toan-2";
                                        //    string template = @"template\export\BCKT1.xlsx";
                                        //    string webRootPath = _hostingEnvironment.WebRootPath;
                                        //    string templatePath = Path.Combine(webRootPath, template);
                                        //    string today = paging.denngay.Day.ToString() + "/" + paging.denngay.Month.ToString() + "/" + paging.denngay.Year.ToString();
                                        //    string fromday = paging.tungay.Day.ToString() + "/" + paging.tungay.Month.ToString() + "/" + paging.tungay.Year.ToString();



                                        //    using (XLWorkbook wbx = new XLWorkbook())
                                        //    {
                                        //        wbx.Worksheets.Add(dtSource);

                                        //        using (MemoryStream stream = new MemoryStream())
                                        //        {
                                        //            wbx.SaveAs(stream);
                                        //            var a = File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                                        //            var response = new HttpResponseMessage(HttpStatusCode.OK)
                                        //            {
                                        //                Content = new ByteArrayContent(stream.ToArray())
                                        //            };
                                        //            response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                                        //            response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                                        //            response.Content.Headers.Add("x-filename", fileName);
                                        //            response.Content.Headers.ContentType = new MediaTypeHeaderValue
                                        //                   ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                        //            response.Content.Headers.ContentDisposition =
                                        //                   new ContentDispositionHeaderValue("attachment")
                                        //                   {
                                        //                       FileName = fileName
                                        //                   };

                                        //            return response;
                                        //        }
                                        //    }
                                        //}

                                        #endregion
                                    }

                                }
                            }
                        }
                        //18 cua vl


                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            def.metadata = result[0];
                            DataTable dt = new DataTable();
                            dt.TableName = "ThongKeDonHangChiTiet";
                            dt.Load(result);

                            XSSFWorkbook wb = new XSSFWorkbook();
                            // Tạo ra 1 sheet
                            ISheet sheet = wb.CreateSheet();

                            string fileName = "Bao-cao-ke-toan-2";
                            string template = @"template\export\BCKT1.xlsx";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string templatePath = Path.Combine(webRootPath, template);
                            string today = paging.denngay.Day.ToString() + "/" + paging.denngay.Month.ToString() + "/" + paging.denngay.Year.ToString();
                            string fromday = paging.tungay.Day.ToString() + "/" + paging.tungay.Month.ToString() + "/" + paging.tungay.Year.ToString();



                            using (XLWorkbook wbx = new XLWorkbook())
                            {
                                wbx.Worksheets.Add(dt);

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wbx.SaveAs(stream);
                                    var a = File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                                    {
                                        Content = new ByteArrayContent(stream.ToArray())
                                    };
                                    response.Content.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type,x-filename");
                                    response.Content.Headers.Add("Access-Control-Expose-Headers", "Authorization,Content-Type,x-filename");
                                    response.Content.Headers.Add("x-filename", fileName);
                                    response.Content.Headers.ContentType = new MediaTypeHeaderValue
                                           ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                                    response.Content.Headers.ContentDisposition =
                                           new ContentDispositionHeaderValue("attachment")
                                           {
                                               FileName = fileName
                                           };

                                    return response;
                                }
                            }


                            return null;

                        }



                    }
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

        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
        private void CreateCellInt(IRow CurrentRow, int CellIndex, int Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
        private void CreateCellfloat(IRow CurrentRow, int CellIndex, double Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            Cell.CellStyle = Style;
        }
        private void CreateCellFormula(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellFormula(Value);
            Cell.CellStyle = Style;
        }
        public MemoryStream writeAccountantTwoToExcel(string templatePath, int sheetnumber, DataTable data, string today, string fromday, int companyid, string Branchlist)
        {
            FileStream file1 = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 3;
            if (sheet != null)
            {
                var table = new List<DATAEXPORT>();
                int datasize = data.Select().Count();
                var tenCuaVatLieu = listVatLieu(companyid, Branchlist);
                var style = sheet.GetRow(1).GetCell(0).CellStyle;
                sheet.GetRow(1).CreateCell(0).CellStyle = style;
                sheet.GetRow(1).GetCell(0).SetCellValue("Từ ngày " + fromday + " đến ngày " + today);
                var styleheader = sheet.GetRow(2).GetCell(0).CellStyle;
                var style1phan = sheet.GetRow(4).GetCell(0).CellStyle;
                var style2phan = sheet.GetRow(3).GetCell(0).CellStyle;
                var styletext = sheet.GetRow(5).GetCell(0).CellStyle;
                XSSFRow row = (XSSFRow)sheet.CreateRow(2);
                row.CreateCell(0).CellStyle = styleheader;
                row.GetCell(0).SetCellValue("STT");
                row.CreateCell(1).CellStyle = styleheader;
                row.GetCell(1).SetCellValue("Ngày");
                var NGAYTRON = new DATAEXPORT
                {
                    STT = 1,
                    Name = "NGAYTRON",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(1)
                };
                table.Add(NGAYTRON);
                row.CreateCell(2).CellStyle = styleheader;
                row.GetCell(2).SetCellValue("Bắt đầu");
                var GIOBATDAU = new DATAEXPORT
                {
                    STT = 2,
                    Name = "GIOBATDAU",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(2)
                };
                table.Add(GIOBATDAU);

                row.CreateCell(3).CellStyle = styleheader;
                row.GetCell(3).SetCellValue("Kết thúc");
                var GIOXONG = new DATAEXPORT
                {
                    STT = 3,
                    Name = "GIOXONG",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(3)
                };
                table.Add(GIOXONG);

                row.CreateCell(4).CellStyle = styleheader;
                row.GetCell(4).SetCellValue("Tên khách hàng");
                var TENKHACHHANG = new DATAEXPORT
                {
                    STT = 4,
                    Name = "TENKHACHHANG",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(4)
                };
                table.Add(TENKHACHHANG);

                row.CreateCell(5).CellStyle = styleheader;
                row.GetCell(5).SetCellValue("Tên dự án");
                var TENDUAN = new DATAEXPORT
                {
                    STT = 5,
                    Name = "TENDUAN",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(5)
                };
                table.Add(TENDUAN);

                row.CreateCell(6).CellStyle = styleheader;
                row.GetCell(6).SetCellValue("Tên hạng mục");
                var TENHANGMUC = new DATAEXPORT
                {
                    STT = 6,
                    Name = "TENHANGMUC",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(6)
                };
                table.Add(TENHANGMUC);

                row.CreateCell(7).CellStyle = styleheader;
                row.GetCell(7).SetCellValue("Tên địa điểm");
                var DIADIEMXD = new DATAEXPORT
                {
                    STT = 7,
                    Name = "DIADIEMXD",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(7)
                };
                table.Add(DIADIEMXD);

                row.CreateCell(8).CellStyle = styleheader;
                row.GetCell(8).SetCellValue("Biển xe");
                var BIENSO = new DATAEXPORT
                {
                    STT = 8,
                    Name = "BIENSO",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(8)
                };
                table.Add(BIENSO);

                row.CreateCell(9).CellStyle = styleheader;
                row.GetCell(9).SetCellValue("Mác bê tông");
                var TENMACBETONG = new DATAEXPORT
                {
                    STT = 9,
                    Name = "TENMACBETONG",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(8)
                };
                table.Add(TENMACBETONG);

                row.CreateCell(10).CellStyle = styleheader;
                row.GetCell(10).SetCellValue("NV kinh doanh");
                var TENNV = new DATAEXPORT
                {
                    STT = 10,
                    Name = "TENNV",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(10)
                };
                table.Add(TENNV);


                row.CreateCell(11).CellStyle = styleheader;
                row.GetCell(11).SetCellValue("Thể tích");
                var M3METRON = new DATAEXPORT
                {
                    STT = 11,
                    Name = "M3METRON",
                    issum = true,
                    namecell = CellReference.ConvertNumToColString(11),
                    is2phan = true
                };
                table.Add(M3METRON);

                int h = 12;
                foreach (var item in tenCuaVatLieu)
                {
                    row.CreateCell(h).CellStyle = styleheader;
                    row.GetCell(h).SetCellValue(item.TENCUAVL);
                    var detail = new DATAEXPORT();
                    if (item.TENLOAIVL == "PHUGIA")
                    {
                        detail.STT = h;
                        detail.Name = item.TENCUAVL;
                        detail.issum = true;
                        detail.namecell = CellReference.ConvertNumToColString(h);
                        detail.is2phan = true;
                    }
                    else
                    {
                        detail.STT = h;
                        detail.Name = item.TENCUAVL;
                        detail.issum = true;
                        detail.namecell = CellReference.ConvertNumToColString(h);
                    }
                    table.Add(detail);
                    if (!item.COPHAIPHUGIA)
                    {
                        row.CreateCell(h + 1).CellStyle = styleheader;
                        row.GetCell(h + 1).SetCellValue("T." + item.TENCUAVL);
                        h++;
                        var detail1 = new DATAEXPORT
                        {
                            STT = h,
                            Name = "T." + item.TENCUAVL,
                            issum = true,
                            namecell = CellReference.ConvertNumToColString(h)
                        };
                        table.Add(detail1);
                    }
                    h++;
                }

                row.CreateCell(h).CellStyle = styleheader;
                row.GetCell(h).SetCellValue("Tên phụ gia");
                var TENPHUGIA = new DATAEXPORT
                {
                    STT = h,
                    Name = "TENPHUGIA",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(h)
                };
                table.Add(TENPHUGIA);

                row.CreateCell(h + 1).CellStyle = styleheader;
                row.GetCell(h + 1).SetCellValue("Trạm trộn");
                var name = new DATAEXPORT
                {
                    STT = h + 1,
                    Name = "name",
                    issum = false,
                    namecell = CellReference.ConvertNumToColString(h + 1)
                };
                table.Add(name);

                int datacol = h + 2;
                try
                {
                    for (int rr = 0; rr < datasize + 1; rr++)
                    {
                        int rowNum = rr + rowStart;

                        try
                        {
                            XSSFRow rowbody = (XSSFRow)sheet.CreateRow(rowNum);
                            for (int i = 0; i < datacol; i++)
                            {
                                if (i == 0)
                                {
                                    if (rr == datasize)
                                    {
                                        rowbody.CreateCell(i).CellStyle = styletext;
                                        rowbody.GetCell(i).SetCellValue("Tổng");
                                    }
                                    else
                                    {
                                        rowbody.CreateCell(i).CellStyle = styletext;
                                        rowbody.GetCell(i).SetCellValue(rr + 1);
                                    }
                                }
                                foreach (var item in table)
                                {
                                    if (i == item.STT)
                                    {
                                        if (rr == datasize)
                                        {
                                            if (item.issum)
                                            {
                                                if (item.is2phan)
                                                {
                                                    rowbody.CreateCell(i).CellStyle = style2phan;
                                                }
                                                else
                                                {
                                                    rowbody.CreateCell(i).CellStyle = style1phan;
                                                }
                                                rowbody.GetCell(i).SetCellFormula("SUM(" + item.namecell + "4:" + item.namecell + "" + (datasize + rowStart).ToString() + ")");
                                            }
                                            else
                                            {

                                                if (item.STT == 6)
                                                {
                                                    rowbody.CreateCell(i).CellStyle = styletext;
                                                    rowbody.GetCell(i).SetCellValue("TỔNG TPCT:");
                                                }
                                                else
                                                {
                                                    rowbody.CreateCell(i).CellStyle = styletext;
                                                    rowbody.GetCell(i).SetCellValue("");
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (item.issum)
                                            {
                                                if (item.is2phan)
                                                {
                                                    rowbody.CreateCell(i).CellStyle = style2phan;
                                                }
                                                else
                                                {
                                                    rowbody.CreateCell(i).CellStyle = style1phan;
                                                }
                                                rowbody.GetCell(i).SetCellValue(double.Parse(data.Rows[rr][item.Name].ToString()));

                                            }
                                            else
                                            {
                                                rowbody.CreateCell(i).CellStyle = styletext;
                                                rowbody.GetCell(i).SetCellValue(data.Rows[rr][item.Name].ToString());
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                    for (int i = 0; i < datacol; i++)
                    {
                        sheet.AutoSizeColumn(i);
                        GC.Collect();
                    }


                }
                catch (Exception ex)
                {

                }
            }

            sheet.ForceFormulaRecalculation = true;

            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);

            return ms;
        }
        public static List<VatLieuDTO> listVatLieu(int companyid, string Branchlist)
        {
            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<VatLieuDTO> rpdonhang = new List<VatLieuDTO>();
                command.CommandText = " SELECT DISTINCT TENCUAVL,COPHAIPHUGIA,TENLOAIVL FROM ";
                command.CommandText += "(";
                if (Branchlist != "" && Branchlist != null)
                {
                    var arrListStr = Branchlist.Split(',');
                    int i = 0;
                    foreach (var item in arrListStr)
                    {
                        if (item != "")
                        {
                            Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (branch != null)
                            {
                                if (i == 0)
                                {
                                    command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                            }
                        }
                        ++i;
                    }
                }
                else
                {
                    if (companyid == 0)
                    {
                        List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                        if (branchlist.Count() == 0)
                        {
                            return null;
                        }
                        int j = 0;
                        foreach (var item in branchlist)
                        {
                            Branch branch = context.Branch.Find(item.BranchId);
                            if (j == 0)
                            {
                                command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                            }
                            else
                            {
                                command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                            }
                            ++j;
                        }
                    }
                    else
                    {
                        List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == companyid).ToList();
                        if (branchlist.Count() == 0)
                        {
                            return null;
                        }
                        int k = 0;
                        foreach (var item in branchlist)
                        {
                            Branch branch = context.Branch.Find(item.BranchId);
                            if (k == 0)
                            {
                                command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                            }
                            else
                            {
                                command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                            }
                            ++k;
                        }
                    }
                }
                command.CommandText += ") rpdonhang ORDER BY TENCUAVL";

                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    var k = 1;
                    while (result.Read())
                    {
                        VatLieuDTO item = new VatLieuDTO();
                        item.STT = (long)k;
                        if (result["TENCUAVL"] is System.DBNull)
                        {
                            item.TENCUAVL = "";

                        }
                        else
                        {
                            item.TENCUAVL = (string)result["TENCUAVL"];
                        }
                        item.COPHAIPHUGIA = (Boolean)result["COPHAIPHUGIA"];
                        item.TENLOAIVL = (string)result["TENLOAIVL"];
                        rpdonhang.Add(item);
                        k++;
                    }

                    return rpdonhang;
                }
            }
        }
        private List<DULIEUTHONGKE> GroupBySum(List<DULIEUTHONGKE> DLThongKe, int type, int socuacat, int socuada, int socuaximang, int socuanuoc, int socuaphugia, DateTime tungay, DateTime denngay)
        {
            var finalresult = new List<DULIEUTHONGKE>();
            var arrayListDLThongKe = DLThongKe.ToArray();
            var listSubList = new List<List<DULIEUTHONGKE>>();
            var fistSubList = new List<DULIEUTHONGKE>();

            fistSubList.Add(DLThongKe.FirstOrDefault());
            listSubList.Add(fistSubList);

            //Lấy Dữ liệu tổng theo Tên khách hàng, tên xe và tên mác bê tông
            for (int i = 1; i < arrayListDLThongKe.Length; i++)
            {
                int first = 0;
                int check = 0;
                foreach (var subItem in listSubList.ToArray())
                {
                    if (type == 0)
                    {
                        if (arrayListDLThongKe[i].TENKHACHHANG == subItem.FirstOrDefault().TENKHACHHANG &&
                            arrayListDLThongKe[i].TENMACBETONG == subItem.FirstOrDefault().TENMACBETONG &&
                            arrayListDLThongKe[i].BIENSO == subItem.FirstOrDefault().BIENSO &&
                            arrayListDLThongKe[i].TENNV == subItem.FirstOrDefault().TENNV &&
                            arrayListDLThongKe[i].TENHANGMUC == subItem.FirstOrDefault().TENHANGMUC)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                    else if (type == 1)
                    {
                        if (arrayListDLThongKe[i].TENMACBETONG == subItem.FirstOrDefault().TENMACBETONG &&
                            arrayListDLThongKe[i].BIENSO == subItem.FirstOrDefault().BIENSO &&
                            arrayListDLThongKe[i].TENNV == subItem.FirstOrDefault().TENNV &&
                            arrayListDLThongKe[i].TENHANGMUC == subItem.FirstOrDefault().TENHANGMUC)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                    else if (type == 2)
                    {
                        if (arrayListDLThongKe[i].TENKHACHHANG == subItem.FirstOrDefault().TENKHACHHANG &&
                            arrayListDLThongKe[i].TENMACBETONG == subItem.FirstOrDefault().TENMACBETONG &&
                            arrayListDLThongKe[i].TENNV == subItem.FirstOrDefault().TENNV &&
                            arrayListDLThongKe[i].TENHANGMUC == subItem.FirstOrDefault().TENHANGMUC)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                    else if (type == 3)
                    {
                        if (arrayListDLThongKe[i].TENKHACHHANG == subItem.FirstOrDefault().TENKHACHHANG &&
                            arrayListDLThongKe[i].BIENSO == subItem.FirstOrDefault().BIENSO &&
                            arrayListDLThongKe[i].TENNV == subItem.FirstOrDefault().TENNV &&
                            arrayListDLThongKe[i].TENHANGMUC == subItem.FirstOrDefault().TENHANGMUC)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                    else if (type == 4)
                    {
                        if (arrayListDLThongKe[i].TENKHACHHANG == subItem.FirstOrDefault().TENKHACHHANG &&
                            arrayListDLThongKe[i].TENMACBETONG == subItem.FirstOrDefault().TENMACBETONG &&
                            arrayListDLThongKe[i].TENNV == subItem.FirstOrDefault().TENNV &&
                            arrayListDLThongKe[i].BIENSO == subItem.FirstOrDefault().BIENSO)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                    else if (type == 5)
                    {
                        if (arrayListDLThongKe[i].TENKHACHHANG == subItem.FirstOrDefault().TENKHACHHANG &&
                             arrayListDLThongKe[i].TENMACBETONG == subItem.FirstOrDefault().TENMACBETONG &&
                             arrayListDLThongKe[i].BIENSO == subItem.FirstOrDefault().BIENSO &&
                             arrayListDLThongKe[i].TENHANGMUC == subItem.FirstOrDefault().TENHANGMUC)
                        {
                            subItem.Add(arrayListDLThongKe[i]);
                            check++;
                        }
                    }
                }
                if (check == first)
                {
                    listSubList.Add(new List<DULIEUTHONGKE>() { arrayListDLThongKe[i] });
                }
            }

            finalresult = new List<DULIEUTHONGKE>();

            foreach (var listSubItem in listSubList)
            {
                DULIEUTHONGKE row = new DULIEUTHONGKE();
                List<string> gioBatDau = new List<string>();
                List<string> gioXong = new List<string>();

                row.STT = listSubItem.FirstOrDefault().STT;
                row.NGAYTRON = tungay.Day + "/" + tungay.Month + " - " + denngay.Day + "/" + denngay.Month;
                row.GIOBATDAU = listSubItem.First().NGAYTRON + " " + listSubItem.First().GIOBATDAU;
                row.GIOXONG = listSubItem.Last().NGAYTRON + " " + listSubItem.Last().GIOXONG;
                row.TENKHACHHANG = listSubItem.FirstOrDefault().TENKHACHHANG;
                row.BIENSO = listSubItem.FirstOrDefault().BIENSO;
                row.TENMACBETONG = listSubItem.FirstOrDefault().TENMACBETONG;
                row.TENHANGMUC = listSubItem.FirstOrDefault().TENHANGMUC;
                row.TENDIADIEMXD = listSubItem.FirstOrDefault().TENDIADIEMXD;
                row.TENDUAN = listSubItem.FirstOrDefault().TENDUAN;
                row.TENNV = listSubItem.FirstOrDefault().TENNV;
                //row.TAIKHOAN = "Tất cả";

                if (type == 1)
                {
                    row.TENKHACHHANG = "Tất cả";
                }
                else if (type == 2)
                {
                    row.BIENSO = "Tất cả";
                }
                else if (type == 3)
                {
                    row.TENMACBETONG = "Tất cả";
                }
                else if (type == 4)
                {
                    row.TENHANGMUC = "Tất cả";
                }
                else if (type == 5)
                {
                    row.TENNV = "Tất cả";
                }

                row.M3METRON = 0;

                row.listcats = new List<float>();
                row.listdas = new List<float>();
                row.listximangs = new List<float>();
                row.listnuocs = new List<float>();
                row.listphugias = new List<float>();
                row.tenphugias = new List<string>();

                for (int i = 0; i < socuacat * 2; i++)
                {
                    row.listcats.Add(0);
                }

                for (int i = 0; i < socuada * 2; i++)
                {
                    row.listdas.Add(0);
                }

                for (int i = 0; i < socuaximang * 2; i++)
                {
                    row.listximangs.Add(0);
                }

                for (int i = 0; i < socuanuoc * 2; i++)
                {
                    row.listnuocs.Add(0);
                }

                for (int i = 0; i < socuaphugia; i++)
                {
                    row.listphugias.Add(0);
                }

                foreach (var subItem in listSubItem)
                {
                    //Lấy danh sách giờ bắt đầu và giờ xong
                    if (type != 0)
                    {
                        gioBatDau.Add(subItem.GIOBATDAU);
                        gioXong.Add(subItem.GIOXONG);
                    }

                    row.M3METRON += subItem.M3METRON;

                    for (int i = 0; i < socuacat * 2; i++)
                    {
                        row.listcats[i] += subItem.listcats[i];
                    }

                    for (int i = 0; i < socuada * 2; i++)
                    {
                        row.listdas[i] += subItem.listdas[i];
                    }

                    for (int i = 0; i < socuaximang * 2; i++)
                    {
                        row.listximangs[i] += subItem.listximangs[i];
                    }

                    for (int i = 0; i < socuanuoc * 2; i++)
                    {
                        row.listnuocs[i] += subItem.listnuocs[i];
                    }

                    for (int i = 0; i < socuaphugia; i++)
                    {
                        row.listphugias[i] += subItem.listphugias[i];
                        row.tenphugias.Add(subItem.tenphugias[i]);
                    }
                }

                //Lấy giá trị giờ bắt đầu là giờ đầu tiên, và giờ xong là giờ cuối cùng của bản ghi
                if (type != 0)
                {
                    DateTime minDate = DateTime.MaxValue;
                    DateTime maxDate = DateTime.MinValue;
                    foreach (string dateString in gioBatDau)
                    {
                        //DateTime date = DateTime.ParseExact(dateString, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture);
                        DateTime date = ConvertToDateTime(dateString);
                        if (date < minDate)
                            minDate = date;
                    }
                    foreach (string dateString in gioXong)
                    {
                        //DateTime date = DateTime.ParseExact(dateString, "d/M/yyyy h:mm tt", CultureInfo.InvariantCulture);
                        DateTime date = ConvertToDateTime(dateString);
                        if (date > maxDate)
                            maxDate = date;
                    }

                    row.GIOBATDAU = minDate.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    row.GIOXONG = maxDate.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                }

                finalresult.Add(row);
            }

            return finalresult;
        }
        public static DateTime ConvertToDateTime(object obj)
        {
            try
            {
                DateTime result = Convert.ToDateTime(obj);
                return result;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        [HttpGet("GetNV/{Branchlist}")]
        public IActionResult GetNV(string Branchlist)
        {

            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<NhanVienDTO> nv = new List<NhanVienDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT TENNV FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            NhanVienDTO item = new NhanVienDTO();
                            item.TENNV = (result["TENNV"] is DBNull) ? String.Empty : (string)result["TENNV"];
                            nv.Add(item);
                        }

                        def.data = nv;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("GetBienSo/{Branchlist}")]
        public IActionResult GetBienSo(string Branchlist)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<XeDTO> nv = new List<XeDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT BIENSO  FROM [" + branch.Dataname + "].[dbo].[XE]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            XeDTO item = new XeDTO();
                            item.BIENSO = (result["BIENSO"] is DBNull) ? String.Empty : (string)result["BIENSO"];
                            nv.Add(item);
                        }

                        def.data = nv;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("GetKH/{Branchlist}")]
        public IActionResult GetKH(string Branchlist)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<KhachHangDTO> nv = new List<KhachHangDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT TENKHACHHANG FROM [" + branch.Dataname + "].[dbo].[KHACHHANG]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            KhachHangDTO item = new KhachHangDTO();
                            item.TENKHACHHANG = (result["TENKHACHHANG"] is DBNull) ? String.Empty : (string)result["TENKHACHHANG"];
                            nv.Add(item);
                        }

                        def.data = nv;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

        }

        [HttpGet("GetHangMuc/{Branchlist}")]
        public IActionResult GetHangMuc(string Branchlist)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<HangMucDTO> nv = new List<HangMucDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT TENHANGMUC FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            HangMucDTO item = new HangMucDTO();
                            item.TENHANGMUC = (result["TENHANGMUC"] is DBNull) ? String.Empty : (string)result["TENHANGMUC"];
                            nv.Add(item);
                        }

                        def.data = nv;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

        }

        [HttpGet("GetTenMacBeTong/{Branchlist}")]
        public IActionResult GetTenMacBeTong(string Branchlist)
        {

            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<DatHangDTO> nv = new List<DatHangDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT TENMACBETONG FROM [" + branch.Dataname + "].[dbo].[MACBETONG]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            DatHangDTO item = new DatHangDTO();
                            item.TENMACBETONG = (result["TENMACBETONG"] is DBNull) ? String.Empty : (string)result["TENMACBETONG"];
                            nv.Add(item);
                        }

                        def.data = nv;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("GetVatLieu")]
        public IActionResult GetVatLieu([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<VatLieuDTO> rpdonhang = new List<VatLieuDTO>();
                    command.CommandText = " SELECT DISTINCT TENCUAVL,COPHAIPHUGIA,TENLOAIVL FROM ";
                    command.CommandText += "(";
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {
                                    if (i == 0)
                                    {
                                        command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                    }
                                }
                            }
                            ++i;
                        }
                    }
                    else
                    {
                        if (paging.companyid == 0)
                        {
                            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                            if (branchlist.Count() == 0)
                            {
                                def.data = null;
                                def.metadata = 0;
                                def.meta = new Meta(200, "Success");
                                return Ok(def);
                            }
                            int j = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (j == 0)
                                {
                                    command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                ++j;
                            }
                        }
                        else
                        {
                            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == paging.companyid).ToList();
                            if (branchlist.Count() == 0)
                            {
                                def.data = null;
                                def.metadata = 0;
                                def.meta = new Meta(200, "Success");
                                return Ok(def);
                            }
                            int k = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (k == 0)
                                {
                                    command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") rpdonhang";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            VatLieuDTO item = new VatLieuDTO();
                            if (result["TENCUAVL"] is System.DBNull)
                            {
                                item.TENCUAVL = "";

                            }
                            else
                            {
                                item.TENCUAVL = (string)result["TENCUAVL"];
                            }
                            item.COPHAIPHUGIA = (Boolean)result["COPHAIPHUGIA"];
                            item.TENLOAIVL = (string)result["TENLOAIVL"];
                            rpdonhang.Add(item);
                        }

                        def.data = rpdonhang;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }

        }
        // GET: api/Slide/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlide(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Slide data = await db.Slide.FindAsync(id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // PUT: api/Slide/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlide(int id, [FromBody] Slide data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if ((userId != data.UserId) || (companyId != data.CompanyId))
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = data.Status;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.SlideId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!SlideExists(data.SlideId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Slide
        [HttpPost]
        public async Task<IActionResult> PostSlide([FromBody] Slide data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //Nếu ko truyền vào ngôn ngữ thì chọn ngôn ngữ mạc định
                        if (data.LanguageId == null)
                        {
                            //Nếu ngôn ngữ mạc định = 0 thì cảnh báo tạo ngôn ngữ 
                            if (languageId == 0)
                            {
                                def.meta = new Meta(210, "Language default is null");
                                return Ok(def);
                            }
                            else
                                data.LanguageId = languageId;
                        }

                        //Nếu ko truyền vào website thì chọn website mạc định
                        if (data.WebsiteId == null)
                        {
                            //Nếu website mạc định = 0 thì cảnh báo tạo website
                            if (websiteId == 0)
                            {
                                def.meta = new Meta(210, "Website default is null");
                                return Ok(def);
                            }
                            else
                                data.WebsiteId = websiteId;
                        }

                        data.CompanyId = companyId;
                        data.UserId = userId;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;

                        db.Slide.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.SlideId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (SlideExists(data.SlideId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Slide/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlide(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Slide data = await db.Slide.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    if ((userId != data.UserId) || (companyId != data.CompanyId))
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.SlideId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!SlideExists(data.SlideId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool SlideExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Slide.Count(e => e.SlideId == id) > 0;
            }
        }


    }
}