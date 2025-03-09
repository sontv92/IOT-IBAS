using DocumentFormat.OpenXml.Drawing;
using IOITWebApp;
using IOITWebApp.Helper;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.Controllers.ApiCMS
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("Dashboard", "Dashboard");

        // GET: api/Order
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<DashboardDTO> rpdonhang = new List<DashboardDTO>();
                    command.CommandText = " SELECT BIENSO,Name  FROM  ";
                    command.CommandText += "(";
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch = context.Branch.Find(Convert.ToInt32(item));
                                if (i == 0)
                                {
                                    command.CommandText += "SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
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
                                    command.CommandText += "SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON \n";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1 \n";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1 \n";
                                    }
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON \n";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1 \n";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1 \n";
                                    }
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
                                    command.CommandText += "SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT distinct  tr.BIENSO,br.Name  FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
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
                            DashboardDTO item = new DashboardDTO();
                            item.donhang = 0;
                            item.Name = result["Name"].ToString();
                            item.TENNV = result["BIENSO"].ToString();
                            item.METKHOITICHLUY = 0;
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

        [HttpGet("GetByPageDH")]
        public IActionResult GetByPageDH([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<DashboardDTO> rpdonhang = new List<DashboardDTO>();
                    command.CommandText = " SELECT SUM(tongdh) as tongdonhang,SUM(tongnv) as tongnhanvien,SUM(donhang) as  donhang,TENNV FROM  \n";
                    command.CommandText += "(";
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch = context.Branch.Find(Convert.ToInt32(item));
                                if (i == 0)
                                {
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from ( \n";

                                    command.CommandText += "SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "' \n";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1 \n";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1 \n";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
                                }
                                else
                                {

                                    command.CommandText += " UNION ALL \n";
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from (";
                                    command.CommandText += " SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
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
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from (";
                                    command.CommandText += "SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL \n";
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from ( \n";
                                    command.CommandText += " SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
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
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from ( \n";
                                    command.CommandText += "SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL \n";
                                    command.CommandText += "select (select count(*) FROM [" + branch.Dataname + "].[dbo].[DATHANG]) as tongdh ,(select count(*) FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]) as tongnv,* from ( \n";
                                    command.CommandText += " SELECT COUNT(*) as donhang,br.TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] br ON br.ID = sa.NHANVIENID LEFT JOIN Branch br1 ON br1.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br1.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br1.Status = 1";
                                    }
                                    command.CommandText += "GROUP BY TENNV) as a";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") rpdonhang GROUP BY TENNV";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            DashboardDTO item = new DashboardDTO();
                            item.donhang = (int)result["donhang"];
                            item.tongdonhang = (int)result["tongdonhang"];
                            item.Name = "";
                            if (result["TENNV"] is System.DBNull)
                            {
                                item.TENNV = "";

                            }
                            else
                            {
                                item.TENNV = (string)result["TENNV"];
                            }
                            item.METKHOITICHLUY = 0;
                            item.tongnhanvien = (int)result["tongnhanvien"];
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


        [HttpGet("GetTyLeDaTron")]
        public IActionResult GetTyLeDaTron([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    DateTime thoigianbatdau = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, 0, 0, 0);
                    DateTime thoigianketthuc = new DateTime(paging.tungay.Year, paging.tungay.Month, paging.tungay.Day, 23, 59, 59);

                    if (paging != null)
                    {
                        if (paging.Branchlist != "" && paging.Branchlist != null && paging.Branchlist != "null" && paging.Branchlist != "undefined")
                        {
                            var arrListStr = paging.Branchlist.Split(',');

                            foreach (var item in arrListStr)
                            {
                                if (item != "")
                                {
                                    Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (branch != null)
                                    {


                                        String subQuerySumTyLeTron = string.Format("select CASE WHEN(SUM(METKHOIDATHANG) - SUM(METKHOITICHLUY)) < 0 THEN 0 ELSE (SUM(METKHOIDATHANG) - SUM(METKHOITICHLUY)) END as CHUATRON, SUM(METKHOITICHLUY) as DATRON   from [" + branch.Dataname + "].[dbo].DATHANG WHERE '{0}' <= NGAYDATHANG AND NGAYDATHANG <= '{1}' \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));


                                        String sqlGetDonHangTrongNgay = string.Format("select DH.MA as N'Mã' ,KH.TENKHACHHANG as N'Khách hàng', NGAYDATHANG as N'Ngày đặt hàng', " +
                                                                                            "METKHOIDATHANG as N'Mét khối đặt hàng', ROUND((ISNULL(DH.METKHOITICHLUY, 0) + ISNULL(DH.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))),2) as N'Mét khối đã trộn', " +
                                                                                            "CONVERT(VARCHAR,ROUND(((ISNULL(DH.METKHOITICHLUY, 0) + ISNULL(DH.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0)))/METKHOIDATHANG * 100),2)) + '%'  as N'Tỷ lệ hoàn thành'" +
                                                                                            "from [" + branch.Dataname + "].[dbo].DATHANG DH LEFT JOIN [" + branch.Dataname + "].[dbo].KHACHHANG KH ON DH.KHACHHANGID = KH.ID " +
                                                                                            "LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON DH.ID = te.DATHANGID " +
                                                                                            "WHERE '{0}' <= NGAYDATHANG AND NGAYDATHANG <= '{1}' GROUP BY  DH.MA ,KH.TENKHACHHANG, NGAYDATHANG, DH.METKHOITICHLUY, DH.METKHOITICHLUY_TEMP, METKHOIDATHANG\n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));

                                        //lay du lieu tu dau thang den thoi diem hien tai
                                        String sqlGetBeTongDaTronThang = string.Format("SELECT ROUND(SUM(M3METRON),2) as BETONGDATRONTRONGTHANG FROM [" + branch.Dataname + "].[dbo].LSCHITIETMETRON WHERE DATEADD(m, DATEDIFF(m, 0, '{0}'), 0) <= GIOBD AND GIOKT <= '{1}' \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));

                                        //lay du lieu trong ngay
                                        String sqlGetBeTongDaTronNgay = string.Format("SELECT ROUND(SUM(M3METRON),2) as BETONGDATRONTRONGNGAY FROM [" + branch.Dataname + "].[dbo].LSCHITIETMETRON WHERE '{0}' <= GIOBD AND GIOKT <= '{1}' \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));

                                        //lay du lieu tong don hang trong ngay
                                        String sqlGetTongDonHangTronNgay = string.Format("SELECT COUNT(1) as TONGDONHANGTRONGNGAY FROM [" + branch.Dataname + "].[dbo].DATHANG WHERE '{0}' <= NGAYDATHANG AND NGAYDATHANG <= '{1}' \n",
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                        CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));

                                        //lay du lieu tong don hang hoa thanh trong ngay
                                        String sqlGetTongDonHangHoanThanhTronNgay = string.Format(" SELECT COUNT(1) as TONGDONHANGHOANTHANHTRONGNGAY FROM [" + branch.Dataname + "].[dbo].DATHANG dh LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON DH.ID = te.DATHANGID WHERE '{0}' <= NGAYDATHANG AND NGAYDATHANG <= '{1}' Group by dh.METKHOITICHLUY, dh.METKHOITICHLUY_TEMP, dh.METKHOIDATHANG Having (ISNULL(DH.METKHOITICHLUY, 0) + ISNULL(DH.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) >= dh.METKHOIDATHANG ",
                                                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianbatdau),
                                                                                                    CommonLib.DateTimeRealDayForSQLToString(thoigianketthuc));

                                        String sqlGetXeTronGanNhat = string.Format("SELECT top 5  a.MALSTRON as MAPHIEU, CONCAT(N'Phiếu ' ,a.MALSTRON, ' - ', \n" +
                                                                                        "(SELECT RIGHT('0' + CAST((DATEDIFF(SECOND, a.GIOBATDAU, a.GIOXONG)) / 3600 AS VARCHAR), 2) + 'h' +\n" +
                                                                                        "RIGHT('0' + CAST(((DATEDIFF(SECOND, a.GIOBATDAU, a.GIOXONG)) / 60) % 60 AS VARCHAR), 2) + 'm' +\n" +
                                                                                        "RIGHT('0' + CAST((DATEDIFF(SECOND, a.GIOBATDAU, a.GIOXONG)) % 60 AS VARCHAR), 2) + 's'), N', Giờ xong: ', FORMAT(a.GIOXONG, 'dd/MM/yyyy hh:mm')) as THOIGIAN,\n" +
                                                                                        "SUM(b.M3METRON ) AS[TOTAL3M]\n" +
                                                                                        "FROM [" + branch.Dataname + "].[dbo].LSTRON a INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON  b\n" +
                                                                                        "ON a.MALSTRON = b.MALSTRON\n" +
                                                                                        "GROUP BY a.MALSTRON, a.GIOXONG,a.GIOBATDAU\n" +
                                                                                        "order by a.GIOXONG desc");


                                        // Tổng thể tích theo ngày trộn

                                        String sqlGetTongTheTichTheoNgay = String.Format($@"DECLARE @desiredMonth INT = " + paging.tungay.Month + "; " +
                                                                                            "DECLARE @desiredYear INT = " + paging.tungay.Year + ";" +
                                                                                            "DECLARE @startDate DATE;" +
                                                                                            "DECLARE @endDate DATE;" +
                                                                                            "SELECT @startDate = DATEFROMPARTS(@desiredYear, @desiredMonth, 1);" +
                                                                                            "SELECT @endDate = EOMONTH(DATEFROMPARTS(@desiredYear, @desiredMonth, 1));" +
                                                                                            "DECLARE @currentDate DATE = @startDate;" +
                                                                                            "DECLARE @results TABLE (Date DATE, SUMMETKHOI Real);" +
                                                                                            "WHILE @currentDate <= @endDate" +
                                                                                            " BEGIN INSERT INTO @results (Date, SUMMETKHOI)" +
                                                                                            " SELECT @currentDate, ISNULL(SUM(ISNULL(B.M3METRON,0)),0)" +
                                                                                            " FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON" +
                                                                                            " WHERE A.NGAYTRON = @currentDate;" +
                                                                                            " SET @currentDate = DATEADD(DAY, 1, @currentDate);" +
                                                                                            " END " +
                                                                                            "SELECT * FROM @results; DROP TABLE #results;");

                                        DataTable dtGetTongTheTichTheoNgay = CommonLib.GetDataBySql(sqlGetTongTheTichTheoNgay.ToString());

                                        TongTheTichTheoNgayTron tongTheTichTheoNgayTron = new TongTheTichTheoNgayTron();
                                        tongTheTichTheoNgayTron.Time = paging.tungay.ToString("MM/yyyy");
                                        if (dtGetTongTheTichTheoNgay.Rows.Count > 0)
                                        {
                                            tongTheTichTheoNgayTron.Day = new List<string>();
                                            tongTheTichTheoNgayTron.SumM3 = new List<double>();
                                            for (int i = 0; i < dtGetTongTheTichTheoNgay.Rows.Count; i++)
                                            {
                                                var day = Convert.ToDateTime(dtGetTongTheTichTheoNgay.Rows[i]["DATE"].ToString());
                                                tongTheTichTheoNgayTron.Day.Add(day.Day.ToString());

                                                var m3 = Math.Round(double.Parse(dtGetTongTheTichTheoNgay.Rows[i]["SUMMETKHOI"].ToString()), 2);
                                                tongTheTichTheoNgayTron.SumM3.Add(m3);
                                                tongTheTichTheoNgayTron.Total += m3;

                                            }
                                            tongTheTichTheoNgayTron.Total = Math.Round(tongTheTichTheoNgayTron.Total, 2);
                                        }
                                        def.tongTheTichTheoNgayTron = tongTheTichTheoNgayTron;

                                        // Vật liệu theo ngày
                                        String sqlVatLieuTheoNgay = String.Format("SELECT D.TENCUAVL TENCUAVL, D.MALOAIVL LOAIVL, SUM(ISNULL(D.SOLUONGTD,0)) + SUM(ISNULL(D.SOLUONGTAY,0)) SUMSOLUONG " +
                                                                                    "FROM [" + branch.Dataname + "].[dbo].LSTRON A INNER JOIN [" + branch.Dataname + "].[dbo].LSCHITIETMETRON B ON A.MALSTRON = B.MALSTRON " +
                                                                                    "INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATTRON C ON C.ID = B.GIAMSATTRONID   \n" +
                                                                                    "INNER JOIN [" + branch.Dataname + "].[dbo].GIAMSATSOLUONG D ON D.STTGIAMSATTRON = C.STT   \n" +
                                                                                    "LEFT JOIN [" + branch.Dataname + "].[dbo].CUAVL F ON F.STTCUAVL = D.STTCUAVL " +
                                                                                    "WHERE A.NGAYTRON = '" + paging.tungay + "' AND F.TRANGTHAI = 'True' " +
                                                                                    "GROUP BY D.TENCUAVL, D.STTCUAVL, D.MALOAIVL");

                                        DataTable dtvatlieutheongay = CommonLib.GetDataBySql(sqlVatLieuTheoNgay.ToString());
                                        VatLieuTheoNgay vatLieuTheoNgay = new VatLieuTheoNgay();
                                        vatLieuTheoNgay.dataPoints = new List<DataPoint>();
                                        vatLieuTheoNgay.VatLieuTheoNgayDetails = new List<VatLieuTheoNgayDetail>();
                                        vatLieuTheoNgay.VatLieuTheoNgays = new List<VatLieuTheoNgayData>();
                                        var colors = new List<string>(new string[] { "DarkOrange", "Gold", "BlueViolet", "DarkSlateGray", "Green", "Blue", "GreenYellow", "MidnightBlue",
                                        "RoyalBlue", "SlateBlue", "Yellow", "MediumAquamarine", "Gray", "Chocolate", "Thistle"});
                                        var vatLieuTheoNgayDatas = new List<VatLieuTheoNgayData>();
                                        if (dtvatlieutheongay.Rows.Count > 0)
                                        {
                                            vatLieuTheoNgay.LOAICUAVL = new List<string>();
                                            vatLieuTheoNgay.SUMSOLUONG = new List<double>();
                                            for (int i = 0; i < dtvatlieutheongay.Rows.Count; i++)
                                            {
                                                var maLoaiVL = dtvatlieutheongay.Rows[i]["LOAIVL"].ToString();
                                                var strLoaiVL = String.Format("SELECT * FROM [" + branch.Dataname + "].[dbo].LOAIVL WHERE MALOAIVL=" + maLoaiVL);
                                                var dtloaiVL = CommonLib.GetDataBySql(strLoaiVL);
                                                var loaiVL = dtloaiVL.Rows[0]["TENLOAIVL"].ToString();

                                                var sum = maLoaiVL == "5" ? Math.Round(double.Parse(dtvatlieutheongay.Rows[i]["SUMSOLUONG"].ToString()), 2) : Math.Round(double.Parse(dtvatlieutheongay.Rows[i]["SUMSOLUONG"].ToString()), 0);



                                                vatLieuTheoNgayDatas.Add(new VatLieuTheoNgayData()
                                                {
                                                    LoaiCuaVL = loaiVL,
                                                    KhoiLuong = sum,
                                                    TenCuaVL = dtvatlieutheongay.Rows[i]["TENCUAVL"].ToString()
                                                });
                                                if (sum > 0)
                                                {
                                                    var checkLoai = vatLieuTheoNgay.LOAICUAVL.FindIndex(x => x.Equals(loaiVL));
                                                    if (checkLoai != -1)
                                                    {
                                                        vatLieuTheoNgay.SUMSOLUONG[checkLoai] += sum;
                                                    }
                                                    else
                                                    {
                                                        vatLieuTheoNgay.LOAICUAVL.Add(loaiVL);
                                                        vatLieuTheoNgay.SUMSOLUONG.Add(sum);
                                                    }

                                                }
                                            }
                                            vatLieuTheoNgay.VatLieuTheoNgays = vatLieuTheoNgayDatas;
                                            List<DataCheckColor> dataCheckColors = new List<DataCheckColor>();
                                            var total = vatLieuTheoNgay.SUMSOLUONG.Sum();
                                            for (int i = 0; i < vatLieuTheoNgay.LOAICUAVL.Count; i++)
                                            {
                                                var a = (int)Math.Round(267 / total);
                                                //vatLieuTheoNgay.dataPoints.Add(new DataPoint()
                                                //{
                                                //    //color = colors[i],
                                                //    indexLabel = (int)Math.Round((double)(100 * vatLieuTheoNgay.SUMSOLUONG[i]) / total) + "%",
                                                //    y = (int)Math.Round((double)(100 * vatLieuTheoNgay.SUMSOLUONG[i]) / total)
                                                //});
                                                dataCheckColors.Add(new DataCheckColor()
                                                {
                                                    LoaiCuaVL = vatLieuTheoNgay.LOAICUAVL[i],
                                                    Color = colors[i]
                                                });
                                            }
                                            vatLieuTheoNgayDatas = vatLieuTheoNgayDatas.OrderBy(x => x.LoaiCuaVL).ToList();
                                            foreach (var vatLieuTheoNgayData in vatLieuTheoNgayDatas)
                                            {
                                                vatLieuTheoNgay.dataPoints.Add(new DataPoint()
                                                {
                                                    x = vatLieuTheoNgayData.TenCuaVL,
                                                    y = vatLieuTheoNgayData.KhoiLuong
                                                });

                                                var cuaVL = dataCheckColors.FirstOrDefault(x => x.LoaiCuaVL.Contains(vatLieuTheoNgayData.LoaiCuaVL));
                                                vatLieuTheoNgay.VatLieuTheoNgayDetails.Add(new VatLieuTheoNgayDetail()
                                                {
                                                    Color = cuaVL?.Color,
                                                    CuaVL = vatLieuTheoNgayData.TenCuaVL + " : " + vatLieuTheoNgayData.KhoiLuong + " KG"
                                                });
                                            }
                                            vatLieuTheoNgay.VatLieuTheoNgayDetails = vatLieuTheoNgay.VatLieuTheoNgayDetails.OrderBy(x => x.Color).ToList();

                                        }
                                        def.vatLieuTheoNgay = vatLieuTheoNgay;
                                        DataTable dtTyletron = CommonLib.GetDataBySql(subQuerySumTyLeTron.ToString());
                                        if (dtTyletron.Rows.Count > 0)
                                        {
                                            List<double[]> lstTyletron = new List<double[]>();
                                            foreach (DataRow dataRow in dtTyletron.Rows)
                                            {
                                                int columnCount = 0;
                                                double[] myTableRow = new double[dtTyletron.Columns.Count];
                                                foreach (DataColumn dc in dtTyletron.Columns)
                                                {
                                                    if (dataRow[dc].ToString() is null || dataRow[dc].ToString() == "")
                                                    {
                                                        myTableRow[columnCount] = 0;
                                                    }
                                                    else
                                                    {
                                                        myTableRow[columnCount] = Math.Round(double.Parse(dataRow[dc].ToString()), 2);
                                                    }

                                                    columnCount++;
                                                }
                                                lstTyletron.Add(myTableRow);
                                            }
                                            var tyletron = lstTyletron.ToArray();
                                            def.data = tyletron;
                                        }

                                        List<ThongKeDonHangTongHopDTO> lstDonHangTrongNgay = new List<ThongKeDonHangTongHopDTO>();

                                        var sumMetKhoiButru = 0;
                                        DataTable dtDonHangTrongNgay = CommonLib.GetDataBySql(sqlGetDonHangTrongNgay.ToString());
                                        foreach (DataColumn col in dtDonHangTrongNgay.Columns)
                                        {
                                            ThongKeDonHangTongHopDTO items = new ThongKeDonHangTongHopDTO();
                                            string colName = col.ColumnName;

                                            if (colName != "ID")
                                            {
                                                items.header = colName;
                                                items.rows = new List<string>();
                                                for (int m = 0; m < dtDonHangTrongNgay.Rows.Count; m++)
                                                {
                                                    var myValue = dtDonHangTrongNgay.Rows[m][colName];
                                                    if (myValue.ToString().Length > 0)
                                                    {

                                                        items.rows.Add(myValue.ToString());
                                                    }
                                                    else
                                                    {
                                                        items.rows.Add("NULL");
                                                    }


                                                }
                                                lstDonHangTrongNgay.Add(items);
                                            }

                                        }
                                        def.data1 = lstDonHangTrongNgay;

                                        DataTable dtBeTongDaTronThang = CommonLib.GetDataBySql(sqlGetBeTongDaTronThang.ToString());
                                        if (dtBeTongDaTronThang.Rows.Count > 0)
                                        {
                                            def.tongBeTongThang = dtBeTongDaTronThang.Rows[0][0].ToString();
                                        }
                                        else
                                        {
                                            def.tongBeTongThang = "0";
                                        }

                                        DataTable dtBeTongDaTronNgay = CommonLib.GetDataBySql(sqlGetBeTongDaTronNgay.ToString());
                                        if (dtBeTongDaTronNgay.Rows.Count > 0)
                                        {
                                            def.tongBeTongNgay = dtBeTongDaTronNgay.Rows[0][0].ToString();
                                        }
                                        else
                                        {
                                            def.tongBeTongNgay = "0";
                                        }
                                        DataTable dtTongDonHangTronNgay = CommonLib.GetDataBySql(sqlGetTongDonHangTronNgay.ToString());
                                        if (dtTongDonHangTronNgay.Rows.Count > 0)
                                        {
                                            def.tongDonHangNgay = dtTongDonHangTronNgay.Rows[0][0].ToString();
                                        }
                                        else
                                        {
                                            def.tongDonHangNgay = "0";
                                        }
                                        DataTable dtTongDonHangHoanThanhTronNgay = CommonLib.GetDataBySql(sqlGetTongDonHangHoanThanhTronNgay.ToString());

                                        if (dtTongDonHangHoanThanhTronNgay.Rows.Count > 0)
                                        {
                                            def.tongDonHangHoanThanh = dtTongDonHangHoanThanhTronNgay.Rows[0][0].ToString();
                                        }
                                        else
                                        {
                                            def.tongDonHangHoanThanh = "0";
                                        }
                                        DataTable dtXeTronGanNhat = CommonLib.GetDataBySql(sqlGetXeTronGanNhat.ToString());
                                        List<string> listCar = new List<string>();
                                        List<double> listM3 = new List<double>();
                                        //List<string> listSoPhutHoanThanh = new List<string>();
                                        //listCar.Add("");
                                        //listM3.Add(0);

                                        if (dtXeTronGanNhat.Rows.Count > 0)
                                        {

                                            for (int i = 0; i < dtXeTronGanNhat.Rows.Count; i++)
                                            {
                                                listCar.Add(dtXeTronGanNhat.Rows[i]["THOIGIAN"].ToString());
                                                listM3.Add(Math.Round(double.Parse(dtXeTronGanNhat.Rows[i]["TOTAL3M"].ToString()), 2));
                                                //listSoPhutHoanThanh.Add(dtXeTronGanNhat.Rows[i]["SOPHUTHOANTHANH"].ToString() + " phút");
                                            }

                                        }
                                        def.lstXeTron = listCar.ToArray();
                                        def.m3XeTron = listM3.ToArray();
                                        //def.lstPhutHoanThanh = listSoPhutHoanThanh.ToArray();
                                        //if (dtTongDonHangHoanThanhTronNgay.Rows.Count > 0)
                                        //{
                                        //    // so sanh va dem
                                        //    int dem = 0;
                                        //    for (int i = 0; i < dtTongDonHangHoanThanhTronNgay.Rows.Count; i++)
                                        //    {
                                        //        if (double.Parse(dtTongDonHangHoanThanhTronNgay.Rows[i]["TichLuy"].ToString()) >= double.Parse(dtTongDonHangHoanThanhTronNgay.Rows[i]["METKHOIDATHANG"].ToString()))
                                        //        {
                                        //            dem++;
                                        //        }
                                        //    }
                                        //    def.tongDonHangHoanThanh = dem.ToString();
                                        //}
                                        //else
                                        //{
                                        //    def.tongDonHangHoanThanh = "0";
                                        //}


                                    }
                                }
                            }
                        }
                        else
                        {
                            def.data1 = new List<ThongKeDonHangTongHopDTO>();
                            def.tongBeTongThang = "0";
                            def.tongDonHangNgay = "0";
                            def.tongDonHangHoanThanh = "0";
                            def.tongBeTongNgay = "0";
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


        [HttpGet("GetByPageChart")]
        public IActionResult GetByPageChart([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<DashboardChartDTO> rpdonhang = new List<DashboardChartDTO>();
                    switch (paging.sort)
                    {
                        case 1:
                            command.CommandText = "  SELECT SUM(METKHOITICHLUY) as METKHOITICHLUY, CONVERT(varchar,DATEPART(HH,NGAYDATHANG)) + N'H' as NGAYDATHANG FROM ";
                            break;
                        case 2:
                            command.CommandText = " SELECT SUM(METKHOITICHLUY) as METKHOITICHLUY, CASE WHEN DATEPART(DW,NGAYDATHANG) = 1 THEN N'Chủ nhật' WHEN DATEPART(DW,NGAYDATHANG) = 2 THEN N'Thứ 2' WHEN DATEPART(DW,NGAYDATHANG) = 3 THEN N'Thứ 3' WHEN DATEPART(DW,NGAYDATHANG) = 4 THEN N'Thứ 4' WHEN DATEPART(DW,NGAYDATHANG) = 5 THEN N'Thứ 5' WHEN DATEPART(DW,NGAYDATHANG) = 6 THEN N'Thứ 6' WHEN DATEPART(DW,NGAYDATHANG) = 7 THEN N'Thứ 7' ELSE N'Chủ nhật' END as NGAYDATHANG   FROM ";
                            break;
                        case 3:
                            command.CommandText = " SELECT SUM(METKHOITICHLUY) as METKHOITICHLUY, CASE WHEN DATEPART(DW,NGAYDATHANG) = 1 THEN N'Chủ nhật' WHEN DATEPART(DW,NGAYDATHANG) = 2 THEN N'Thứ 2' WHEN DATEPART(DW,NGAYDATHANG) = 3 THEN N'Thứ 3' WHEN DATEPART(DW,NGAYDATHANG) = 4 THEN N'Thứ 4' WHEN DATEPART(DW,NGAYDATHANG) = 5 THEN N'Thứ 5' WHEN DATEPART(DW,NGAYDATHANG) = 6 THEN N'Thứ 6' WHEN DATEPART(DW,NGAYDATHANG) = 7 THEN N'Thứ 7' ELSE N'Chủ nhật' END as NGAYDATHANG   FROM ";
                            break;
                        case 4:
                            command.CommandText = "  SELECT SUM(METKHOITICHLUY) as METKHOITICHLUY, CONVERT(varchar,DATEPART(DD,NGAYDATHANG)) as NGAYDATHANG FROM  ";
                            break;
                        default:
                            break;
                    }
                    command.CommandText += "(";
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch = context.Branch.Find(Convert.ToInt32(item));
                                if (i == 0)
                                {
                                    command.CommandText += "SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
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
                                    command.CommandText += "SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
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
                                    command.CommandText += "SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT tr.GIOXONG AS NGAYDATHANG,SUM(trde.M3METRON) as METKHOITICHLUY FROM [" + branch.Dataname + "].[dbo].[LSDATHANG] sa LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "' LEFT JOIN [" + branch.Dataname + "].[dbo].LSTRON tr ON tr.STTLSDATHANG = sa.STT  LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] trde ON trde.[MALSTRON] = tr.MALSTRON";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
                                    }
                                    command.CommandText += " GROUP BY tr.GIOXONG ";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") rpdonhang";
                    switch (paging.sort)
                    {
                        case 1:
                            command.CommandText += " GROUP BY CONVERT(varchar,DATEPART(HH,NGAYDATHANG))";
                            break;
                        case 2:
                            command.CommandText += " GROUP BY DATEPART(DW,NGAYDATHANG)";
                            break;
                        case 3:
                            command.CommandText += " GROUP BY DATEPART(DW,NGAYDATHANG)";
                            break;
                        case 4:
                            command.CommandText += " GROUP BY CONVERT(varchar,DATEPART(DD,NGAYDATHANG))";
                            break;
                        default:
                            break;
                    }
                    command.CommandText += " ORDER BY NGAYDATHANG Desc";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            DashboardChartDTO item = new DashboardChartDTO();
                            item.METKHOITICHLUY = Math.Round((Double)result["METKHOITICHLUY"], 1);
                            item.NGAYDATHANG = (String)result["NGAYDATHANG"];

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
    }
}