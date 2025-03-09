using IOITWebApp;
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
using System.Data;


namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("customer", "customer");
        private static string functionCode = "QLCP";
        private IHostingEnvironment _hostingEnvironment;

        public CustomerController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        private int socuavlmax;
        public string[] header;
        public List<string[]> data;
        private DataTable CreateGridTable(string dbname)
        {
            // Create an unbound DataGridView by declaring a column count.
            var dt = new DataTable();

            // Lấy số liệu các mác bê tông
            string connectString = string.Format("data source = {0}; initial catalog = {1}; UID= {2}; PWD={3}", "dangnhap.net,5000", dbname, "tramtron", "ttsmart!@#!@#");
            DataTable dtMacBeTong = CommonLib.GetDataBySql("SELECT * FROM MACBETONG");
            DataTable dtCuaVL = CommonLib.GetDataBySql("SELECT * FROM CUAVL");

            // Lấy STT và mã định mức vật liệu.
            dt.Columns.Add(new DataColumn("STT", typeof(String)));
            dt.Columns.Add(new DataColumn("Mác BT", typeof(String)));

            socuavlmax = dtCuaVL.Rows.Count;
            if (socuavlmax > 0)
            {
                header = new string[socuavlmax + 5];
                data = new List<string[]>();
                header[0] = "STT";
                header[1] = "Mác BT";
                int counter = 2;
                // Lấy số liệu các thành phần của mác bê tông
                foreach (DataRow rowCuavl in dtCuaVL.Rows)
                {
                    if (!dt.Columns.Contains(rowCuavl["TENCUAVL"].ToString()))
                    {
                        dt.Columns.Add(new DataColumn(rowCuavl["TENCUAVL"].ToString(), typeof(String)));
                        header[counter] = rowCuavl["TENCUAVL"].ToString();
                        counter++;
                    }
                }

                // Lấy STT và mã định mức vật liệu.
                header[counter] = "Cường Độ";
                header[counter + 1] = "Cốt Liệu Max";
                header[counter + 2] = "Độ sụt";
                dt.Columns.Add(new DataColumn("Cường Độ", typeof(String)));
                dt.Columns.Add(new DataColumn("Cốt Liệu Max", typeof(String)));
                dt.Columns.Add(new DataColumn("Độ sụt", typeof(String)));
                //  dt.Columns.Add(new DataColumn("Độ sụt", typeof(String)));
                int counter2 = 1;
                foreach (DataRow rowMac in dtMacBeTong.Rows)
                {

                    // Chuyền dữ liệu vào grid table
                    var row = new String[socuavlmax + 5];
                    int i = 0;
                    int maMac = (int)rowMac["MAMACBETONG"];
                    row[0] = counter2.ToString();
                    counter2++;
                    // Chuyền dữ liệu bản ghi của mác bê tông
                    row[i++] = rowMac["MAMACBETONG"].ToString();
                    row[i++] = rowMac["TENMACBETONG"].ToString();

                    // Lấy số liệu các thành phần của mác bê tông
                    DataTable dtSoLuongVL = CommonLib.GetDataBySql(string.Format("SELECT * FROM SOLUONGVL WHERE MAMACBETONG = {0}", maMac));

                    foreach (DataRow rowSoluongvl in dtSoLuongVL.Rows)
                    {
                        row[i++] = Decimal.Parse(rowSoluongvl["SOLUONG"].ToString(), System.Globalization.NumberStyles.Any).ToString();
                    }

                    // Lấy STT và mã định mức vật liệu.
                    row[i++] = rowMac["CUONGDO"].ToString();
                    row[i++] = rowMac["COTLIEUMAX"].ToString();
                    row[i] = rowMac["DOSUT"].ToString();
                    /*
                     * 
                     *  var row = new String[socuavlmax + 5];
                    int i = 0;

                    // Chuyền dữ liệu bản ghi của mác bê tông
                    row[i++] = macbetong.MAMACBETONG.ToString();
                    row[i++] = macbetong.TENMACBETONG;

                    // Lấy số liệu các thành phần của mác bê tông
                    foreach (SOLUONGVL soluongvl in macbetong.SOLUONGVLs.ToList<SOLUONGVL>())
                    {
                        row[i++] = Decimal.Parse(soluongvl.SOLUONG.GetValueOrDefault().ToString(), System.Globalization.NumberStyles.Any).ToString();
                    }

                    // Lấy STT và mã định mức vật liệu.
                    row[i++] = macbetong.CUONGDO.ToString();
                    row[i++] = macbetong.COTLIEUMAX.ToString();
                    row[i] = macbetong.DOSUT;
                     * */
                    // Thêm vào bảng
                    dt.Rows.Add(row);
                    data.Add(row);
                }
            }
            return dt;
        }

        // GET: api/Customer
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
            if (paging != null && paging.Branchlist != null)
            {
                using (var context = new CNTTVNWebContext())
                {
                    var arrListStr = paging.Branchlist.Split(',');
                    Branch branch = context.Branch.Find(Convert.ToInt32(arrListStr[0]));
                    DataTable dtResult = CreateGridTable(branch.Dataname);
                    if (data != null)
                        def.metadata = data.Count();
                    def.data = data;
                    if (paging.page_size > 0 && data != null)
                    {
                        //    var topRows = grvDLThongKe.Select().Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        def.data = data.Skip((paging.page - 1) * paging.page_size).Take(paging.page_size); ;
                    }
                    def.data1 = header;
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }

                //using (var context = new CNTTVNWebContext())
                //using (var command = context.Database.GetDbConnection().CreateCommand())
                //{

                //    List<CustomerbodyDTO> rpdonhang = new List<CustomerbodyDTO>();
                //    command.CommandText = " SELECT MAMACBETONG,TENMACBETONG,CUONGDO,VL_1,VL_2,VL_3,VL_4,VL_5,VL_6,VL_7,VL_8,VL_9,COTLIEUMAX,DOSUT,name  FROM ";
                //    command.CommandText += "(";
                //    if (paging.Branchlist != "" && paging.Branchlist != null)
                //    {
                //        var arrListStr = paging.Branchlist.Split(',');
                //        int i = 0;
                //        foreach (var item in arrListStr)
                //        {
                //            if (item != "")
                //            {
                //                Branch branch = context.Branch.Find(Convert.ToInt32(item));
                //                if (i == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //            }
                //            ++i;
                //        }
                //    }
                //    else
                //    {
                //        if (paging.companyid == 0)
                //        {
                //            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                //            if (branchlist.Count() == 0)
                //            {
                //                def.data = null;
                //                def.metadata = 0;
                //                def.meta = new Meta(200, "Success");
                //                return Ok(def);
                //            }
                //            int j = 0;
                //            foreach (var item in branchlist)
                //            {
                //                Branch branch = context.Branch.Find(item.BranchId);
                //                if (j == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                ++j;
                //            }
                //        }
                //        else
                //        {
                //            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == paging.companyid).ToList();
                //            if (branchlist.Count() == 0)
                //            {
                //                def.data = null;
                //                def.metadata = 0;
                //                def.meta = new Meta(200, "Success");
                //                return Ok(def);
                //            }
                //            int k = 0;
                //            foreach (var item in branchlist)
                //            {
                //                Branch branch = context.Branch.Find(item.BranchId);
                //                if (k == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                ++k;
                //            }
                //        }

                //    }
                //    command.CommandText += ") nv";

                //    string sql = command.CommandText;
                //    string sqlRun = string.Format("SELECT COUNT(*) FROM ({0}) AS A", sql);
                //    if (paging.query != null)
                //    {
                //        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                //    }
                //    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                //    command.CommandText += " SELECT *  FROM #Result ";
                //    if (paging.order_by != null)
                //    {
                //        command.CommandText += " ORDER BY " + paging.order_by;
                //    }
                //    else
                //    {
                //        command.CommandText += " ORDER BY nv.name asc";
                //    }
                //    command.CommandText += " OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY;";

                //    command.CommandText = sqlRun + "\n" + command.CommandText;
                //    command.CommandText += " DROP TABLE #Result; ";
                //    context.Database.OpenConnection();
                //    using (var result = command.ExecuteReader())
                //    {
                //        result.Read();
                //        def.metadata = result[0];
                //        result.NextResult();
                //        while (result.Read())
                //        {
                //            CustomerbodyDTO item = new CustomerbodyDTO();
                //            item.MAMACBETONG = (int)result["MAMACBETONG"];
                //            item.TENMACBETONG = (string)result["TENMACBETONG"];
                //            item.CUONGDO = (int)result["CUONGDO"];
                //            item.VL_1 = (Single)result["VL_1"];
                //            item.VL_2 = (Single)result["VL_2"];
                //            item.VL_3 = (Single)result["VL_3"];
                //            item.VL_4 = (Single)result["VL_4"];
                //            item.VL_5 = (Single)result["VL_5"];
                //            item.VL_6 = (Single)result["VL_6"];
                //            item.VL_7 = (Single)result["VL_7"];
                //            item.VL_8 = (Single)result["VL_8"];
                //            item.VL_9 = (Single)result["VL_9"];
                //            item.COTLIEUMAX = (int)result["COTLIEUMAX"];
                //            item.DOSUT = (string)result["DOSUT"];
                //            item.name = (string)result["name"];
                //            rpdonhang.Add(item);
                //        }

                //        def.data = rpdonhang;
                //    }
                //    def.meta = new Meta(200, "Success");
                //    return Ok(def);
                //}
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

                    List<VatLieuCusDTO> rpdonhang = new List<VatLieuCusDTO>();
                    command.CommandText = " SELECT DISTINCT TENCUAVL,COPHAIPHUGIA,TENLOAIVL,MACUAVL FROM ";
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
                                        command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                    }
                                    else
                                    {
                                        command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
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
                                    command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
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
                                    command.CommandText += "SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT dh.TENCUAVL,sa.COPHAIPHUGIA,sa.TENLOAIVL,dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[LOAIVL] sa LEFT JOIN [" + branch.Dataname + "].[dbo].[CUAVL] dh ON sa.MALOAIVL = dh.MALOAIVL";
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
                            VatLieuCusDTO item = new VatLieuCusDTO();
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
                            item.MACUAVL = (int)result["MACUAVL"];
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
            if (paging.Branchlist == null)
            {
                return null;
            }
            if (paging != null)
            {

                using (var context = new CNTTVNWebContext())
                {
                    var arrListStr = paging.Branchlist.Split(',');
                    Branch branch = context.Branch.Find(Convert.ToInt32(arrListStr[0]));
                    DataTable dtResult = CreateGridTable(branch.Dataname);
                    if (data != null)
                        def.metadata = data.Count();
                    def.data = data;
                    //if (paging.page_size > 0 && data != null)
                    //{
                    //    //    var topRows = grvDLThongKe.Select().Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    //    def.data = data.Skip((paging.page - 1) * paging.page_size).Take(paging.page_size); ;
                    //}
                    def.data1 = header;

                    XSSFWorkbook wb = new XSSFWorkbook();
                    // Tạo ra 1 sheet
                    ISheet sheet = wb.CreateSheet();

                    string fileName = "Quan-tri-cap-phoi";
                    string template = @"template\export\QTCP.xlsx";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string templatePath = Path.Combine(webRootPath, template);
                    MemoryStream ms = writeAccountantTwoToExcel2(templatePath, 0, branch);

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
                    //  def.meta = new Meta(200, "Success");
                    //     return Ok(def);
                }
                //         using (var context = new CNTTVNWebContext())
                //using (var command = context.Database.GetDbConnection().CreateCommand())
                //{

                //    List<CustomerbodyDTO> rpdonhang = new List<CustomerbodyDTO>();
                //    command.CommandText = " SELECT MAMACBETONG,TENMACBETONG,CUONGDO,VL_1,VL_2,VL_3,VL_4,VL_5,VL_6,VL_7,VL_8,VL_9,COTLIEUMAX,DOSUT,name INTO #Result FROM ";
                //    command.CommandText += "(";
                //    if (paging.Branchlist != "" && paging.Branchlist != null)
                //    {
                //        var arrListStr = paging.Branchlist.Split(',');
                //        int i = 0;
                //        foreach (var item in arrListStr)
                //        {
                //            if (item != "")
                //            {
                //                Branch branch = context.Branch.Find(Convert.ToInt32(item));
                //                if (i == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //            }
                //            ++i;
                //        }
                //    }
                //    else
                //    {
                //        if (paging.companyid == 0)
                //        {
                //            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                //            if (branchlist.Count() == 0)
                //            {
                //                return null;
                //            }
                //            int j = 0;
                //            foreach (var item in branchlist)
                //            {
                //                Branch branch = context.Branch.Find(item.BranchId);
                //                if (j == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                ++j;
                //            }
                //        }
                //        else
                //        {
                //            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == paging.companyid).ToList();
                //            if (branchlist.Count() == 0)
                //            {
                //                return null;
                //            }
                //            int k = 0;
                //            foreach (var item in branchlist)
                //            {
                //                Branch branch = context.Branch.Find(item.BranchId);
                //                if (k == 0)
                //                {
                //                    command.CommandText += "select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                else
                //                {
                //                    command.CommandText += " UNION ALL select ma.MAMACBETONG ,ma.TENMACBETONG ,ma.CUONGDO ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 1) as VL_1 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 2) as VL_2 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 3) as VL_3 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 4) as VL_4 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 5) as VL_5 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 6) as VL_6 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 7) as VL_7 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 8) as VL_8 ,(SELECT SOLUONG FROM [" + branch.Dataname + "].dbo.SOLUONGVL WHERE MAMACBETONG =  ma.MAMACBETONG AND MACUAVL = 9) as VL_9 ,ma.COTLIEUMAX ,ma.DOSUT, br.name from [" + branch.Dataname + "].dbo.MACBETONG ma LEFT JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' WHERE br.Status = 1";
                //                }
                //                ++k;
                //            }
                //        }

                //    }
                //    command.CommandText += ") nv";
                //    if (paging.query != null)
                //    {
                //        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                //    }
                //    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                //    command.CommandText += " SELECT *  FROM #Result ";
                //    if (paging.order_by != null)
                //    {
                //        command.CommandText += " ORDER BY " + paging.order_by;
                //    }
                //    else
                //    {
                //        command.CommandText += " ORDER BY nv.name asc";
                //    }
                //    command.CommandText += " DROP TABLE #Result; ";
                //    context.Database.OpenConnection();
                //    using (var result = command.ExecuteReader())
                //    {
                //        result.Read();
                //        def.metadata = result[0];
                //        result.NextResult();
                //        while (result.Read())
                //        {
                //            CustomerbodyDTO item = new CustomerbodyDTO();
                //            item.MAMACBETONG = (int)result["MAMACBETONG"];
                //            item.TENMACBETONG = (string)result["TENMACBETONG"];
                //            item.CUONGDO = (int)result["CUONGDO"];
                //            item.VL_1 = (Single)result["VL_1"];
                //            item.VL_2 = (Single)result["VL_2"];
                //            item.VL_3 = (Single)result["VL_3"];
                //            item.VL_4 = (Single)result["VL_4"];
                //            item.VL_5 = (Single)result["VL_5"];
                //            item.VL_6 = (Single)result["VL_6"];
                //            item.VL_7 = (Single)result["VL_7"];
                //            item.VL_8 = (Single)result["VL_8"];
                //            item.VL_9 = (Single)result["VL_9"];
                //            item.COTLIEUMAX = (int)result["COTLIEUMAX"];
                //            item.DOSUT = (string)result["DOSUT"];
                //            item.name = (string)result["name"];
                //            rpdonhang.Add(item);
                //        }

                //        def.data = rpdonhang;

                //        // khởi tạo wb rỗng
                //        XSSFWorkbook wb = new XSSFWorkbook();
                //        // Tạo ra 1 sheet
                //        ISheet sheet = wb.CreateSheet();

                //        string fileName = "Quan-tri-cap-phoi";
                //        string template = @"template\export\QTCP.xlsx";
                //        string webRootPath = _hostingEnvironment.WebRootPath;
                //        string templatePath = Path.Combine(webRootPath, template);
                //        MemoryStream ms = writeAccountantTwoToExcel(templatePath, 0, rpdonhang);

                //        if (!string.IsNullOrEmpty(fileName))
                //        {
                //            var response = new HttpResponseMessage(HttpStatusCode.OK)
                //            {
                //                Content = new ByteArrayContent(ms.ToArray())
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
                //    return null;
                //}

            }
            else
            {
                return null;
            }
            return null;
        }
        public MemoryStream writeAccountantTwoToExcel(string templatePath, int sheetnumber, List<CustomerbodyDTO> data)
        {
            FileStream file1 = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 4;
            if (sheet != null)
            {
                int datasize = data.Count();
                int datacol = 15;
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
                                        row.GetCell(i).SetCellValue(data[rr].TENMACBETONG);
                                    }
                                }
                                else if (i == 2)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(C5:E" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_1, 2));
                                    }
                                }
                                else if (i == 3)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(D5:E" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_2, 2));
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
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_3, 2));
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
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_4, 2));
                                    }
                                }
                                else if (i == 6)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(G5:G" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_5, 2));
                                    }
                                }
                                else if (i == 7)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(H5:H" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_6, 2));
                                    }
                                }
                                else if (i == 8)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(I5:I" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_7, 2));
                                    }
                                }
                                else if (i == 9)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(J5:J" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_8, 2));
                                    }
                                }
                                else if (i == 10)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellFormula("SUM(K5:K" + (datasize + rowStart).ToString() + ")");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_9, 2));
                                    }
                                }
                                else if (i == 11)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].CUONGDO);
                                    }
                                }
                                else if (i == 12)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].COTLIEUMAX);
                                    }
                                }
                                else if (i == 13)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].DOSUT);
                                    }
                                }
                                else if (i == 14)
                                {
                                    if (rr == datasize)
                                    {
                                        row.GetCell(i).SetCellValue("");
                                    }
                                    else
                                    {
                                        row.GetCell(i).SetCellValue(data[rr].name);
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

        public MemoryStream writeAccountantTwoToExcel2(string templatePath, int sheetnumber, Branch branch)
        {
            FileStream file1 = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file1);
            ISheet sheet = workbook.GetSheetAt(sheetnumber);
            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            int rowStart = 4;
            if (sheet != null)
            {
                int datasize = data.Count();
                int datacol = header.Count();
                try
                {
                    //Lấy danh sách style template
                    List<ICellStyle> rowStyle = new List<ICellStyle>();
                    for (int i = 0; i <= datacol; i++)
                    {
                        rowStyle.Add(sheet.GetRow(rowStart).GetCell(i).CellStyle);
                    }

                    int rowNum = rowStart;
                    // dien header
                    XSSFRow rowheader = (XSSFRow)sheet.GetRow(rowNum - 1);
                    for (int i = 0; i < datacol; i++)
                    {
                        rowheader.GetCell(i).SetCellValue(header[i]);
                    }
                    rowheader.GetCell(datacol).SetCellValue("Trạm trộn");
                    // fill data
                    for (int i = 0; i < data.Count; i++)
                    {
                        XSSFRow row = (XSSFRow)sheet.CreateRow(rowNum);
                        rowNum++;
                        for (int j = 0; j <= datacol; j++)
                        {
                            row.CreateCell(j).CellStyle = rowStyle[j];
                        }
                        // set tên chi nhánh
                        row.GetCell(datacol).SetCellValue(branch.Name);
                        string[] datarow = data[i];
                        for (int k = 0; k < datarow.Length; k++)
                        {
                            row.GetCell(k).SetCellValue(datarow[k]);
                        }
                    }

                    //for (int rr = 0; rr < datasize + 1; rr++)
                    //{
                    //    int rowNum = rr + rowStart;

                    //    try
                    //    {
                    //        XSSFRow row = (XSSFRow)sheet.CreateRow(rowNum);
                    //        for (int i = 0; i < datacol; i++)
                    //        {
                    //            row.CreateCell(i).CellStyle = rowStyle[i];
                    //            if (i == 0)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("Tổng");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(rr + 1);
                    //                }
                    //            }
                    //            else if (i == 1)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(data[rr].TENMACBETONG);
                    //                }
                    //            }
                    //            else if (i == 2)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(C5:E" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_1, 2));
                    //                }
                    //            }
                    //            else if (i == 3)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(D5:E" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_2, 2));
                    //                }
                    //            }
                    //            else if (i == 4)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(E5:E" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_3, 2));
                    //                }
                    //            }
                    //            else if (i == 5)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(F5:F" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_4, 2));
                    //                }
                    //            }
                    //            else if (i == 6)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(G5:G" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_5, 2));
                    //                }
                    //            }
                    //            else if (i == 7)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(H5:H" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_6, 2));
                    //                }
                    //            }
                    //            else if (i == 8)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(I5:I" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_7, 2));
                    //                }
                    //            }
                    //            else if (i == 9)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(J5:J" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_8, 2));
                    //                }
                    //            }
                    //            else if (i == 10)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellFormula("SUM(K5:K" + (datasize + rowStart).ToString() + ")");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(System.Math.Round(data[rr].VL_9, 2));
                    //                }
                    //            }
                    //            else if (i == 11)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(data[rr].CUONGDO);
                    //                }
                    //            }
                    //            else if (i == 12)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(data[rr].COTLIEUMAX);
                    //                }
                    //            }
                    //            else if (i == 13)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(data[rr].DOSUT);
                    //                }
                    //            }
                    //            else if (i == 14)
                    //            {
                    //                if (rr == datasize)
                    //                {
                    //                    row.GetCell(i).SetCellValue("");
                    //                }
                    //                else
                    //                {
                    //                    row.GetCell(i).SetCellValue(data[rr].name);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }

                    //}

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
    }
}


