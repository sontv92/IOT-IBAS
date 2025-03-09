using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace IOITWebApp.Controllers.ApiCMS
{
    [Route("api/[controller]")]
    [ApiController]
    public class XeController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("quanlyxe", "quanlyxe");
        private static string functionCode = "QLDMX";
        private readonly IConfiguration _configuration;
        public XeController(IConfiguration configuration)
        {
            _configuration = configuration;
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

                    List<XeDTO> xe = new List<XeDTO>();
                    List<ThongKeDonHangTongHopDTO> lstxe = new List<ThongKeDonHangTongHopDTO>();
                    command.CommandText = " SELECT ID,Ma, BIENSO,TENLAIXE,LASTUPDATED INTO #Result FROM ";
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
                                    command.CommandText += "SELECT ID, Ma, BIENSO,TENLAIXE,LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[XE]";
                                }

                            }
                            ++i;
                        }
                    }

                    command.CommandText += ") xe";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE BIENSO LIKE N'%" + HttpUtility.UrlDecode(paging.query)+"%'";
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ";
                    command.CommandText += " ORDER BY LASTUPDATED DESC";
                    //if (paging.order_by != null)
                    //{
                    //    command.CommandText += " ORDER BY " + paging.order_by;
                    //}
                    //else
                    //{
                    //    command.CommandText += " ORDER BY Ma asc";
                    //}
                    command.CommandText += " OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY;";
                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        def.metadata = result[0];
                        result.NextResult();

                        //DataTable dtresult = new DataTable();

                        //dtresult.Load(result);

                        //foreach (DataColumn col in dtresult.Columns)
                        //{
                        //    ThongKeDonHangTongHopDTO items = new ThongKeDonHangTongHopDTO();
                        //    string colName = col.ColumnName;


                        //    if (colName != "ID")
                        //    {
                        //        items.header = colName;
                        //        items.rows = new List<string>();
                        //        for (int m = 0; m < dtresult.Rows.Count; m++)
                        //        {
                        //            var myValue = dtresult.Rows[m][colName];
                        //            items.rows.Add(myValue.ToString());
                        //        }
                        //        lstxe.Add(items);
                        //    }





                        //}
                        //def.data = lstxe;
                        // def.data = JsonConvert.SerializeObject(lstxe); ;


                        while (result.Read())
                        {
                            XeDTO item = new XeDTO();
                            item.BIENSO = result["BIENSO"] == null ? string.Empty : (string)result["BIENSO"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                            item.TENLAIXE = (result["TENLAIXE"] is DBNull) ? String.Empty : (string)result["TENLAIXE"];
                            item.ID = (Guid)result["ID"];
                            xe.Add(item);
                        }

                        def.data = xe;
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
        
        public IActionResult PostXe([FromBody]XeDTO xe)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (xe == null)
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
                if (xe != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (xe.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(xe.BranchId));
                            //sinh ID tu dong
                            xe.ID = CustomGuid.NewSequentialId();
                            xe.Ma = CommonLib.GetSo("XE", "Ma", "XE1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                            xe.ISSYNC = false;
                            xe.SYSCCHENGE = false;

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[XE]([ID], [Ma], [BIENSO], [TENLAIXE], [ISSYNC], [SYSCCHENGE],[LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramBienSo,@paramTenLaiXe,@paramISSYNC,@paramSYSCCHENGE, Getdate())";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = xe.ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = xe.Ma;
                            command.Parameters.Add(paramMa);

                            var paramBienSo = command.CreateParameter();
                            paramBienSo.ParameterName = "@paramBienSo";
                            paramBienSo.Value = xe.BIENSO;
                            command.Parameters.Add(paramBienSo);

                            var paramTenLaiXe = command.CreateParameter();
                            paramTenLaiXe.ParameterName = "@paramTenLaiXe";
                            paramTenLaiXe.Value = xe.TENLAIXE;
                            command.Parameters.Add(paramTenLaiXe);

                            var paramISSYNC = command.CreateParameter();
                            paramISSYNC.ParameterName = "@paramISSYNC";
                            paramISSYNC.Value = xe.ISSYNC;
                            command.Parameters.Add(paramISSYNC);

                            var paramSYSCCHENGE = command.CreateParameter();
                            paramSYSCCHENGE.ParameterName = "@paramSYSCCHENGE";
                            paramSYSCCHENGE.Value = xe.SYSCCHENGE;
                            command.Parameters.Add(paramSYSCCHENGE);

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
        
        public IActionResult PutXe(Guid ID, [FromBody] XeDTO xe)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (xe == null)
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
                if (xe != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (xe.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(xe.BranchId));
                            //sinh ID tu dong
                            //khachhang.ID = CustomGuid.NewSequentialId();
                            //khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_", branch.Dataname);
                            //khachhang.ISSYNC = false;
                            //khachhang.SYSCCHENGE = false;

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[XE] SET [BIENSO]= @paramBienSo ,[TENLAIXE] = @paramTenLaiXe,[LASTUPDATED] = Getdate()  WHERE ID = @paramID";

                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = ID;
                            command.Parameters.Add(paramID);

                            var paramBienSo = command.CreateParameter();
                            paramBienSo.ParameterName = "@paramBienSo";
                            paramBienSo.Value = xe.BIENSO;
                            command.Parameters.Add(paramBienSo);

                            var paramTenLaiXe = command.CreateParameter();
                            paramTenLaiXe.ParameterName = "@paramTenLaiXe";
                            paramTenLaiXe.Value = xe.TENLAIXE;
                            command.Parameters.Add(paramTenLaiXe);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);


                        }
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            def.meta = new Meta(200, "Cap nhat thanh cong !");
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

        [HttpDelete("{id}")]
        
        public IActionResult DeleteXe(string ID)
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
                            command.CommandText += "BEGIN TRANSACTION [Tran1xe] BEGIN TRY ";

                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[XE]  WHERE ID = @paramID;";

                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            
                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3', Getdate());";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = Id;
                            command.Parameters.Add(paramID);

                            var paramTableName = command.CreateParameter();
                            paramTableName.ParameterName = "@paramTableName";
                            paramTableName.Value = "XE";
                            command.Parameters.Add(paramTableName);

                            var paramLastupdated = command.CreateParameter();
                            paramLastupdated.ParameterName = "@paramLastupdated";
                            paramLastupdated.Value = DateTime.Now.ToString();
                            command.Parameters.Add(paramLastupdated);



                            command.CommandText += "COMMIT TRANSACTION [Tran1xe] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1xe] END CATCH";



                        }
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            def.meta = new Meta(200, "Xoa khach hang thanh cong !");
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
    }
}