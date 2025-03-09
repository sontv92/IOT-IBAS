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
    public class ThongKeChiTietVatTuController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("thongkechitietvattu", "thongkechitietvattu");
        private static string functionCode = "TKCTVT";
        private IHostingEnvironment _hostingEnvironment;
        public ThongKeChiTietVatTuController(IHostingEnvironment hostingEnvironment)
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
                    List<ThongKeChiTietVatTuDTO> xe = new List<ThongKeChiTietVatTuDTO>();
                    List<ThongKeChiTietVatTuTongHopDTO> lstxe = new List<ThongKeChiTietVatTuTongHopDTO>();
                    List<ThongKeChiTietVatTuTongHopDTO> lstTong = new List<ThongKeChiTietVatTuTongHopDTO>();
                    if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                    {
                        paging.TENKHACHHANG = "";
                    }
                    if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                    {
                        paging.TENMACBETONG = "";
                    }
                    if (paging.Branchlist is null || paging.Branchlist == "undefined" || paging.Branchlist == "null")
                    {
                        paging.Branchlist = "";
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

                                        var listDeActive = new List<ListCuaVLDeactive>();
                                        _TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL, B.TRANGTHAI FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");
                                        if (_TableCuaVL != null && _TableCuaVL.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in _TableCuaVL.Rows)
                                            {
                                                string tenCuaVL = CommonLib.ConvertToString(row["TENCUAVL"].ToString());
                                                string maCuaVL = CommonLib.ConvertToString(row["STTCUAVL"].ToString());

                                                var isActive = CommonLib.ConvertToBool(row["TRANGTHAI"].ToString());
                                                if (!isActive)
                                                {
                                                    var listCuaVLDeactive = new ListCuaVLDeactive()
                                                    {
                                                        STTCUAVL = maCuaVL,
                                                        TENCUAVL = tenCuaVL
                                                    };
                                                    listDeActive.Add(listCuaVLDeactive);
                                                }

                                                bool cophaiPhuGia = CommonLib.ConvertToBool(row["COPHAIPHUGIA"].ToString());
                                                if (maCuaVL.Trim() != "")
                                                {
                                                    if (maCuaVL.Trim() != "")
                                                    {
                                                        // dạng p.[Sand 1], p.[Sand 2], p.[Stone 1], p.[Stone 2], p.[Cement 1], p.[Cement 2], p.[Cement 3], p.[Cement 4], p.[Water], p.[Adm 1], p.[Adm 2]
                                                        cuaVLSOLUONG1 += string.Format("ISNULL(p.[{0}],0) [{0}], ", maCuaVL);
                                                        // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                        cuaVLSOLUONG2 += string.Format("[{0}], ", maCuaVL);

                                                        //dạng , D.[CP_Cát 1], B.[Cát 1], C.[T_Cát 1], (B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) N'Sai số_Cát 1', abs(B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) / nullif(D.[CP_Cát 1], 0) * 100 '%_Cát 1'
                                                        if (cophaiPhuGia)
                                                        {
                                                            selectByCuaVL += string.Format(", ISNULL(round(B.[{0}], 2),0) N'{1}' \n", maCuaVL, tenCuaVL);
                                                        }
                                                        else
                                                        {
                                                            selectByCuaVL += string.Format(", ISNULL(round(B.[{0}], 0),0) N'{1}' \n", maCuaVL, tenCuaVL);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (cuaVLSOLUONG1.EndsWith(", ")) cuaVLSOLUONG1 = cuaVLSOLUONG1.Substring(0, cuaVLSOLUONG1.Length - 2);
                                        if (cuaVLSOLUONG2.EndsWith(", ")) cuaVLSOLUONG2 = cuaVLSOLUONG2.Substring(0, cuaVLSOLUONG2.Length - 2);

                                        string tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";


                                        if (!paging.TENKHACHHANG.Equals("")) tenKHCond = string.Format("H.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG.ToString());
                                        if (!paging.TENMACBETONG.Equals("")) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG.ToString());
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");


                                        String subQuerySumSOLUONG = string.Format("	SELECT [MACHITIETMETRON], {5} \n" +
                                                                                    "		FROM ( \n" +
                                                                                    "		SELECT DISTINCT B.MACHITIETMETRON, D.STTCUAVL STTCUAVL\n" +
                                                                                    "			, (ISNULL(D.SOLUONGTD,0) + ISNULL(D.SOLUONGTAY,0)) SUMSOLUONG \n" +
                                                                                    "		FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                                                    "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID   \n" +
                                                                                    "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT   \n" +
                                                                                    "		WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                                    "           AND {2} AND {3} AND {4} \n" +
                                                                                    "	) AS j  \n" +
                                                                                    "	PIVOT (SUM(SUMSOLUONG) FOR [STTCUAVL] in ({6})) AS p \n",
                                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                                    tenKHCond, tenMacBeTongCond, cheDo,
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
                                                        " {6} \n" +
                                                        "FROM ( \n" +
                                                        "	SELECT DISTINCT B.MACHITIETMETRON MACHITIETMETRON_MAIN, B.MALSTRON N'Mã phiếu' \n" +
                                                        "		, FORMAT(B.GIOKT, 'HH:mm:ss dd/MM/yyy') N'Ngày trộn' \n" +
                                                        "		, H.TENKHACHHANG N'Khách hàng', A.TENMACBETONG N'Tên mác BT', round(B.M3METRON, 2) N'Khối lượng' \n" +
                                                        "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                        "		LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                                        "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                        "       AND {2} AND {3} AND {4} \n" +
                                                        ") AS A LEFT JOIN ( \n" +
                                                        "	 {5}\n" +
                                                        ") AS B ON A.MACHITIETMETRON_MAIN = B.MACHITIETMETRON \n",
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                        tenKHCond, tenMacBeTongCond, cheDo,
                                                        subQuerySumSOLUONG,
                                                        selectByCuaVL);



                                        command.CommandText += sql.ToString();
                                        command.CommandText += ") as ChiTiet; SELECT COUNT(*) AS COUNTS FROM #Result ; SELECT *  FROM #Result ORDER BY MACHITIETMETRON_MAIN  OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY; DROP TABLE #Result;";
                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        dtSource.Columns.Remove("MACHITIETMETRON_MAIN");
                                        foreach (var cuaVL in listDeActive)
                                        {
                                            dtSource.Columns.Remove(cuaVL.TENCUAVL);
                                        }
                                        var dataTable = CommonLib.AsEnumerable(dtSource);
                                        List<ThongKeChiTietVatTuGridDTO> listData = new List<ThongKeChiTietVatTuGridDTO>();
                                        foreach (var dataItem in dataTable)
                                        {
                                            var data = new ThongKeChiTietVatTuGridDTO()
                                            {
                                                STT = int.Parse(string.Format("{0}", dataItem.ItemArray[0])),
                                                MAPHIEU = dataItem.ItemArray[1].ToString(),
                                                NGAYTRON = DateTime.ParseExact(dataItem.ItemArray[2].ToString(), "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                TENKHACHHANG = dataItem.ItemArray[3].ToString(),
                                                TENMACBETONG = dataItem.ItemArray[4].ToString(),
                                                M3METRON = (double)dataItem.ItemArray[5],

                                            };
                                            data.CoulumnVL = new List<double>();
                                            for (var col = 6; col < dtSource.Columns.Count; col++)
                                            {
                                                data.CoulumnVL.Add((double)dataItem.ItemArray[col]);
                                            }
                                            listData.Add(data);
                                        }
                                        listData = listData.OrderBy(x => x.TENKHACHHANG).ToList();
                                        List<ThongKeChiTietVatTuGroupGridDTO> result = new List<ThongKeChiTietVatTuGroupGridDTO>();
                                        switch (paging.GroupBy)
                                        {
                                            case "KH":
                                                var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    var groupsMACKH = itemGrp.GroupBy(x => x.TENMACBETONG);

                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACKH)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                }
                                                break;
                                            case "MA":
                                                var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                foreach (var itemGrp in groupsMAC)
                                                {
                                                    var groupsKHChild = itemGrp.GroupBy(x => x.TENKHACHHANG);

                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemKH in groupsKHChild)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemKH.Key,
                                                            Data = itemKH.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemKH.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemKH.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                }
                                                break;
                                            case "NT":
                                                var groupsNT = listData.GroupBy(x => x.NGAYTRON.ToString("dd/MM/yyyy"));
                                                foreach (var itemGrp in groupsNT)
                                                {
                                                    var groupsMACNT = itemGrp.GroupBy(x => x.TENMACBETONG);
                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACNT)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                }
                                                break;
                                            default: // Mặc định group theo khách hàng
                                                groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    var groupsMACDF = itemGrp.GroupBy(x => x.TENMACBETONG);
                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACDF)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                }
                                                break;
                                        }
                                        def.ColumnNames = new List<ColumnName>();
                                        for (int j = 0; j <= 5; j++)
                                        {
                                            var columnName = new ColumnName()
                                            {
                                                Name = dtSource.Columns[j].ColumnName,
                                                IsActive = true
                                            };
                                            def.ColumnNames.Add(columnName);
                                        }

                                        foreach (DataRow row in _TableCuaVL.Rows)
                                        {
                                            var columnName = new ColumnName()
                                            {
                                                Name = CommonLib.ConvertToString(row["TENCUAVL"].ToString()),
                                                IsActive = CommonLib.ConvertToBool(row["TRANGTHAI"].ToString())
                                            };
                                            def.ColumnNames.Add(columnName);
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

                if (paging.TENKHACHHANG is null || paging.TENKHACHHANG == "undefined" || paging.TENKHACHHANG == "null")
                {
                    paging.TENKHACHHANG = "";
                }
                if (paging.TENMACBETONG is null || paging.TENMACBETONG == "undefined" || paging.TENMACBETONG == "null")
                {
                    paging.TENMACBETONG = "";
                }
                if (paging.Branchlist is null || paging.Branchlist == "undefined" || paging.Branchlist == "null")
                {
                    paging.Branchlist = "";
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
                                        Company company = context.Company.Where(c => c.CompanyId == Convert.ToInt32(branch.CompanyId)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                        _TableCuaVL = CommonLib.GetDataBySql("SELECT B.TENCUAVL, A.TENLOAIVL, A.COPHAIPHUGIA, B.STTCUAVL, B.TRANGTHAI FROM [" + branch.Dataname + "].[dbo].LOAIVL A INNER JOIN [" + branch.Dataname + "].[dbo].CUAVL B ON A.MALOAIVL = B.MALOAIVL ORDER BY B.STTCUAVL");
                                        var listDeActive = new List<ListCuaVLDeactive>();
                                        if (_TableCuaVL != null && _TableCuaVL.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in _TableCuaVL.Rows)
                                            {
                                                string tenCuaVL = CommonLib.ConvertToString(row["TENCUAVL"].ToString());
                                                string maCuaVL = CommonLib.ConvertToString(row["STTCUAVL"].ToString());

                                                var isActive = CommonLib.ConvertToBool(row["TRANGTHAI"].ToString());
                                                if (!isActive)
                                                {
                                                    var listCuaVLDeactive = new ListCuaVLDeactive()
                                                    {
                                                        STTCUAVL = maCuaVL,
                                                        TENCUAVL = tenCuaVL
                                                    };
                                                    listDeActive.Add(listCuaVLDeactive);
                                                }

                                                bool cophaiPhuGia = CommonLib.ConvertToBool(row["COPHAIPHUGIA"].ToString());
                                                if (maCuaVL.Trim() != "")
                                                {
                                                    // dạng p.[Sand 1], p.[Sand 2], p.[Stone 1], p.[Stone 2], p.[Cement 1], p.[Cement 2], p.[Cement 3], p.[Cement 4], p.[Water], p.[Adm 1], p.[Adm 2]
                                                    cuaVLSOLUONG1 += string.Format("ISNULL(p.[{0}],0) [{0}], ", maCuaVL);
                                                    // dạng [Sand 1],[Sand 2],[Stone 1],[Stone 2],[Cement 1],[Cement 2],[Cement 3],[Cement 4],[Water],[Adm 1],[Adm 2]
                                                    cuaVLSOLUONG2 += string.Format("[{0}], ", maCuaVL);

                                                    //dạng , D.[CP_Cát 1], B.[Cát 1], C.[T_Cát 1], (B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) N'Sai số_Cát 1', abs(B.[Cát 1] + C.[T_Cát 1] - D.[CP_Cát 1]) / nullif(D.[CP_Cát 1], 0) * 100 '%_Cát 1'
                                                    if (cophaiPhuGia)
                                                    {
                                                        selectByCuaVL += string.Format(", ISNULL(round(B.[{0}], 2),0) N'{1}' \n", maCuaVL, tenCuaVL);
                                                    }
                                                    else
                                                    {
                                                        selectByCuaVL += string.Format(", ISNULL(round(B.[{0}], 0),0) N'{1}' \n", maCuaVL, tenCuaVL);
                                                    }
                                                }
                                            }
                                        }
                                        if (cuaVLSOLUONG1.EndsWith(", ")) cuaVLSOLUONG1 = cuaVLSOLUONG1.Substring(0, cuaVLSOLUONG1.Length - 2);
                                        if (cuaVLSOLUONG2.EndsWith(", ")) cuaVLSOLUONG2 = cuaVLSOLUONG2.Substring(0, cuaVLSOLUONG2.Length - 2);

                                        string tenKHCond, xeCond, tenMacBeTongCond, tenHangMucCond, nvkdCond, cheDo;
                                        tenKHCond = xeCond = tenMacBeTongCond = tenHangMucCond = nvkdCond = cheDo = "1=1";


                                        if (!paging.TENKHACHHANG.Equals("")) tenKHCond = string.Format("H.TENKHACHHANG = N'{0}'", paging.TENKHACHHANG.ToString());
                                        if (!paging.TENMACBETONG.Equals("")) tenMacBeTongCond = string.Format("A.TENMACBETONG = N'{0}'", paging.TENMACBETONG.ToString());
                                        if (paging.CHEDO.Equals("NORMAL")) cheDo = string.Format("A.CHEDO = N'{0}'", "NORMAL");
                                        if (paging.CHEDO.Equals("SIM")) cheDo = string.Format("A.CHEDO = N'{0}'", "SIM");


                                        String subQuerySumSOLUONG = string.Format("	SELECT [MACHITIETMETRON], {5} \n" +
                                                                                                                            "		FROM ( \n" +
                                                                                                                            "		SELECT DISTINCT B.MACHITIETMETRON, D.STTCUAVL STTCUAVL\n" +
                                                                                                                            "			, (ISNULL(D.SOLUONGTD,0) + ISNULL(D.SOLUONGTAY,0)) SUMSOLUONG \n" +
                                                                                                                            "		FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                                                                                            "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID   \n" +
                                                                                                                            "			INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT   \n" +
                                                                                                                            "		WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                                                                                            "           AND {2} AND {3} AND {4} \n" +
                                                                                                                            "	) AS j  \n" +
                                                                                                                            "	PIVOT (SUM(SUMSOLUONG) FOR [STTCUAVL] in ({6})) AS p \n",
                                                                                                                            CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                                                                            CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                                                                                            tenKHCond, tenMacBeTongCond, cheDo,
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
                                                        " {6} \n" +
                                                        "FROM ( \n" +
                                                        "	SELECT DISTINCT B.MACHITIETMETRON MACHITIETMETRON_MAIN, B.MALSTRON N'Mã phiếu' \n" +
                                                        "		, FORMAT(B.GIOKT, 'HH:mm:ss dd/MM/yyy') N'Ngày trộn' \n" +
                                                        "		, H.TENKHACHHANG N'Khách hàng', A.TENMACBETONG N'Tên mác BT', round(B.M3METRON, 2) N'Khối lượng' \n" +
                                                        "	FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON   \n" +
                                                        "		LEFT JOIN [" + branch.Dataname + "].[dbo].LSDATHANG H ON H.STT = A.STTLSDATHANG   \n" +
                                                        "	WHERE '{0}' <= A.GIOBATDAU AND A.GIOXONG <= '{1}' \n" +
                                                        "       AND {2} AND {3} AND {4} \n" +
                                                        ") AS A LEFT JOIN ( \n" +
                                                        "	 {5}\n" +
                                                        ") AS B ON A.MACHITIETMETRON_MAIN = B.MACHITIETMETRON \n",
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc),
                                                        tenKHCond, tenMacBeTongCond, cheDo,
                                                        subQuerySumSOLUONG,
                                                        selectByCuaVL);
                                        DataTable dtSource = CommonLib.GetDataBySql(sql);
                                        dtSource.Columns.Remove("MACHITIETMETRON_MAIN");
                                        foreach (var cuaVL in listDeActive)
                                        {
                                            dtSource.Columns.Remove(cuaVL.TENCUAVL);
                                        }
                                        var dataTable = CommonLib.AsEnumerable(dtSource);
                                        List<ThongKeChiTietVatTuGridDTO> listData = new List<ThongKeChiTietVatTuGridDTO>();
                                        foreach (var dataItem in dataTable)
                                        {
                                            var data = new ThongKeChiTietVatTuGridDTO()
                                            {
                                                STT = int.Parse(string.Format("{0}", dataItem.ItemArray[0])),
                                                MAPHIEU = dataItem.ItemArray[1].ToString(),
                                                NGAYTRON = DateTime.ParseExact(dataItem.ItemArray[2].ToString(), "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                TENKHACHHANG = dataItem.ItemArray[3].ToString(),
                                                TENMACBETONG = dataItem.ItemArray[4].ToString(),
                                                M3METRON = (double)dataItem.ItemArray[5],

                                            };
                                            data.CoulumnVL = new List<double>();
                                            for (var col = 6; col < dtSource.Columns.Count; col++)
                                            {
                                                data.CoulumnVL.Add((double)dataItem.ItemArray[col]);
                                            }
                                            listData.Add(data);
                                        }
                                        listData = listData.OrderBy(x => x.TENKHACHHANG).ToList();
                                        List<ThongKeChiTietVatTuGroupGridDTO> result = new List<ThongKeChiTietVatTuGroupGridDTO>();
                                        var groupName = string.Empty;
                                        switch (paging.GroupBy)
                                        {
                                            case "KH":
                                                var groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    var groupsMACKH = itemGrp.GroupBy(x => x.TENMACBETONG);

                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACKH)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                    groupName = "Khách hàng: ";
                                                }
                                                break;
                                            case "MA":
                                                var groupsMAC = listData.GroupBy(x => x.TENMACBETONG);
                                                foreach (var itemGrp in groupsMAC)
                                                {
                                                    var groupsKHChild = itemGrp.GroupBy(x => x.TENKHACHHANG);

                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemKH in groupsKHChild)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemKH.Key,
                                                            Data = itemKH.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemKH.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemKH.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                    groupName = "Mác bê tông: ";
                                                }
                                                break;
                                            case "NT":
                                                var groupsNT = listData.GroupBy(x => x.NGAYTRON.ToString("dd/MM/yyyy"));
                                                foreach (var itemGrp in groupsNT)
                                                {
                                                    var groupsMACNT = itemGrp.GroupBy(x => x.TENMACBETONG);
                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACNT)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                    groupName = "Ngày trộn: ";
                                                }
                                                break;
                                            default: // Mặc định group theo khách hàng
                                                groupsKH = listData.GroupBy(x => x.TENKHACHHANG);
                                                foreach (var itemGrp in groupsKH)
                                                {
                                                    var groupsMACDF = itemGrp.GroupBy(x => x.TENMACBETONG);
                                                    List<ThongKeChiTietVatTuGroupGridChildDTO> resultChild = new List<ThongKeChiTietVatTuGroupGridChildDTO>();
                                                    foreach (var itemMac in groupsMACDF)
                                                    {
                                                        var dataChild = new ThongKeChiTietVatTuGroupGridChildDTO()
                                                        {
                                                            Key = itemMac.Key,
                                                            Data = itemMac.ToList(),
                                                            Expanded = false,
                                                            TotalM3METRON = itemMac.Sum(x => x.M3METRON),
                                                        };
                                                        dataChild.TotalColumnVL = new List<double>();

                                                        for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                        {
                                                            dataChild.TotalColumnVL.Add(itemMac.Sum(p => p.CoulumnVL[x]));
                                                        }
                                                        resultChild.Add(dataChild);
                                                    }

                                                    var data = new ThongKeChiTietVatTuGroupGridDTO()
                                                    {
                                                        Key = itemGrp.Key,
                                                        Data = resultChild.ToList(),
                                                        Expanded = false,
                                                        TotalM3METRON = itemGrp.Sum(x => x.M3METRON),
                                                    };
                                                    data.TotalColumnVL = new List<double>();

                                                    for (var x = 0; x < dtSource.Columns.Count - 6; x++)
                                                    {
                                                        data.TotalColumnVL.Add(itemGrp.Sum(p => p.CoulumnVL[x]));
                                                    }
                                                    result.Add(data);
                                                    groupName = "Khách hàng: ";
                                                }
                                                break;
                                        }
                                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                        using (var package = new ExcelPackage())
                                        {
                                            var alphabet = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                                            var arrAlphabet = alphabet.Split(",");
                                            var lastAlphabet = string.Empty;
                                            lastAlphabet = arrAlphabet[dtSource.Columns.Count - 1];

                                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Thống kê chi tiết vật tư");
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Merge = true;
                                            worksheet.Cells["A1:" + lastAlphabet + "1"].Value = "THỐNG KÊ CHI TIẾT VẬT TƯ";
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

                                            #endregion

                                            #region Nhóm theo
                                            //Nhóm theo
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Merge = true;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Value = "* Nhóm theo: " + groupName.Substring(0, groupName.IndexOf(":"));
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.Font.Italic = true;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.Font.Bold = true;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.Font.Color.SetColor(Color.Black);
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.HorizontalAlignment =
                                                ExcelHorizontalAlignment.Left;
                                            worksheet.Cells["A6:" + lastAlphabet + "6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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

                                            if (result.Count() > 0)
                                            {
                                                int rowFirts = 8;
                                                for (int i = 0; i < result.Count(); i++)
                                                {
                                                    var elementGroup = result.ElementAt(i);
                                                    ////Row merge 1
                                                    var region = "A" + rowFirts + ":" + lastAlphabet + rowFirts;
                                                    //worksheet.Cells[region].Merge = true;
                                                    //if (elementGroup.Key.GetType() == typeof(DateTime))
                                                    //{
                                                    //    worksheet.Cells[region].Value = groupName + Convert.ToDateTime(elementGroup.Key).ToString("dd/MM/yyyy");
                                                    //}
                                                    //else
                                                    //{
                                                    //    worksheet.Cells[region].Value = groupName + elementGroup.Key;
                                                    //}
                                                    //worksheet.Cells[region].Style.Font.Italic = true;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                    //worksheet.Cells[region].Style.HorizontalAlignment =
                                                    //    ExcelHorizontalAlignment.Left;
                                                    //worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                    int rowFirts2 = rowFirts;
                                                    for (int k = 0; k < elementGroup.Data.Count(); k++)
                                                    {
                                                        var elementGroupChild = elementGroup.Data.ElementAt(k);
                                                        //Row merge 2
                                                        var region2 = "A" + rowFirts2 + ":" + lastAlphabet + rowFirts2;
                                                        worksheet.Cells[region2].Merge = true;
                                                        if (elementGroup.Key.GetType() == typeof(DateTime))
                                                        {
                                                            worksheet.Cells[region2].Value = groupName + Convert.ToDateTime(elementGroup.Key).ToString("dd/MM/yyyy") + " - " + paging.GroupBy == "MA" ? "Khách hàng: " + elementGroupChild.Key : "Mác: " + elementGroupChild.Key;
                                                        }
                                                        else
                                                        {
                                                            worksheet.Cells[region2].Value = groupName + elementGroup.Key + " - " + (paging.GroupBy == "MA" ? "Khách hàng: " + elementGroupChild.Key : "Mác: " + elementGroupChild.Key);
                                                        }
                                                        //worksheet.Cells[region2].Value = paging.GroupBy == "MA" ? "Khách hàng: " + elementGroupChild.Key : "Mác: " + elementGroupChild.Key;
                                                        worksheet.Cells[region2].Style.Font.Italic = true;
                                                        worksheet.Cells[region2].Style.Font.Bold = true;
                                                        worksheet.Cells[region2].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region2].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Left;
                                                        worksheet.Cells[region2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        rowFirts2 = rowFirts2 + 1;
                                                        var stt = 1;
                                                        for (var j = 0; j < elementGroupChild.Data.Count(); j++)
                                                        {
                                                            int row = rowFirts2;
                                                            var element = elementGroupChild.Data.ElementAt(j);
                                                            int column = 1;

                                                            //STT
                                                            worksheet.Cells[row, column].Value = stt;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Phiếu
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.MAPHIEU;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            //Ngày trộn
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.NGAYTRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;
                                                            worksheet.Cells[row, column].Style.Numberformat.Format = "HH:mm:ss dd/MM/yyyy";

                                                            //Khách hàng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENKHACHHANG;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;


                                                            //Mác
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.TENMACBETONG;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;


                                                            //Tổng khối lượng
                                                            column++;
                                                            worksheet.Cells[row, column].Value = element?.M3METRON;
                                                            worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                ExcelVerticalAlignment.Center;

                                                            foreach (var itemElement in element.CoulumnVL)
                                                            {
                                                                column++;
                                                                worksheet.Cells[row, column].Value = itemElement;
                                                                worksheet.Cells[row, column].Style.VerticalAlignment =
                                                                    ExcelVerticalAlignment.Center;
                                                            }
                                                            rowFirts2++;
                                                            stt++;
                                                        }
                                                        // rowFirts2 = rowFirts2 + elementGroupChild.Data.Count();

                                                        //Row merge total
                                                        region2 = "A" + rowFirts2 + ":" + "E" + rowFirts2;
                                                        worksheet.Cells[region2].Merge = true;
                                                        worksheet.Cells[region2].Value = "Tổng:";
                                                        worksheet.Cells[region2].Style.Font.Italic = true;
                                                        worksheet.Cells[region2].Style.Font.Bold = true;
                                                        worksheet.Cells[region2].Style.Font.Color.SetColor(Color.Black);
                                                        worksheet.Cells[region2].Style.HorizontalAlignment =
                                                            ExcelHorizontalAlignment.Center;
                                                        worksheet.Cells[region2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                        region2 = "F" + rowFirts2;
                                                        worksheet.Cells[region2].Value = elementGroupChild.TotalM3METRON;
                                                        worksheet.Cells[region2].Style.Font.Bold = true;
                                                        worksheet.Cells[region2].Style.VerticalAlignment =
                                                                 ExcelVerticalAlignment.Center;

                                                        count = 6;
                                                        foreach (var itemElementGr in elementGroupChild.TotalColumnVL)
                                                        {
                                                            region2 = arrAlphabet[count] + rowFirts2;
                                                            worksheet.Cells[region2].Value = itemElementGr;
                                                            worksheet.Cells[region2].Style.Font.Bold = true;
                                                            worksheet.Cells[region2].Style.VerticalAlignment =
                                                                     ExcelVerticalAlignment.Center;

                                                            count++;
                                                        }

                                                        rowFirts2++;
                                                    }
                                                    rowFirts = rowFirts2;

                                                    ////Row merge total
                                                    //region = "A" + rowFirts + ":" + "E" + rowFirts;
                                                    //worksheet.Cells[region].Merge = true;
                                                    //worksheet.Cells[region].Value = "Tổng:";
                                                    //worksheet.Cells[region].Style.Font.Italic = true;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.Font.Color.SetColor(Color.Black);
                                                    //worksheet.Cells[region].Style.HorizontalAlignment =
                                                    //    ExcelHorizontalAlignment.Center;
                                                    //worksheet.Cells[region].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                    //region = "F" + rowFirts;
                                                    //worksheet.Cells[region].Value = elementGroup.TotalM3METRON;
                                                    //worksheet.Cells[region].Style.Font.Bold = true;
                                                    //worksheet.Cells[region].Style.VerticalAlignment =
                                                    //         ExcelVerticalAlignment.Center;

                                                    //count = 6;
                                                    //foreach (var itemElementGr in elementGroup.TotalColumnVL)
                                                    //{
                                                    //    region = arrAlphabet[count] + rowFirts;
                                                    //    worksheet.Cells[region].Value = itemElementGr;
                                                    //    worksheet.Cells[region].Style.Font.Bold = true;
                                                    //    worksheet.Cells[region].Style.VerticalAlignment =
                                                    //             ExcelVerticalAlignment.Center;

                                                    //    count++;
                                                    //}

                                                    //rowFirts++;
                                                }
                                                string modelRange = "A8:" + lastAlphabet + rowFirts;
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
                                }
                                ++ii;
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