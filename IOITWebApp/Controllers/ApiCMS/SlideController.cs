using IOITWebApp.Models;
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
using System;
using System.Collections.Generic;
using System.Data;
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

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SlideController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("slide", "slide");
        private static string functionCode = "TKĐH";
        private IHostingEnvironment _hostingEnvironment;

        public SlideController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/Slide
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            if (paging.TENKHACHHANG == "null" || paging.TENKHACHHANG == "undefined")
            {
                paging.TENKHACHHANG = "";
            }
            if (paging.BIENSO == "null" || paging.BIENSO == "undefined")
            {
                paging.BIENSO = "";
            }
            if (paging.TENMACBETONG == "null" || paging.TENMACBETONG == "undefined")
            {
                paging.TENMACBETONG = "";
            }
            if (paging.Branchlist == "null" || paging.Branchlist == "undefined")
            {
                paging.Branchlist = "";
            }
            if (paging.TENNV == "null" || paging.TENNV == "undefined")
            {
                paging.TENNV = "";
            }
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

                    List<rpthongkeDTO> rpdonhang = new List<rpthongkeDTO>();
                    command.CommandText = "  SELECT MALSTRON, MACHITIETMETRON, NGAYTRON, GIOBATDAU, GIOXONG, BIENSO, TENMACBETONG, M3METRON, SOLUONG, SOLUONGT, ISNULL(COPHAIPHUGIA,0) COPHAIPHUGIA, ISNULL(TENCUAVL,'') TENCUAVL, ISNULL(TENLOAIVL,'') TENLOAIVL, USERNAME,TENKHACHHANG,TENDUAN,DIADIEMXD,TENNV,TENHANGMUC,name FROM ";
                    
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
                                        command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL] ";
                                        command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL] ";
                                        command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                        command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON] ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL] ";
                                        command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL] ";
                                        command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL] ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL] ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON] ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL] ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL] ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
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
                                    command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                            rpthongkeDTO item = new rpthongkeDTO();
                            item.MALSTRON = (long)result["MALSTRON"];
                            item.MACHITIETMETRON = (long)result["MACHITIETMETRON"];
                            item.NGAYTRON = (DateTime)result["NGAYTRON"];
                            item.GIOBATDAU = (DateTime)result["GIOBATDAU"];
                            item.GIOXONG = (DateTime)result["GIOXONG"];
                            item.BIENSO = (string)result["BIENSO"];
                            item.TENMACBETONG = (string)result["TENMACBETONG"];
                            item.M3METRON = (Single)result["M3METRON"];
                            item.SOLUONG = (Single)result["SOLUONG"];
                            item.SOLUONGT = (Single)result["SOLUONGT"];
                            if (result["COPHAIPHUGIA"] is System.DBNull)
                            {
                                item.COPHAIPHUGIA = false;

                            }
                            else
                            {
                                item.COPHAIPHUGIA = (bool)result["COPHAIPHUGIA"];
                            }

                            item.TENCUAVL = (string)result["TENCUAVL"];
                            item.TENLOAIVL = (string)result["TENLOAIVL"];
                            if (result["USERNAME"] is System.DBNull)
                            {
                                item.USERNAME = "";

                            }
                            else
                            {
                                item.USERNAME = (string)result["USERNAME"];
                            }
                            item.TENKHACHHANG = (string)result["TENKHACHHANG"];
                            item.TENDUAN = (string)result["TENDUAN"];
                            item.DIADIEMXD = (string)result["DIADIEMXD"];
                            if (result["TENNV"] is System.DBNull)
                            {
                                item.TENNV = "";

                            }
                            else
                            {
                                item.TENNV = (string)result["TENNV"];
                            }
                            item.TENHANGMUC = (string)result["TENHANGMUC"];
                            item.name = (string)result["name"];
                            rpdonhang.Add(item);
                        }
                        if (rpdonhang.Count > 0)
                        {
                            DataTable grvDLThongKe = new DataTable();
                            grvDLThongKe.Columns.Add(new DataColumn("NGAYTRON", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("GIOBATDAU", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("GIOXONG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENKHACHHANG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("BIENSO", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENMACBETONG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENNV", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("M3METRON", typeof(String)));

                            // Lấy tên số liệu các thành phần của mác bê tông.
                            var tenCuaVatLieu = listVatLieu(paging.companyid, paging.Branchlist);


                            foreach (var item in tenCuaVatLieu)
                            {
                                if (item.TENCUAVL != "")
                                {
                                    grvDLThongKe.Columns.Add(new DataColumn(item.TENCUAVL, typeof(String)));
                                    if (!item.COPHAIPHUGIA)
                                    {
                                        grvDLThongKe.Columns.Add(new DataColumn("T." + item.TENCUAVL, typeof(String)));
                                    }
                                }

                            }

                            if (paging.status == 1)
                            {
                                grvDLThongKe.Columns.Add(new DataColumn("TENPHUGIA", typeof(String)));
                            }
                            grvDLThongKe.Columns.Add(new DataColumn("name", typeof(String)));
                            //Lấy tên loại và số cửa các cửa vật liệu                
                            int socuavatlieu = 0;
                            int socuacat = 0;
                            int socuada = 0;
                            int socuaximang = 0;
                            int socuanuoc = 0;
                            int socuaphugia = 0;
                            int maxSoCua = 0;

                            List<string> STTLOAIVLs = new List<string>();

                            foreach (var item in tenCuaVatLieu)
                            {
                                if (item.TENLOAIVL == "CAT")
                                {
                                    socuacat++;
                                    STTLOAIVLs.Add("CAT");
                                }
                                else if (item.TENLOAIVL == "DA")
                                {
                                    socuada++;
                                    STTLOAIVLs.Add("DA");
                                }
                                else if (item.TENLOAIVL == "XIMANG")
                                {
                                    socuaximang++;
                                    STTLOAIVLs.Add("XIMANG");
                                }
                                else if (item.TENLOAIVL == "NUOC")
                                {
                                    socuanuoc++;
                                    STTLOAIVLs.Add("NUOC");
                                }
                                else if (item.TENLOAIVL == "PHUGIA")
                                {
                                    socuaphugia++;
                                    STTLOAIVLs.Add("PHUGIA");
                                }
                                socuavatlieu++;
                            }
                            maxSoCua = socuacat;
                            if (maxSoCua < socuada) maxSoCua = socuada;
                            if(maxSoCua< socuaximang) maxSoCua = socuaximang;
                            if(maxSoCua< socuanuoc) maxSoCua = socuanuoc;
                            if(maxSoCua< socuaphugia) maxSoCua = socuaphugia;

                            var DLThongKe = new List<DULIEUTHONGKE>();
                            var finalresult = new List<DULIEUTHONGKE>();
                            DULIEUTHONGKE rowThongKe = new DULIEUTHONGKE();
                            for (int i = 0; i < rpdonhang.Count; i++)
                            {
                                if (i % socuavatlieu == 0)
                                {
                                    if (i != 0)
                                    {
                                        //Đưa vào danh sách Dữ liệu thống kê
                                        DLThongKe.Add(rowThongKe);
                                    }
                                    rowThongKe = new DULIEUTHONGKE();
                                    rowThongKe.STT = rpdonhang[i].MACHITIETMETRON.ToString();
                                    rowThongKe.NGAYTRON = rpdonhang[i].NGAYTRON.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    //Định dạng thời gian về dạng hh:mm AM/PM
                                    TimeSpan timespan = new TimeSpan(rpdonhang[i].GIOBATDAU.Hour, rpdonhang[i].GIOBATDAU.Minute, 00);
                                    DateTime time = DateTime.Today.Add(timespan);
                                    rowThongKe.GIOBATDAU = time.ToString("hh:mm tt");

                                    rowThongKe.NGAYGIOTRON = rpdonhang[i].NGAYTRON.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + time.ToString("hh:mm");

                                    timespan = new TimeSpan(rpdonhang[i].GIOXONG.Hour, rpdonhang[i].GIOXONG.Minute, 00);
                                    time = DateTime.Today.Add(timespan);
                                    rowThongKe.GIOXONG = time.ToString("hh:mm tt");

                                    rowThongKe.TENKHACHHANG = rpdonhang[i].TENKHACHHANG;
                                    rowThongKe.BIENSO = rpdonhang[i].BIENSO;
                                    rowThongKe.TENMACBETONG = rpdonhang[i].TENMACBETONG;
                                    rowThongKe.TENHANGMUC = rpdonhang[i].TENHANGMUC;
                                    rowThongKe.TENDUAN = rpdonhang[i].TENDUAN;
                                    rowThongKe.TENDIADIEMXD = rpdonhang[i].DIADIEMXD;
                                    rowThongKe.TENNV = rpdonhang[i].TENNV;
                                    rowThongKe.M3METRON = rpdonhang[i].M3METRON;
                                    //rowThongKe.TAIKHOAN = rpdonhang[i].USERNAME;

                                    //Khởi tạo các list
                                    rowThongKe.listcats = new List<float>();
                                    rowThongKe.listdas = new List<float>();
                                    rowThongKe.listximangs = new List<float>();
                                    rowThongKe.listnuocs = new List<float>();
                                    rowThongKe.listphugias = new List<float>();
                                    rowThongKe.tenphugias = new List<string>();

                                    //Lấy dữ liệu các thành phần                            
                                    if (rpdonhang[i].TENLOAIVL == "CAT")
                                    {
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "DA")
                                    {
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "XIMANG")
                                    {
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "NUOC")
                                    {
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "PHUGIA")
                                    {
                                        rowThongKe.listphugias.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.tenphugias.Add(rpdonhang[i].TENCUAVL);
                                    }
                                    rowThongKe.name = rpdonhang[i].name;

                                }
                                else
                                {
                                    //Lấy dữ liệu các thành phần                            
                                    if (rpdonhang[i].TENLOAIVL == "CAT")
                                    {
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "DA")
                                    {
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "XIMANG")
                                    {
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "NUOC")
                                    {
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "PHUGIA")
                                    {
                                        rowThongKe.listphugias.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.tenphugias.Add(rpdonhang[i].TENCUAVL);
                                    }

                                    if (i == (rpdonhang.Count - 1))
                                    {
                                        //Đưa vào danh sách Dữ liệu thống kê
                                        DLThongKe.Add(rowThongKe);
                                    }
                                }
                            }
                            if (paging.status == 1)
                            {
                                finalresult = DLThongKe.Where(x => (paging.TENKHACHHANG == "" || x.TENKHACHHANG == paging.TENKHACHHANG)
                                                            && (paging.BIENSO == "" || x.BIENSO == paging.BIENSO)
                                                            && (paging.TENMACBETONG == "" || x.TENMACBETONG == paging.TENMACBETONG)
                                                             && (paging.TENNV == "" || x.TENNV == paging.TENNV)
                                                            ).ToList();
                            }//Trường hợp xem thống kê tổng
                            else
                            {
                                //Type = 0 Tính tổng theo Khách hàng, Xe trộn, Mác bê tông
                                //Type = 1 Trường hợp không xem theo khách hàng
                                //Type = 2 Trường hợp không xem theo xe trộn
                                //Type = 3 Trường hợp không xem theo mác bê tông
                                //Type = 4 truong hop khong xem theo hang muc

                                //Lấy Dữ liệu tổng theo Tên khách hàng, tên xe và tên mác bê tông
                                finalresult = GroupBySum(DLThongKe, 0, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);

                                //Trường hợp không xem theo khách hàng, cột khách hàng để là "Tất cả"
                                if (paging.ckbKhachHang == false)
                                {
                                    finalresult = GroupBySum(finalresult, 1, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }

                                //Trường hợp không xem theo Xe trộn, Cột Xe trộn để "Tất cả"
                                if (paging.ckbXeTron == false)
                                {
                                    finalresult = GroupBySum(finalresult, 2, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }

                                //Trường hợp không xem theo Mác bê tông, mác bê tông để "Tất cả"
                                if (paging.ckbMacBeTong == false)
                                {
                                    finalresult = GroupBySum(finalresult, 3, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }
                            }
                            if (finalresult.Count > 0)
                            {
                                //Đưa dữ liệu vào datatable để hiện thị trên gridview
                                int stt = 1;
                                int sttdaucat = 0;
                                int sttdauda = 0;
                                int sttdauximang = 0;
                                int sttdaunuoc = 0;
                                int sttdauphugia = 0;

                                List<string> row = new List<string>();

                                foreach (var item in finalresult)
                                {
                                    row = new List<string>();
                                    sttdaucat = 0;
                                    sttdauda = 0;
                                    sttdauximang = 0;
                                    sttdaunuoc = 0;
                                    sttdauphugia = 0;

                                    row.Add(item.NGAYTRON);
                                    row.Add(item.GIOBATDAU);
                                    row.Add(item.GIOXONG);
                                    row.Add(item.TENKHACHHANG);
                                    row.Add(item.BIENSO);
                                    row.Add(item.TENMACBETONG);
                                    row.Add(item.TENNV);
                                    if (item.M3METRON == 0)
                                    {
                                        row.Add("0");
                                    }
                                    else
                                    {
                                        row.Add(Math.Round(item.M3METRON, 1, MidpointRounding.AwayFromZero).ToString("#,##0.00"));
                                    }

                                    foreach (var itemLOAIVL in STTLOAIVLs)
                                    {
                                        if (itemLOAIVL == "CAT")
                                        {
                                            if (item.listcats.Count() - sttdaucat <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                var gatri1 = item.listcats[sttdaucat++];
                                                if (gatri1 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri1, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                                var gatri2 = item.listcats[sttdaucat++];
                                                if (gatri2 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri2, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                            }
                                        }
                                        else if (itemLOAIVL == "DA")
                                        {
                                            if (item.listdas.Count() - sttdauda <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                var gatri1 = item.listdas[sttdauda++];
                                                if (gatri1 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri1, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                                var gatri2 = item.listdas[sttdauda++];
                                                if (gatri2 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri2, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                            }
                                        }
                                        else if (itemLOAIVL == "XIMANG")
                                        {
                                            if (item.listximangs.Count() - sttdauximang <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                var gatri1 = item.listximangs[sttdauximang++];
                                                if (gatri1 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri1, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                                var gatri2 = item.listximangs[sttdauximang++];
                                                if (gatri2 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri2, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                            }
                                        }
                                        else if (itemLOAIVL == "NUOC")
                                        {
                                            if (item.listnuocs.Count() - sttdaunuoc <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                var gatri1 = item.listnuocs[sttdaunuoc++];
                                                if (gatri1 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri1, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                                var gatri2 = item.listnuocs[sttdaunuoc++];
                                                if (gatri2 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri2, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0"));
                                                }
                                            }
                                        }
                                        else if (itemLOAIVL == "PHUGIA")
                                        {
                                            if (item.listphugias.Count() - sttdauphugia <= 0)
                                            {
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                var gatri1 = item.listphugias[sttdauphugia++];
                                                if (gatri1 == 0)
                                                {
                                                    row.Add("0");
                                                }
                                                else
                                                {
                                                    row.Add(Math.Round(gatri1, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"));
                                                }

                                            }
                                        }
                                    }

                                    if (paging.status == 1)
                                    {
                                        if (item.tenphugias.Count() - sttdauphugia <= 0)
                                        {
                                            row.Add("0");
                                        }
                                        else
                                        {
                                            //Lấy tên phụ gia
                                            string tenphugia = item.tenphugias[0];
                                            for (int i = 1; i < socuaphugia; i++)
                                            {
                                                tenphugia = tenphugia + ", " + item.tenphugias[i];
                                            }

                                            row.Add(tenphugia);
                                        }
                                    }
                                    row.Add(item.name);
                                    grvDLThongKe.Rows.Add(row.ToArray());
                                }

                                def.metadata = grvDLThongKe.Select().Count();
                                if (paging.page_size > 0)
                                {
                                    var topRows = grvDLThongKe.Select().Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                                    def.data = topRows;
                                }
                                //Tính tổng các thành phần
                                List<decimal> tongcats = new List<decimal>();
                                List<decimal> tongdas = new List<decimal>();
                                List<decimal> tongximangs = new List<decimal>();
                                List<decimal> tongnuocs = new List<decimal>();
                                List<decimal> tongphugias = new List<decimal>();
                                decimal tongVatLieu = 0;
                                float tongM3Khoi = 0;

                                //Khởi tạo giá trị 0 cho list tổng các thành phần
                                for (int i = 0; i < socuacat; i++)
                                {
                                    tongcats.Add(0);
                                }
                                for (int i = 0; i < socuada; i++)
                                {
                                    tongdas.Add(0);
                                }
                                for (int i = 0; i < socuaximang; i++)
                                {
                                    tongximangs.Add(0);
                                }
                                for (int i = 0; i < socuanuoc; i++)
                                {
                                    tongnuocs.Add(0);
                                }
                                for (int i = 0; i < socuaphugia; i++)
                                {
                                    tongphugias.Add(0);
                                }

                                //Tính tổng các thành phần
                                foreach (var item in finalresult)
                                {
                                    tongM3Khoi += item.M3METRON;

                                    sttdaucat = 0;
                                    sttdauda = 0;
                                    sttdauximang = 0;
                                    sttdaunuoc = 0;
                                    sttdauphugia = 0;

                                    for (int i = 0; i < socuacat; i++)
                                    {
                                        if (item.listcats.Count() - sttdaucat <= 0)
                                        {
                                            tongcats[i] += 0;
                                        }
                                        else
                                        {
                                            tongcats[i] += Decimal.Round((decimal)item.listcats[sttdaucat], 1) + Decimal.Round((decimal)item.listcats[sttdaucat + 1], 1);
                                        }
                                        sttdaucat += 2;
                                    }
                                    for (int i = 0; i < socuada; i++)
                                    {
                                        if (item.listdas.Count() - sttdauda <= 0)
                                        {
                                            tongdas[i] += 0;
                                        }
                                        else
                                        {
                                            tongdas[i] += Decimal.Round((decimal)item.listdas[sttdauda], 1) + Decimal.Round((decimal)item.listdas[sttdauda + 1], 1);
                                        }
                                        sttdauda += 2;
                                    }
                                    for (int i = 0; i < socuaximang; i++)
                                    {
                                        if (item.listximangs.Count() - sttdauximang <= 0)
                                        {
                                            tongximangs[i] += 0;
                                        }
                                        else
                                        {
                                            tongximangs[i] += Decimal.Round((decimal)item.listximangs[sttdauximang], 1) + Decimal.Round((decimal)item.listximangs[sttdauximang + 1], 1);
                                        }
                                        sttdauximang += 2;
                                    }
                                    for (int i = 0; i < socuanuoc; i++)
                                    {
                                        if (item.listnuocs.Count() - sttdaunuoc <= 0)
                                        {
                                            tongnuocs[i] += 0;
                                        }
                                        else
                                        {
                                            tongnuocs[i] += Decimal.Round((decimal)item.listnuocs[sttdaunuoc], 1) + Decimal.Round((decimal)item.listnuocs[sttdaunuoc + 1], 1);
                                        }
                                        sttdaunuoc += 2;
                                    }
                                    for (int i = 0; i < socuaphugia; i++)
                                    {
                                        if (item.listphugias.Count() - sttdauphugia <= 0)
                                        {
                                            tongphugias[i] += 0;
                                        }
                                        else
                                        {
                                            tongphugias[i] += Decimal.Round((decimal)item.listphugias[sttdauphugia], 1);
                                        }
                                        sttdauphugia++;
                                    }
                                }

                                //Tính tổng Vật liệu
                                for (int i = 0; i < socuacat; i++)
                                {
                                    tongVatLieu += tongcats[i];
                                }
                                for (int i = 0; i < socuada; i++)
                                {
                                    tongVatLieu += tongdas[i];
                                }
                                for (int i = 0; i < socuaximang; i++)
                                {
                                    tongVatLieu += tongximangs[i];
                                }
                                for (int i = 0; i < socuanuoc; i++)
                                {
                                    tongVatLieu += tongnuocs[i];
                                }
                                for (int i = 0; i < socuaphugia; i++)
                                {
                                    tongVatLieu += tongphugias[i];
                                }
                                DataTable Tong = new DataTable();
                                Tong.Columns.Add(new DataColumn("VATLIEU", typeof(String)));
                                Tong.Columns.Add(new DataColumn("CAT", typeof(String)));
                                Tong.Columns.Add(new DataColumn("DA", typeof(String)));
                                Tong.Columns.Add(new DataColumn("XIMANG", typeof(String)));
                                Tong.Columns.Add(new DataColumn("NUOC", typeof(String)));
                                Tong.Columns.Add(new DataColumn("PHUGIA", typeof(String)));

                                for (int i = 0; i < maxSoCua; i++)
                                {
                                    var row1 = new List<string>();
                                    row1.Add((i + 1).ToString());
                                    row1.Add("0");
                                    row1.Add("0");
                                    row1.Add("0");
                                    row1.Add("0");
                                    row1.Add("0");
                                    Tong.Rows.Add(row1.ToArray());
                                }
                                for (int i = 0; i < socuacat; i++)
                                {
                                    Tong.Rows[i]["CAT"] = tongcats[i].ToString("#,##0.0");
                                }

                                for (int i = 0; i < socuada; i++)
                                {
                                    try
                                    {
                                        Tong.Rows[i]["DA"] = tongdas[i].ToString("#,##0.0");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                for (int i = 0; i < socuaximang; i++)
                                {
                                    Tong.Rows[i]["XIMANG"] = tongximangs[i].ToString("#,##0.0");
                                }

                                for (int i = 0; i < socuanuoc; i++)
                                {
                                    Tong.Rows[i]["NUOC"] = tongnuocs[i].ToString("#,##0.0");
                                }

                                for (int i = 0; i < socuaphugia; i++)
                                {
                                    Tong.Rows[i]["PHUGIA"] = tongphugias[i].ToString("#,##0.00");
                                }
                                var row12 = new List<string>();
                                row12.Add("DV");
                                row12.Add("KG");
                                row12.Add("KG");
                                row12.Add("KG");
                                row12.Add("LÍT");
                                row12.Add("KG");
                                Tong.Rows.Add(row12.ToArray());
                                def.data1 = Tong;
                                def.tongKg = Math.Round(tongVatLieu, 1, MidpointRounding.AwayFromZero).ToString("#,##0.0") + " kg";
                                def.tongm3 = Math.Round(tongM3Khoi, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00") + " m3";
                            }

                        }

                        def.meta = new Meta(200, "Success");
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

        [HttpGet("GetReport")]
        public HttpResponseMessage GetReport([FromQuery] FilteredPagination paging)
        {
            if (paging.TENKHACHHANG == "null" || paging.TENKHACHHANG == "undefined")
            {
                paging.TENKHACHHANG = "";
            }
            if (paging.BIENSO == "null" || paging.BIENSO == "undefined")
            {
                paging.BIENSO = "";
            }
            if (paging.TENMACBETONG == "null" || paging.TENMACBETONG == "undefined")
            {
                paging.TENMACBETONG = "";
            }
            if (paging.Branchlist == "null" || paging.Branchlist == "undefined")
            {
                paging.Branchlist = "";
            }
            if (paging.TENNV == "null" || paging.TENNV == "undefined")
            {
                paging.TENNV = "";
            }
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

                    List<rpthongkeDTO> rpdonhang = new List<rpthongkeDTO>();
              //      command.CommandText = "  SELECT MALSTRON, MACHITIETMETRON, NGAYTRON, GIOBATDAU, GIOXONG, BIENSO, TENMACBETONG, M3METRON, SOLUONG, SOLUONGT,COPHAIPHUGIA, TENCUAVL, TENLOAIVL, USERNAME,TENKHACHHANG,TENDUAN,DIADIEMXD,TENNV,TENHANGMUC,name FROM\n ";
                  command.CommandText = "  SELECT MALSTRON, MACHITIETMETRON, NGAYTRON, GIOBATDAU, GIOXONG, BIENSO, TENMACBETONG, M3METRON, SOLUONG, SOLUONGT, ISNULL(COPHAIPHUGIA,0) COPHAIPHUGIA, ISNULL(TENCUAVL,'') TENCUAVL, ISNULL(TENLOAIVL,'') TENLOAIVL, USERNAME,TENKHACHHANG,TENDUAN,DIADIEMXD,TENNV,TENHANGMUC,name FROM ";
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
                                        command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                        command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                        command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                        command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                        command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                        command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                        command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
                                    if (paging.query != null)
                                    {
                                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query) + " AND br.Status = 1";
                                    }
                                    else
                                    {
                                        command.CommandText += " WHERE br.Status = 1";
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
                                return null;
                            }
                            int k = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (k == 0)
                                {
                                    command.CommandText += "SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                                    command.CommandText += " UNION ALL SELECT [t1].[MALSTRON], [t0].[MACHITIETMETRON], [t2].[NGAYTRON], [t2].[GIOBATDAU], [t2].[GIOXONG], [t2].[BIENSO], [t2].[TENMACBETONG], [t1].[M3METRON], [t0].[SOLUONG], [t0].[SOLUONGT], [t4].[COPHAIPHUGIA], [t3].[TENCUAVL], [t4].[TENLOAIVL], [t2].[USERNAME],[t5].TENKHACHHANG,[t5].TENDUAN,[t5].DIADIEMXD,[t5].TENNV,[t5].TENHANGMUC, br.name FROM [" + branch.Dataname + "].[dbo].[LSCHITIETMETRONLSCUAVL] AS [t0]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCHITIETMETRON] AS [t1] ON [t1].[MACHITIETMETRON] = [t0].[MACHITIETMETRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSTRON] AS [t2] ON [t2].[MALSTRON] = [t1].[MALSTRON]\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSDATHANG] AS [t5] ON [t5].STT = [t2].STTLSDATHANG\n ";
                                    command.CommandText += "LEFT JOIN [" + branch.Dataname + "].[dbo].[LSCUAVL] AS [t3] ON [t3].[MACUAVL] = [t0].[MACUAVL]\n ";
                                    command.CommandText += "LEFT OUTER JOIN [" + branch.Dataname + "].[dbo].[LSLOAIVL] AS [t4] ON [t4].[MALOAIVL] = [t3].[MALOAIVL]\n ";
                                    command.CommandText += "LEFT JOIN Branch br ON br.Dataname = '" + branch.Dataname + "'";
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
                            try
                            {
                                rpthongkeDTO item = new rpthongkeDTO();
                                item.MALSTRON = (long)result["MALSTRON"];
                                item.MACHITIETMETRON = (long)result["MACHITIETMETRON"];
                                item.NGAYTRON = (DateTime)result["NGAYTRON"];
                                item.GIOBATDAU = (DateTime)result["GIOBATDAU"];
                                item.GIOXONG = (DateTime)result["GIOXONG"];
                                item.BIENSO = (string)result["BIENSO"];
                                item.TENMACBETONG = (string)result["TENMACBETONG"];
                                item.M3METRON = (Single)result["M3METRON"];
                                item.SOLUONG = (Single)result["SOLUONG"];
                                item.SOLUONGT = (Single)result["SOLUONGT"];
                                item.COPHAIPHUGIA = (bool)result["COPHAIPHUGIA"];
                                item.TENCUAVL = (string)result["TENCUAVL"];
                                item.TENLOAIVL = (string)result["TENLOAIVL"];
                                if (result["USERNAME"] is System.DBNull)
                                {
                                    item.USERNAME = "";

                                }
                                else
                                {
                                    item.USERNAME = (string)result["USERNAME"];
                                }
                                item.TENKHACHHANG = (string)result["TENKHACHHANG"];
                                item.TENDUAN = (string)result["TENDUAN"];
                                item.DIADIEMXD = (string)result["DIADIEMXD"];
                                if (result["TENNV"] is System.DBNull)
                                {
                                    item.TENNV = "";

                                }
                                else
                                {
                                    item.TENNV = (string)result["TENNV"];
                                }
                                item.TENHANGMUC = (string)result["TENHANGMUC"];
                                item.name = (string)result["name"];
                                rpdonhang.Add(item);
                            }catch (Exception ext)
                            {
                                string loi = ext.ToString();
                            }

                        }
                        if (rpdonhang.Count > 0)
                        {
                            DataTable grvDLThongKe = new DataTable();
                            grvDLThongKe.Columns.Add(new DataColumn("NGAYTRON", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("GIOBATDAU", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("GIOXONG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENKHACHHANG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENDUAN", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENHANGMUC", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("DIADIEMXD", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("BIENSO", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENMACBETONG", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("TENNV", typeof(String)));
                            grvDLThongKe.Columns.Add(new DataColumn("M3METRON", typeof(String)));

                            // Lấy tên số liệu các thành phần của mác bê tông.
                            var tenCuaVatLieu = listVatLieu(paging.companyid, paging.Branchlist);


                            foreach (var item in tenCuaVatLieu)
                            {
                                if (item.TENCUAVL != "")
                                {
                                    grvDLThongKe.Columns.Add(new DataColumn(item.TENCUAVL, typeof(String)));
                                    if (!item.COPHAIPHUGIA)
                                    {
                                        grvDLThongKe.Columns.Add(new DataColumn("T." + item.TENCUAVL, typeof(String)));
                                    }
                                }
                            }

                            if (paging.status == 1)
                            {
                                grvDLThongKe.Columns.Add(new DataColumn("TENPHUGIA", typeof(String)));
                            }
                            grvDLThongKe.Columns.Add(new DataColumn("name", typeof(String)));
                            //Lấy tên loại và số cửa các cửa vật liệu                
                            int socuavatlieu = 0;
                            int socuacat = 0;
                            int socuada = 0;
                            int socuaximang = 0;
                            int socuanuoc = 0;
                            int socuaphugia = 0;

                            List<string> STTLOAIVLs = new List<string>();

                            foreach (var item in tenCuaVatLieu)
                            {
                                if (item.TENLOAIVL == "CAT")
                                {
                                    socuacat++;
                                    STTLOAIVLs.Add("CAT");
                                }
                                else if (item.TENLOAIVL == "DA")
                                {
                                    socuada++;
                                    STTLOAIVLs.Add("DA");
                                }
                                else if (item.TENLOAIVL == "XIMANG")
                                {
                                    socuaximang++;
                                    STTLOAIVLs.Add("XIMANG");
                                }
                                else if (item.TENLOAIVL == "NUOC")
                                {
                                    socuanuoc++;
                                    STTLOAIVLs.Add("NUOC");
                                }
                                else if (item.TENLOAIVL == "PHUGIA")
                                {
                                    socuaphugia++;
                                    STTLOAIVLs.Add("PHUGIA");
                                }
                                socuavatlieu++;
                            }

                            var DLThongKe = new List<DULIEUTHONGKE>();
                            var finalresult = new List<DULIEUTHONGKE>();
                            DULIEUTHONGKE rowThongKe = new DULIEUTHONGKE();
                            for (int i = 0; i < rpdonhang.Count; i++)
                            {
                                if (i % socuavatlieu == 0)
                                {
                                    if (i != 0)
                                    {
                                        //Đưa vào danh sách Dữ liệu thống kê
                                        DLThongKe.Add(rowThongKe);
                                    }
                                    rowThongKe = new DULIEUTHONGKE();
                                    rowThongKe.STT = rpdonhang[i].MACHITIETMETRON.ToString();
                                    rowThongKe.NGAYTRON = rpdonhang[i].NGAYTRON.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    //Định dạng thời gian về dạng hh:mm AM/PM
                                    TimeSpan timespan = new TimeSpan(rpdonhang[i].GIOBATDAU.Hour, rpdonhang[i].GIOBATDAU.Minute, 00);
                                    DateTime time = DateTime.Today.Add(timespan);
                                    rowThongKe.GIOBATDAU = time.ToString("hh:mm tt");

                                    rowThongKe.NGAYGIOTRON = rpdonhang[i].NGAYTRON.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + time.ToString("hh:mm");

                                    timespan = new TimeSpan(rpdonhang[i].GIOXONG.Hour, rpdonhang[i].GIOXONG.Minute, 00);
                                    time = DateTime.Today.Add(timespan);
                                    rowThongKe.GIOXONG = time.ToString("hh:mm tt");

                                    rowThongKe.TENKHACHHANG = rpdonhang[i].TENKHACHHANG;
                                    rowThongKe.TENDUAN = rpdonhang[i].TENDUAN;
                                    rowThongKe.TENHANGMUC = rpdonhang[i].TENHANGMUC;
                                    rowThongKe.DIADIEMXD = rpdonhang[i].DIADIEMXD;
                                    rowThongKe.BIENSO = rpdonhang[i].BIENSO;
                                    rowThongKe.TENMACBETONG = rpdonhang[i].TENMACBETONG;
                                    rowThongKe.TENNV = rpdonhang[i].TENNV;
                                    rowThongKe.M3METRON = rpdonhang[i].M3METRON;
                                    //Khởi tạo các list
                                    rowThongKe.listcats = new List<float>();
                                    rowThongKe.listdas = new List<float>();
                                    rowThongKe.listximangs = new List<float>();
                                    rowThongKe.listnuocs = new List<float>();
                                    rowThongKe.listphugias = new List<float>();
                                    rowThongKe.tenphugias = new List<string>();

                                    //Lấy dữ liệu các thành phần                            
                                    if (rpdonhang[i].TENLOAIVL == "CAT")
                                    {
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "DA")
                                    {
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "XIMANG")
                                    {
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "NUOC")
                                    {
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "PHUGIA")
                                    {
                                        rowThongKe.listphugias.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.tenphugias.Add(rpdonhang[i].TENCUAVL);
                                    }
                                    rowThongKe.name = rpdonhang[i].name;

                                }
                                else
                                {
                                    //Lấy dữ liệu các thành phần                            
                                    if (rpdonhang[i].TENLOAIVL == "CAT")
                                    {
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listcats.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "DA")
                                    {
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listdas.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "XIMANG")
                                    {
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listximangs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "NUOC")
                                    {
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.listnuocs.Add(rpdonhang[i].SOLUONGT);
                                    }
                                    else if (rpdonhang[i].TENLOAIVL == "PHUGIA")
                                    {
                                        rowThongKe.listphugias.Add(rpdonhang[i].SOLUONG);
                                        rowThongKe.tenphugias.Add(rpdonhang[i].TENCUAVL);
                                    }

                                    if (i == (rpdonhang.Count - 1))
                                    {
                                        //Đưa vào danh sách Dữ liệu thống kê
                                        DLThongKe.Add(rowThongKe);
                                    }
                                }
                            }
                            if (paging.status == 1)
                            {
                                finalresult = DLThongKe.Where(x => (paging.TENKHACHHANG == "" || x.TENKHACHHANG == paging.TENKHACHHANG)
                                                            && (paging.BIENSO == "" || x.BIENSO == paging.BIENSO)
                                                            && (paging.TENMACBETONG == "" || x.TENMACBETONG == paging.TENMACBETONG)
                                                             && (paging.TENNV == "" || x.TENNV == paging.TENNV)
                                                            ).ToList();
                            }//Trường hợp xem thống kê tổng
                            else
                            {
                                //Type = 0 Tính tổng theo Khách hàng, Xe trộn, Mác bê tông
                                //Type = 1 Trường hợp không xem theo khách hàng
                                //Type = 2 Trường hợp không xem theo xe trộn
                                //Type = 3 Trường hợp không xem theo mác bê tông
                                //Type = 4 truong hop khong xem theo hang muc

                                //Lấy Dữ liệu tổng theo Tên khách hàng, tên xe và tên mác bê tông
                                finalresult = GroupBySum(DLThongKe, 0, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);

                                //Trường hợp không xem theo khách hàng, cột khách hàng để là "Tất cả"
                                if (paging.ckbKhachHang == false)
                                {
                                    finalresult = GroupBySum(finalresult, 1, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }

                                //Trường hợp không xem theo Xe trộn, Cột Xe trộn để "Tất cả"
                                if (paging.ckbXeTron == false)
                                {
                                    finalresult = GroupBySum(finalresult, 2, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }

                                //Trường hợp không xem theo Mác bê tông, mác bê tông để "Tất cả"
                                if (paging.ckbMacBeTong == false)
                                {
                                    finalresult = GroupBySum(finalresult, 3, socuacat, socuada, socuaximang, socuanuoc, socuaphugia, paging.tungay, paging.denngay);
                                }
                            }
                            if (finalresult.Count > 0)
                            {
                                //Đưa dữ liệu vào datatable để hiện thị trên gridview
                                int stt = 1;
                                int sttdaucat = 0;
                                int sttdauda = 0;
                                int sttdauximang = 0;
                                int sttdaunuoc = 0;
                                int sttdauphugia = 0;

                                List<string> row = new List<string>();

                                foreach (var item in finalresult)
                                {
                                    row = new List<string>();
                                    sttdaucat = 0;
                                    sttdauda = 0;
                                    sttdauximang = 0;
                                    sttdaunuoc = 0;
                                    sttdauphugia = 0;

                                    row.Add(item.NGAYTRON);
                                    row.Add(item.GIOBATDAU);
                                    row.Add(item.GIOXONG);
                                    row.Add(item.TENKHACHHANG);
                                    row.Add(item.TENDUAN);
                                    row.Add(item.TENHANGMUC);
                                    row.Add(item.DIADIEMXD);
                                    row.Add(item.BIENSO);
                                    row.Add(item.TENMACBETONG);
                                    row.Add(item.TENNV);
                                    row.Add(Math.Round(item.M3METRON, 2, MidpointRounding.AwayFromZero).ToString("#,##0.##"));
                                    foreach (var itemLOAIVL in STTLOAIVLs)
                                    {
                                        if (itemLOAIVL == "CAT")
                                        {
                                            if (item.listcats.Count() - sttdaucat <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                row.Add(Decimal.Round((decimal)item.listcats[sttdaucat++], 1).ToString());
                                                row.Add(Decimal.Round((decimal)item.listcats[sttdaucat++], 1).ToString());
                                            }
                                        }
                                        else if (itemLOAIVL == "DA")
                                        {
                                            if (item.listdas.Count() - sttdauda <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                row.Add(Decimal.Round((decimal)item.listdas[sttdauda++], 1).ToString());
                                                row.Add(Decimal.Round((decimal)item.listdas[sttdauda++], 1).ToString());
                                            }
                                        }
                                        else if (itemLOAIVL == "XIMANG")
                                        {
                                            if (item.listximangs.Count() - sttdauximang <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                row.Add(Decimal.Round((decimal)item.listximangs[sttdauximang++], 1).ToString());
                                                row.Add(Decimal.Round((decimal)item.listximangs[sttdauximang++], 1).ToString());
                                            }
                                        }
                                        else if (itemLOAIVL == "NUOC")
                                        {
                                            if (item.listnuocs.Count() - sttdaunuoc <= 0)
                                            {
                                                row.Add("0");
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                row.Add(Decimal.Round((decimal)item.listnuocs[sttdaunuoc++], 1).ToString());
                                                row.Add(Decimal.Round((decimal)item.listnuocs[sttdaunuoc++], 1).ToString());
                                            }
                                        }
                                        else if (itemLOAIVL == "PHUGIA")
                                        {
                                            if (item.listphugias.Count() - sttdauphugia <= 0)
                                            {
                                                row.Add("0");
                                            }
                                            else
                                            {
                                                row.Add(Decimal.Round((decimal)item.listphugias[sttdauphugia++], 2).ToString());
                                            }
                                        }
                                    }

                                    if (paging.status == 1)
                                    {
                                        if (item.tenphugias.Count() - sttdauphugia <= 0)
                                        {
                                            row.Add("0");
                                        }
                                        else
                                        {
                                            //Lấy tên phụ gia
                                            string tenphugia = item.tenphugias[0];
                                            for (int i = 1; i < socuaphugia; i++)
                                            {
                                                tenphugia = tenphugia + ", " + item.tenphugias[i];
                                            }
                                            row.Add(tenphugia);
                                        }
                                    }
                                    row.Add(item.name);
                                    grvDLThongKe.Rows.Add(row.ToArray());
                                }
                                // khởi tạo wb rỗng
                                XSSFWorkbook wb = new XSSFWorkbook();
                                // Tạo ra 1 sheet
                                ISheet sheet = wb.CreateSheet();

                                string fileName = "Bao-cao-ke-toan-2";
                                string template = @"template\export\BCKT1.xlsx";
                                string webRootPath = _hostingEnvironment.WebRootPath;
                                string templatePath = Path.Combine(webRootPath, template);
                                string today = paging.denngay.Day.ToString() + "/" + paging.denngay.Month.ToString() + "/" + paging.denngay.Year.ToString();
                                string fromday = paging.tungay.Day.ToString() + "/" + paging.tungay.Month.ToString() + "/" + paging.tungay.Year.ToString();
                                MemoryStream ms = writeAccountantTwoToExcel(templatePath, 0, grvDLThongKe, today, fromday, paging.companyid, paging.Branchlist);

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
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {
                                    if (i == 0)
                                    {
                                        command.CommandText += "SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
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
                                    command.CommandText += "SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
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
                                    command.CommandText += "SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENNV AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") rpdonhang WHERE Name IS NOT NULL";

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

        [HttpGet("GetBienSo")]
        public IActionResult GetBienSo([FromQuery] FilteredPagination paging)
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
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {
                                    if (i == 0)
                                    {
                                        command.CommandText += "SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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
                                    command.CommandText += "SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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
                                    command.CommandText += "SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT BIENSO AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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

        [HttpGet("GetKH")]
        public IActionResult GetKH([FromQuery] FilteredPagination paging)
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
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {
                                    if (i == 0)
                                    {
                                        command.CommandText += "SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
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
                                    command.CommandText += "SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
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
                                    command.CommandText += "SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENKHACHHANG AS Name FROM [" + branch.Dataname + "].[dbo].[LSDATHANG]";
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

        [HttpGet("GetTenMacBeTong")]
        public IActionResult GetTenMacBeTong([FromQuery] FilteredPagination paging)
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
                                Branch branch = context.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(x => x.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if (branch != null)
                                {
                                    if (i == 0)
                                    {
                                        command.CommandText += "SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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
                                    command.CommandText += "SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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
                                    command.CommandText += "SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT TENMACBETONG AS Name FROM [" + branch.Dataname + "].[dbo].[LSTRON]";
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


