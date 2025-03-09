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
    public class DuAnController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("quanlyduan", "quanlyduan");
        private static string functionCode = "QLDMDA";
        private readonly IConfiguration _configuration;
        public DuAnController(IConfiguration configuration)
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

                    List<DuAnDTO> xe = new List<DuAnDTO>();
                    command.CommandText = " SELECT Ma,ID,MaLK,TENDUAN,DIADIEMXD,TENHANGMUC,LASTUPDATED, ISSYNC INTO #Result FROM ";
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
                                    command.CommandText += "SELECT Ma,ID,MaLK,TENDUAN,DIADIEMXD,TENHANGMUC,LASTUPDATED,ISSYNC FROM [" + branch.Dataname + "].[dbo].[DUAN]";
                                }

                            }
                            ++i;
                        }
                    }

                    command.CommandText += ") duan";
                    command.CommandText += " WHERE ISSYNC IS NULL ";
                    if (paging.query != null)
                    {
                        //command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                        command.CommandText += "AND TENDUAN LIKE '%" + HttpUtility.UrlDecode(paging.query) + "%'";
                    }
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT *  FROM #Result ORDER BY LASTUPDATED DESC";
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
                            DuAnDTO item = new DuAnDTO();
                            item.MALIENKET = (result["MaLK"] is DBNull) ? String.Empty : (string)result["MaLK"];
                            item.TENDUAN = result["TENDUAN"] == null ? string.Empty : (string)result["TENDUAN"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                            item.DIADIEMXD = (result["DIADIEMXD"] is DBNull) ? String.Empty : (string)result["DIADIEMXD"];
                            item.TENHANGMUC = (result["TENHANGMUC"] is DBNull) ? String.Empty : (string)result["TENHANGMUC"];
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
        public IActionResult PostXe([FromBody]DuAnDTO da)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (da == null)
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
                if (da != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (da.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(da.BranchId));
                            //sinh ID tu dong
                            da.ID = CustomGuid.NewSequentialId();
                            da.Ma = CommonLib.GetSo("DUAN", "Ma", "DA1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                            

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[DUAN]([ID], [Ma],[MaLK],[TENDUAN],[DIADIEMXD],[TENHANGMUC],[LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramMaLienKet,@paramTenDUAN,@paramDIADIEMXD,@paramTENHANGMUC,Getdate())";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = da.ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = da.Ma;
                            command.Parameters.Add(paramMa);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (da.MALIENKET is null) ? String.Empty : (string)da.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTenDUAN = command.CreateParameter();
                            paramTenDUAN.ParameterName = "@paramTenDUAN";
                            paramTenDUAN.Value = da.TENDUAN;
                            command.Parameters.Add(paramTenDUAN);

                            var paramDIADIEMXD = command.CreateParameter();
                            paramDIADIEMXD.ParameterName = "@paramDIADIEMXD";
                            paramDIADIEMXD.Value = da.DIADIEMXD;
                            paramDIADIEMXD.Value = (da.DIADIEMXD is null) ? String.Empty : (string)da.DIADIEMXD;
                            command.Parameters.Add(paramDIADIEMXD);

                            var paramTENHANGMUC = command.CreateParameter();
                            paramTENHANGMUC.ParameterName = "@paramTENHANGMUC";
                            paramTENHANGMUC.Value = da.TENHANGMUC;
                            paramTENHANGMUC.Value = (da.TENHANGMUC is null) ? String.Empty : (string)da.TENHANGMUC;
                            command.Parameters.Add(paramTENHANGMUC);

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
        public IActionResult PutDuAn(Guid ID, [FromBody] DuAnDTO da)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (da == null)
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
                if (da != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (da.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(da.BranchId));
                            //sinh ID tu dong
                            //khachhang.ID = CustomGuid.NewSequentialId();
                            //khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_", branch.Dataname);
                            //khachhang.ISSYNC = false;
                            //khachhang.SYSCCHENGE = false;

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[DUAN] SET [MaLK]= @paramMaLienKet, [TENDUAN]= @paramTenDUAN ,[DIADIEMXD] = @paramDIADIEMXD ,[TENHANGMUC]= @paramTENHANGMUC,[LASTUPDATED] = Getdate()  WHERE ID = @paramID";

                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (da.MALIENKET is null) ? String.Empty : (string)da.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);


                            var paramTenDUAN = command.CreateParameter();
                            paramTenDUAN.ParameterName = "@paramTenDUAN";
                            paramTenDUAN.Value = da.TENDUAN;
                            command.Parameters.Add(paramTenDUAN);

                            var paramDIADIEMXD = command.CreateParameter();
                            paramDIADIEMXD.ParameterName = "@paramDIADIEMXD";
                            paramDIADIEMXD.Value = (da.DIADIEMXD is null) ? String.Empty : (string)da.DIADIEMXD;
                            command.Parameters.Add(paramDIADIEMXD);

                            var paramTENHANGMUC = command.CreateParameter();
                            paramTENHANGMUC.ParameterName = "@paramTENHANGMUC";
                            paramTENHANGMUC.Value = (da.TENHANGMUC is null) ? String.Empty : (string)da.TENHANGMUC;
                            command.Parameters.Add(paramTENHANGMUC);

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
        public IActionResult DeleteDuAn(string ID)
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

                            // check dự án đang được sử dụng hay không
                            command.CommandText += "SELECT Ma FROM [" + branch.Dataname + "].[dbo].[DUAN] WHERE ID = @paramID;";
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
                                    command.CommandText = "SELECT * FROM [" + branch.Dataname + "].[dbo].[DATHANG]  WHERE MADUAN = @paramMa;";
                                    var paramMa = command.CreateParameter();
                                    paramMa.ParameterName = "@paramMa";
                                    paramMa.Value = ma;
                                    command.Parameters.Add(paramMa);

                                    context.Database.OpenConnection();
                                    using (var resultDathang = command.ExecuteReader())
                                    {
                                        resultDathang.Read();
                                        var maDathang = string.Empty;
                                        if (resultDathang.HasRows)
                                        {
                                            maDathang = (string)resultDathang["Ma"];
                                        }
                                        if (!string.IsNullOrEmpty(maDathang))
                                        {
                                            def.meta = new Meta(212, "Dự án này đang được sử dụng tại đơn hàng " + maDathang + ", không thể xóa !");
                                            return Ok(def);
                                        }
                                        else
                                        {
                                            context.Database.CloseConnection();
                                            command.CommandText = string.Empty;
                                            command.CommandText += "BEGIN TRANSACTION [Tran1da] BEGIN TRY ";

                                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[DuAn]  WHERE ID = @paramID;";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";
                                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";

                                            var paramTableName = command.CreateParameter();
                                            paramTableName.ParameterName = "@paramTableName";
                                            paramTableName.Value = "DUAN";
                                            command.Parameters.Add(paramTableName);

                                            var paramLastupdated = command.CreateParameter();
                                            paramLastupdated.ParameterName = "@paramLastupdated";
                                            paramLastupdated.Value = DateTime.Now.ToString();
                                            command.Parameters.Add(paramLastupdated);

                                            command.CommandText += "COMMIT TRANSACTION [Tran1da] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1da] END CATCH";

                                            context.Database.OpenConnection();
                                            using (var result = command.ExecuteReader())
                                            {
                                                result.Read();
                                                def.meta = new Meta(200, "Xoa khach hang thanh cong !");
                                                return Ok(def);

                                            }
                                        }
                                    }
                                }
                            }
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
        [HttpGet("{branchid}")]
        public IActionResult GetListDuAn(int branchid)
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

            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<DuAnDTO> rpdonhang = new List<DuAnDTO>();
                Branch branch = context.Branch.Find(branchid);
                command.CommandText += "SELECT dh.TENDUAN FROM [" + branch.Dataname + "].[dbo].[DUAN] dh";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    var j = 1;
                    while (result.Read())
                    {
                        DuAnDTO item = new DuAnDTO();
                        if (result["TENDUAN"] is System.DBNull)
                        {
                            item.TENDUAN = "";

                        }
                        else
                        {
                            item.TENDUAN = (string)result["TENDUAN"];
                        }
                        rpdonhang.Add(item);
                        j++;
                    }
                    def.data = rpdonhang;
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }
    }
}