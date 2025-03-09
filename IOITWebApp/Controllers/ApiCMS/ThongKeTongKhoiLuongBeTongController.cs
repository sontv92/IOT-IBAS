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
    public class ThongKeTongKhoiLuongBeTongController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("thongketongkhoiluongbetong", "thongketongkhoiluongbetong");
        private static string functionCode = "TKTKLBT";
        private IHostingEnvironment _hostingEnvironment;
        public ThongKeTongKhoiLuongBeTongController(IHostingEnvironment hostingEnvironment)
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

                                    List<ThongKeChiTietKhoiLuongBeTongDTO> lstResult = new List<ThongKeChiTietKhoiLuongBeTongDTO>();
                                    if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                                    {
                                        paging.TENKHACHHANG = "";
                                    }
                                    if (paging.TENDUAN is null || paging.TENDUAN == "undefined" || paging.TENDUAN == "null")
                                    {
                                        paging.TENDUAN = "";
                                    }
                                    if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                                    {
                                        paging.TENMACBETONG = "";
                                    }
                                    if (paging.CHEDO is null || paging.CHEDO == "undefined" || paging.CHEDO == "null")
                                    {
                                        paging.CHEDO = "";
                                    }
                                    if (paging.GroupBy is null || paging.GroupBy == "undefined" || paging.GroupBy == "null")
                                    {
                                        paging.GroupBy = "";
                                    }

                                    int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                    int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                    int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                    int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                    DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                    DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);

                                    if (paging != null)
                                    {
                                        string maPhieuCond, conTruongCond, taiXeCond, tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        maPhieuCond = conTruongCond = taiXeCond = tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";

                                        if (!string.IsNullOrEmpty(paging.TENKHACHHANG)) tenKHCond = string.Format("C.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG);
                                        if (!string.IsNullOrEmpty(paging.TENDUAN)) conTruongCond = string.Format("C.TENDUAN= N'{0}'", paging.TENDUAN);
                                        if (!string.IsNullOrEmpty(paging.TENMACBETONG)) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG);
                                        if (paging.CHEDO.Equals("Normal")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", paging.CHEDO);

                                        String sql = string.Format("SELECT A.NGAYTRON, C.TENKHACHHANG , C.TENDUAN , A.TENMACBETONG\n" +
                                            "	, SUM(M3METRON) SumM3METRON, COUNT(B.SOTTMETRON) SOTTMETRON \n" +
                                            "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                            "   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG   \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                            "        AND {7} AND {8} AND {9} AND {10} \n" +
                                            "GROUP BY A.NGAYTRON, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG \n" +
                                            "ORDER BY A.NGAYTRON \n",
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                            maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                            xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        // command.CommandText = "select * INTO #Result FROM (";

                                        command.CommandText = sql.ToString();
                                        //command.CommandText += ") as ChiTiet; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT * FROM #Result ORDER BY STTCUAVL_MAIN OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        var dataTable = CommonLib.AsEnumerable(dtSource);

                                        List<ThongKeTongKhoiLuongBeTongDTO> listData = dataTable.Select(row => new ThongKeTongKhoiLuongBeTongDTO()
                                        {
                                            NGAYTRON = (DateTime)row["NGAYTRON"],
                                            TENDUAN = row["TENDUAN"] != DBNull.Value ? (string)row["TENDUAN"] : "",
                                            SumM3METRON = row["SumM3METRON"] != DBNull.Value ? (double)row["SumM3METRON"] : 0,
                                            TENKHACHHANG = row["TENKHACHHANG"] != DBNull.Value ? (string)row["TENKHACHHANG"] : "",
                                            TENMACBETONG = row["TENMACBETONG"] != DBNull.Value ? (string)row["TENMACBETONG"] : "",
                                            SOTTMETRON = (int)row["SOTTMETRON"],
                                        }).ToList();

                                        def.metadata = listData.Count();
                                        List<ThongKeTongKhoiLuongBeTongGroupDTO> result = new List<ThongKeTongKhoiLuongBeTongGroupDTO>();
                                        switch (paging.GroupBy)
                                        {
                                            case "CT":
                                                var groupsCT = listData.GroupBy(x => x.TENDUAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x=>x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            case "KH":
                                                var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            case "MA":
                                                var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                foreach (var itemGrp in groupsMAC)
                                                {
                                                    result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            default: // Mặc định group theo ngày lập
                                                var groups = listData.GroupBy(x => x.NGAYTRON);
                                                foreach (var itemGrp in groups)
                                                {
                                                    result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                        }

                                        def.data = result;
                                        def.metadata = result.Count();

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
                                        if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                                        {
                                            paging.TENKHACHHANG = "";
                                        }
                                        if (paging.TENDUAN is null || paging.TENDUAN == "undefined" || paging.TENDUAN == "null")
                                        {
                                            paging.TENDUAN = "";
                                        }
                                        if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                                        {
                                            paging.TENMACBETONG = "";
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


                                        string maPhieuCond, conTruongCond, taiXeCond, tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        maPhieuCond = conTruongCond = taiXeCond = tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";

                                        if (!string.IsNullOrEmpty(paging.TENKHACHHANG)) tenKHCond = string.Format("C.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG);
                                        if (!string.IsNullOrEmpty(paging.TENDUAN)) conTruongCond = string.Format("C.TENDUAN= N'{0}'", paging.TENDUAN);
                                        if (!string.IsNullOrEmpty(paging.TENMACBETONG)) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG);
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", paging.CHEDO);
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", paging.CHEDO);

                                        String sql = string.Format("SELECT A.NGAYTRON, C.TENKHACHHANG , C.TENDUAN , A.TENMACBETONG\n" +
                                            "	, SUM(M3METRON) SumM3METRON, COUNT(B.SOTTMETRON) SOTTMETRON \n" +
                                            "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                            "   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG   \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                            "        AND {7} AND {8} AND {9} AND {10} \n" +
                                            "GROUP BY A.NGAYTRON, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG \n" +
                                            "ORDER BY A.NGAYTRON \n",
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                            maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                            xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        command.CommandText = sql.ToString();
                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        List<ThongKeTongKhoiLuongBeTongGroupDTO> result = new List<ThongKeTongKhoiLuongBeTongGroupDTO>();
                                        if (dtSource.Rows.Count > 0)
                                        {
                                            var dataTable = CommonLib.AsEnumerable(dtSource);

                                            List<ThongKeTongKhoiLuongBeTongDTO> listData = dataTable.Select(row => new ThongKeTongKhoiLuongBeTongDTO()
                                            {
                                                NGAYTRON = (DateTime)row["NGAYTRON"],
                                                TENDUAN = row["TENDUAN"] != DBNull.Value ? (string)row["TENDUAN"] : "",
                                                SumM3METRON = row["SumM3METRON"] != DBNull.Value ? (double)row["SumM3METRON"] : 0,
                                                TENKHACHHANG = row["TENKHACHHANG"] != DBNull.Value ? (string)row["TENKHACHHANG"] : "",
                                                TENMACBETONG = row["TENMACBETONG"] != DBNull.Value ? (string)row["TENMACBETONG"] : "",
                                                SOTTMETRON = (int)row["SOTTMETRON"],
                                            }).ToList();

                                            var groupName = string.Empty;
                                            switch (paging.GroupBy)
                                            {
                                                case "CT":
                                                    var groupsCT = listData.GroupBy(x => x.TENDUAN);
                                                    foreach (var itemGrp in groupsCT)
                                                    {
                                                        result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Công trường: ";
                                                    }
                                                    break;
                                                case "KH":
                                                    var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                    foreach (var itemGrp in groupsKH)
                                                    {
                                                        result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Khách hàng: ";
                                                    }
                                                    break;
                                                case "MA":
                                                    var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                    foreach (var itemGrp in groupsMAC)
                                                    {
                                                        result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Mác: ";
                                                    }
                                                    break;
                                                default: // Mặc định group theo ngày lập
                                                    var groups = listData.GroupBy(x => x.NGAYTRON);
                                                    foreach (var itemGrp in groups)
                                                    {
                                                        result.Add(new ThongKeTongKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            TotalSumM3METRON = itemGrp.Sum(x => x.SumM3METRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Ngày lập: ";
                                                    }
                                                    break;
                                            }

                                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                            using (var package = new ExcelPackage())
                                            {
                                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê tổng khối lượng bê tông");
                                                worksheet.Cells["A1:E1"].Merge = true;
                                                worksheet.Cells["A1:E1"].Value = "THỐNG KÊ TỔNG KHỐI LƯỢNG BÊ TÔNG";
                                                worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                                                worksheet.Cells["A1:E1"].Style.Font.Size = 16;
                                                worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A1:E1"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                worksheet.Cells["A2:E2"].Merge = true;
                                                worksheet.Cells["A2:E2"].Value = "Báo cáo được tạo vào ngày " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                                                worksheet.Cells["A2:E2"].Style.Font.Italic = true;
                                                worksheet.Cells["A2:E2"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A2:E2"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A2:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                //Điều kiện lọc
                                                worksheet.Cells["A3:E3"].Merge = true;
                                                worksheet.Cells["A3:E3"].Value = "* Điều kiện:";
                                                worksheet.Cells["A3:E3"].Style.Font.Italic = true;
                                                worksheet.Cells["A3:E3"].Style.Font.Bold = true;
                                                worksheet.Cells["A3:E3"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A3:E3"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A3:E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                #region Điều kiện
                                                // Khách hàng
                                                worksheet.Cells["A4:E4"].Merge = true;
                                                var valueFilterKhachhang = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENKHACHHANG))
                                                {
                                                    valueFilterKhachhang = paging.TENKHACHHANG;
                                                }
                                                worksheet.Cells["A4:E4"].Value = "- Khách hàng: " + valueFilterKhachhang;
                                                worksheet.Cells["A4:E4"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A4:E4"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A4:E4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Dự án
                                                worksheet.Cells["A5:E5"].Merge = true;
                                                var valueFilterDuan = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENDUAN))
                                                {
                                                    valueFilterDuan = paging.MAPHIEU;
                                                }
                                                worksheet.Cells["A5:E5"].Value = "- Công trường: " + valueFilterDuan;
                                                worksheet.Cells["A5:E5"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A5:E5"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Tên mác be tông
                                                worksheet.Cells["A6:E6"].Merge = true;
                                                var valueFilterMac = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENMACBETONG))
                                                {
                                                    valueFilterMac = paging.TENMACBETONG;
                                                }
                                                worksheet.Cells["A6:E6"].Value = "- Tên mác bê tông: " + valueFilterMac;
                                                worksheet.Cells["A6:E6"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A6:E6"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A6:E6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Chế độ
                                                worksheet.Cells["A7:E7"].Merge = true;
                                                var valueFilterChedo = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.CHEDO))
                                                {
                                                    valueFilterChedo = paging.CHEDO;
                                                }
                                                worksheet.Cells["A7:E7"].Value = "- Chế độ: " + valueFilterChedo;
                                                worksheet.Cells["A7:E7"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A7:E7"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A7:E7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                #endregion

                                                #region Nhóm theo
                                                //Nhóm theo
                                                worksheet.Cells["A8:E8"].Merge = true;
                                                worksheet.Cells["A8:E8"].Value = "* Nhóm theo: " + groupName.Substring(0, groupName.IndexOf(":"));
                                                worksheet.Cells["A8:E8"].Style.Font.Italic = true;
                                                worksheet.Cells["A8:E8"].Style.Font.Bold = true;
                                                worksheet.Cells["A8:E8"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A8:E8"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A8:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                #endregion

                                                worksheet.Cells["A10"].Value = "Khách hàng";
                                                worksheet.Cells["A10"].Style.Font.Bold = true;

                                                worksheet.Cells["B10"].Value = "Công trường";
                                                worksheet.Cells["B10"].Style.Font.Bold = true;

                                                worksheet.Cells["C10"].Value = "Mác";
                                                worksheet.Cells["C10"].Style.Font.Bold = true;

                                                worksheet.Cells["D10"].Value = "Tổng khối lượng";
                                                worksheet.Cells["D10"].Style.Font.Bold = true;

                                                worksheet.Cells["E10"].Value = "Số mẻ";
                                                worksheet.Cells["E10"].Style.Font.Bold = true;


                                                worksheet.Cells["A10:E10"].Style.Font.Color.SetColor(Color.White);
                                                worksheet.Cells["A10:E10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells["A10:E10"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                                if (result.Count() > 0)
                                                {
                                                    int rowFirts = 11;
                                                    for (int i = 0; i < result.Count(); i++)
                                                    {
                                                        var elementGroup = result.ElementAt(i);
                                                        //Row merge
                                                        var region = "A" + rowFirts + ":" + "E" + rowFirts;
                                                        worksheet.Cells[region].Merge = true;
                                                        if (elementGroup.Key.GetType() == typeof(DateTime))
                                                        {
                                                            worksheet.Cells[region].Value = groupName + Convert.ToDateTime(elementGroup.Key).ToString("dd/MM/yyyy");
                                                        }
                                                        else
                                                        {
                                                            worksheet.Cells[region].Value = groupName + elementGroup.Key;
                                                        }
                                                        worksheet.Cells[region].Style.Font.Italic = true;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Left;
                                                        worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        rowFirts = rowFirts + 1;
                                                        for (var j = 0; j < elementGroup.Data.Count(); j++)
                                                        {
                                                            int row = rowFirts + j;
                                                            var element = elementGroup.Data.ElementAt(j);
                                                            int column = 1;
                                                         
                                                            //Khách hàng
                                                            worksheet.Cells[row, column].Value = element?.TENKHACHHANG;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Công trường
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENDUAN;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Mác
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENMACBETONG;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;


                                                            //Tổng khối lượng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.SumM3METRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,#";

                                                            //Số mẻ
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.SOTTMETRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,#";


                                                        }
                                                        rowFirts = rowFirts + elementGroup.Data.Count();

                                                        //Row merge total
                                                        region = "A" + rowFirts + ":" + "C" + rowFirts;
                                                        worksheet.Cells[region].Merge = true;
                                                        worksheet.Cells[region].Value = "Tổng:";
                                                        worksheet.Cells[region].Style.Font.Italic = true;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Center;
                                                        worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        region = "D" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalSumM3METRON;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,#";

                                                        region = "E" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalSOTTMETRON;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,#";


                                                        rowFirts++;
                                                    }
                                                    string modelRange = "A11:E" + (listData.Count() + result.Count() * 2 + 11);
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
                                        }
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

        [HttpGet("GetTenTaiXe/{Branchlist}")]
        public IActionResult GetTenTaiXe(string Branchlist)
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
                    command.CommandText += "SELECT DISTINCT TENLAIXE FROM [" + branch.Dataname + "].[dbo].[LSTRON] WHERE ISNULL(TENLAIXE, '') <> ''";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            XeDTO item = new XeDTO();
                            item.TENLAIXE = (result["TENLAIXE"] is DBNull) ? String.Empty : (string)result["TENLAIXE"];
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

        [HttpGet("GetMaPhieu/{Branchlist}")]
        public IActionResult GetMaPhieu(string Branchlist)
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

                    List<MaLichSuTronDTO> nv = new List<MaLichSuTronDTO>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT MALSTRON FROM [" + branch.Dataname + "].[dbo].[LSTRON]";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            MaLichSuTronDTO item = new MaLichSuTronDTO();
                            item.MALSTRON = (result["MALSTRON"] is DBNull) ? 0 : (long)result["MALSTRON"];
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
    }
}
