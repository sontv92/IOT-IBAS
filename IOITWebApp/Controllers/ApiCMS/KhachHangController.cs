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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class KhachHangController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("khachhang", "khachhang");
        private static string functionCode = "QLKH";
        private readonly IConfiguration _configuration;
        public KhachHangController(IConfiguration configuration)
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

                    List<KhachHangDTO> khachhang = new List<KhachHangDTO>();
                    command.CommandText = " SELECT Ma,MaLK,TENKHACHHANG,SDT,DIACHI,ID,LASTUPDATED INTO #Result FROM ";
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
                                    command.CommandText += "SELECT Ma,MaLK,TENKHACHHANG,SDT,DIACHI,ID,LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[KHACHHANG]";
                                }

                            }
                            ++i;
                        }
                    }

                    command.CommandText += ") kh";
                    command.CommandText += " WHERE Ma NOT LIKE N'%KH2%'";
                    if (paging.query != null)
                    {

                        command.CommandText += " AND TENKHACHHANG LIKE N'%" + HttpUtility.UrlDecode(paging.query) + "%'";
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
                        while (result.Read())
                        {
                            KhachHangDTO item = new KhachHangDTO();
                            item.TENKHACHHANG = result["TENKHACHHANG"] == null ? string.Empty : (string)result["TENKHACHHANG"];
                            item.MALIENKET = (result["MaLK"] is DBNull) ? String.Empty : (string)result["MaLK"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];

                            item.DIACHI = (result["DIACHI"] is DBNull) ? String.Empty : (string)result["DIACHI"];
                            item.SDT = (result["SDT"] is DBNull) ? String.Empty : (string)result["SDT"];
                            item.ID = (Guid)result["ID"];
                            khachhang.Add(item);
                        }

                        def.data = khachhang;
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
        public IActionResult PostKhachHang([FromBody] KhachHangDTO khachhang)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (khachhang == null)
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
                if (khachhang != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (khachhang.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(khachhang.BranchId));
                            //sinh ID tu dong
                            khachhang.ID = CustomGuid.NewSequentialId();
                            khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                            khachhang.ISSYNC = false;
                            khachhang.SYSCCHENGE = false;

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[KHACHHANG]([ID], [Ma], [MaLK], [TENKHACHHANG], [SDT], [ISSYNC], [SYSCCHENGE], [DIACHI],[LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramMaLienKet,@paramTenKH,@paramSoDT,@paramISSYNC,@paramSYSCCHENGE,@paramDiaChi,@paramLastUpdate)";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = khachhang.ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = khachhang.Ma;
                            command.Parameters.Add(paramMa);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (khachhang.MALIENKET is null) ? String.Empty : (string)khachhang.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTenKH = command.CreateParameter();
                            paramTenKH.ParameterName = "@paramTenKH";
                            paramTenKH.Value = khachhang.TENKHACHHANG;
                            command.Parameters.Add(paramTenKH);

                            var paramSoDT = command.CreateParameter();
                            paramSoDT.ParameterName = "@paramSoDT";
                            paramSoDT.Value = khachhang.SDT;
                            command.Parameters.Add(paramSoDT);

                            var paramISSYNC = command.CreateParameter();
                            paramISSYNC.ParameterName = "@paramISSYNC";
                            paramISSYNC.Value = khachhang.ISSYNC;
                            command.Parameters.Add(paramISSYNC);

                            var paramSYSCCHENGE = command.CreateParameter();
                            paramSYSCCHENGE.ParameterName = "@paramSYSCCHENGE";
                            paramSYSCCHENGE.Value = khachhang.SYSCCHENGE;
                            command.Parameters.Add(paramSYSCCHENGE);

                            var paramDiaChi = command.CreateParameter();
                            paramDiaChi.ParameterName = "@paramDiaChi";
                            paramDiaChi.Value = (khachhang.DIACHI is null) ? String.Empty : (string)khachhang.DIACHI;
                            command.Parameters.Add(paramDiaChi);

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
        public IActionResult PutKhachHang(Guid ID, [FromBody] KhachHangDTO khachhang)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (khachhang == null)
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
                if (khachhang != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (khachhang.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(khachhang.BranchId));
                            //sinh ID tu dong
                            //khachhang.ID = CustomGuid.NewSequentialId();
                            //khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_", branch.Dataname);
                            //khachhang.ISSYNC = false;
                            //khachhang.SYSCCHENGE = false;

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[KHACHHANG] SET [MaLK]= @paramMaLienKet, [TENKHACHHANG]= @paramTenKH ,[SDT] = @paramSoDT ,[DIACHI]= @paramDiaChi,[LASTUPDATED] = @paramLastUpdate  WHERE ID = @paramID";

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
                            paramMaLienKet.Value = (khachhang.MALIENKET is null) ? String.Empty : (string)khachhang.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTenKH = command.CreateParameter();
                            paramTenKH.ParameterName = "@paramTenKH";
                            paramTenKH.Value = khachhang.TENKHACHHANG;
                            command.Parameters.Add(paramTenKH);

                            var paramSoDT = command.CreateParameter();
                            paramSoDT.ParameterName = "@paramSoDT";
                            paramSoDT.Value = khachhang.SDT;
                            command.Parameters.Add(paramSoDT);


                            var paramDiaChi = command.CreateParameter();
                            paramDiaChi.ParameterName = "@paramDiaChi";
                            paramDiaChi.Value = (khachhang.DIACHI is null) ? String.Empty : (string)khachhang.DIACHI;
                            command.Parameters.Add(paramDiaChi);

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
        public IActionResult DeleteKhachHang(string ID)
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

                            // check khách hàng đang được sử dụng hay không
                            command.CommandText += "SELECT Ma FROM [" + branch.Dataname + "].[dbo].[KHACHHANG] WHERE ID = @paramID;";
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
                                    command.CommandText = "SELECT * FROM [" + branch.Dataname + "].[dbo].[DATHANG]  WHERE MAKHACHHANG = @paramMa;";
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
                                            def.meta = new Meta(212, "Khách hàng này đang được sử dụng tại đơn hàng " + maDathang + ", không thể xóa !");
                                            return Ok(def);
                                        }
                                        else
                                        {
                                            context.Database.CloseConnection();
                                            command.CommandText = string.Empty;
                                            command.CommandText += "BEGIN TRANSACTION [Tran1kh] BEGIN TRY ";

                                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[KHACHHANG]  WHERE ID = @paramID;";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";
                                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";

                                            var paramTableName = command.CreateParameter();
                                            paramTableName.ParameterName = "@paramTableName";
                                            paramTableName.Value = "KHACHHANG";
                                            command.Parameters.Add(paramTableName);

                                            var paramLastupdated = command.CreateParameter();
                                            paramLastupdated.ParameterName = "@paramLastupdated";
                                            paramLastupdated.Value = DateTime.Now.ToString();
                                            command.Parameters.Add(paramLastupdated);

                                            command.CommandText += "COMMIT TRANSACTION [Tran1kh] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1kh] END CATCH";

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
    }
}