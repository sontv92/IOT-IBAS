using IOITWebApp;
using IOITWebApp.Helper;
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
    public class DatHangController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("dathang", "dathang");
        private static string functionCode = "QLDH";
        private IHostingEnvironment _hostingEnvironment;
        public DatHangController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/Order

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

                    List<DatHangDTO> donhang = new List<DatHangDTO>();
                    command.CommandText = " SELECT KHACHHANGID,NHANVIENID,DUANID,MACBETONGID, ID, Ma, TENKHACHHANG,TENDUAN,TONGSOPHIEU,METKHOIDATHANG,METKHOITICHLUY,NGAYDATHANG,TENMACBETONG,NGAYDATHANGTITLE,TENNV,LASTUPDATED INTO #Result FROM ";
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
                                    command.CommandText += "SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU, sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU, sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
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
                                    command.CommandText += "SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU, sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU, sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
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
                                    command.CommandText += "SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU ,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, (ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))) AS METKHOITICHLUY, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID " +
                                        "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV,sa.LASTUPDATED";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") nv";
                    command.CommandText += " WHERE Ma NOT LIKE N'%DH2%'";
                    if (paging.query != null)
                    {
                        var a = paging.query;
                        command.CommandText += " AND " + HttpUtility.UrlDecode(paging.query);
                    }
                    if (!string.IsNullOrEmpty(paging.TENKHACHHANG) && paging.TENKHACHHANG != "undefined")
                    {
                        command.CommandText += " AND TENKHACHHANG LIKE '%" + paging.TENKHACHHANG + "%'";
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ";
                    command.CommandText += " ORDER BY NGAYDATHANG DESC";

                    //if (paging.order_by != null)
                    //{
                    //    command.CommandText += " ORDER BY " + paging.order_by;
                    //}
                    //else
                    //{
                    //    command.CommandText += " ORDER BY nv.NGAYDATHANG asc";
                    //}
                    command.CommandText += " OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY;";
                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        def.metadata = result[0];
                        result.NextResult();
                        while (result.Read())
                        {
                            DatHangDTO item = new DatHangDTO();
                            item.TENKHACHHANG = (result["TENKHACHHANG"] is DBNull) ? String.Empty : (string)result["TENKHACHHANG"];
                            item.TENDUAN = (result["TENDUAN"] is DBNull) ? String.Empty : (string)result["TENDUAN"];
                            item.TENMACBETONG = (result["TENMACBETONG"] is DBNull) ? String.Empty : (string)result["TENMACBETONG"];
                            item.METKHOIDATHANG = Math.Round((Single)result["METKHOIDATHANG"], 1);
                            item.TONGSOPHIEU = (int)(result["TONGSOPHIEU"]);
                            item.METKHOITICHLUY = Math.Round((double)result["METKHOITICHLUY"], 1);
                            item.NGAYDATHANG = (DateTime)result["NGAYDATHANG"];
                            item.Ma = (result["Ma"] is DBNull) ? String.Empty : (string)result["Ma"];
                            item.ID = (Guid)result["ID"];
                            item.KHACHHANGID = (Guid)result["KHACHHANGID"];
                            item.NHANVIENID = (Guid)result["NHANVIENID"];
                            item.DUANID = (Guid)result["DUANID"];
                            item.MACBETONGID = (Guid)result["MACBETONGID"];

                            item.NGAYDATHANGTITLE = (string)result["NGAYDATHANGTITLE"];
                            if (result["TENNV"] is System.DBNull)
                            {
                                item.TENNV = "";

                            }
                            else
                            {
                                item.TENNV = (string)result["TENNV"];
                            }
                            donhang.Add(item);
                        }

                        def.data = donhang;
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

        [HttpGet("GetMacBeTong/{Branchlist}")]
        public IActionResult GetMacBeTong(string Branchlist)
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

                    List<MacBeTongDTO> mac = new List<MacBeTongDTO>();
                    command.CommandText = " SELECT ID,TENMACBETONG INTO #Result FROM ";
                    command.CommandText += "(";

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist.ToString()));

                    command.CommandText += "SELECT ID,TENMACBETONG FROM [" + branch.Dataname + "].[dbo].[MACBETONG]";

                    command.CommandText += ") MBT";


                    command.CommandText += " SELECT *  FROM #Result order by TENMACBETONG ASC; ";


                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        //result.Read();
                        //def.metadata = result[0];
                        //result.NextResult();
                        while (result.Read())
                        {
                            MacBeTongDTO item = new MacBeTongDTO();
                            item.TENMACBETONG = (result["TENMACBETONG"] is DBNull) ? String.Empty : (string)result["TENMACBETONG"];
                            item.MACBETONGID = (Guid)result["ID"];
                            mac.Add(item);
                        }

                        def.data = mac;
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
        [HttpGet("GetNhanVien/{Branchlist}")]
        public IActionResult GetNhanVien(string Branchlist)
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
                    command.CommandText = " SELECT ID,TENNV INTO #Result FROM ";
                    command.CommandText += "(";

                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT ID,TENNV FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]";

                    command.CommandText += ") NV";

                    command.CommandText += " SELECT *  FROM #Result order by TENNV ASC; ";


                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        //result.Read();
                        //def.metadata = result[0];
                        //result.NextResult();
                        while (result.Read())
                        {
                            NhanVienDTO item = new NhanVienDTO();
                            item.TENNV = (result["TENNV"] is DBNull) ? String.Empty : (string)result["TENNV"];
                            item.NHANVIENID = (Guid)result["ID"];
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
        [HttpGet("GetKhachHang/{Branchlist}")]
        public IActionResult GetKhachHang(string Branchlist)
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
                    command.CommandText = " SELECT ID,TENKHACHHANG INTO #Result FROM ";
                    command.CommandText += "(";
                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT ID,TENKHACHHANG FROM [" + branch.Dataname + "].[dbo].[KHACHHANG]";
                    command.CommandText += ") kh";
                    command.CommandText += " SELECT *  FROM #Result order by TENKHACHHANG ASC; ";

                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        //result.Read();
                        //def.metadata = result[0];
                        //result.NextResult();
                        while (result.Read())
                        {
                            KhachHangDTO item = new KhachHangDTO();
                            item.TENKHACHHANG = (result["TENKHACHHANG"] is DBNull) ? String.Empty : (string)result["TENKHACHHANG"];
                            item.KHACHHANGID = (Guid)result["ID"];
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
        [HttpGet("GetDuAn/{Branchlist}")]
        public IActionResult GetDuAn(string Branchlist)
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

                    List<DuAnDTO> da = new List<DuAnDTO>();
                    command.CommandText = " SELECT ID,TENDUAN INTO #Result FROM ";
                    command.CommandText += "(";
                    Branch branch = context.Branch.Find(Convert.ToInt32(Branchlist));
                    command.CommandText += "SELECT ID,TENDUAN FROM [" + branch.Dataname + "].[dbo].[DUAN]";
                    command.CommandText += ") da";

                    command.CommandText += " SELECT *  FROM #Result order by TENDUAN ASC; ";

                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        //result.Read();
                        //def.metadata = result[0];
                        //result.NextResult();
                        while (result.Read())
                        {
                            DuAnDTO item = new DuAnDTO();
                            item.TENDUAN = (result["TENDUAN"] is DBNull) ? String.Empty : (string)result["TENDUAN"];
                            item.DUANID = (Guid)result["ID"];
                            da.Add(item);
                        }

                        def.data = da;
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

        [HttpPost]
        public IActionResult PostDatHang([FromBody] DatHangDTO dathang)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (dathang == null)
                {

                    return BadRequest("Owner object is null");
                }


                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
                {
                    def.meta = new Meta(222, "No permission");
                    return Ok(def);
                }
                if (dathang != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (dathang.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(dathang.BranchId));
                            //sinh ID tu dong
                            dathang.ID = CustomGuid.NewSequentialId();
                            dathang.Ma = CommonLib.GetSo("DATHANG", "Ma", "DH1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);


                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[DATHANG] ([ID], [Ma], [KHACHHANGID], [NHANVIENID], [DUANID], [MACBETONGID], [METKHOIDATHANG], [NGAYDATHANG], [TONGSOPHIEU], [METKHOITICHLUY],[MAKHACHHANG],[MADUAN],[MAMACBETONG]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramKHACHHANGID,@paramNHANVIENID,@paramDUANID,@paramMACBETONGID,@paramMETKHOIDATHANG,@paramNGAYDATHANG,@paramTONGSOPHIEU,@paramMETKHOITICHLUY,(SELECT Ma FROM [" + branch.Dataname + "].[dbo].KHACHHANG where ID = @paramKHACHHANGID),(SELECT Ma FROM [" + branch.Dataname + "].[dbo].DUAN where ID = @paramDUANID),(SELECT Ma FROM [" + branch.Dataname + "].[dbo].MACBETONG where ID = @paramMACBETONGID))";

                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = dathang.ID;
                            command.Parameters.Add(paramID);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = dathang.Ma;
                            command.Parameters.Add(paramMa);

                            var paramKHACHHANGID = command.CreateParameter();
                            paramKHACHHANGID.ParameterName = "@paramKHACHHANGID";
                            paramKHACHHANGID.Value = dathang.KHACHHANGID;
                            command.Parameters.Add(paramKHACHHANGID);

                            var paramNHANVIENID = command.CreateParameter();
                            paramNHANVIENID.ParameterName = "@paramNHANVIENID";
                            paramNHANVIENID.Value = dathang.NHANVIENID;
                            command.Parameters.Add(paramNHANVIENID);

                            var paramDUANID = command.CreateParameter();
                            paramDUANID.ParameterName = "@paramDUANID";
                            paramDUANID.Value = dathang.DUANID;
                            command.Parameters.Add(paramDUANID);

                            var paramMACBETONGID = command.CreateParameter();
                            paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                            paramMACBETONGID.Value = dathang.MACBETONGID;
                            command.Parameters.Add(paramMACBETONGID);

                            var paramMETKHOIDATHANG = command.CreateParameter();
                            paramMETKHOIDATHANG.ParameterName = "@paramMETKHOIDATHANG";
                            paramMETKHOIDATHANG.Value = dathang.METKHOIDATHANG;
                            command.Parameters.Add(paramMETKHOIDATHANG);

                            var paramNGAYDATHANG = command.CreateParameter();
                            paramNGAYDATHANG.ParameterName = "@paramNGAYDATHANG";
                            var ngayDatHang = Convert.ToDateTime(dathang.NGAYDATHANG.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm:ss")); 
                            paramNGAYDATHANG.Value = ngayDatHang;
                            command.Parameters.Add(paramNGAYDATHANG);

                            var paramTONGSOPHIEU = command.CreateParameter();
                            paramTONGSOPHIEU.ParameterName = "@paramTONGSOPHIEU";
                            paramTONGSOPHIEU.Value = dathang.TONGSOPHIEU;
                            command.Parameters.Add(paramTONGSOPHIEU);

                            var paramMETKHOITICHLUY = command.CreateParameter();
                            paramMETKHOITICHLUY.ParameterName = "@paramMETKHOITICHLUY";
                            paramMETKHOITICHLUY.Value = dathang.METKHOITICHLUY;
                            command.Parameters.Add(paramMETKHOITICHLUY);

                        }
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            def.meta = new Meta(200, "Them moi thanh cong !");
                            return Ok(def);

                        }


                    }
                }
                else
                {
                    return StatusCode(200, "Unsuccess");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }


        [HttpPut("{id}")]
        public IActionResult PutDatHang(Guid ID, [FromBody] DatHangDTO dathang)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (dathang == null)
                {

                    return BadRequest("Owner object is null");
                }


                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
                {
                    def.meta = new Meta(222, "No permission");
                    return Ok(def);
                }
                if (dathang != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (dathang.BranchId != 0)
                        {
                            Branch branch = context.Branch.Find(Convert.ToInt32(dathang.BranchId));

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[DATHANG] SET [KHACHHANGID]= @paramKHACHHANGID ,[NHANVIENID] = @paramNHANVIENID ,[DUANID]= @paramDUANID,[MACBETONGID]= @paramMACBETONGID,[METKHOIDATHANG]= @paramMETKHOIDATHANG,[TONGSOPHIEU]= @paramTONGSOPHIEU, [TONGSOPHIEU_TEMP]=0, [MAKHACHHANG] = (select Ma from [" + branch.Dataname + "].[dbo].KHACHHANG where ID =  @paramKHACHHANGID),[MADUAN] = (select Ma from [" + branch.Dataname + "].[dbo].DUAN where ID =  @paramDUANID),[MAMACBETONG]= (select Ma from [" + branch.Dataname + "].[dbo].MACBETONG where ID =  @paramMACBETONGID) WHERE ID = @paramID";

                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = ID;
                            command.Parameters.Add(paramID);

                            var paramKHACHHANGID = command.CreateParameter();
                            paramKHACHHANGID.ParameterName = "@paramKHACHHANGID";
                            paramKHACHHANGID.Value = dathang.KHACHHANGID;
                            command.Parameters.Add(paramKHACHHANGID);

                            var paramNHANVIENID = command.CreateParameter();
                            paramNHANVIENID.ParameterName = "@paramNHANVIENID";
                            paramNHANVIENID.Value = dathang.NHANVIENID;
                            command.Parameters.Add(paramNHANVIENID);

                            var paramDUANID = command.CreateParameter();
                            paramDUANID.ParameterName = "@paramDUANID";
                            paramDUANID.Value = dathang.DUANID;
                            command.Parameters.Add(paramDUANID);

                            var paramMACBETONGID = command.CreateParameter();
                            paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                            paramMACBETONGID.Value = dathang.MACBETONGID;
                            command.Parameters.Add(paramMACBETONGID);

                            var paramMETKHOIDATHANG = command.CreateParameter();
                            paramMETKHOIDATHANG.ParameterName = "@paramMETKHOIDATHANG";
                            paramMETKHOIDATHANG.Value = dathang.METKHOIDATHANG;
                            command.Parameters.Add(paramMETKHOIDATHANG);

                            var paramTONGSOPHIEU = command.CreateParameter();
                            paramTONGSOPHIEU.ParameterName = "@paramTONGSOPHIEU";
                            paramTONGSOPHIEU.Value = dathang.TONGSOPHIEU;
                            command.Parameters.Add(paramTONGSOPHIEU);

                            //var paramMETKHOITICHLUY = command.CreateParameter();
                            //paramMETKHOITICHLUY.ParameterName = "@paramMETKHOITICHLUY";
                            //paramMETKHOITICHLUY.Value = dathang.METKHOITICHLUY;
                            //command.Parameters.Add(paramMETKHOITICHLUY);

                            context.Database.OpenConnection();
                            using (var result = command.ExecuteReader())
                            {
                                result.Read();

                                var dhqr = $"SELECT ISNULL(METKHOITICHLUY,0) as METKHOITICHLUY,  ISNULL(METKHOITICHLUY_TEMP,0) as METKHOITICHLUY_TEMP FROM [{branch.Dataname}].[dbo].[DATHANG] WHERE ID = '{dathang.ID}'";
                                var dathang_tichluy = DapperHepper.Query<DatHangDTO>(LocalSettings.ConnectString, dhqr)?.FirstOrDefault();

                                var dh_tempqr = $"SELECT SUM(ISNULL(METKHOITICHLUY_BUTRU,0)) FROM [{branch.Dataname}].[dbo].[DATHANG_TEMP] WHERE DATHANGID = '{dathang.ID}'";
                                double tichluy_butru = 0;
                                var dathang_temp = DapperHepper.Query<double>(LocalSettings.ConnectString, dh_tempqr);
                                if (dathang_temp != null)
                                {
                                    tichluy_butru = dathang_temp.FirstOrDefault();
                                }

                                var sotru = dathang_tichluy.METKHOITICHLUY + dathang_tichluy.METKHOITICHLUY_TEMP + tichluy_butru;

                                var hieuso = dathang.METKHOITICHLUY - sotru;

                                // Thực hiện insert vào bảng Dathang_temp
                                //sinh ID tu dong
                                var id = CustomGuid.NewSequentialId();
                                var qrInsert = $"INSERT INTO [{branch.Dataname}].[dbo].[DATHANG_TEMP] ([ID], [DATHANGID], [Ma], [METKHOITICHLUY_BUTRU], [LASTUPDATED]) VALUES ('{id}', '{dathang.ID}', '{dathang.Ma}', {hieuso}, '{DateTime.Now}')";
                                DapperHepper.Execute(LocalSettings.ConnectString, qrInsert);

                                def.meta = new Meta(200, "Cap nhat thanh cong !");
                                return Ok(def);

                            }
                        }
                        def.meta = new Meta(200, "Cap nhat thanh cong !");
                        return Ok(def);
                    }
                }
                else
                {
                    return StatusCode(200, "Unsuccess");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDatHang(string ID)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (ID == null)
                {

                    return BadRequest("Owner object is null");
                }


                //check role
                var identity = (ClaimsIdentity)User.Identity;
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
                {
                    def.meta = new Meta(222, "No permission");
                    return Ok(def);
                }
                if (ID != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        string[] IdList = ID.Split("_");
                        int branchID = 0;
                        Guid Id = System.Guid.Empty;


                        for (int i = 0; i < IdList.Length; i++)
                        {
                            if (i == 0)
                            {
                                Guid g = new Guid(IdList[i].ToString());
                                Id = g;
                            }
                            if (i == 1)
                            {
                                branchID = int.Parse(IdList[i].ToString());
                            }

                        }

                        if (branchID > 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(branchID));
                            // check đơn hàng đang được sử dụng hay không
                            command.CommandText += "SELECT Ma FROM [" + branch.Dataname + "].[dbo].[DATHANG] WHERE ID = @paramID;";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = Id;
                            command.Parameters.Add(paramID);

                            context.Database.OpenConnection();
                            using (var resultMa = command.ExecuteReader())
                            {
                                resultMa.Read();

                                var ma = (string)resultMa["Ma"];
                                if (!string.IsNullOrEmpty(ma))
                                {
                                    context.Database.CloseConnection();
                                    command.CommandText = "SELECT * FROM [" + branch.Dataname + "].[dbo].[THONGSOTRON]  WHERE MADATHANG = @paramMa;";
                                    var paramMa = command.CreateParameter();
                                    paramMa.ParameterName = "@paramMa";
                                    paramMa.Value = ma;
                                    command.Parameters.Add(paramMa);

                                    context.Database.OpenConnection();
                                    using (var resultThongsotron = command.ExecuteReader())
                                    {
                                        resultThongsotron.Read();
                                        var maDonHang = string.Empty;
                                        if (resultThongsotron.HasRows)
                                        {
                                            maDonHang = (string)resultThongsotron["MADATHANG"];
                                        }
                                        if (!string.IsNullOrEmpty(maDonHang))
                                        {
                                            def.meta = new Meta(212, "Đơn hàng đang được sử dụng, không thể xóa !");
                                            return Ok(def);
                                        }
                                        else
                                        {
                                            context.Database.CloseConnection();
                                            command.CommandText = string.Empty;
                                            command.CommandText += "BEGIN TRANSACTION [Tran1dh] BEGIN TRY ";

                                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[DATHANG]  WHERE ID = @paramID;";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";


                                            var paramTableName = command.CreateParameter();
                                            paramTableName.ParameterName = "@paramTableName";
                                            paramTableName.Value = "DATHANG";
                                            command.Parameters.Add(paramTableName);

                                            var paramLastupdated = command.CreateParameter();
                                            paramLastupdated.ParameterName = "@paramLastupdated";
                                            paramLastupdated.Value = DateTime.Now.ToString();
                                            command.Parameters.Add(paramLastupdated);

                                            command.CommandText += "COMMIT TRANSACTION [Tran1dh] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1dh] END CATCH";

                                            context.Database.OpenConnection();
                                            using (var result = command.ExecuteReader())
                                            {
                                                result.Read();
                                                def.meta = new Meta(200, "Xóa đơn hàng thành công !");
                                                return Ok(def);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        def.meta = new Meta(200, "Xoa đơn hàng không thành công !");
                        return Ok(def);
                    }
                }
                else
                {
                    return StatusCode(200, "Unsuccess");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }
    }
}