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

                                    List<CanReportDto> lstResult = new List<CanReportDto>();
                                    List<CanReportDto> lstTong = new List<CanReportDto>();

                                    int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                    int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                    int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                    int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                    DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                    DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);


                                    string dieuKienBienSo = string.IsNullOrEmpty(paging.BIENSO) || paging.BIENSO == "undefined" ? "1=1" : $"BienXe LIKE N'%{paging.BIENSO}%'";
                                    string dieuKienVatLieu = string.IsNullOrEmpty(paging.VATLIEU) || paging.VATLIEU == "undefined" ? "1=1" : $"TenVatLieu LIKE N'%{paging.VATLIEU}%'";
                                    string dieuKienKhachHang = string.IsNullOrEmpty(paging.TenKH) || paging.TenKH == "undefined" ? "1=1" : $"KhachHang LIKE N'%{paging.TenKH}%'";
                                    string dieuKienNguoiCan = string.IsNullOrEmpty(paging.NGUOICAN) || paging.NGUOICAN == "undefined" ? "1=1" : $"ISNULL(UserName2, UserName1) LIKE N'%{paging.NGUOICAN}%'";

                                    string dieuKienKieuCan = "1=1";
                                    if (!string.IsNullOrEmpty(paging.KIEUCAN) && paging.KIEUCAN != "All" && paging.KIEUCAN != "undefined")
                                    {
                                        // Giả sử cột trong DB là TrangThaiCan, bạn sửa lại nếu khác
                                        dieuKienKieuCan = $"LOAICAN = N'{paging.KIEUCAN}'";
                                    }

                                    String sql = string.Format($@"SELECT
                                                                    A.MaPhieu N'Số phiếu',ISNULL(A.ThoiGianCanLan2, A.ThoiGianCanLan1) N'Ngày', 
                                                                    A.LoaiCan N'Loại cân', A.BienXe N'Biển số', A.LaiXe N'Lái xe', A.KhachHang N'Khách hàng',
                                                                    A.TenHangHoa N'Hàng hóa', A.SoNiemChi N'Số niêm chì',
                                                                    A.KhoiLuongCanLan1 N'KL cân bì', A.ThoiGianCanLan1 N'TG cân bì',
                                                                    A.KhoiLuongCanLan2 N'KL cân hàng', A.ThoiGianCanLan2 N'TG cân hàng',
                                                                    A.KhoiLuongTapChat N'KL tạp chất',
                                                                    ABS(ISNULL(A.KhoiLuongCanLan2, 0) - ISNULL(A.KhoiLuongCanLan1, 0)) - ISNULL(A.KhoiLuongTapChat, 0) N'KL hàng',
                                                                    (ABS(ISNULL(A.KhoiLuongCanLan2, 0) - ISNULL(A.KhoiLuongCanLan1, 0)) - ISNULL(A.KhoiLuongTapChat, 0)) * A.HeSoQuyDoi N'KL quy đổi',
							                                                    A.KhoiLuongDat N'KL đặt hàng',							
							                                                    A.KhoiLuongCanTD + A.KhoiLuongCanTay N'KL cân tinh',
							                                                    A.ThoiGianBDCanLieu N'TG bắt đầu cân tinh',
							                                                    A.ThoiGianKTCanLieu N'TG kết thúc cân tinh'
                                                                    --B.TenVL,
                                                                    --X.MAXDATE
                                                                    FROM [{branch.Dataname}].[dbo].[LSCan] A

                                                                        CROSS APPLY
                                                                                    (
                                                                                        SELECT
                                                                                            CASE
                                                                                                WHEN A.ThoiGianCanLan1 IS NULL THEN A.ThoiGianCanLan2
                                                                                                WHEN A.ThoiGianCanLan2 IS NULL THEN A.ThoiGianCanLan1
                                                                                                WHEN A.ThoiGianCanLan1 >= A.ThoiGianCanLan2 THEN A.ThoiGianCanLan1
                                                                                                ELSE A.ThoiGianCanLan2
                                                                                            END AS MAXDATE
                                                                                    ) X

                                                                                    --LEFT JOIN VatLieuTramTron B
                                                                                    --    ON B.DBName = A.ListTramTron
                                                                                    --   AND B.TenVL = A.ListVatLieuTramTron
                                                                    WHERE 
                                                                        ISNULL(ThoiGianCanLan2, ThoiGianCanLan1) >= '{CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau)}'
                                                                        AND ISNULL(ThoiGianCanLan2, ThoiGianCanLan1) <= '{CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc)}'
                                                                        AND {dieuKienBienSo}
                                                                        AND {dieuKienVatLieu}
                                                                        AND {dieuKienKhachHang}
                                                                        AND {dieuKienKieuCan}
                                                                        AND {dieuKienNguoiCan}");

                                    command.CommandText += sql.ToString();

                                    DataTable dtSource = CommonLib.GetDataBySql(sql);
                                    var dataTable = CommonLib.AsEnumerable(dtSource);

                                    List<CanReportDto> listData = dataTable.Select(row => new CanReportDto()
                                    {
                                        SoPhieu = row["SoPhieu"] != DBNull.Value ? row["SoPhieu"].ToString() : "",
                                        Ngay = row["Ngay"] != DBNull.Value ? Convert.ToDateTime(row["Ngay"]) : (DateTime?)null,
                                        KhachHang = row["KhachHang"] != DBNull.Value ? row["KhachHang"].ToString() : "",
                                        BienSo = row["BienSo"] != DBNull.Value ? row["BienSo"].ToString() : "",
                                        LaiXe = row["LaiXe"] != DBNull.Value ? row["LaiXe"].ToString() : "",
                                        HangHoa = row["HangHoa"] != DBNull.Value ? row["HangHoa"].ToString() : "",
                                        CanLan1 = row["CanLan1"] != DBNull.Value ? Convert.ToDecimal(row["CanLan1"]) : (decimal?)null,
                                        CanLan2 = row["CanLan2"] != DBNull.Value ? Convert.ToDecimal(row["CanLan2"]) : (decimal?)null,
                                        KhoiLuongHang = row["KhoiLuongHang"] != DBNull.Value ? Convert.ToDecimal(row["KhoiLuongHang"]) : (decimal?)null,
                                        KhoiLuongQuyDoi = row["KhoiLuongQuyDoi"] != DBNull.Value ? Convert.ToDecimal(row["KhoiLuongQuyDoi"]) : (decimal?)null,
                                        DonVi = row["DonVi"] != DBNull.Value ? row["DonVi"].ToString() : "",
                                        ThoiGianCanLan1 = row["ThoiGianCanLan1"] != DBNull.Value ? Convert.ToDateTime(row["ThoiGianCanLan1"]) : (DateTime?)null,
                                        ThoiGianCanLan2 = row["ThoiGianCanLan2"] != DBNull.Value ? Convert.ToDateTime(row["ThoiGianCanLan2"]) : (DateTime?)null,
                                        NguoiCan = row["NguoiCan"] != DBNull.Value ? row["NguoiCan"].ToString() : ""
                                    }).ToList();


                                    List<CanReportGroupDTO> result = new List<CanReportGroupDTO>();

                                    switch (paging.GroupBy)
                                    {
                                        case "KH": // Group theo khách hàng
                                            var groupByKH = listData.GroupBy(x => x.KhachHang);
                                            foreach (var itemGrp in groupByKH)
                                            {
                                                result.Add(new CanReportGroupDTO
                                                {
                                                    Key = itemGrp.Key,
                                                    Data = itemGrp.ToList(),
                                                    Expanded = false,
                                                    TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                    TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                    TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                    ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                });
                                            }
                                            break;

                                        case "VL": // Group theo vật liệu
                                            var groupByVL = listData.GroupBy(x => x.HangHoa);
                                            foreach (var itemGrp in groupByVL)
                                            {
                                                result.Add(new CanReportGroupDTO
                                                {
                                                    Key = itemGrp.Key,
                                                    Data = itemGrp.ToList(),
                                                    Expanded = false,
                                                    TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                    TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                    TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                    ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                });
                                            }
                                            break;

                                        default: // Mặc định group theo ngày
                                            var groupByNgay = listData.GroupBy(x => x.Ngay?.ToString("yyyy-MM-dd")); // Format ngày nếu cần gom theo ngày
                                            foreach (var itemGrp in groupByNgay)
                                            {
                                                result.Add(new CanReportGroupDTO
                                                {
                                                    Key = itemGrp.Key,
                                                    Data = itemGrp.ToList(),
                                                    Expanded = false,
                                                    TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                    TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                    TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                    ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                });
                                            }
                                            break;
                                    }

                                    def.data = result;
                                    def.metadata = listData.Count();
                                    def.meta = new Meta(200, "Success");
                                    return Ok(def);

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

                                        List<CanReportDto> lstResult = new List<CanReportDto>();

                                        int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                        int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                        int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                        int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                        DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                        DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);


                                        string dieuKienBienSo = string.IsNullOrEmpty(paging.BIENSO) || paging.BIENSO == "undefined" ? "1=1" : $"BienXe LIKE N'%{paging.BIENSO}%'";
                                        string dieuKienVatLieu = string.IsNullOrEmpty(paging.VATLIEU) || paging.VATLIEU == "undefined" ? "1=1" : $"TenVatLieu LIKE N'%{paging.VATLIEU}%'";
                                        string dieuKienKhachHang = string.IsNullOrEmpty(paging.TenKH) || paging.TenKH == "undefined" ? "1=1" : $"KhachHang LIKE N'%{paging.TenKH}%'";
                                        string dieuKienNguoiCan = string.IsNullOrEmpty(paging.NGUOICAN) || paging.NGUOICAN == "undefined" ? "1=1" : $"ISNULL(UserName2, UserName1) LIKE N'%{paging.NGUOICAN}%'";

                                        string dieuKienKieuCan = "1=1";
                                        if (!string.IsNullOrEmpty(paging.KIEUCAN) && paging.KIEUCAN != "All" && paging.KIEUCAN != "undefined")
                                        {
                                            // Giả sử cột trong DB là TrangThaiCan, bạn sửa lại nếu khác
                                            dieuKienKieuCan = $"KieuCan = N'{paging.KIEUCAN}'";
                                        }

                                        String sql = string.Format($@"SELECT 
                                                                        MaPhieu AS SoPhieu,
                                                                        ISNULL(ThoiGianCanLan2, ThoiGianCanLan1) AS Ngay,
                                                                        KhachHang AS KhachHang,
                                                                        BienXe AS BienSo,
                                                                        LaiXe AS LaiXe,
                                                                        TenVatLieu AS HangHoa,
                                                                        KhoiLuongCanLan1 AS CanLan1,
                                                                        KhoiLuongCanLan2 AS CanLan2,
                                                                        KhoiLuongHang AS KhoiLuongHang,
                                                                        (KhoiLuongHang / NULLIF(HeSoQuyDoi, 0)) AS KhoiLuongQuyDoi,
                                                                        DonViQuyDoi AS DonVi,
                                                                        ThoiGianCanLan1 AS ThoiGianCanLan1,
                                                                        ThoiGianCanLan2 AS ThoiGianCanLan2,
                                                                        ISNULL(UserName2, UserName1) AS NguoiCan
                                                                    FROM [{branch.Dataname}].[dbo].[LSCan]
                                                                    WHERE 
                                                                        ISNULL(ThoiGianCanLan2, ThoiGianCanLan1) >= '{CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau)}'
                                                                        AND ISNULL(ThoiGianCanLan2, ThoiGianCanLan1) <= '{CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc)}'
                                                                        AND {dieuKienBienSo}
                                                                        AND {dieuKienVatLieu}
                                                                        AND {dieuKienKhachHang}
                                                                        AND {dieuKienKieuCan}
                                                                        AND {dieuKienNguoiCan}");

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);

                                        List<CanReportGroupDTO> result = new List<CanReportGroupDTO>();
                                        if (dtSource.Rows.Count > 0)
                                        {
                                            var dataTable = CommonLib.AsEnumerable(dtSource);

                                            List<CanReportDto> listData = dataTable.Select(row => new CanReportDto()
                                            {
                                                SoPhieu = row["SoPhieu"] != DBNull.Value ? row["SoPhieu"].ToString() : "",
                                                Ngay = row["Ngay"] != DBNull.Value ? Convert.ToDateTime(row["Ngay"]) : (DateTime?)null,
                                                KhachHang = row["KhachHang"] != DBNull.Value ? row["KhachHang"].ToString() : "",
                                                BienSo = row["BienSo"] != DBNull.Value ? row["BienSo"].ToString() : "",
                                                LaiXe = row["LaiXe"] != DBNull.Value ? row["LaiXe"].ToString() : "",
                                                HangHoa = row["HangHoa"] != DBNull.Value ? row["HangHoa"].ToString() : "",
                                                CanLan1 = row["CanLan1"] != DBNull.Value ? Convert.ToDecimal(row["CanLan1"]) : (decimal?)null,
                                                CanLan2 = row["CanLan2"] != DBNull.Value ? Convert.ToDecimal(row["CanLan2"]) : (decimal?)null,
                                                KhoiLuongHang = row["KhoiLuongHang"] != DBNull.Value ? Convert.ToDecimal(row["KhoiLuongHang"]) : (decimal?)null,
                                                KhoiLuongQuyDoi = row["KhoiLuongQuyDoi"] != DBNull.Value ? Convert.ToDecimal(row["KhoiLuongQuyDoi"]) : (decimal?)null,
                                                DonVi = row["DonVi"] != DBNull.Value ? row["DonVi"].ToString() : "",
                                                ThoiGianCanLan1 = row["ThoiGianCanLan1"] != DBNull.Value ? Convert.ToDateTime(row["ThoiGianCanLan1"]) : (DateTime?)null,
                                                ThoiGianCanLan2 = row["ThoiGianCanLan2"] != DBNull.Value ? Convert.ToDateTime(row["ThoiGianCanLan2"]) : (DateTime?)null,
                                                NguoiCan = row["NguoiCan"] != DBNull.Value ? row["NguoiCan"].ToString() : ""
                                            }).ToList();

                                            var groupName = string.Empty;
                                            switch (paging.GroupBy)
                                            {
                                                case "KH": // Group theo khách hàng
                                                    var groupByKH = listData.GroupBy(x => x.KhachHang);
                                                    foreach (var itemGrp in groupByKH)
                                                    {
                                                        result.Add(new CanReportGroupDTO
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                            TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                            TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                            ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                        });
                                                    }
                                                    groupName = "Khách hàng: ";
                                                    break;

                                                case "VL": // Group theo vật liệu
                                                    var groupByVL = listData.GroupBy(x => x.HangHoa);
                                                    foreach (var itemGrp in groupByVL)
                                                    {
                                                        result.Add(new CanReportGroupDTO
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                            TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                            TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                            ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                        });
                                                    }
                                                    groupName = "Vật liệu: ";
                                                    break;

                                                default: // Mặc định group theo ngày
                                                    var groupByNgay = listData.GroupBy(x => x.Ngay?.ToString("yyyy-MM-dd")); // Format ngày nếu cần gom theo ngày
                                                    foreach (var itemGrp in groupByNgay)
                                                    {
                                                        result.Add(new CanReportGroupDTO
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalCanLan1 = itemGrp.Sum(x => x.CanLan1 ?? 0),
                                                            TotalCanLan2 = itemGrp.Sum(x => x.CanLan2 ?? 0),
                                                            TotalKhoiLuongHang = itemGrp.Sum(x => x.KhoiLuongHang ?? 0),
                                                            ToTalKhoiLuongQuyDoi = itemGrp.Sum(x => x.KhoiLuongQuyDoi ?? 0),
                                                        });
                                                    }
                                                    groupName = "Ngày lập: ";
                                                    break;
                                            }

                                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                            using (var package = new ExcelPackage())
                                            {
                                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Báo cáo chi tiết trạm cân");
                                                worksheet.Cells["A1:N1"].Merge = true;
                                                worksheet.Cells["A1:N1"].Value = "BÁO CÁO CHI TIẾT TRẠM CÂN";
                                                worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                                                worksheet.Cells["A1:N1"].Style.Font.Size = 16;
                                                worksheet.Cells["A1:N1"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A1:N1"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                worksheet.Cells["A2:N2"].Merge = true;
                                                worksheet.Cells["A2:N2"].Value = "Báo cáo được tạo vào ngày " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                                                worksheet.Cells["A2:N2"].Style.Font.Italic = true;
                                                worksheet.Cells["A2:N2"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A2:N2"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                //Điều kiện lọc

                                                worksheet.Cells["A3:N3"].Merge = true;
                                                worksheet.Cells["A3:N3"].Value = "* Điều kiện lọc:";
                                                worksheet.Cells["A3:N3"].Style.Font.Italic = true;
                                                worksheet.Cells["A3:N3"].Style.Font.Bold = true;
                                                worksheet.Cells["A3:N3"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A3:N3"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A3:N3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                int row = 4;

                                                void WriteCondition(string label, string value)
                                                {
                                                    worksheet.Cells[$"A{row}:N{row}"].Merge = true;
                                                    worksheet.Cells[$"A{row}"].Value = $"- {label}: {(string.IsNullOrEmpty(value) || value == "undefined" ? "Tất cả" : value)}";
                                                    row++;
                                                }

                                                WriteCondition("Từ ngày", paging.tungay.ToString("dd-MM-yyyy"));
                                                WriteCondition("Đến ngày", paging.denngay.ToString("dd-MM-yyyy"));
                                                WriteCondition("Biển số", paging.BIENSO);
                                                WriteCondition("Khách hàng", paging.TenKH);
                                                WriteCondition("Hàng hóa", paging.VATLIEU);
                                                WriteCondition("Người cân", paging.NGUOICAN);
                                                WriteCondition("Kiểu cân", paging.KIEUCAN);

                                                #region Nhóm theo

                                                worksheet.Cells[$"A{row}:N{row}"].Merge = true;
                                                worksheet.Cells[$"A{row}"].Value = "* Nhóm theo: " + groupName;
                                                worksheet.Cells[$"A{row}"].Style.Font.Bold = true;
                                                row += 2;

                                                #endregion

                                                // Header
                                                worksheet.Cells[$"A{row}"].Value = "Số phiếu";
                                                worksheet.Cells[$"B{row}"].Value = "Ngày";
                                                worksheet.Cells[$"C{row}"].Value = "Khách hàng";
                                                worksheet.Cells[$"D{row}"].Value = "Biển số";
                                                worksheet.Cells[$"E{row}"].Value = "Lái xe";
                                                worksheet.Cells[$"F{row}"].Value = "Hàng hóa";
                                                worksheet.Cells[$"G{row}"].Value = "Đơn vị";
                                                worksheet.Cells[$"H{row}"].Value = "Thời gian cân lần 1";
                                                worksheet.Cells[$"I{row}"].Value = "Thời gian cân lần 2";
                                                worksheet.Cells[$"J{row}"].Value = "Người cân";
                                                worksheet.Cells[$"K{row}"].Value = "Cân lần 1";
                                                worksheet.Cells[$"L{row}"].Value = "Cân lần 2";
                                                worksheet.Cells[$"M{row}"].Value = "Khối lượng hàng";
                                                worksheet.Cells[$"N{row}"].Value = "Khối lượng quy đổi";




                                                worksheet.Cells[$"A{row}:N{row}"].Style.Font.Bold = true;
                                                worksheet.Cells[$"A{row}:N{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells[$"A{row}:N{row}"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                                worksheet.Cells[$"A{row}:N{row}"].Style.Font.Color.SetColor(Color.Black);
                                                row++;

                                                if (result.Count() > 0)
                                                {
                                                    int rowFirts = row;
                                                    for (int i = 0; i < result.Count(); i++)
                                                    {
                                                        var elementGroup = result.ElementAt(i);
                                                        //Row merge
                                                        var region = "A" + rowFirts + ":" + "N" + rowFirts;
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
                                                            row = rowFirts + j;
                                                            var element = elementGroup.Data.ElementAt(j);
                                                            int column = 1;
                                                            //Phiếu
                                                            worksheet.Cells[row, column].Value = element?.SoPhieu;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Ngày lập
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.Ngay;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "dd/mm/yyyy";

                                                            //Khách hàng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.KhachHang;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Biển số
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.BienSo;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Lái xe
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.LaiXe;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Hàng hóa
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.HangHoa;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Đơn vị
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.DonVi;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Thời gian cân lần 1
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.ThoiGianCanLan1;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "dd/mm/yyyy";

                                                            //Thời gian cân lần 2
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.ThoiGianCanLan2;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "dd/mm/yyyy";

                                                            //Người cân
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.NguoiCan;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Cân lần 1
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.CanLan1;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                            //Cân lần 2
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.CanLan1;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                            //Khối lượng hàng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.CanLan1;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                            //Khối lượng quy đổi
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.CanLan1;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                        }
                                                        rowFirts = rowFirts + elementGroup.Data.Count();

                                                        //Row merge total
                                                        region = "A" + rowFirts + ":" + "J" + rowFirts;
                                                        worksheet.Cells[region].Merge = true;
                                                        worksheet.Cells[region].Value = "Tổng:";
                                                        worksheet.Cells[region].Style.Font.Italic = true;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Center;
                                                        worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        region = "K" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalCanLan1;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        region = "L" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalCanLan2;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        region = "M" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalKhoiLuongHang;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        region = "N" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.ToTalKhoiLuongQuyDoi;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        rowFirts++;
                                                    }
                                                    string modelRange = "A13:N" + (listData.Count() + result.Count() * 2 + 13);
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

                    List<string> nv = new List<string>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT BienXe FROM [" + branch.Dataname + "].[dbo].[LSCan] WHERE ISNULL(BienXe, '') <> ''";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            nv.Add((result["BienXe"] is DBNull) ? String.Empty : (string)result["BienXe"]);
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

                    List<string> nv = new List<string>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT KhachHang FROM [" + branch.Dataname + "].[dbo].[LSCan] WHERE ISNULL(KhachHang, '') <> ''";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            nv.Add((result["KhachHang"] is DBNull) ? String.Empty : (string)result["KhachHang"]);
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

        [HttpGet("GetVatLieu/{Branchlist}")]
        public IActionResult GetVatLieu(string Branchlist)
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

                    List<string> nv = new List<string>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT DISTINCT TenVatLieu FROM [" + branch.Dataname + "].[dbo].[LSCan] WHERE ISNULL(TenVatLieu, '') <> ''";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {

                        while (result.Read())
                        {
                            nv.Add((result["TenVatLieu"] is DBNull) ? String.Empty : (string)result["TenVatLieu"]);
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

        [HttpGet("GetNguoiCan/{Branchlist}")]
        public IActionResult GetNguoiCan(string Branchlist)
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

                    List<string> nv = new List<string>();

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText = $@"
                                                SELECT DISTINCT UserName1 AS UserName FROM [{branch.Dataname}].[dbo].[LSCan] WHERE ISNULL(UserName1, '') <> ''
                                                UNION 
                                                SELECT DISTINCT UserName2 AS UserName FROM [{branch.Dataname}].[dbo].[LSCan] WHERE ISNULL(UserName2, '') <> ''";

                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            string user = result["UserName"] as string;
                            if (!string.IsNullOrEmpty(user))
                                nv.Add(user);
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