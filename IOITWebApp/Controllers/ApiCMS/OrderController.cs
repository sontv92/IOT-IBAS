using IOITWebApp;
using IOITWebApp.Models;
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

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "BCDH";
        private IHostingEnvironment _hostingEnvironment;
        public OrderController(IHostingEnvironment hostingEnvironment)
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

                    List<rpdonhangDTO> rpdonhang = new List<rpdonhangDTO>();
                    command.CommandText = " SELECT TENKHACHHANG,TENDUAN,METKHOIDATHANG,METKHOITICHLUY,NGAYDATHANG,Code,Name,TENMACBETONG,NGAYDATHANGTITLE,TENNV INTO #Result FROM ";
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
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
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
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
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
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") nv";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ";
                    if (paging.order_by != null)
                    {
                        command.CommandText += " ORDER BY " + paging.order_by;
                    }
                    else
                    {
                        command.CommandText += " ORDER BY nv.NGAYDATHANG asc";
                    }
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
                            rpdonhangDTO item = new rpdonhangDTO();
                            item.TENKHACHHANG = (string)result["TENKHACHHANG"];
                            item.TENDUAN = (string)result["TENDUAN"];
                            item.TENMACBETONG = (string)result["TENMACBETONG"];
                            item.METKHOIDATHANG = Math.Round((Single)result["METKHOIDATHANG"],1);
                            item.METKHOITICHLUY = Math.Round((Single)result["METKHOITICHLUY"], 1);
                            item.NGAYDATHANG = (DateTime)result["NGAYDATHANG"];
                            item.Code = (string)result["Code"];
                            item.Name = (string)result["Name"];
                            item.NGAYDATHANGTITLE = (string)result["NGAYDATHANGTITLE"];
                            if (result["TENNV"] is System.DBNull)
                            {
                                item.TENNV = "";

                            }
                            else
                            {
                                item.TENNV = (string)result["TENNV"];
                            }
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


        [HttpGet("GetReport")]
        public HttpResponseMessage GetReport([FromQuery] FilteredPagination paging)
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

                    List<rpdonhangDTO> rpdonhang = new List<rpdonhangDTO>();
                    command.CommandText = " SELECT TENKHACHHANG,TENDUAN,METKHOIDATHANG,METKHOITICHLUY,NGAYDATHANG,Code,Name,TENMACBETONG,NGAYDATHANGTITLE, TENNV INTO #Result FROM ";
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
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
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

                                return null;
                            }
                            int j = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (j == 0)
                                {
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                ++j;
                            }
                        }
                        else
                        {
                            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == paging.companyid).ToList();
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
                                    command.CommandText += "SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT kh.ID as  MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG, da.TENDUAN as TENDUAN , sa.[METKHOIDATHANG], sa.METKHOITICHLUY, sa.[NGAYDATHANG], br.Code, br.Name,mac.TENMACBETONG as TENMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID LEFT JOIN [" + branch.Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") nv";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ";
                    if (paging.order_by != null)
                    {
                        command.CommandText += " ORDER BY " + paging.order_by;
                    }
                    else
                    {
                        command.CommandText += " ORDER BY NGAYDATHANG asc";
                    }
                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        def.metadata = result[0];
                        result.NextResult();
                        while (result.Read())
                        {
                            rpdonhangDTO item = new rpdonhangDTO();
                            item.TENKHACHHANG = (string)result["TENKHACHHANG"];
                            item.TENDUAN = (string)result["TENDUAN"];
                            item.TENMACBETONG = (string)result["TENMACBETONG"];
                            item.METKHOIDATHANG = Math.Round((Single)result["METKHOIDATHANG"],1);
                            item.METKHOITICHLUY = Math.Round((Single)result["METKHOITICHLUY"],1);
                            item.NGAYDATHANG = (DateTime)result["NGAYDATHANG"];
                            item.Code = (string)result["Code"];
                            item.Name = (string)result["Name"];
                            item.NGAYDATHANGTITLE = (string)result["NGAYDATHANGTITLE"];
                            if (result["TENNV"] is System.DBNull)
                            {
                                item.TENNV = "";

                            }
                            else
                            {
                                item.TENNV = (string)result["TENNV"];
                            }
                            rpdonhang.Add(item);
                        }
                        // khởi tạo wb rỗng
                        XSSFWorkbook wb = new XSSFWorkbook();
                        // Tạo ra 1 sheet
                        ISheet sheet = wb.CreateSheet();

                        string fileName = "Bao-cao-ke-toan-2";
                        string template = @"template\export\BCKT2.xlsx";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string templatePath = Path.Combine(webRootPath, template);
                        string today = paging.denngay.Day.ToString() + "/" + paging.denngay.Month.ToString() + "/" + paging.denngay.Year.ToString();
                        string fromday = paging.tungay.Day.ToString() + "/" + paging.tungay.Month.ToString() + "/" + paging.tungay.Year.ToString();
                        MemoryStream ms = writeAccountantTwoToExcel(templatePath, 0, rpdonhang, today, fromday);

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var response = new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new ByteArrayContent(ms.ToArray())
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
            else
            {
                return null;
            }
        }
        public MemoryStream writeAccountantTwoToExcel(string templatePath, int sheetnumber, List<rpdonhangDTO> data, string today, string fromday)
        {
            FileStream file1 = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 4;
            if (sheet != null)
            {
                //XSSFRow rowhead = (XSSFRow)sheet.CreateRow(1);
                var style = sheet.GetRow(1).GetCell(0).CellStyle;
                sheet.GetRow(1).CreateCell(0).CellStyle = style;
                sheet.GetRow(1).GetCell(0).SetCellValue("Từ ngày " + fromday + " đến ngày " + today);
                int datasize = data.Count();
                int datacol = 9;
                try
                {
                    //Lấy danh sách style template
                    List<ICellStyle> rowStyle = new List<ICellStyle>();
                    for (int i = 0; i < datacol; i++)
                    {
                        rowStyle.Add(sheet.GetRow(rowStart).GetCell(i).CellStyle);
                    }

                    for (int rr = 0; rr < datasize + 1; rr++)
                    {
                        int rowNum = rr + rowStart;

                        try
                        {
                            XSSFRow row = (XSSFRow)sheet.CreateRow(rowNum);
                            for (int i = 0; i < datacol; i++)
                            {
                                row.CreateCell(i).CellStyle = rowStyle[i];
                                if (i == 0)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("Tổng");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(rr + 1);
                                    }
                                }
                                else if (i == 1)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].TENKHACHHANG);
                                    }
                                }
                                else if (i == 2)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].TENDUAN);
                                    }
                                }
                                else if (i == 3)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].TENMACBETONG);
                                    }
                                }
                                else if (i == 4)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(E5:E" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].METKHOIDATHANG, 2));
                                    }
                                }
                                else if (i == 5)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(F5:F" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].METKHOITICHLUY, 2));
                                    }
                                }
                                else if (i == 6)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].Name);
                                    }
                                }
                                else if (i == 7)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].NGAYDATHANGTITLE);
                                    }
                                }
                                else if (i == 8)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].TENNV);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }

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

        [HttpGet("GetNV")]
        public IActionResult GetNV([FromQuery] FilteredPagination paging)
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

                    List<BienSoDTO> rpdonhang = new List<BienSoDTO>();
                    command.CommandText = " SELECT DISTINCT Name FROM ";
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
                                    command.CommandText += "SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
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
                                    command.CommandText += "SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
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
                                    command.CommandText += "SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT nv.TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].NHANVIEN nv ON nv.ID = sa.NHANVIENID";
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
                            BienSoDTO item = new BienSoDTO();
                            if (result["Name"] is System.DBNull)
                            {
                                item.Name = "";

                            }
                            else
                            {
                                item.Name = (string)result["Name"];
                            }
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
        // GET: api/Order
        [HttpGet("GetByBranch")]
        public IActionResult GetByBranchAsync([FromQuery] FilteredPagination paging)
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

                    List<rpdonhangBranchDTO> rpdonhang = new List<rpdonhangBranchDTO>();
                    command.CommandText = "  SELECT METKHOIDATHANG, METKHOITICHLUY,BranchId,Name,TENNV,NGAYDATHANG INTO #Result FROM  ";
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
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
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
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
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
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                                    command.CommandText += "GROUP BY br.Name,br.BranchId, nv1.TENNV,sa.NGAYDATHANG";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") nv";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                    }
                    command.CommandText += " SELECT SUM(METKHOIDATHANG) AS METKHOIDATHANG, SUM(METKHOITICHLUY) AS METKHOITICHLUY,BranchId,Name FROM #Result ";
                    command.CommandText += " GROUP BY BranchId,Name";
                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            rpdonhangBranchDTO item = new rpdonhangBranchDTO();
                            item.METKHOIDATHANG = Math.Round((Double)result["METKHOIDATHANG"],1);
                            item.METKHOITICHLUY = Math.Round((Double)result["METKHOITICHLUY"],1);
                            item.Name = (string)result["Name"];
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
        [HttpGet("GetByUser")]
        public IActionResult GetByUserAsync(int id)
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
            using (var db = new CNTTVNWebContext())
            {
                IOITWebApp.Models.EF.User user = db.User.Find(id);
                var BranchId = user.BranchId;
                string[] arrListStr = BranchId.Split(',');
                int[] convertedItems = Array.ConvertAll<string, int>(arrListStr, int.Parse);
                IQueryable<Branch> data = db.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => convertedItems.Contains(x.BranchId));
                if (data == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }

                def.meta = new Meta(200, "Success");
                def.data = data.ToList();
                return Ok(def);
            }
        }
        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
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
                    Order data = await db.Order.FindAsync(id);

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

        [HttpPut("ChangeOrderStatus/{OrderId}/{Status}")]
        public async Task<IActionResult> ChangeOrderStatus(int OrderId, byte Status)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thực hiện thao tác này!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Order order = db.Order.Where(o => o.OrderId == OrderId && o.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (order == null)
                        {
                            def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                        order.OrderStatusId = Status;
                        db.Update(order);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (order.OrderStatusId != (int)Const.OrderStatus.DELIVERY)
                            {
                                //Gửi mail thay đổi trạng thái đơn hàng trừ trạng thái đang giao hàng
                                try
                                {
                                    OrderWebDTO orderWeb = new OrderWebDTO();
                                    var config = await db.Config.FindAsync(1);
                                    if (config != null)
                                    {
                                        var customer = await db.Customer.Where(e => e.CustomerId == order.CustomerId).FirstOrDefaultAsync();
                                        if (customer != null)
                                        {
                                            orderWeb.PassHash = null;
                                            orderWeb.OrderStatusId = order.OrderStatusId;
                                            orderWeb.PaymentMethodId = order.PaymentMethodId;
                                            orderWeb.PaymentStatusId = order.PaymentStatusId;
                                            orderWeb.Code = order.Code;
                                            orderWeb.CreatedAt = order.CreatedAt != null ? order.CreatedAt : DateTime.Now;
                                            orderWeb.OrderDelivery = order.OrderDelivery != null ? order.OrderDelivery : 0;
                                            orderWeb.OrderDiscount = order.OrderDiscount != null ? order.OrderDiscount : 0;
                                            orderWeb.OrderPaid = order.OrderPaid != null ? order.OrderPaid : 0;
                                            orderWeb.OrderTotal = order.OrderTotal != null ? order.OrderTotal : 0;

                                            //Lấy địa chỉ
                                            orderWeb.Address = "";
                                            var ca = await db.CustomerAddress.Where(e => e.CustomerAddressId == order.CustomerAddressId).FirstOrDefaultAsync();
                                            if (ca != null)
                                            {
                                                orderWeb.FullName = ca.Name;
                                                orderWeb.Phone = ca.Phone;
                                                orderWeb.Address = ca.Address;
                                                var district = await db.District.Where(e => e.DistrictId == ca.DistrictId).FirstOrDefaultAsync();
                                                if (district != null) orderWeb.Address += ", " + district.Name;
                                                var province = await db.Province.Where(e => e.ProvinceId == ca.ProvinceId).FirstOrDefaultAsync();
                                                if (province != null) orderWeb.Address += ", " + province.Name;
                                            }
                                            //Lấy chi tiết đơn hàng
                                            var orderItem = await db.OrderItem.Where(e => e.OrderId == order.OrderId
                                            && e.Status != (int)Const.Status.DELETED).Select(e => new OrderItemDTO
                                            {
                                                ProductId = e.ProductId,
                                                Quantity = e.Quantity,
                                                Price = e.Price,
                                                PriceTotal = e.PriceTotal
                                            }).ToListAsync();
                                            foreach (var item in orderItem)
                                            {
                                                var pro = await db.Product.Where(e => e.ProductId == item.ProductId).FirstOrDefaultAsync();
                                                if (pro != null)
                                                {
                                                    item.ProductUrl = pro.Url;
                                                    item.ProductImage = pro.Image;
                                                    item.ProductName = pro.Name;
                                                }
                                            }
                                            orderWeb.listOrderItem = orderItem;
                                            string url_temp = "";
                                            string subject = "";
                                            string title = "";
                                            string link = "";
                                            bool check = false;

                                            url_temp = "order-change-status.html";
                                            subject = config.EmailSender + " - Cập nhật thông tin đơn hàng " + order.Code;
                                            check = Utils.sendEmail(config, orderWeb, url_temp, subject, 2, customer.FullName, customer.Email);

                                            if (check)
                                            {
                                                order.IsSentMail = true;
                                            }
                                            else
                                            {
                                                order.IsSentMail = false;
                                            }
                                            db.Order.Update(order);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                                catch { }

                                if (order.IsSentMail == false)
                                    def.meta = new Meta(233, "Không gửi được Email");
                            }
                            if (order.OrderId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thay đổi trạng thái đơn hàng thành công!");
                            def.data = OrderId;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!OrderExists(order.OrderId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        [HttpPut("ChangePaymentOrderStatus/{OrderId}/{Status}")]
        public async Task<IActionResult> ChangePaymentOrderStatus(int OrderId, byte Status)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thực hiện thao tác này!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Order order = db.Order.Where(o => o.OrderId == OrderId && o.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (order == null)
                        {
                            def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                            return Ok(def);
                        }
                        order.PaymentStatusId = Status;
                        db.Update(order);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (order.OrderId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Thay đổi trạng thái thanh toán đơn hàng thành công!");
                            def.data = OrderId;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!OrderExists(order.OrderId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy đơn hàng. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi xảy ra trên hệ thống. Xin vui lòng thử lại sau!");
                return Ok(def);
            }
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, [FromBody] Order data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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

                //if (data.CustomerId == null || data.CustomerId == "")
                //{
                //    def.meta = new Meta(211, "CustomerId Null!");
                //    return Ok(def);
                //}

                //if (data.OrderStatusId == null || data.OrderStatusId == "")
                //{
                //    def.meta = new Meta(211, "OrderStatusId Null!");
                //    return Ok(def);
                //}

                //if (data.CreatedAt == null || data.CreatedAt == "")
                //{
                //    def.meta = new Meta(211, "CreatedAt Null!");
                //    return Ok(def);
                //}
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
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
                            if (!OrderExists(data.OrderId))
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

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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

                //if (data.CustomerId == null || data.CustomerId == "")
                //{
                //    def.meta = new Meta(211, "CustomerId Null!");
                //    return Ok(def);
                //}

                //if (data.OrderStatusId == null || data.OrderStatusId == "")
                //{
                //    def.meta = new Meta(211, "OrderStatusId Null!");
                //    return Ok(def);
                //}

                //if (data.CreatedAt == null || data.CreatedAt == "")
                //{
                //    def.meta = new Meta(211, "CreatedAt Null!");
                //    return Ok(def);
                //}
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Order.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
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
                            if (OrderExists(data.OrderId))
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

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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
                    Order data = await db.Order.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Order.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderId > 0)
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
                            if (!OrderExists(data.OrderId))
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

        private bool OrderExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Order.Count(e => e.OrderId == id) > 0;
            }
        }
    }
}


