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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IOITWebApp.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeTongVatTuController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("thongketongvattu", "thongketongvattu");
        private static string functionCode = "TKTVT";
        private IHostingEnvironment _hostingEnvironment;
        public ThongKeTongVatTuController(IHostingEnvironment hostingEnvironment)
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
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {

                                    List<ThongKeTongVatTuDTO> lstResult = new List<ThongKeTongVatTuDTO>();
                                    List<ThongKeTongVatTuDTO> lstTong = new List<ThongKeTongVatTuDTO>();
                                    if (paging.CVL is null || paging.CVL == "undefined" || paging.CVL == "null")
                                    {
                                        paging.CVL = "";
                                    }
                                    if (paging.CHEDO is null || paging.CHEDO == "undefined" || paging.CHEDO == "null")
                                    {
                                        paging.CHEDO = "";
                                    }

                                    int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                    int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                    int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                    int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                    DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                    DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);

                                    if (paging != null)
                                    {
                                        string tenCUAVLCond, cheDo;
                                        tenCUAVLCond = cheDo = "1=1";


                                        if (!string.IsNullOrEmpty(paging.CVL)) tenCUAVLCond = string.Format("F.TENCUAVL = N'{0}'", paging.CVL);
                                        if (paging.CHEDO.Equals("Normal")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("Simulation")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");

                                        command.CommandText = "select * INTO #Result FROM (";

                                        String sql = string.Format("SELECT F.STTCUAVL STTCUAVL_MAIN, F.TENCUAVL N'Tên cửa vật liệu', SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) N'Khối lượng', 'Kg' N'Đơn vị', SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) - SUM(ISNULL(D.SOLUONGCP,0)) N'Sai số (kg)', ISNULL((ABS(SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) - SUM(ISNULL(D.SOLUONGCP,0))) / nullif(SUM(ISNULL(D.SOLUONGCP,0)), 0) * 100), 0) N'% sai số' \n" +
                                                                    "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL F ON F.STTCUAVL= D.STTCUAVL    \n" +
                                                                    "WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                    "    AND {2} AND {3} \n" +
                                                                    "GROUP BY F.TENCUAVL, F.STTCUAVL \n",
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                    tenCUAVLCond, cheDo);

                                        command.CommandText += sql.ToString();
                                        command.CommandText += ") as ChiTiet; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT * FROM #Result ORDER BY STTCUAVL_MAIN OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        context.Database.OpenConnection();
                                        using (var result = command.ExecuteReader())
                                        {
                                            result.Read();
                                            def.metadata = result[0];
                                            result.NextResult();

                                            DataTable dtresult = new DataTable();

                                            dtresult.Load(result);
                                            dtresult.Columns.Remove("STTCUAVL_MAIN");

                                            DataTable newTable = dtresult.Clone();

                                            string list = "";
                                            DataRow rowTong = newTable.NewRow();
                                            rowTong["Tên cửa vật liệu"] = "TỔNG";

                                            foreach (DataColumn col in newTable.Columns)
                                            {
                                                string type = col.DataType.Name.ToString().ToUpper();
                                                string colName = col.ColumnName;
                                                if (colName != "STT" && colName != "MACHITIETMETRON_MAIN" && colName != "Tên cửa vật liệu" && colName != "Đơn vị")
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
                                            newTable.Rows.Add(rowTong);

                                            foreach (DataColumn col in dtresult.Columns)
                                            {
                                                ThongKeTongVatTuDTO items = new ThongKeTongVatTuDTO();
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
                                                    lstResult.Add(items);
                                                }

                                            }
                                            foreach (DataColumn col in newTable.Columns)
                                            {
                                                ThongKeTongVatTuDTO items = new ThongKeTongVatTuDTO();
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
                                            def.data = lstResult;

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
                        }
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

        [HttpGet("GetReportChiTiet")]
        public HttpResponseMessage GetReportChiTiet([FromQuery] FilteredPagination paging)
        {
            try
            {
                DefaultResponse def = new DefaultResponse();
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
                {
                    return null;
                }
                if (paging != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        if (paging.Branchlist != "" && paging.Branchlist != null)
                        {
                            var arrListStr = paging.Branchlist.Split(',');
                            foreach (var item in arrListStr)
                            {
                                if (item != "")
                                {
                                    Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (branch != null)
                                    {

                                        List<ThongKeTongVatTuDTO> lstResult = new List<ThongKeTongVatTuDTO>();
                                        List<ThongKeTongVatTuDTO> lstTong = new List<ThongKeTongVatTuDTO>();
                                        if (paging.CVL is null || paging.CVL == "undefined" || paging.CVL == "null")
                                        {
                                            paging.CVL = "";
                                        }
                                        if (paging.CHEDO is null || paging.CHEDO == "undefined" || paging.CHEDO == "null")
                                        {
                                            paging.CHEDO = "";
                                        }

                                        int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                        int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                        int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                        int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                        DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                        DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);


                                        string tenCUAVLCond, cheDo;
                                        tenCUAVLCond = cheDo = "1=1";


                                        if (!string.IsNullOrEmpty(paging.CVL)) tenCUAVLCond = string.Format("F.TENCUAVL = N'{0}'", paging.CVL);
                                        if (paging.CHEDO.Equals("Normal")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("Simulation")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");

                                        String sql = string.Format("SELECT F.STTCUAVL STT, F.TENCUAVL N'Tên cửa vật liệu', SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) N'Khối lượng', 'Kg' N'Đơn vị', SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) - SUM(ISNULL(D.SOLUONGCP,0)) N'Sai số (kg)', ISNULL((ABS(SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) - SUM(ISNULL(D.SOLUONGCP,0))) / nullif(SUM(ISNULL(D.SOLUONGCP,0)), 0) * 100), 0) N'% sai số' \n" +
                                                                    "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT    \n" +
                                                                    "    INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL F ON F.STTCUAVL= D.STTCUAVL    \n" +
                                                                    "WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                    "    AND {2} AND {3} \n" +
                                                                    "GROUP BY F.TENCUAVL, F.STTCUAVL \n",
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                    tenCUAVLCond, cheDo);

                                        command.CommandText = sql.ToString();
                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        context.Database.OpenConnection();
                                        //using (var result = command.ExecuteReader())
                                        //{
                                        //result.Read();
                                        //def.metadata = result[0];
                                        //result.NextResult();

                                        //DataTable dtresult = new DataTable();

                                        //dtresult.Load(result);

                                        //DataTable newTable = dtresult.Clone();

                                        string list = "";
                                        DataRow rowTong = dtSource.NewRow();
                                        rowTong["Tên cửa vật liệu"] = "TỔNG";
                                        foreach (DataColumn col in dtSource.Columns)
                                        {
                                            string type = col.DataType.Name.ToString().ToUpper();
                                            string colName = col.ColumnName;
                                            if (colName != "STT" && colName != "MACHITIETMETRON_MAIN" && colName != "Tên cửa vật liệu" && colName != "Đơn vị")
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
                                        dtSource.Rows.Add(rowTong);

                                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                        using (var package = new ExcelPackage())
                                        {
                                            var alphabet = "A,B,C,D,E,F";
                                            var arrAlphabet = alphabet.Split(",");
                                            var lastAlphabet = string.Empty;
                                            lastAlphabet = arrAlphabet[dtSource.Columns.Count - 1];

                                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê chi tiết hợp đồng");
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Merge = true;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Value = "THỐNG KÊ TỔNG VẬT TƯ";
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

                                            // Cửa VL
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Merge = true;
                                            var valueFilterCVL = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.CVL))
                                            {
                                                valueFilterCVL = paging.CVL;
                                            }
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Value = "- Cửa vật liệu: " + valueFilterCVL;
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A4:" + lastAlphabet + "4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            // Chế độ
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Merge = true;
                                            var valueFilterCD = "Tất cả";
                                            if (!string.IsNullOrEmpty(paging.CHEDO))
                                            {
                                                valueFilterCD = paging.CHEDO;
                                            }
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Value = "- Chế độ: " + valueFilterCD;
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A5:" + lastAlphabet + "5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                            #endregion


                                            var index = 7;
                                            var cell = string.Empty;

                                            var count = 0;
                                            foreach (DataColumn col in dtSource.Columns)
                                            {
                                                cell = arrAlphabet[count] + "7";
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
                                                int rowFirts = 8;
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

                                                        column++;
                                                    }

                                                }
                                                //Row merge total
                                                var regionTotal = dtSource.Rows.Count + 7;
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Value = "Tổng";
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Merge = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Style.Font.Italic = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Style.Font.Bold = true;
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Center;
                                                worksheet.Cells["A" + regionTotal + ":" + "B" + regionTotal].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                                string modelRange = "A7:" + lastAlphabet + (dtSource.Rows.Count + 7);
                                                var modelTable = worksheet.Cells[modelRange];
                                                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                                {
                                                    Content = new ByteArrayContent(package.GetAsByteArray())
                                                };
                                                return response;
                                            }
                                            worksheet.Cells["A:F"].AutoFitColumns();
                                        }

                                        #region Comment
                                        //dtSource.TableName = "ThongKeTongVatTu";

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

                                        // }
                                        return null;
                                    }
                                }
                            }
                        }
                        return null;
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
    }
}
