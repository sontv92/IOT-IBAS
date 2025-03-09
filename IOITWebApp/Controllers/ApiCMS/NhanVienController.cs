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
    public class NhanVienController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("quanlynhanvien", "quanlynhanvien");
        private static string functionCode = "DMNV";
        private readonly IConfiguration _configuration;
        public NhanVienController(IConfiguration configuration)
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

                    List<NhanVienDTO> xe = new List<NhanVienDTO>();
                    command.CommandText = " SELECT Ma,ID,MaLK,MATHENV,TENNV,SDT,GHICHU,LASTUPDATED INTO #Result FROM ";
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
                                    command.CommandText += "SELECT Ma,ID,MaLK,MATHENV,TENNV,SDT,GHICHU,LASTUPDATED FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]";
                                }

                            }
                            ++i;
                        }
                    }

                    command.CommandText += ") nv";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                        //command.CommandText += " WHERE TENNV LIKE '%" + HttpUtility.UrlDecode(paging.query) + "%'";
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
                            NhanVienDTO item = new NhanVienDTO();
                            item.MALIENKET = (result["MaLK"] is DBNull) ? String.Empty : (string)result["MaLK"];
                            item.TENNV = result["TENNV"] == null ? string.Empty : (string)result["TENNV"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                            item.MATHENV = (result["MATHENV"] is DBNull) ? String.Empty : (string)result["MATHENV"];
                            item.SDT = (result["SDT"] is DBNull) ? String.Empty : (string)result["SDT"];
                            item.GHICHU = (result["GHICHU"] is DBNull) ? String.Empty : (string)result["GHICHU"];
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
        public IActionResult PostNhanVien([FromBody] NhanVienDTO nv)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (nv == null)
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
                if (nv != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (nv.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(nv.BranchId));
                            //sinh ID tu dong
                            nv.ID = CustomGuid.NewSequentialId();
                            nv.Ma = CommonLib.GetSo("NHANVIEN", "Ma", "NV1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[NHANVIEN] ([ID], [Ma], [MaLK], [MATHENV], [TENNV], [SDT], [GHICHU],[LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramID,@paramMa,@paramMaLienKet,@paramMATHENV,@paramTENNV,@paramSDT,@paramGHICHU,Getdate())";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = nv.ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = nv.Ma;
                            command.Parameters.Add(paramMa);

                            var paramMATHENV = command.CreateParameter();
                            paramMATHENV.ParameterName = "@paramMATHENV";
                            paramMATHENV.Value = (nv.MATHENV is null) ? String.Empty : (string)nv.MATHENV;
                            command.Parameters.Add(paramMATHENV);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (nv.MALIENKET is null) ? String.Empty : (string)nv.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTENNV = command.CreateParameter();
                            paramTENNV.ParameterName = "@paramTENNV";
                            paramTENNV.Value = nv.TENNV;
                            command.Parameters.Add(paramTENNV);



                            var paramSDT = command.CreateParameter();
                            paramSDT.ParameterName = "@paramSDT";
                            paramSDT.Value = nv.SDT;
                            command.Parameters.Add(paramSDT);

                            var paramGHICHU = command.CreateParameter();
                            paramGHICHU.ParameterName = "@paramGHICHU";
                            paramGHICHU.Value = (nv.GHICHU is null) ? String.Empty : (string)nv.GHICHU;
                            command.Parameters.Add(paramGHICHU);

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
        public IActionResult PutNhanVien(Guid ID, [FromBody] NhanVienDTO nv)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (nv == null)
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
                if (nv != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {


                        if (nv.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(nv.BranchId));
                            //sinh ID tu dong
                            //khachhang.ID = CustomGuid.NewSequentialId();
                            //khachhang.Ma = CommonLib.GetSo("KHACHHANG", "Ma", "KH1_", branch.Dataname);
                            //khachhang.ISSYNC = false;
                            //khachhang.SYSCCHENGE = false;

                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[NHANVIEN] SET [MaLK]= @paramMaLienKet ,[MATHENV]= @paramMATHENV ,[TENNV] = @paramTENNV ,[SDT]= @paramSDT ,[GHICHU]= @paramGHICHU,[LASTUPDATED]= Getdate() WHERE ID = @paramID";

                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = ID;
                            command.Parameters.Add(paramID);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);


                            var paramMATHENV = command.CreateParameter();
                            paramMATHENV.ParameterName = "@paramMATHENV";
                            paramMATHENV.Value = nv.MATHENV;
                            command.Parameters.Add(paramMATHENV);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (nv.MALIENKET is null) ? String.Empty : (string)nv.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTENNV = command.CreateParameter();
                            paramTENNV.ParameterName = "@paramTENNV";
                            paramTENNV.Value = nv.TENNV;
                            command.Parameters.Add(paramTENNV);



                            var paramSDT = command.CreateParameter();
                            paramSDT.ParameterName = "@paramSDT";
                            paramSDT.Value = nv.SDT;
                            command.Parameters.Add(paramSDT);

                            var paramGHICHU = command.CreateParameter();
                            paramGHICHU.ParameterName = "@paramGHICHU";
                            paramGHICHU.Value = (nv.GHICHU is null) ? String.Empty : (string)nv.GHICHU;
                            command.Parameters.Add(paramGHICHU);

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
        public IActionResult DeleteNhanVien(string ID)
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
                            command.CommandText = "SELECT * FROM [" + branch.Dataname + "].[dbo].[DATHANG]  WHERE NHANVIENID = @paramID;";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = Id;
                            command.Parameters.Add(paramID);

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
                                    def.meta = new Meta(212, "Nhân viên này đang được sử dụng tại đơn hàng " + maDathang + ", không thể xóa !");
                                    return Ok(def);
                                }
                                else
                                {
                                    context.Database.CloseConnection();
                                    command.CommandText = string.Empty;
                                    command.CommandText += "BEGIN TRANSACTION [Tran1nv] BEGIN TRY ";

                                    command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[NHANVIEN]  WHERE ID = @paramID;";
                                    command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";
                                    //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                                    

                                    var paramTableName = command.CreateParameter();
                                    paramTableName.ParameterName = "@paramTableName";
                                    paramTableName.Value = "NHANVIEN";
                                    command.Parameters.Add(paramTableName);

                                    var paramLastupdated = command.CreateParameter();
                                    paramLastupdated.ParameterName = "@paramLastupdated";
                                    paramLastupdated.Value = DateTime.Now.ToString();
                                    command.Parameters.Add(paramLastupdated);

                                    command.CommandText += "COMMIT TRANSACTION [Tran1nv] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1nv] END CATCH";

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