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
using System.Globalization;
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
    public class QuanLyCapPhoiController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("QuanLyCapPhoi", "QuanLyCapPhoi");
        private static string functionCode = "QLCP";
        private readonly IConfiguration _configuration;
        public QuanLyCapPhoiController(IConfiguration configuration)
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
                    Branch branch = new Branch();
                    List<CapPhoiDTO> capphoi = new List<CapPhoiDTO>();
                    var count = 0;
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;

                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                branch = context.Branch.Find(Convert.ToInt32(item));
                                if (i == 0)
                                {
                                    command.CommandText = "SELECT COUNT(*) as COUNT FROM [" + branch.Dataname + "].[dbo].CUAVL";
                                    context.Database.OpenConnection();
                                    var resultCount = command.ExecuteReader();
                                    resultCount.Read();
                                    count = resultCount.GetInt32(0);
                                    context.Database.CloseConnection();

                                    command.CommandText = string.Empty;
                                    command.CommandText += "IF(OBJECT_ID('tempdb..#TempTable') IS NOT NULL) BEGIN DROP TABLE  [" + branch.Dataname + "].[dbo].#TempTable END; ";
                                    command.CommandText += "SELECT * INTO [" + branch.Dataname + "].[dbo].#TempTable FROM ( SELECT [MACBETONGID], [MACUAVL], [SOLUONG] FROM [" + branch.Dataname + "].[dbo].SOLUONGVL ) SOLUONGVLResults ";
                                    command.CommandText += "PIVOT (SUM([SOLUONG]) FOR [MACUAVL] IN (";
                                    for (int k = 1; k <= count; k++)
                                    {
                                        if (k < count)
                                            command.CommandText += "[" + k + "],";
                                        else
                                            command.CommandText += "[" + k + "]))";
                                    }
                                    //    "[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18])) " +
                                    command.CommandText += "AS PivotTable;";
                                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #TempTable ;";
                                    command.CommandText += "select [MACBETONGID],mbt.Ma,mbt.MaLK,mbt.TENMACBETONG,mbt.CUONGDO, mbt.COTLIEUMAX, mbt.DOSUT,mbt.LASTUPDATED,";
                                    for (int k = 1; k <= count; k++)
                                    {
                                        if (k < count)
                                            command.CommandText += "[" + k + "],";
                                        else
                                            command.CommandText += "[" + k + "] ";
                                    }
                                    //command.CommandText += "[1] as CAT1 ,[2] as CAT2 ,[3] as CAT3 ,[4] as DA1 ,[5] as DA2 ,[6] as DA3 ,[7] as XM1 ,[8] as XM2 ,[9] as XM3 ,[10] as XM4 ,[11] as NUOC1 ,[12] as NUOC2 ,";
                                    //command.CommandText += "[13] as PHUGIA1 ,[14] as PHUGIA2 ,[15] as PHUGIA3 ,[16] as PHUGIA4 ,[17] as PHUGIA5 ,[18] as PHUGIA6 ";
                                    command.CommandText += "from  [" + branch.Dataname + "].[dbo].#TempTable tmp INNER JOIN [" + branch.Dataname + "].[dbo].MACBETONG mbt ON tmp.MACBETONGID = mbt.ID";
                                    if (paging.query != null)
                                    {
                                        //command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                                        command.CommandText += " WHERE TENMACBETONG LIKE N'%" + HttpUtility.UrlDecode(paging.query) + "%'";
                                    }
                                    //if (paging.order_by != null)
                                    //{
                                    //    command.CommandText += " ORDER BY " + paging.order_by;
                                    //}
                                    //else
                                    //{
                                    //    command.CommandText += " ORDER BY Ma asc";
                                    //}
                                    command.CommandText += " ORDER BY Ma DESC";
                                    command.CommandText += " OFFSET " + (paging.page - 1) * paging.page_size + " ROWS FETCH NEXT " + paging.page_size + " ROWS ONLY;";
                                    command.CommandText += " DROP TABLE #TempTable; ";
                                }

                            }
                            ++i;
                        }
                    }
                    List<VatLieuDTO> TenCuaVL = new List<VatLieuDTO>();
                    using (var command2 = context.Database.GetDbConnection().CreateCommand())
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            context.Database.OpenConnection();
                            command2.CommandText = "SELECT cuavl.TENCUAVL, cuavl.TRANGTHAI FROM [" + branch.Dataname + "].[dbo].CUAVL as cuavl WHERE MACUAVL = " + i;
                            var resultCount = command2.ExecuteReader();
                            resultCount.Read();
                            TenCuaVL.Add(new VatLieuDTO()
                            {
                                TENCUAVL = resultCount.GetString(0),
                                TRANGTHAI = resultCount.GetBoolean(1)
                            });
                            context.Database.CloseConnection();
                        }
                    }
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        def.metadata = result[0];
                        result.NextResult();
                        while (result.Read())
                        {
                            CapPhoiDTO item = new CapPhoiDTO();
                            item.MACBETONGID = (Guid)result["MACBETONGID"];
                            item.Ma = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                            item.MALIENKET = (result["MaLK"] is DBNull) ? String.Empty : (string)result["MaLK"];
                            item.TENMACBETONG = (result["TENMACBETONG"] is DBNull) ? String.Empty : (string)result["TENMACBETONG"];

                            item.CUONGDO = (result["CUONGDO"] is DBNull) ? string.Empty : result["CUONGDO"].ToString();
                            item.DOSUT = (result["DOSUT"] is DBNull) ? String.Empty : (string)result["DOSUT"];
                            item.COTLIEUMAX = (result["COTLIEUMAX"] is DBNull) ? 0 : (int)result["COTLIEUMAX"];

                            item.VatLieus = new List<VatLieuDTO>();
                            for (int i = 1; i <= count; i++)
                            {
                                var itemCuaVL = i.ToString();
                                VatLieuDTO vatLieu = new VatLieuDTO()
                                {
                                    MACUAVL = i,
                                    VALUE = (decimal)((result[itemCuaVL] is DBNull) ? 0 : Math.Round((Double)result[itemCuaVL], 2)),
                                    TENCUAVL = TenCuaVL[i - 1].TENCUAVL,
                                    TRANGTHAI = TenCuaVL[i - 1].TRANGTHAI
                                };
                                item.VatLieus.Add(vatLieu);
                            }

                            //item.CAT1 = (result["CAT1"] is DBNull) ? 0 : Math.Round((Double)result["CAT1"], 2);
                            //item.CAT2 = (result["CAT2"] is DBNull) ? 0 : Math.Round((Double)result["CAT2"], 2);
                            //item.CAT3 = (result["CAT3"] is DBNull) ? 0 : Math.Round((Double)result["CAT3"], 2);
                            //item.DA1 = (result["DA1"] is DBNull) ? 0 : Math.Round((Double)result["DA1"], 2);
                            //item.DA2 = (result["DA2"] is DBNull) ? 0 : Math.Round((Double)result["DA2"], 2);
                            //item.DA3 = (result["DA3"] is DBNull) ? 0 : Math.Round((Double)result["DA3"], 2);
                            //item.XM1 = (result["XM1"] is DBNull) ? 0 : Math.Round((Double)result["XM1"], 2);
                            //item.XM2 = (result["XM2"] is DBNull) ? 0 : Math.Round((Double)result["XM2"], 2);
                            //item.XM3 = (result["XM3"] is DBNull) ? 0 : Math.Round((Double)result["XM3"], 2);
                            //item.XM4 = (result["XM4"] is DBNull) ? 0 : Math.Round((Double)result["XM4"], 2);
                            //item.NUOC1 = (result["NUOC1"] is DBNull) ? 0 : Math.Round((Double)result["NUOC1"], 2);
                            //item.NUOC2 = (result["NUOC2"] is DBNull) ? 0 : Math.Round((Double)result["NUOC2"], 2);
                            //item.PHUGIA1 = (result["PHUGIA1"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA1"], 2);


                            //item.PHUGIA2 = (result["PHUGIA2"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA2"], 2);
                            //item.PHUGIA3 = (result["PHUGIA3"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA3"], 2);
                            //item.PHUGIA4 = (result["PHUGIA4"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA4"], 2);
                            //item.PHUGIA5 = (result["PHUGIA5"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA5"], 2);
                            //item.PHUGIA6 = (result["PHUGIA6"] is DBNull) ? 0 : Math.Round((Double)result["PHUGIA6"], 2);



                            capphoi.Add(item);
                        }

                        def.data = capphoi;
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


        public IActionResult PostCapPhoi([FromBody] CapPhoiDTO capphoi)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (capphoi == null)
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
                if (capphoi != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        command.CommandText += "BEGIN TRANSACTION [Tran1] BEGIN TRY ";
                        if (capphoi.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(capphoi.BranchId));
                            //sinh ID tu dong
                            capphoi.ID = CustomGuid.NewSequentialId();
                            capphoi.MACBETONGID = CustomGuid.NewSequentialId();
                            capphoi.Ma = CommonLib.GetSo("MACBETONG", "Ma", "MAC1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);

                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[MACBETONG]([ID], [Ma], [MaLK], [TENMACBETONG], [CUONGDO], [COTLIEUMAX], [DOSUT],[LASTUPDATED]) ";
                            command.CommandText += "VALUES (@paramMACBETONGID,@paramMa,@paramMaLienKet,@paramTenMacBeTong,@paramCUONGDO,@paramCOTLIEUMAX,@paramDOSUT,Getdate());";
                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                            var paramMACBETONGID = command.CreateParameter();
                            paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                            paramMACBETONGID.Value = capphoi.MACBETONGID;
                            command.Parameters.Add(paramMACBETONGID);

                            var paramMa = command.CreateParameter();
                            paramMa.ParameterName = "@paramMa";
                            paramMa.Value = capphoi.Ma;
                            command.Parameters.Add(paramMa);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (capphoi.MALIENKET is null) ? String.Empty : (string)capphoi.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTenMacBeTong = command.CreateParameter();
                            paramTenMacBeTong.ParameterName = "@paramTenMacBeTong";
                            paramTenMacBeTong.Value = capphoi.TENMACBETONG;
                            command.Parameters.Add(paramTenMacBeTong);

                            var paramCUONGDO = command.CreateParameter();
                            paramCUONGDO.ParameterName = "@paramCUONGDO";
                            paramCUONGDO.Value = capphoi.CUONGDO;
                            command.Parameters.Add(paramCUONGDO);

                            var paramCOTLIEUMAX = command.CreateParameter();
                            paramCOTLIEUMAX.ParameterName = "@paramCOTLIEUMAX";
                            paramCOTLIEUMAX.Value = capphoi.COTLIEUMAX;
                            command.Parameters.Add(paramCOTLIEUMAX);

                            var paramDOSUT = command.CreateParameter();
                            paramDOSUT.ParameterName = "@paramDOSUT";
                            paramDOSUT.Value = capphoi.DOSUT;
                            command.Parameters.Add(paramDOSUT);



                            //INSER VALUE SOLUONGVL
                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SOLUONGVL] ([MACBETONGID], [MACUAVL], [SOLUONG], [ID], [Ma],[MAMAC],[LASTUPDATED]) VALUES";

                            for (int i = 0; i < capphoi.VatLieus.Count; i++)
                            {
                                if (capphoi.VatLieus[i].VALUE == null)
                                {
                                    capphoi.VatLieus[i].VALUE = 0;
                                }
                                if (i < capphoi.VatLieus.Count - 1)
                                    command.CommandText += "(@paramMACBETONGID, " + capphoi.VatLieus[i].STT + ", " + capphoi.VatLieus[i].VALUE.Value.ToString(CultureInfo.InvariantCulture) + ", " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL, @paramMa,Getdate()),";
                                else
                                    command.CommandText += "(@paramMACBETONGID, " + capphoi.VatLieus[i].STT + ", " + capphoi.VatLieus[i].VALUE.Value.ToString(CultureInfo.InvariantCulture) + ", " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL, @paramMa,Getdate());";
                            }

                            //command.CommandText += "VALUES (@paramMACBETONGID, '1', @paramCAT1, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL, @paramMa,Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '2', @paramCAT2, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '3', @paramCAT3, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '4', @paramDA1, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '5', @paramDA2, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '6', @paramDA3, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '7', @paramXM1, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '8', @paramXM2, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '9', @paramXM3, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '10', @paramXM4, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '11', @paramNUOC1, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '12', @paramNUOC2, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '13', @paramPHUGIA1, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '14', @paramPHUGIA2, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '15', @paramPHUGIA3, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '16', @paramPHUGIA4, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '17', @paramPHUGIA5, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '18', @paramPHUGIA6, " + "'" + CustomGuid.NewSequentialId() + "'" + ", @paramMaSOLUONGVL,@paramMa, Getdate());";

                            var paramMaSOLUONGVL = command.CreateParameter();
                            paramMaSOLUONGVL.ParameterName = "@paramMaSOLUONGVL";
                            paramMaSOLUONGVL.Value = CommonLib.GetSo("SOLUONGVL", "Ma", "SL1_", branch.Dataname);
                            command.Parameters.Add(paramMaSOLUONGVL);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            //var paramCAT1 = command.CreateParameter();
                            //paramCAT1.ParameterName = "@paramCAT1";
                            //paramCAT1.Value = capphoi.CAT1;
                            //command.Parameters.Add(paramCAT1);

                            //var paramCAT2 = command.CreateParameter();
                            //paramCAT2.ParameterName = "@paramCAT2";
                            //paramCAT2.Value = capphoi.CAT2;
                            //command.Parameters.Add(paramCAT2);

                            //var paramCAT3 = command.CreateParameter();
                            //paramCAT3.ParameterName = "@paramCAT3";
                            //paramCAT3.Value = capphoi.CAT3;
                            //command.Parameters.Add(paramCAT3);

                            //var paramDA1 = command.CreateParameter();
                            //paramDA1.ParameterName = "@paramDA1";
                            //paramDA1.Value = capphoi.DA1;
                            //command.Parameters.Add(paramDA1);

                            //var paramDA2 = command.CreateParameter();
                            //paramDA2.ParameterName = "@paramDA2";
                            //paramDA2.Value = capphoi.DA2;
                            //command.Parameters.Add(paramDA2);

                            //var paramDA3 = command.CreateParameter();
                            //paramDA3.ParameterName = "@paramDA3";
                            //paramDA3.Value = capphoi.DA3;
                            //command.Parameters.Add(paramDA3);

                            //var paramXM1 = command.CreateParameter();
                            //paramXM1.ParameterName = "@paramXM1";
                            //paramXM1.Value = capphoi.XM1;
                            //command.Parameters.Add(paramXM1);

                            //var paramXM2 = command.CreateParameter();
                            //paramXM2.ParameterName = "@paramXM2";
                            //paramXM2.Value = capphoi.XM2;
                            //command.Parameters.Add(paramXM2);

                            //var paramXM3 = command.CreateParameter();
                            //paramXM3.ParameterName = "@paramXM3";
                            //paramXM3.Value = capphoi.XM3;
                            //command.Parameters.Add(paramXM3);

                            //var paramXM4 = command.CreateParameter();
                            //paramXM4.ParameterName = "@paramXM4";
                            //paramXM4.Value = capphoi.XM4;
                            //command.Parameters.Add(paramXM4);

                            //var paramNUOC1 = command.CreateParameter();
                            //paramNUOC1.ParameterName = "@paramNUOC1";
                            //paramNUOC1.Value = capphoi.NUOC1;
                            //command.Parameters.Add(paramNUOC1);

                            //var paramNUOC2 = command.CreateParameter();
                            //paramNUOC2.ParameterName = "@paramNUOC2";
                            //paramNUOC2.Value = capphoi.NUOC2;
                            //command.Parameters.Add(paramNUOC2);

                            //var paramPHUGIA1 = command.CreateParameter();
                            //paramPHUGIA1.ParameterName = "@paramPHUGIA1";
                            //paramPHUGIA1.Value = capphoi.PHUGIA1;
                            //command.Parameters.Add(paramPHUGIA1);

                            //var paramPHUGIA2 = command.CreateParameter();
                            //paramPHUGIA2.ParameterName = "@paramPHUGIA2";
                            //paramPHUGIA2.Value = capphoi.PHUGIA2;
                            //command.Parameters.Add(paramPHUGIA2);

                            //var paramPHUGIA3 = command.CreateParameter();
                            //paramPHUGIA3.ParameterName = "@paramPHUGIA3";
                            //paramPHUGIA3.Value = capphoi.PHUGIA3;
                            //command.Parameters.Add(paramPHUGIA3);

                            //var paramPHUGIA4 = command.CreateParameter();
                            //paramPHUGIA4.ParameterName = "@paramPHUGIA4";
                            //paramPHUGIA4.Value = capphoi.PHUGIA4;
                            //command.Parameters.Add(paramPHUGIA4);

                            //var paramPHUGIA5 = command.CreateParameter();
                            //paramPHUGIA5.ParameterName = "@paramPHUGIA5";
                            //paramPHUGIA5.Value = capphoi.PHUGIA5;
                            //command.Parameters.Add(paramPHUGIA5);

                            //var paramPHUGIA6 = command.CreateParameter();
                            //paramPHUGIA6.ParameterName = "@paramPHUGIA6";
                            //paramPHUGIA6.Value = capphoi.PHUGIA6;
                            //command.Parameters.Add(paramPHUGIA6);

                            command.CommandText += "COMMIT TRANSACTION [Tran1] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1] END CATCH";

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

        [HttpPut("{macbetongid}")]

        public IActionResult PutCapPhoi(Guid MACBETONGID, [FromBody] CapPhoiDTO capphoi)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (capphoi == null)
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
                if (capphoi != null)
                {
                    using (var context = new CNTTVNWebContext())
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        command.CommandText += "BEGIN TRANSACTION [Tran1] BEGIN TRY ";
                        if (capphoi.BranchId != 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(capphoi.BranchId));
                            //Xoa du lieu cua bang SOLUONGVL theo MACBETONGID
                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL]  WHERE MACBETONGID = @paramMACBETONGID;";
                            //Cap nhat du lieu tren bang MACBETONG
                            command.CommandText += "UPDATE TOP(1) [" + branch.Dataname + "].[dbo].[MACBETONG] SET [MaLK]= @paramMaLienKet ,[TENMACBETONG]= @paramTenMacBeTong ,[CUONGDO] = @paramCUONGDO ,[COTLIEUMAX]= @paramCOTLIEUMAX,[DOSUT]=@paramDOSUT, [LASTUPDATED]= Getdate()  WHERE ID = @paramMACBETONGID;";


                            capphoi.MACBETONGID = MACBETONGID;



                            var paramMACBETONGID = command.CreateParameter();
                            paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                            paramMACBETONGID.Value = capphoi.MACBETONGID;
                            command.Parameters.Add(paramMACBETONGID);

                            var paramMaLienKet = command.CreateParameter();
                            paramMaLienKet.ParameterName = "@paramMaLienKet";
                            paramMaLienKet.Value = (capphoi.MALIENKET is null) ? String.Empty : (string)capphoi.MALIENKET;
                            command.Parameters.Add(paramMaLienKet);

                            var paramTenMacBeTong = command.CreateParameter();
                            paramTenMacBeTong.ParameterName = "@paramTenMacBeTong";
                            paramTenMacBeTong.Value = capphoi.TENMACBETONG;
                            command.Parameters.Add(paramTenMacBeTong);

                            var paramCUONGDO = command.CreateParameter();
                            paramCUONGDO.ParameterName = "@paramCUONGDO";
                            paramCUONGDO.Value = capphoi.CUONGDO;
                            command.Parameters.Add(paramCUONGDO);

                            var paramCOTLIEUMAX = command.CreateParameter();
                            paramCOTLIEUMAX.ParameterName = "@paramCOTLIEUMAX";
                            paramCOTLIEUMAX.Value = capphoi.COTLIEUMAX;
                            command.Parameters.Add(paramCOTLIEUMAX);

                            var paramDOSUT = command.CreateParameter();
                            paramDOSUT.ParameterName = "@paramDOSUT";
                            paramDOSUT.Value = capphoi.DOSUT;
                            command.Parameters.Add(paramDOSUT);

                            var paramLastUpdate = command.CreateParameter();
                            paramLastUpdate.ParameterName = "@paramLastUpdate";
                            paramLastUpdate.Value = DateTime.Now;
                            command.Parameters.Add(paramLastUpdate);

                            //INSER VALUE SOLUONGVL
                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SOLUONGVL] ([MACBETONGID], [MACUAVL], [SOLUONG], [ID],[MAMAC],[LASTUPDATED]) VALUES ";
                            for (int i = 0; i < capphoi.VatLieus.Count; i++)
                            {
                                if (capphoi.VatLieus[i].VALUE == null)
                                {
                                    capphoi.VatLieus[i].VALUE = 0;
                                }
                                if (i < capphoi.VatLieus.Count - 1)
                                    command.CommandText += "(@paramMACBETONGID, " + capphoi.VatLieus[i].MACUAVL + ", " + capphoi.VatLieus[i].VALUE.Value.ToString(CultureInfo.InvariantCulture) + ", " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                                else
                                    command.CommandText += "(@paramMACBETONGID, " + capphoi.VatLieus[i].MACUAVL + ", " + capphoi.VatLieus[i].VALUE.Value.ToString(CultureInfo.InvariantCulture) + ", " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate());";
                            }
                            //command.CommandText += "VALUES (@paramMACBETONGID, '1', @paramCAT1, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '2', @paramCAT2, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '3', @paramCAT3, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '4', @paramDA1, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '5', @paramDA2, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '6', @paramDA3, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '7', @paramXM1, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '8', @paramXM2, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '9', @paramXM3, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '10', @paramXM4, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '11', @paramNUOC1, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '12', @paramNUOC2, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '13', @paramPHUGIA1, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '14', @paramPHUGIA2, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '15', @paramPHUGIA3, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '16', @paramPHUGIA4, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '17', @paramPHUGIA5, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate()),";
                            //command.CommandText += " (@paramMACBETONGID, '18', @paramPHUGIA6, " + "'" + CustomGuid.NewSequentialId() + "'" + ",(SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] where ID = @paramMACBETONGID)" + ",Getdate());";

                            //var paramCAT1 = command.CreateParameter();
                            //paramCAT1.ParameterName = "@paramCAT1";
                            //paramCAT1.Value = capphoi.CAT1;
                            //command.Parameters.Add(paramCAT1);

                            //var paramCAT2 = command.CreateParameter();
                            //paramCAT2.ParameterName = "@paramCAT2";
                            //paramCAT2.Value = capphoi.CAT2;
                            //command.Parameters.Add(paramCAT2);

                            //var paramCAT3 = command.CreateParameter();
                            //paramCAT3.ParameterName = "@paramCAT3";
                            //paramCAT3.Value = capphoi.CAT3;
                            //command.Parameters.Add(paramCAT3);

                            //var paramDA1 = command.CreateParameter();
                            //paramDA1.ParameterName = "@paramDA1";
                            //paramDA1.Value = capphoi.DA1;
                            //command.Parameters.Add(paramDA1);

                            //var paramDA2 = command.CreateParameter();
                            //paramDA2.ParameterName = "@paramDA2";
                            //paramDA2.Value = capphoi.DA2;
                            //command.Parameters.Add(paramDA2);

                            //var paramDA3 = command.CreateParameter();
                            //paramDA3.ParameterName = "@paramDA3";
                            //paramDA3.Value = capphoi.DA3;
                            //command.Parameters.Add(paramDA3);

                            //var paramXM1 = command.CreateParameter();
                            //paramXM1.ParameterName = "@paramXM1";
                            //paramXM1.Value = capphoi.XM1;
                            //command.Parameters.Add(paramXM1);

                            //var paramXM2 = command.CreateParameter();
                            //paramXM2.ParameterName = "@paramXM2";
                            //paramXM2.Value = capphoi.XM2;
                            //command.Parameters.Add(paramXM2);

                            //var paramXM3 = command.CreateParameter();
                            //paramXM3.ParameterName = "@paramXM3";
                            //paramXM3.Value = capphoi.XM3;
                            //command.Parameters.Add(paramXM3);

                            //var paramXM4 = command.CreateParameter();
                            //paramXM4.ParameterName = "@paramXM4";
                            //paramXM4.Value = capphoi.XM4;
                            //command.Parameters.Add(paramXM4);

                            //var paramNUOC1 = command.CreateParameter();
                            //paramNUOC1.ParameterName = "@paramNUOC1";
                            //paramNUOC1.Value = capphoi.NUOC1;
                            //command.Parameters.Add(paramNUOC1);

                            //var paramNUOC2 = command.CreateParameter();
                            //paramNUOC2.ParameterName = "@paramNUOC2";
                            //paramNUOC2.Value = capphoi.NUOC2;
                            //command.Parameters.Add(paramNUOC2);

                            //var paramPHUGIA1 = command.CreateParameter();
                            //paramPHUGIA1.ParameterName = "@paramPHUGIA1";
                            //paramPHUGIA1.Value = capphoi.PHUGIA1;
                            //command.Parameters.Add(paramPHUGIA1);

                            //var paramPHUGIA2 = command.CreateParameter();
                            //paramPHUGIA2.ParameterName = "@paramPHUGIA2";
                            //paramPHUGIA2.Value = capphoi.PHUGIA2;
                            //command.Parameters.Add(paramPHUGIA2);

                            //var paramPHUGIA3 = command.CreateParameter();
                            //paramPHUGIA3.ParameterName = "@paramPHUGIA3";
                            //paramPHUGIA3.Value = capphoi.PHUGIA3;
                            //command.Parameters.Add(paramPHUGIA3);

                            //var paramPHUGIA4 = command.CreateParameter();
                            //paramPHUGIA4.ParameterName = "@paramPHUGIA4";
                            //paramPHUGIA4.Value = capphoi.PHUGIA4;
                            //command.Parameters.Add(paramPHUGIA4);

                            //var paramPHUGIA5 = command.CreateParameter();
                            //paramPHUGIA5.ParameterName = "@paramPHUGIA5";
                            //paramPHUGIA5.Value = capphoi.PHUGIA5;
                            //command.Parameters.Add(paramPHUGIA5);

                            //var paramPHUGIA6 = command.CreateParameter();
                            //paramPHUGIA6.ParameterName = "@paramPHUGIA6";
                            //paramPHUGIA6.Value = capphoi.PHUGIA6;
                            //command.Parameters.Add(paramPHUGIA6);

                            command.CommandText += "COMMIT TRANSACTION [Tran1] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1] END CATCH";

                        }
                        log.Error("Query:" + command.CommandText);
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            def.meta = new Meta(200, "Chinh sua thanh cong !");
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
        public IActionResult DeleteCapPhoi(string ID)
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
                        Guid MACBETONGID = System.Guid.Empty;


                        for (int i = 0; i < IdList.Length; i++)
                        {
                            if (i == 0)
                            {
                                Guid g = new Guid(IdList[i].ToString());
                                MACBETONGID = g;
                            }
                            if (i == 1)
                            {
                                branchID = int.Parse(IdList[i].ToString());
                            }


                        }

                        if (branchID > 0)
                        {

                            Branch branch = context.Branch.Find(Convert.ToInt32(branchID));

                            // check Mác bê tông đang được sử dụng hay không
                            command.CommandText += "SELECT Ma FROM [" + branch.Dataname + "].[dbo].[MACBETONG] WHERE ID = @paramID;";
                            var paramID = command.CreateParameter();
                            paramID.ParameterName = "@paramID";
                            paramID.Value = MACBETONGID;
                            command.Parameters.Add(paramID);

                            context.Database.OpenConnection();
                            using (var resultMa = command.ExecuteReader())
                            {
                                resultMa.Read();

                                var ma = (string)resultMa["Ma"];
                                if (!string.IsNullOrEmpty(ma))
                                {
                                    context.Database.CloseConnection();
                                    command.CommandText = "SELECT * FROM [" + branch.Dataname + "].[dbo].[DATHANG]  WHERE MAMACBETONG = @paramMa;";
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
                                            def.meta = new Meta(212, "Mác bê tông này đang được sử dụng tại đơn hàng " + maDathang + ", không thể xóa !");
                                            return Ok(def);
                                        }
                                        else
                                        {
                                            context.Database.CloseConnection();
                                            command.CommandText = string.Empty;
                                            command.CommandText += "BEGIN TRANSACTION [Tran1cp] BEGIN TRY ";


                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES(@paramTableName,@paramID,'3',Getdate());";

                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 1 AND MACBETONGID = @paramMID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 2 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 3 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 4 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 5 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 6 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 7 AND MACBETONGID = @paramID),'3',Getdate());";
                                            command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SynInfo]([TableName],[RecordID],[ChangeType],[Lastupdated]) VALUES('SOLUONGVL',(SELECT ID FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL] WHERE MACUAVL = 8 AND MACBETONGID = @paramID),'3',Getdate());";

                                            //xoa trong bang SOLUONGVL
                                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[SOLUONGVL]  WHERE MACBETONGID = @paramID;";
                                            //Xoa trong bang MACBETONG
                                            command.CommandText += "DELETE FROM [" + branch.Dataname + "].[dbo].[MACBETONG]  WHERE ID = @paramID;";

                                            //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";

                                            var paramMID = command.CreateParameter();
                                            paramMID.ParameterName = "@paramMID";
                                            paramMID.Value = MACBETONGID.ToString();
                                            //paramID.DbType = DbType.Guid;
                                            command.Parameters.Add(paramMID);


                                            var paramTableName = command.CreateParameter();
                                            paramTableName.ParameterName = "@paramTableName";
                                            paramTableName.Value = "MACBETONG";
                                            command.Parameters.Add(paramTableName);

                                            var paramTableNameVL = command.CreateParameter();
                                            paramTableNameVL.ParameterName = "@paramTableNameVL";
                                            paramTableNameVL.Value = "SOLUONGVL";
                                            command.Parameters.Add(paramTableNameVL);

                                            var paramLastupdated = command.CreateParameter();
                                            paramLastupdated.ParameterName = "@paramLastupdated";
                                            paramLastupdated.Value = DateTime.Now.ToString();
                                            command.Parameters.Add(paramLastupdated);





                                            command.CommandText += "COMMIT TRANSACTION [Tran1cp] END TRY BEGIN CATCH ROLLBACK TRANSACTION [Tran1cp] END CATCH";

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
                            def.meta = new Meta(200, "Xoa g thanh cong !");
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