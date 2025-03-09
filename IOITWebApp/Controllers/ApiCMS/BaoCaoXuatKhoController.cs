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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Reflection;
using System.Drawing;
using OfficeOpenXml.Style;

namespace IOITWebApp.Controllers.ApiCMS
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoXuatKhoController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("baocaoxuatkho", "baocaoxuatkho");
        private static string functionCode = "BCXK";
        private IHostingEnvironment _hostingEnvironment;
        public BaoCaoXuatKhoController(IHostingEnvironment hostingEnvironment)
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

                                    List<BaoCaoXuatKhoDTO> lstResult = new List<BaoCaoXuatKhoDTO>();
                                    if (paging.MALIENKETKH is null || paging.MALIENKETKH == "undefined" || paging.MALIENKETKH == "null")
                                    {
                                        paging.MALIENKETKH = "";
                                    }
                                    if (paging.MALIENKETMAC is null || paging.MALIENKETMAC == "undefined" || paging.MALIENKETMAC == "null")
                                    {
                                        paging.MALIENKETMAC = "";
                                    }
                                    if (paging.CVL is null || paging.CVL == "undefined" || paging.CVL == "null")
                                    {
                                        paging.CVL = "";
                                    }

                                    if (paging.MADVCS is null || paging.MADVCS == "undefined" || paging.MADVCS == "null")
                                    {
                                        paging.MADVCS = "";
                                    }
                                    if (paging.MAGD is null || paging.MAGD == "undefined" || paging.MAGD == "null")
                                    {
                                        paging.MAGD = "";
                                    }
                                    if (paging.QUYEN is null || paging.QUYEN == "undefined" || paging.QUYEN == "null")
                                    {
                                        paging.QUYEN = "";
                                    }
                                    if (paging.MAKHO is null || paging.MAKHO == "undefined" || paging.MAKHO == "null")
                                    {
                                        paging.MAKHO = "";
                                    }
                                    if (paging.TKNO is null || paging.TKNO == "undefined" || paging.TKNO == "null")
                                    {
                                        paging.TKNO = "";
                                    }
                                    if (paging.TKCO is null || paging.TKCO == "undefined" || paging.TKCO == "null")
                                    {
                                        paging.TKCO = "";
                                    }
                                    int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                    int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                    int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                    int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                    DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                    DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);

                                    if (paging != null)
                                    {
                                        string cond, lkkh, lkmac;
                                        cond = lkkh = lkmac = "1=1";

                                        if (!string.IsNullOrEmpty(paging.CVL)) cond = string.Format("F.TENCUAVL = N'{0}'", paging.CVL);
                                        if (!string.IsNullOrEmpty(paging.MALIENKETKH)) lkkh = string.Format("G.MaLKKhachHang = N'{0}'", paging.MALIENKETKH);
                                        if (!string.IsNullOrEmpty(paging.MALIENKETMAC)) lkkh = string.Format("G.MaLKMac = N'{0}'", paging.MALIENKETMAC);

                                        string dvcs, magd, quyen, makho, tkno, tkco;
                                        dvcs = magd = quyen = makho = tkno = tkco = string.Empty;
                                        if (!string.IsNullOrEmpty(paging.MADVCS))
                                            dvcs = string.Format("N'{0}'", paging.MADVCS);
                                        else dvcs = "N'CTY'";

                                        if (!string.IsNullOrEmpty(paging.MAGD))
                                            magd = string.Format("N'{0}'", paging.MAGD);
                                        else magd = "N'4'";

                                        if (!string.IsNullOrEmpty(paging.QUYEN))
                                            quyen = string.Format("N'{0}'", paging.QUYEN);
                                        else quyen = "N'PX22'";

                                        if (!string.IsNullOrEmpty(paging.MAKHO))
                                            makho = string.Format("N'{0}'", paging.MAKHO);
                                        else makho = "N'KHO1'";

                                        if (!string.IsNullOrEmpty(paging.TKNO))
                                            tkno = string.Format("N'{0}'", paging.TKNO);
                                        else tkno = "N'154'";

                                        if (!string.IsNullOrEmpty(paging.TKCO))
                                            tkco = string.Format("N'{0}'", paging.TKCO);
                                        else tkco = "N'1521'";

                                        String sql = string.Format("SELECT " + dvcs + " MADVCS, " + magd + " MAGD, G.MaLKKhachHang MAKHACHHANG, G.TENKHACHHANG MANGUOINHAN, \n" +
                                                                        "N' ' DIENGIAI, FORMAT(A.NGAYTRON, 'dd/MM/yyyy') NGAYTRON, " + quyen + " MAQUYENSO, N' ' SOCHUNGTU, F.TENCUAVL MAVATTU, \n" +
                                                                        "" + makho + " MAKHO, round(SUM((ISNULL(D.SOLUONGTD, 0))) + SUM((ISNULL(D.SOLUONGTAY, 0))), 0) SOLUONG, N' ' GIANGOAITE,N' ' TIENNGOAITE, \n" +
                                                                        "N' ' MANGOAITE, N' ' TYGIA, N' ' GIA, N' ' TIEN," + tkno + " TKNO, " + tkco + " TKCO, N' ' MADUAN, \n" +
                                                                        "N' ' MAPHI, G.MaLKMac MASANPHAM, N' ' MABPHT, N' ' SOLSX, N' ' MATRAM, N'' KLTRON, N' ' KLGIAO \n" +
                                                                    "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON \n" +
                                                                      "INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].CUAVL F ON F.STTCUAVL = D.STTCUAVL \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG G ON A.STTLSDATHANG = G.STT \n" +
                                                                    "WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                        "AND {2} AND {3} AND {4} AND ISNULL(F.TRANGTHAI, 0) <> 0 \n" +
                                                                    "GROUP BY F.TENCUAVL, F.STTCUAVL, G.MaLKKhachHang, G.TENKHACHHANG, G.MaLKMac, A.NGAYTRON ORDER BY G.TENKHACHHANG, MASANPHAM \n",
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                    cond, lkkh, lkmac);

                                        command.CommandText = sql.ToString();

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                       // dtSource.Columns.Remove("MALSTRON");

                                        var dataTable = CommonLib.AsEnumerable(dtSource);
                                        List<BaoCaoXuatKhoNewDTO> listData = dataTable.Select(row => new BaoCaoXuatKhoNewDTO()
                                        {
                                            MADVCS = row["MADVCS"] != DBNull.Value ? (string)row["MADVCS"] : "",
                                            NGAYTRON = row["NGAYTRON"] != DBNull.Value ? (string)row["NGAYTRON"] : "",
                                            DIENGIAI = row["DIENGIAI"] != DBNull.Value ? (string)row["DIENGIAI"] : "",
                                            GIA = row["GIA"] != DBNull.Value ? (string)row["GIA"] : "",
                                            GIANGOAITE = row["GIANGOAITE"] != DBNull.Value ? (string)row["GIANGOAITE"] : "",
                                            KLGIAO = row["KLGIAO"] != DBNull.Value ? (string)row["KLGIAO"] : "",
                                            KLTRON = row["KLTRON"] != DBNull.Value ? (string)row["KLTRON"] : "",
                                            MABPHT = row["MABPHT"] != DBNull.Value ? (string)row["MABPHT"] : "",
                                            MADUAN = row["MADUAN"] != DBNull.Value ? (string)row["MADUAN"] : "",
                                            MAKHACHHANG = row["MAKHACHHANG"] != DBNull.Value ? (string)row["MAKHACHHANG"] : "",
                                            MAGD = row["MAGD"] != DBNull.Value ? (string)row["MAGD"] : "",
                                            MAKHO = row["MAKHO"] != DBNull.Value ? (string)row["MAKHO"] : "",
                                            MANGOAITE = row["MANGOAITE"] != DBNull.Value ? (string)row["MANGOAITE"] : "",
                                            MANGUOINHAN = row["MANGUOINHAN"] != DBNull.Value ? (string)row["MANGUOINHAN"] : "",
                                            MAPHI = row["MAPHI"] != DBNull.Value ? (string)row["MAPHI"] : "",
                                            MAQUYENSO = row["MAQUYENSO"] != DBNull.Value ? (string)row["MAQUYENSO"] : "",
                                            MASANPHAM = row["MASANPHAM"] != DBNull.Value ? (string)row["MASANPHAM"] : "",
                                            MATRAM = row["MATRAM"] != DBNull.Value ? (string)row["MATRAM"] : "",
                                            MAVATTU = row["MAVATTU"] != DBNull.Value ? (string)row["MAVATTU"] : "",
                                            SOCHUNGTU = row["SOCHUNGTU"] != DBNull.Value ? (string)row["SOCHUNGTU"] : "",
                                            SOLSX = row["SOLSX"] != DBNull.Value ? (string)row["SOLSX"] : "",
                                            SOLUONG = (double)row["SOLUONG"],
                                            TIEN = row["TIEN"] != DBNull.Value ? (string)row["TIEN"] : "",
                                            TIENNGOAITE = row["TIENNGOAITE"] != DBNull.Value ? (string)row["TIENNGOAITE"] : "",
                                            TKCO = row["TKCO"] != DBNull.Value ? (string)row["TKCO"] : "",
                                            TKNO = row["TKNO"] != DBNull.Value ? (string)row["TKNO"] : "",
                                            TYGIA = row["TYGIA"] != DBNull.Value ? (string)row["TYGIA"] : "",
                                        }).ToList();

                                        List<BaoCaoXuatKhoGroupDTO> result = new List<BaoCaoXuatKhoGroupDTO>();
                                        switch (paging.GroupBy)
                                        {
                                            case "KH":
                                                var groupsCT = listData.GroupBy(x => x.MANGUOINHAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    }); ;
                                                }
                                                break;
                                            case "NT":
                                                var groupsNT = listData.GroupBy(x => x.NGAYTRON);
                                                foreach (var itemGrp in groupsNT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    }); ;
                                                }
                                                break;
                                            case "SP":
                                                var groupsSP = listData.GroupBy(x => x.MASANPHAM);
                                                foreach (var itemGrp in groupsSP)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    }); ;
                                                }
                                                break;
                                            default: // Mặc định group theo khách hàng
                                                groupsCT = listData.GroupBy(x => x.MANGUOINHAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    }); ;
                                                }
                                                break;
                                        }
                                        def.data = result;
                                        def.metadata = listData.Count();
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

                                        List<BaoCaoXuatKhoDTO> lstResult = new List<BaoCaoXuatKhoDTO>();
                                        if (paging.MALIENKETKH is null || paging.MALIENKETKH == "undefined" || paging.MALIENKETKH == "null")
                                        {
                                            paging.MALIENKETKH = "";
                                        }
                                        if (paging.MALIENKETMAC is null || paging.MALIENKETMAC == "undefined" || paging.MALIENKETMAC == "null")
                                        {
                                            paging.MALIENKETMAC = "";
                                        }
                                        if (paging.CVL is null || paging.CVL == "undefined" || paging.CVL == "null")
                                        {
                                            paging.CVL = "";
                                        }

                                        if (paging.MADVCS is null || paging.MADVCS == "undefined" || paging.MADVCS == "null")
                                        {
                                            paging.MADVCS = "";
                                        }
                                        if (paging.MAGD is null || paging.MAGD == "undefined" || paging.MAGD == "null")
                                        {
                                            paging.MAGD = "";
                                        }
                                        if (paging.QUYEN is null || paging.QUYEN == "undefined" || paging.QUYEN == "null")
                                        {
                                            paging.QUYEN = "";
                                        }
                                        if (paging.MAKHO is null || paging.MAKHO == "undefined" || paging.MAKHO == "null")
                                        {
                                            paging.MAKHO = "";
                                        }
                                        if (paging.TKNO is null || paging.TKNO == "undefined" || paging.TKNO == "null")
                                        {
                                            paging.TKNO = "";
                                        }
                                        if (paging.TKCO is null || paging.TKCO == "undefined" || paging.TKCO == "null")
                                        {
                                            paging.TKCO = "";
                                        }

                                        int hourTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(0, 2)) : 0;
                                        int minuteTungay = !string.IsNullOrEmpty(paging.timetungay) ? int.Parse(paging.timetungay.Substring(3)) : 0;

                                        int hourDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(0, 2)) : 23;
                                        int minuteDenngay = !string.IsNullOrEmpty(paging.timedenngay) ? int.Parse(paging.timedenngay.Substring(3)) : 59;

                                        DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, hourTungay, minuteTungay, 0);
                                        DateTime thoigianketthuc = new DateTime(paging.denngay.Year, paging.denngay.Month, paging.denngay.Day, hourDenngay, minuteDenngay, 0);


                                        string cond, lkkh, lkmac;
                                        cond = lkkh = lkmac = "1=1";

                                        if (!string.IsNullOrEmpty(paging.CVL)) cond = string.Format("F.TENCUAVL = N'{0}'", paging.CVL);
                                        if (!string.IsNullOrEmpty(paging.MALIENKETKH)) lkkh = string.Format("G.MaLKKhachHang = N'{0}'", paging.MALIENKETKH);
                                        if (!string.IsNullOrEmpty(paging.MALIENKETMAC)) lkkh = string.Format("G.MaLKMac = N'{0}'", paging.MALIENKETMAC);

                                        string dvcs, magd, quyen, makho, tkno, tkco;
                                        dvcs = magd = quyen = makho = tkno = tkco = string.Empty;
                                        if (!string.IsNullOrEmpty(paging.MADVCS))
                                            dvcs = string.Format("N'{0}'", paging.MADVCS);
                                        else dvcs = "N'CTY'";

                                        if (!string.IsNullOrEmpty(paging.MAGD))
                                            magd = string.Format("N'{0}'", paging.MAGD);
                                        else magd = "N'4'";

                                        if (!string.IsNullOrEmpty(paging.QUYEN))
                                            quyen = string.Format("N'{0}'", paging.QUYEN);
                                        else quyen = "N'PX22'";

                                        if (!string.IsNullOrEmpty(paging.MAKHO))
                                            makho = string.Format("N'{0}'", paging.MAKHO);
                                        else makho = "N'KHO1'";

                                        if (!string.IsNullOrEmpty(paging.TKNO))
                                            tkno = string.Format("N'{0}'", paging.TKNO);
                                        else tkno = "N'154'";

                                        if (!string.IsNullOrEmpty(paging.TKCO))
                                            tkco = string.Format("N'{0}'", paging.TKCO);
                                        else tkco = "N'1521'";

                                        String sql = string.Format("SELECT " + dvcs + " MADVCS, " + magd + " MAGD, G.MaLKKhachHang MAKHACHHANG, G.TENKHACHHANG MANGUOINHAN, \n" +
                                                                        "N' ' DIENGIAI, FORMAT(A.NGAYTRON, 'dd/MM/yyyy') NGAYTRON, " + quyen + " MAQUYENSO, N' ' SOCHUNGTU, F.TENCUAVL MAVATTU, \n" +
                                                                        "" + makho + " MAKHO, round(SUM((ISNULL(D.SOLUONGTD, 0))) + SUM((ISNULL(D.SOLUONGTAY, 0))), 0) SOLUONG, N' ' GIANGOAITE,N' ' TIENNGOAITE, \n" +
                                                                        "N' ' MANGOAITE, N' ' TYGIA, N' ' GIA, N' ' TIEN," + tkno + " TKNO, " + tkco + " TKCO, N' ' MADUAN, \n" +
                                                                        "N' ' MAPHI, G.MaLKMac MASANPHAM, N' ' MABPHT, N' ' SOLSX, N' ' MATRAM, N'' KLTRON, N' ' KLGIAO \n" +
                                                                    "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON \n" +
                                                                      "INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].CUAVL F ON F.STTCUAVL = D.STTCUAVL \n" +
                                                                      "LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG G ON A.STTLSDATHANG = G.STT \n" +
                                                                    "WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                        "AND {2} AND {3} AND {4} AND ISNULL(F.TRANGTHAI, 0) <> 0 \n" +
                                                                    "GROUP BY F.TENCUAVL, F.STTCUAVL, G.MaLKKhachHang, G.TENKHACHHANG, G.MaLKMac, A.NGAYTRON ORDER BY G.TENKHACHHANG, MASANPHAM \n",
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                    cond, lkkh, lkmac);

                                        command.CommandText = sql.ToString();

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                       // dtSource.Columns.Remove("MALSTRON");

                                        var dataTable = CommonLib.AsEnumerable(dtSource);
                                        List<BaoCaoXuatKhoNewDTO> listData = dataTable.Select(row => new BaoCaoXuatKhoNewDTO()
                                        {
                                            MADVCS = row["MADVCS"] != DBNull.Value ? (string)row["MADVCS"] : "",
                                            NGAYTRON = row["NGAYTRON"] != DBNull.Value ? (string)row["NGAYTRON"] : "",
                                            DIENGIAI = row["DIENGIAI"] != DBNull.Value ? (string)row["DIENGIAI"] : "",
                                            GIA = row["GIA"] != DBNull.Value ? (string)row["GIA"] : "",
                                            GIANGOAITE = row["GIANGOAITE"] != DBNull.Value ? (string)row["GIANGOAITE"] : "",
                                            KLGIAO = row["KLGIAO"] != DBNull.Value ? (string)row["KLGIAO"] : "",
                                            KLTRON = row["KLTRON"] != DBNull.Value ? (string)row["KLTRON"] : "",
                                            MABPHT = row["MABPHT"] != DBNull.Value ? (string)row["MABPHT"] : "",
                                            MADUAN = row["MADUAN"] != DBNull.Value ? (string)row["MADUAN"] : "",
                                            MAKHACHHANG = row["MAKHACHHANG"] != DBNull.Value ? (string)row["MAKHACHHANG"] : "",
                                            MAGD = row["MAGD"] != DBNull.Value ? (string)row["MAGD"] : "",
                                            MAKHO = row["MAKHO"] != DBNull.Value ? (string)row["MAKHO"] : "",
                                            MANGOAITE = row["MANGOAITE"] != DBNull.Value ? (string)row["MANGOAITE"] : "",
                                            MANGUOINHAN = row["MANGUOINHAN"] != DBNull.Value ? (string)row["MANGUOINHAN"] : "",
                                            MAPHI = row["MAPHI"] != DBNull.Value ? (string)row["MAPHI"] : "",
                                            MAQUYENSO = row["MAQUYENSO"] != DBNull.Value ? (string)row["MAQUYENSO"] : "",
                                            MASANPHAM = row["MASANPHAM"] != DBNull.Value ? (string)row["MASANPHAM"] : "",
                                            MATRAM = row["MATRAM"] != DBNull.Value ? (string)row["MATRAM"] : "",
                                            MAVATTU = row["MAVATTU"] != DBNull.Value ? (string)row["MAVATTU"] : "",
                                            SOCHUNGTU = row["SOCHUNGTU"] != DBNull.Value ? (string)row["SOCHUNGTU"] : "",
                                            SOLSX = row["SOLSX"] != DBNull.Value ? (string)row["SOLSX"] : "",
                                            SOLUONG = (double)row["SOLUONG"],
                                            TIEN = row["TIEN"] != DBNull.Value ? (string)row["TIEN"] : "",
                                            TIENNGOAITE = row["TIENNGOAITE"] != DBNull.Value ? (string)row["TIENNGOAITE"] : "",
                                            TKCO = row["TKCO"] != DBNull.Value ? (string)row["TKCO"] : "",
                                            TKNO = row["TKNO"] != DBNull.Value ? (string)row["TKNO"] : "",
                                            TYGIA = row["TYGIA"] != DBNull.Value ? (string)row["TYGIA"] : "",
                                        }).ToList();

                                        List<BaoCaoXuatKhoGroupDTO> result = new List<BaoCaoXuatKhoGroupDTO>();
                                        var groupName = string.Empty;
                                        switch (paging.GroupBy)
                                        {
                                            case "KH":
                                                var groupsCT = listData.GroupBy(x => x.MANGUOINHAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    });
                                                    groupName = "Người nhận: ";
                                                }
                                                break;
                                            case "NT":
                                                var groupsNT = listData.GroupBy(x => x.NGAYTRON);
                                                foreach (var itemGrp in groupsNT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    });
                                                    groupName = "Ngày trộn: ";
                                                }
                                                break;
                                            case "SP":
                                                var groupsSP = listData.GroupBy(x => x.MASANPHAM);
                                                foreach (var itemGrp in groupsSP)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    });
                                                    groupName = "Mã sản phẩm: ";
                                                }
                                                break;
                                            default: // Mặc định group theo khách hàng
                                                groupsCT = listData.GroupBy(x => x.MANGUOINHAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new BaoCaoXuatKhoGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalSOLUONG = itemGrp.Sum(x => x.SOLUONG),
                                                    });
                                                    groupName = "Người nhận: ";
                                                }
                                                break;
                                        }
                                        string template = @"template\export\BCXK_THAOANH.xlsx";
                                        string webRootPath = _hostingEnvironment.WebRootPath;
                                        string templatePath = Path.Combine(webRootPath, template);
                                        var existingFile = new FileInfo(templatePath);
                                        using (var package = new ExcelPackage(existingFile))
                                        {
                                           var worksheet = package.Workbook.Worksheets.First();
                                           worksheet.Columns[3].Width = 17.5;

                                            if (result.Count() > 0)
                                            {
                                                int rowFirts = 2;
                                                for (int i = 0; i < result.Count(); i++)
                                                {
                                                    var elementGroup = result.ElementAt(i);
                                                    //Row merge
                                                    var region = "A" + rowFirts + ":" + "AA" + rowFirts;
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

                                                        worksheet.Cells[row, column].Value = element?.MADVCS;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAGD;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAKHACHHANG;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MANGUOINHAN;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;
                                                       // worksheet.Cells[row, column].Style.WrapText = true;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.DIENGIAI;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;


                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.NGAYTRON;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[row, column].Style.Numberformat.Format = "dd/mm/yyyy";

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAQUYENSO;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.SOCHUNGTU;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAVATTU;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAKHO;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.SOLUONG;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.GIANGOAITE;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.TIENNGOAITE;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MANGOAITE;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.TYGIA;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.GIA;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;


                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.TIEN;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.TKNO;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.TKCO;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MADUAN;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MAPHI;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MASANPHAM;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MABPHT;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.SOLSX;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.MATRAM;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.KLTRON;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                        column++;
                                                        worksheet.Cells[row, column].Value = element?.KLGIAO;
                                                        worksheet.Cells[row, column].Style.VerticalAlignment =
                                                            ExcelVerticalAlignment.Center;

                                                    }
                                                    rowFirts = rowFirts + elementGroup.Data.Count();

                                                    //Row merge total
                                                    //region = "A" + rowFirts + ":" + "J" + rowFirts;
                                                    //worksheet.Cells[region].Merge = true;
                                                    //worksheet.Cells[region].Value = "Tổng:";
                                                    //worksheet.Cells[region].Style.Font.Italic = true;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                    //worksheet.Cells[region].Style.HorizontalAlignment =
                                                    //    ExcelHorizontalAlignment.Center;
                                                    //worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                    //region = "K" + rowFirts;
                                                    //worksheet.Cells[region].Value = elementGroup.TotalSOLUONG;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.VerticalAlignment =
                                                    //         ExcelVerticalAlignment.Center;

                                                    //region = "L" + rowFirts + ":" + "AA" + rowFirts;
                                                    //worksheet.Cells[region].Merge = true;
                                                    //worksheet.Cells[region].Value = "";
                                                    //worksheet.Cells[region].Style.Font.Italic = true;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                    //worksheet.Cells[region].Style.HorizontalAlignment =
                                                    //    ExcelHorizontalAlignment.Center;
                                                    //worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                                    //rowFirts++;
                                                }
                                                string modelRange = "A2:AA" + (listData.Count() + result.Count() + 1);
                                                var modelTable = worksheet.Cells[modelRange];
                                                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                                //modelCells.LoadFromCollection(Collection: model, PrintHeaders: true);
                                              //  worksheet.Cells["A:AZ"].AutoFitColumns();

                                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                                {
                                                    Content = new ByteArrayContent(package.GetAsByteArray())
                                                };
                                                return response;
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
        [HttpGet("getmalienketkh/{branchid}")]
        public IActionResult GetMaLienKetKH(int branchid)
        {
            DefaultResponse def = new DefaultResponse();
            if (branchid == null)
            {
                def.meta = new Meta(222, "Vui lòng chọn trạm");
                return Ok(def);
            }
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<MaLienKetDTO> rpMalienKet = new List<MaLienKetDTO>();
                Branch branch = context.Branch.Find(branchid);
                command.CommandText += "SELECT DISTINCT dh.MaLKKhachHang FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] dh";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var item = (result["MaLKKhachHang"] is DBNull) ? String.Empty : (string)result["MaLKKhachHang"];
                        rpMalienKet.Add(new MaLienKetDTO() { MALIENKET = item });
                    }
                    def.data = rpMalienKet;
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }
        [HttpGet("getmalienketmac/{branchid}")]
        public IActionResult GetMaLienKetMAC(int branchid)
        {
            DefaultResponse def = new DefaultResponse();
            if (branchid == null)
            {
                def.meta = new Meta(222, "Vui lòng chọn trạm");
                return Ok(def);
            }
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<MaLienKetDTO> rpMalienKet = new List<MaLienKetDTO>();
                Branch branch = context.Branch.Find(branchid);
                command.CommandText += "SELECT DISTINCT dh.MaLKMac FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] dh";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var item = (result["MaLKMac"] is DBNull) ? String.Empty : (string)result["MaLKMac"];
                        rpMalienKet.Add(new MaLienKetDTO() { MALIENKET = item });
                    }
                    def.data = rpMalienKet;
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }
    }
}
