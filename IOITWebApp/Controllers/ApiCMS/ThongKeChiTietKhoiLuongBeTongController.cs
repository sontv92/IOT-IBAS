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
using NPOI.SS.Util;
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
    public class ThongKeChiTietKhoiLuongBeTongController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("thongkechitietkhoiluongbetong", "thongkechitietkhoiluongbetong");
        private static string functionCode = "TKCTKLBT";
        private IHostingEnvironment _hostingEnvironment;
        public ThongKeChiTietKhoiLuongBeTongController(IHostingEnvironment hostingEnvironment)
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
                                    if (paging.MAPHIEU is null || paging.MAPHIEU == "undefined" || paging.MAPHIEU == "null")
                                    {
                                        paging.MAPHIEU = "";
                                    }
                                    if (paging.TENHANGMUC is null || paging.TENHANGMUC == "undefined" || paging.TENHANGMUC == "null")
                                    {
                                        paging.TENHANGMUC = "";
                                    }
                                    if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                                    {
                                        paging.TENKHACHHANG = "";
                                    }
                                    if (paging.TENDUAN is null || paging.TENDUAN == "undefined" || paging.TENDUAN == "null")
                                    {
                                        paging.TENDUAN = "";
                                    }
                                    if (paging.TAIXE is null || paging.TAIXE == "undefined" || paging.TAIXE == "null")
                                    {
                                        paging.TAIXE = "";
                                    }
                                    if (paging.BIENSO is null || paging.BIENSO == "undefined" || paging.BIENSO == "null")
                                    {
                                        paging.BIENSO = "";
                                    }
                                    if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                                    {
                                        paging.TENMACBETONG = "";
                                    }
                                    if (paging.TENNV is null || paging.TENNV == "undefined" || paging.TENNV == "null")
                                    {
                                        paging.TENNV = "";
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

                                        if (!string.IsNullOrEmpty(paging.MAPHIEU)) maPhieuCond = string.Format("A.SOPHIEU = N'{0}'", paging.MAPHIEU.Trim());
                                        if (!string.IsNullOrEmpty(paging.TENHANGMUC)) tenHangMucCond = string.Format("C.TENHANGMUC = N'{0}'", paging.TENHANGMUC);
                                        if (!string.IsNullOrEmpty(paging.TENKHACHHANG)) tenKHCond = string.Format("C.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG);
                                        if (!string.IsNullOrEmpty(paging.TENDUAN)) conTruongCond = string.Format("C.TENDUAN= N'{0}'", paging.TENDUAN);
                                        if (!string.IsNullOrEmpty(paging.TAIXE)) taiXeCond = string.Format("A.TENLAIXE = N'{0}'", paging.TAIXE);
                                        if (!string.IsNullOrEmpty(paging.BIENSO)) xeCond = string.Format("A.BIENSO = N'{0}'", paging.BIENSO);
                                        if (!string.IsNullOrEmpty(paging.TENMACBETONG)) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG);
                                        if (!string.IsNullOrEmpty(paging.TENNV)) nvkdCond = string.Format("C.TENNV = N'{0}'", paging.TENNV);
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", paging.CHEDO);
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", paging.CHEDO);

                                        //String sql = string.Format("SELECT A.SOPHIEU SOPHIEU, A.NGAYTRON NGAYTRON, C.TENKHACHHANG TENKHACHHANG, C.TENDUAN TENDUAN, A.TENMACBETONG TENMACBETONG \n" +
                                        //                            "	, C.TENNV TENNV, A.TENLAIXE TENLAIXE, A.BIENSO BIENSO, SUM(M3METRON) M3METRON, COUNT(B.SOTTMETRON) SOTTMETRON--, M3METRON \n" +
                                        //                            "	, SUM(M3METRON) / MAX(B.SOTTMETRON) M3TRENMETRON, A.MALSTRON TONGVATTU \n" +
                                        //                            "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                        //                            "   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG   \n" +
                                        //                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                        //                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                        //                            "        AND {7} AND {8} AND {9} AND {10} \n" +
                                        //                            "GROUP BY A.SOPHIEU, A.NGAYTRON, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG, C.TENNV, A.TENLAIXE, A.BIENSO, A.MALSTRON \n" +
                                        //                            "ORDER BY A.NGAYTRON \n",
                                        //                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        //                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        //                            maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                        //                            xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        string sql = string.Format("SELECT SOPHIEU, FORMAT(GIOXONG, 'HH:mm:ss dd/MM/yyy') NGAYTRON, TENKHACHHANG, TENDUAN, TENMACBETONG \n" +
                                            "		, TENNV, TENLAIXE, BIENSO, M3METRON, SOTTMETRON \n" +
                                            "		, M3TRENMETRON, TONGVATTU \n" +
                                            "FROM ( \n" +
                                            "	SELECT  A.SOPHIEU, A.GIOXONG, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG \n" +
                                            "		, C.TENNV, A.TENLAIXE, A.BIENSO, SUM(M3METRON) M3METRON, COUNT(B.SOTTMETRON) SOTTMETRON--, M3METRON  \n" +
                                            "		, SUM(M3METRON) / MAX(B.SOTTMETRON) M3TRENMETRON, A.MALSTRON \n" +
                                            "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                            "	   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG    \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                            "        AND {7} AND {8} AND {9} AND {10} \n" +
                                            "	GROUP BY A.MALSTRON, A.SOPHIEU, A.GIOXONG, A.TENMACBETONG, A.TENLAIXE, A.BIENSO, C.TENKHACHHANG, C.TENDUAN, C.TENNV \n" +
                                            ") AS A INNER JOIN ( \n" +
                                            "	SELECT A.MALSTRON, SUM(isnull(D.SOLUONGTD, 0) + isnull(D.SOLUONGTAY, 0)) TONGVATTU  \n" +
                                            "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                            "	   INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID    \n" +
                                            "	   INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT    \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}'  \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" + // các điều kiện từ 7 --> 10 là của bảng C, subquery này không có bảng C nên k cần đưa vào
                                            "	GROUP BY A.MALSTRON, A.SOPHIEU, A.GIOXONG, A.TENMACBETONG, A.TENLAIXE, A.BIENSO \n" +
                                            ") AS B ON A.MALSTRON = B.MALSTRON",
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                            maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                            xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        // command.CommandText = "select * INTO #Result FROM (";

                                        command.CommandText = sql.ToString();
                                        //command.CommandText += ") as ChiTiet; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT * FROM #Result ORDER BY STTCUAVL_MAIN OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        var dataTable = CommonLib.AsEnumerable(dtSource);

                                        List<ThongKeChiTietKhoiLuongBeTongDTO> listData = dataTable.Select(row => new ThongKeChiTietKhoiLuongBeTongDTO()
                                        {
                                            SOPHIEU = row["SOPHIEU"] != null ? (int)row["SOPHIEU"] : 0,
                                            NGAYTRON = row["NGAYTRON"] != DBNull.Value ? (string)row["NGAYTRON"] : "",
                                            TENDUAN = row["TENDUAN"] != DBNull.Value ? (string)row["TENDUAN"] : "",
                                            BIENSO = row["BIENSO"] != DBNull.Value ? (string)row["BIENSO"] : "",
                                            M3METRON = row["M3METRON"] != null ? (double)row["M3METRON"] : 0,
                                            TENKHACHHANG = row["TENKHACHHANG"] != DBNull.Value ? (string)row["TENKHACHHANG"] : "",
                                            TENLAIXE = row["TENLAIXE"] != DBNull.Value ? (string)row["TENLAIXE"] : "",
                                            TENMACBETONG = row["TENMACBETONG"] != DBNull.Value ? (string)row["TENMACBETONG"] : "",
                                            TENNV = row["TENNV"] != DBNull.Value ? (string)row["TENNV"] : "",
                                            M3TRENMETRON = (double)row["M3TRENMETRON"],
                                            SOTTMETRON = (int)row["SOTTMETRON"],
                                            TONGVATTU = (double)row["TONGVATTU"]
                                        }).ToList();

                                        List<ThongKeChiTietKhoiLuongBeTongGroupDTO> result = new List<ThongKeChiTietKhoiLuongBeTongGroupDTO>();
                                        switch (paging.GroupBy)
                                        {
                                            case "CT":
                                                var groupsCT = listData.GroupBy(x => x.TENDUAN);
                                                foreach (var itemGrp in groupsCT)
                                                {
                                                    result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                        TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            case "NV":
                                                var groupsNV = listData.GroupBy(x => x.TENNV);
                                                foreach (var itemGrp in groupsNV)
                                                {
                                                    result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                        TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            case "KH":
                                                var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                        TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            case "MA":
                                                var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                foreach (var itemGrp in groupsMAC)
                                                {
                                                    result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                        TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                    }); ;
                                                }
                                                break;
                                            default: // Mặc định group theo ngày lập
                                                var groups = listData.GroupBy(x => x.NGAYTRON);
                                                foreach (var itemGrp in groups)
                                                {
                                                    result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = itemGrp.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                        TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                        TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON),
                                                        ToTalTONGVATTU = itemGrp.Sum(x => x.TONGVATTU)
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

                                        List<ThongKeChiTietKhoiLuongBeTongDTO> lstResult = new List<ThongKeChiTietKhoiLuongBeTongDTO>();
                                        if (paging.MAPHIEU is null || paging.MAPHIEU == "undefined" || paging.MAPHIEU == "null")
                                        {
                                            paging.MAPHIEU = "";
                                        }
                                        if (paging.TENHANGMUC is null || paging.TENHANGMUC == "undefined" || paging.TENHANGMUC == "null")
                                        {
                                            paging.TENHANGMUC = "";
                                        }
                                        if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                                        {
                                            paging.TENKHACHHANG = "";
                                        }
                                        if (paging.TENDUAN is null || paging.TENDUAN == "undefined" || paging.TENDUAN == "null")
                                        {
                                            paging.TENDUAN = "";
                                        }
                                        if (paging.TAIXE is null || paging.TAIXE == "undefined" || paging.TAIXE == "null")
                                        {
                                            paging.TAIXE = "";
                                        }
                                        if (paging.BIENSO is null || paging.BIENSO == "undefined" || paging.BIENSO == "null")
                                        {
                                            paging.BIENSO = "";
                                        }
                                        if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                                        {
                                            paging.TENMACBETONG = "";
                                        }
                                        if (paging.TENNV is null || paging.TENNV == "undefined" || paging.TENNV == "null")
                                        {
                                            paging.TENNV = "";
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


                                        string maPhieuCond, conTruongCond, taiXeCond, tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        maPhieuCond = conTruongCond = taiXeCond = tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";

                                        if (!string.IsNullOrEmpty(paging.MAPHIEU)) maPhieuCond = string.Format("A.SOPHIEU = N'{0}'", paging.MAPHIEU.Trim());
                                        if (!string.IsNullOrEmpty(paging.TENHANGMUC)) tenHangMucCond = string.Format("C.TENHANGMUC = N'{0}'", paging.TENHANGMUC);
                                        if (!string.IsNullOrEmpty(paging.TENKHACHHANG)) tenKHCond = string.Format("C.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG);
                                        if (!string.IsNullOrEmpty(paging.TENDUAN)) conTruongCond = string.Format("C.TENDUAN= N'{0}'", paging.TENDUAN);
                                        if (!string.IsNullOrEmpty(paging.TAIXE)) taiXeCond = string.Format("A.TENLAIXE = N'{0}'", paging.TAIXE);
                                        if (!string.IsNullOrEmpty(paging.BIENSO)) xeCond = string.Format("A.BIENSO = N'{0}'", paging.BIENSO);
                                        if (!string.IsNullOrEmpty(paging.TENMACBETONG)) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG);
                                        if (!string.IsNullOrEmpty(paging.TENNV)) nvkdCond = string.Format("C.TENNV = N'{0}'", paging.TENNV);
                                        if (paging.CHEDO.Equals("Normal")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");

                                        //String sql = string.Format("SELECT A.SOPHIEU SOPHIEU, FORMAT(A.GIOXONG, 'HH:mm:ss dd/MM/yyy') NGAYTRON, C.TENKHACHHANG TENKHACHHANG, C.TENDUAN TENDUAN, A.TENMACBETONG TENMACBETONG \n" +
                                        //                           "	, C.TENNV TENNV, A.TENLAIXE TENLAIXE, A.BIENSO BIENSO, SUM(M3METRON) M3METRON, COUNT(B.SOTTMETRON) SOTTMETRON--, M3METRON \n" +
                                        //                           "	, SUM(M3METRON) / MAX(B.SOTTMETRON) M3TRENMETRON \n" +
                                        //                           "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                        //                           "   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG   \n" +
                                        //                           "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                        //                           "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                        //                           "        AND {7} AND {8} AND {9} AND {10} \n" +
                                        //                           "GROUP BY A.SOPHIEU, A.GIOXONG, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG, C.TENNV, A.TENLAIXE, A.BIENSO \n" +
                                        //                           "ORDER BY A.GIOXONG \n",
                                        //                           CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        //                           CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                        //                           maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                        //                           xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        string sql = string.Format("SELECT SOPHIEU, FORMAT(GIOXONG, 'HH:mm:ss dd/MM/yyy') NGAYTRON, TENKHACHHANG, TENDUAN, TENMACBETONG \n" +
                                            "		, TENNV, TENLAIXE, BIENSO, M3METRON, SOTTMETRON \n" +
                                            "		, M3TRENMETRON, TONGVATTU \n" +
                                            "FROM ( \n" +
                                            "	SELECT  A.SOPHIEU, A.GIOXONG, C.TENKHACHHANG, C.TENDUAN, A.TENMACBETONG \n" +
                                            "		, C.TENNV, A.TENLAIXE, A.BIENSO, SUM(M3METRON) M3METRON, COUNT(B.SOTTMETRON) SOTTMETRON--, M3METRON  \n" +
                                            "		, SUM(M3METRON) / MAX(B.SOTTMETRON) M3TRENMETRON, A.MALSTRON \n" +
                                            "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                            "	   LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG C ON C.STT = A.STTLSDATHANG    \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" +
                                            "        AND {7} AND {8} AND {9} AND {10} \n" +
                                            "	GROUP BY A.MALSTRON, A.SOPHIEU, A.GIOXONG, A.TENMACBETONG, A.TENLAIXE, A.BIENSO, C.TENKHACHHANG, C.TENDUAN, C.TENNV \n" +
                                            ") AS A INNER JOIN ( \n" +
                                            "	SELECT A.MALSTRON, SUM(isnull(D.SOLUONGTD, 0) + isnull(D.SOLUONGTAY, 0)) TONGVATTU  \n" +
                                            "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON    \n" +
                                            "	   INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID    \n" +
                                            "	   INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT    \n" +
                                            "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}'  \n" +
                                            "        AND {2} AND {3} AND {4} AND {5} AND {6}  \n" + // các điều kiện từ 7 --> 10 là của bảng C, subquery này không có bảng C nên k cần đưa vào
                                            "	GROUP BY A.MALSTRON, A.SOPHIEU, A.GIOXONG, A.TENMACBETONG, A.TENLAIXE, A.BIENSO \n" +
                                            ") AS B ON A.MALSTRON = B.MALSTRON",
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                            maPhieuCond, tenHangMucCond, tenKHCond, conTruongCond, taiXeCond,
                                            xeCond, tenMacBeTongCond, nvkdCond, cheDo);

                                        DataTable dtSource = CommonLib.GetDataBySql(sql);

                                        List<ThongKeChiTietKhoiLuongBeTongGroupDTO> result = new List<ThongKeChiTietKhoiLuongBeTongGroupDTO>();
                                        if (dtSource.Rows.Count > 0)
                                        {
                                            var dataTable = CommonLib.AsEnumerable(dtSource);

                                            List<ThongKeChiTietKhoiLuongBeTongDTO> listData = dataTable.Select(row => new ThongKeChiTietKhoiLuongBeTongDTO()
                                            {
                                                SOPHIEU = row["SOPHIEU"] != null ? (int)row["SOPHIEU"] : 0,
                                                 NGAYTRON = row["NGAYTRON"] != DBNull.Value ? (string)row["NGAYTRON"] : "",
                                                TENDUAN = row["TENDUAN"] != DBNull.Value ? (string)row["TENDUAN"] : "",
                                                BIENSO = row["BIENSO"] != DBNull.Value ? (string)row["BIENSO"] : "",
                                                M3METRON = row["M3METRON"] != null ? (double)row["M3METRON"] : 0,
                                                TENKHACHHANG = row["TENKHACHHANG"] != DBNull.Value ? (string)row["TENKHACHHANG"] : "",
                                                TENLAIXE = row["TENLAIXE"] != DBNull.Value ? (string)row["TENLAIXE"] : "",
                                                TENMACBETONG = row["TENMACBETONG"] != DBNull.Value ? (string)row["TENMACBETONG"] : "",
                                                TENNV = row["TENNV"] != DBNull.Value ? (string)row["TENNV"] : "",
                                                M3TRENMETRON = (double)row["M3TRENMETRON"],
                                                SOTTMETRON = (int)row["SOTTMETRON"],
                                            }).ToList();

                                            var groupName = string.Empty;
                                            switch (paging.GroupBy)
                                            {
                                                case "CT":
                                                    var groupsCT = listData.GroupBy(x => x.TENDUAN);
                                                    foreach (var itemGrp in groupsCT)
                                                    {
                                                        result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                            TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Công trường: ";
                                                    }
                                                    break;
                                                case "NV":
                                                    var groupsNV = listData.GroupBy(x => x.TENNV);
                                                    foreach (var itemGrp in groupsNV)
                                                    {
                                                        result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                            TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Nhân viên: ";
                                                    }
                                                    break;
                                                case "KH":
                                                    var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                    foreach (var itemGrp in groupsKH)
                                                    {
                                                        result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                            TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Khách hàng: ";
                                                    }
                                                    break;
                                                case "MA":
                                                    var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                    foreach (var itemGrp in groupsMAC)
                                                    {
                                                        result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                            TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Mác: ";
                                                    }
                                                    break;
                                                default: // Mặc định group theo ngày lập
                                                    var groups = listData.GroupBy(x => x.NGAYTRON.Substring(9));
                                                    foreach (var itemGrp in groups)
                                                    {
                                                        result.Add(new ThongKeChiTietKhoiLuongBeTongGroupDTO()
                                                        {
                                                            Key = itemGrp.Key,
                                                            Data = itemGrp.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                            TotalM3TRENMETRON = itemGrp.Sum(x => x.M3TRENMETRON),
                                                            TotalSOTTMETRON = itemGrp.Sum(x => x.SOTTMETRON)
                                                        });
                                                        groupName = "Ngày lập: ";
                                                    }
                                                    break;
                                            }

                                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                            using (var package = new ExcelPackage())
                                            {
                                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê chi tiết khối lượng bê tông");
                                                worksheet.Cells["A1:K1"].Merge = true;
                                                worksheet.Cells["A1:K1"].Value = "THỐNG KÊ CHI TIẾT KHỐI LƯỢNG BÊ TÔNG";
                                                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                                                worksheet.Cells["A1:K1"].Style.Font.Size = 16;
                                                worksheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A1:K1"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                worksheet.Cells["A2:K2"].Merge = true;
                                                worksheet.Cells["A2:K2"].Value = "Báo cáo được tạo vào ngày " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                                                worksheet.Cells["A2:K2"].Style.Font.Italic = true;
                                                worksheet.Cells["A2:K2"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A2:K2"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A2:K2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                //Điều kiện lọc
                                                worksheet.Cells["A3:K3"].Merge = true;
                                                worksheet.Cells["A3:K3"].Value = "* Điều kiện:";
                                                worksheet.Cells["A3:K3"].Style.Font.Italic = true;
                                                worksheet.Cells["A3:K3"].Style.Font.Bold = true;
                                                worksheet.Cells["A3:K3"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A3:K3"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A3:K3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                #region Điều kiện
                                                //mã phiếu
                                                worksheet.Cells["A4:K4"].Merge = true;
                                                var valueFilterMaPhieu = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.MAPHIEU))
                                                {
                                                    valueFilterMaPhieu = paging.MAPHIEU;
                                                }
                                                worksheet.Cells["A4:K4"].Value = "- Mã phiếu: " + valueFilterMaPhieu;
                                                worksheet.Cells["A4:K4"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A4:K4"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A4:K4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Hạng mục
                                                worksheet.Cells["A5:K5"].Merge = true;
                                                var valueFilterHangmuc = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENHANGMUC))
                                                {
                                                    valueFilterHangmuc = paging.TENHANGMUC;
                                                }
                                                worksheet.Cells["A5:K5"].Value = "- Hạng mục: " + valueFilterHangmuc;
                                                worksheet.Cells["A5:K5"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A5:K5"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A5:K5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Khách hàng
                                                worksheet.Cells["A6:K6"].Merge = true;
                                                var valueFilterKhachhang = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENKHACHHANG))
                                                {
                                                    valueFilterKhachhang = paging.TENKHACHHANG;
                                                }
                                                worksheet.Cells["A6:K6"].Value = "- Khách hàng: " + valueFilterKhachhang;
                                                worksheet.Cells["A6:K6"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A6:K6"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A6:K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Dự án
                                                worksheet.Cells["A7:K7"].Merge = true;
                                                var valueFilterDuan = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENDUAN))
                                                {
                                                    valueFilterDuan = paging.MAPHIEU;
                                                }
                                                worksheet.Cells["A7:K7"].Value = "- Công trường: " + valueFilterDuan;
                                                worksheet.Cells["A7:K7"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A7:K7"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A7:K7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Tài xế
                                                worksheet.Cells["A8:K8"].Merge = true;
                                                var valueFilterTx = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TAIXE))
                                                {
                                                    valueFilterTx = paging.TAIXE;
                                                }
                                                worksheet.Cells["A8:K8"].Value = "- Tài xế: " + valueFilterTx;
                                                worksheet.Cells["A8:K8"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A8:K8"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A8:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Biển số
                                                worksheet.Cells["A9:K9"].Merge = true;
                                                var valueFilterBs = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.BIENSO))
                                                {
                                                    valueFilterBs = paging.BIENSO;
                                                }
                                                worksheet.Cells["A9:K9"].Value = "- Biển số: " + valueFilterBs;
                                                worksheet.Cells["A9:K9"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A9:K9"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A9:K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Tên mác be tông
                                                worksheet.Cells["A10:K10"].Merge = true;
                                                var valueFilterMac = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENMACBETONG))
                                                {
                                                    valueFilterMac = paging.TENMACBETONG;
                                                }
                                                worksheet.Cells["A10:K10"].Value = "- Tên mác bê tông: " + valueFilterMac;
                                                worksheet.Cells["A10:K10"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A10:K10"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A10:K10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Nhân viên
                                                worksheet.Cells["A11:K11"].Merge = true;
                                                var valueFilterNV = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.TENNV))
                                                {
                                                    valueFilterNV = paging.TENNV;
                                                }
                                                worksheet.Cells["A11:K11"].Value = "- Nhân viên: " + valueFilterNV;
                                                worksheet.Cells["A11:K11"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A11:K11"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A11:K11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                // Chế độ
                                                worksheet.Cells["A12:K12"].Merge = true;
                                                var valueFilterCheDo = "Tất cả";
                                                if (!string.IsNullOrEmpty(paging.CHEDO))
                                                {
                                                    valueFilterCheDo = paging.CHEDO;
                                                }
                                                worksheet.Cells["A12:K12"].Value = "- Chế độ: " + valueFilterCheDo;
                                                worksheet.Cells["A12:K12"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A12:K12"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A12:K12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                #endregion

                                                #region Nhóm theo
                                                //Nhóm theo
                                                worksheet.Cells["A13:K13"].Merge = true;
                                                worksheet.Cells["A13:K13"].Value = "* Nhóm theo: " + groupName.Substring(0, groupName.IndexOf(":"));
                                                worksheet.Cells["A13:K13"].Style.Font.Italic = true;
                                                worksheet.Cells["A13:K13"].Style.Font.Bold = true;
                                                worksheet.Cells["A13:K13"].Style.Font.Color.SetColor(Color.Black);
                                                worksheet.Cells["A13:K13"].Style.HorizontalAlignment =
                                                    ExcelHorizontalAlignment.Left;
                                                worksheet.Cells["A13:K13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                #endregion

                                                worksheet.Cells["A15"].Value = "Phiếu";
                                                worksheet.Cells["A15"].Style.Font.Bold = true;

                                                worksheet.Cells["B15"].Value = "Ngày lập";
                                                worksheet.Cells["B15"].Style.Font.Bold = true;

                                                worksheet.Cells["C15"].Value = "Khách hàng";
                                                worksheet.Cells["C15"].Style.Font.Bold = true;

                                                worksheet.Cells["D15"].Value = "Công trường";
                                                worksheet.Cells["D15"].Style.Font.Bold = true;

                                                worksheet.Cells["E15"].Value = "Mác";
                                                worksheet.Cells["E15"].Style.Font.Bold = true;

                                                worksheet.Cells["F15"].Value = "Nhân viên";
                                                worksheet.Cells["F15"].Style.Font.Bold = true;

                                                worksheet.Cells["G15"].Value = "Tài xế";
                                                worksheet.Cells["G15"].Style.Font.Bold = true;

                                                worksheet.Cells["H15"].Value = "Xe";
                                                worksheet.Cells["H15"].Style.Font.Bold = true;

                                                worksheet.Cells["I15"].Value = "Tổng khối lượng";
                                                worksheet.Cells["I15"].Style.Font.Bold = true;

                                                worksheet.Cells["J15"].Value = "Số mẻ";
                                                worksheet.Cells["J15"].Style.Font.Bold = true;

                                                worksheet.Cells["K15"].Value = "M3/Mẻ";
                                                worksheet.Cells["K15"].Style.Font.Bold = true;



                                                worksheet.Cells["A15:K15"].Style.Font.Color.SetColor(Color.White);
                                                worksheet.Cells["A15:K15"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                worksheet.Cells["A15:K15"].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                                if (result.Count() > 0)
                                                {
                                                    int rowFirts = 16;
                                                    for (int i = 0; i < result.Count(); i++)
                                                    {
                                                        var elementGroup = result.ElementAt(i);
                                                        //Row merge
                                                        var region = "A" + rowFirts + ":" + "K" + rowFirts;
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
                                                            //Phiếu
                                                            worksheet.Cells[row, column].Value = element?.SOPHIEU;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Ngày lập
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.NGAYTRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "dd/mm/yyyy";

                                                            //Khách hàng
                                                            column++;
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

                                                            //Nhân viên
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENNV;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Tài xế
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENLAIXE;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Xe
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.BIENSO;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Tổng khối lượng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.M3METRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                            //Số mẻ
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.SOTTMETRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                            //M3/Mẻ
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.M3TRENMETRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "#,##";

                                                        }
                                                        rowFirts = rowFirts + elementGroup.Data.Count();

                                                        //Row merge total
                                                        region = "A" + rowFirts + ":" + "H" + rowFirts;
                                                        worksheet.Cells[region].Merge = true;
                                                        worksheet.Cells[region].Value = "Tổng:";
                                                        worksheet.Cells[region].Style.Font.Italic = true;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Center;
                                                        worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        region = "I" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalM3METRON;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        region = "J" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalSOTTMETRON;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        region = "K" + rowFirts;
                                                        worksheet.Cells[region].Value = elementGroup.TotalM3TRENMETRON;
                                                        worksheet.Cells[region].Style.Font.Bold = true;
                                                        worksheet.Cells[region].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;
                                                        worksheet.Cells[region].Style.Numberformat.Format = "#,##";

                                                        rowFirts++;
                                                    }
                                                    string modelRange = "A16:K" + (listData.Count() + result.Count() * 2 + 16);
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
