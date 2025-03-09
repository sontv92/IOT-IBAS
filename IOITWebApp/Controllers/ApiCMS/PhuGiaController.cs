using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
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

namespace IOITWebApp.Controllers.ApiCMS
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhuGiaController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("quanlyphugia", "quanlyphugia");
        private static string functionCode = "QLDMPG";
        private readonly IConfiguration _configuration;
        public PhuGiaController(IConfiguration configuration)
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

                    List<PhuGiaDTO> xe = new List<PhuGiaDTO>();
                    command.CommandText = " SELECT Ma,ID, TENPG,NHACUNGCAP,LASTUPDATED INTO #Result FROM ";
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
                                    command.CommandText += "SELECT Ma,ID, TENPG,NHACUNGCAP,LASTUPDATED,ISSYNC FROM [" + branch.Dataname + "].[dbo].[PHUGIA]";
                                }

                            }
                            ++i;
                        }
                    }

                    command.CommandText += ") pg";
                    command.CommandText += " WHERE Ma NOT LIKE N'%PG2%'";
                    if (paging.query != null)
                    {
                        command.CommandText += " AND " + HttpUtility.UrlDecode(paging.query);
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ";
                    command.CommandText += " ORDER BY Ma DESC";
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
                        while (result.Read())
                        {
                            PhuGiaDTO item = new PhuGiaDTO();
                            item.TENPG = result["TENPG"] == null ? string.Empty : (string)result["TENPG"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                            item.NHACUNGCAP = (result["NHACUNGCAP"] is DBNull) ? String.Empty : (string)result["NHACUNGCAP"];
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
        public IActionResult PostNhanVien([FromBody]PhuGiaDTO pg)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (pg == null)
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
                if (pg != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (pg.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(pg.BranchId));
                            //sinh ID tu dong
                            pg.ID = CustomGuid.NewSequentialId();
                            pg.Ma = CommonLib.GetSo("PHUGIA", "Ma", "VL1_", branch.Dataname);

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[PHUGIA] ([ID], [Ma], [TENPG], [NHACUNGCAP], [LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramTENPG,@paramNHACUNGCAP,Getdate())";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = pg.ID;
                            command.Parameters.Add(paramID);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = pg.Ma;
                            command.Parameters.Add(paramMa);

                            var paramTENPG = command.CreateParameter();
                            paramTENPG.ParameterName = "@paramTENPG";
                            paramTENPG.Value = (pg.TENPG is null) ? string.Empty : pg.TENPG.ToString();
                            command.Parameters.Add(paramTENPG);

                            var paramNHACUNGCAP = command.CreateParameter();
                            paramNHACUNGCAP.ParameterName = "@paramNHACUNGCAP";
                            paramNHACUNGCAP.Value = (pg.NHACUNGCAP is null) ? string.Empty : pg.NHACUNGCAP.ToString();
                            command.Parameters.Add(paramNHACUNGCAP);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);



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
        public IActionResult PutPhuGia(Guid ID, [FromBody] PhuGiaDTO pg)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (pg == null)
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
                if (pg != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (pg.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(pg.BranchId));
                            //sinh ID tu dong
                            //khachhang.ID = CustomGuid.NewSequentialId();
                            //khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_", branch.Dataname);
                            //khachhang.ISSYNC = false;
                            //khachhang.SYSCCHENGE = false;

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[PHUGIA] SET [TENPG]= @paramTENPG ,[NHACUNGCAP] = @paramNHACUNGCAP, [LASTUPDATED] = Getdate()  WHERE ID = @paramID";

                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = ID;
                            command.Parameters.Add(paramID);


                            var paramTENPG = command.CreateParameter();
                            paramTENPG.ParameterName = "@paramTENPG";
                            paramTENPG.Value = pg.TENPG;
                            command.Parameters.Add(paramTENPG);

                            var paramNHACUNGCAP = command.CreateParameter();
                            paramNHACUNGCAP.ParameterName = "@paramNHACUNGCAP";
                            paramNHACUNGCAP.Value = pg.NHACUNGCAP;
                            command.Parameters.Add(paramNHACUNGCAP);

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
        public IActionResult DeletePhuGia(string ID)
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
                            command.CommandText += "BEGIN TRANSACTION [Tran1pg] BEGIN TRY ";

                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[PHUGIA]  WHERE ID = @paramID;";
                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = Id;
                            command.Parameters.Add(paramID);

                            var paramTableName = command.CreateParameter();
                            paramTableName.ParameterName = "@paramTableName";
                            paramTableName.Value = "PHUGIA";
                            command.Parameters.Add(paramTableName);

                            var paramLastupdated = command.CreateParameter();
                            paramLastupdated.ParameterName = "@paramLastupdated";
                            paramLastupdated.Value = DateTime.Now.ToString();
                            command.Parameters.Add(paramLastupdated);

                            command.CommandText += "COMMIT TRANSACTION [Tran1pg] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1pg] END CATCH";

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