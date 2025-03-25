using DocumentFormat.OpenXml.Drawing.Charts;
using IOITWebApp;
using IOITWebApp.Helper;
using IOITWebApp.Models;
using IOITWebApp.Models.Bravo;
using IOITWebApp.Models.Common;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using IOITWebApp.Models.Station;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.Functions;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.Controllers.ApiBravo
{
    [Route("api/[controller]")]
    [ApiController]
    public class BravoDataController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("BravoData", "BravoData");
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public BravoDataController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequestModel loginModel)
        {
            APIResponseData def = new APIResponseData();
            try
            {
                if (loginModel != null)
                {
                    string username = loginModel.username;

                    using (var db = new CNTTVNWebContext())
                    {
                        var userRole = new List<UserRole>();
                        var user = db.User.Where(e => e.UserName == username && e.Status != (int)Const.Status.DELETED).ToList();
                        //if (user.Count > 0)
                        //{
                        string password = user.FirstOrDefault().KeyLock.Trim() + user.FirstOrDefault().RegEmail.Trim() + user.FirstOrDefault().UserId + Utils.GetMD5Hash(loginModel.password.Trim());
                        password = Utils.GetMD5Hash(password);
                        var userLogin1 = from person in db.User
                                         where person.UserName == username && person.Password == password && person.Status != (int)Const.Status.DELETED
                                         select new UserPartnerLogin()
                                         {
                                             userId = person.UserId,
                                             userName = person.UserName,
                                             //email = person.Email,
                                             fullName = person.FullName,
                                             //password = person.Password,
                                             // phone = person.Phone,
                                             status = person.Status,
                                             isRoleGroup = person.IsRoleGroup != null ? (bool)person.IsRoleGroup : true,
                                         }
                                     ;
                        var userLogin = userLogin1.FirstOrDefault();
                        if (userLogin != null)
                        {
                            //check if user lock
                            if (userLogin.status == (int)Const.Status.LOCK)
                            {
                                def.meta = new Meta(223, "User Locked");
                                return Ok(def);
                            }

                            var userId = userLogin.userId;
                            List<MenuDTO> listFunctionRole = new List<MenuDTO>();
                            //lấy danh sách quyền theo chức năng, nếu danh sách quyền theo chức năng null thì lấy
                            //danh sách quyền theo nhóm quyền

                            if (!userLogin.isRoleGroup)
                            {
                                var listFR = db.FunctionRole.Where(e => e.TargetId == userId && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                                && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Function.Location).ToList();
                                foreach (var itemFR in listFR)
                                {
                                    //check exits
                                    var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                    if (fr.Count > 0)
                                    {
                                        string key1 = fr.FirstOrDefault().ActiveKey;
                                        if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                        {
                                            key1 = plusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                        }
                                        fr.FirstOrDefault().ActiveKey = key1;
                                    }
                                    else
                                    {
                                        MenuDTO menu = new MenuDTO();
                                        menu.MenuId = itemFR.FunctionId;
                                        menu.Code = itemFR.Function.Code;
                                        menu.Name = itemFR.Function.Name;
                                        menu.Url = itemFR.Function.Url;
                                        menu.Icon = itemFR.Function.Icon;
                                        menu.MenuParent = (int)itemFR.Function.FunctionParentId;
                                        menu.ActiveKey = itemFR.ActiveKey;
                                        listFunctionRole.Add(menu);
                                    }
                                }
                            }
                            else
                            {
                                //get list user role
                                userRole = db.UserRole.Where(e => e.UserId == userId && e.Status == (int)Const.Status.NORMAL).ToList();
                                //get list function role
                                foreach (var item in userRole)
                                {
                                    var listFRR = db.FunctionRole.Where(e => e.TargetId == item.RoleId && e.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                        && e.Status == (int)Const.Status.NORMAL).OrderBy(e => e.Function.Location).ToList();
                                    foreach (var itemFR in listFRR)
                                    {
                                        //check exits
                                        var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                        if (fr.Count > 0)
                                        {
                                            string key1 = fr.FirstOrDefault().ActiveKey;
                                            if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                            {
                                                key1 = plusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                            }
                                            fr.FirstOrDefault().ActiveKey = key1;
                                        }
                                        else
                                        {
                                            Models.EF.Function function = db.Function.Where(e => e.FunctionId == itemFR.FunctionId).FirstOrDefault();
                                            if (function != null)
                                            {
                                                MenuDTO menu = new MenuDTO();
                                                menu.MenuId = itemFR.FunctionId;
                                                menu.Code = function.Code;
                                                menu.Name = function.Name;
                                                menu.Url = function.Url;
                                                menu.Icon = function.Icon;
                                                menu.MenuParent = (int)function.FunctionParentId;
                                                menu.ActiveKey = itemFR.ActiveKey;
                                                listFunctionRole.Add(menu);
                                            }
                                        }
                                    }
                                }
                            }

                            string access_key = "";
                            int count = listFunctionRole.Count;
                            if (count > 0)
                            {
                                for (int i = 0; i < count - 1; i++)
                                {
                                    if (listFunctionRole[i].ActiveKey != "000000000")
                                    {
                                        access_key += listFunctionRole[i].Code + ":" + listFunctionRole[i].ActiveKey + "-";
                                    }
                                }

                                access_key = access_key + listFunctionRole[count - 1].Code + ":" + listFunctionRole[count - 1].ActiveKey;
                            }

                            //userLogin.access_key = access_key;
                            var claims = new List<Claim>
                                {
                                    //new Claim(JwtRegisteredClaimNames.Email, userLogin.email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    //new Claim(ClaimTypes.NameIdentifier, userLogin.userId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.fullName),
                                        //new Claim("UserId", userLogin.userId != null ? userLogin.userId.ToString() : ""),
                                     new Claim("AccessKey", access_key != null ? access_key : ""),
                                };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));

                            var token = new JwtSecurityToken(
                                _configuration["AppSettings:JwtIssuer"],
                                _configuration["AppSettings:JwtIssuer"],
                                claims,
                                expires: expires,
                                signingCredentials: creds
                            );
                            var response = new LoginResponseModel() { 
                                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                                access_key = null
                            };

                            def.data = response;
                            def.meta = new Meta(200, "success");
                            return Ok(def);
                        }
                        else
                        {
                            //check if email exist
                            var existed = db.User.Where(e => e.UserName == username && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (existed != null)
                            {
                                def.meta = new Meta(213, "Invalid data");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(404, "Not found");
                                return Ok(def);
                            }
                        }
                    }
                }
                else
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Info("Exception:" + e);
                def.meta = new Meta(500, "Error Server");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPost("dmvattu")]
        public IActionResult PostVatTu([FromBody] DanhMucVatTuRequestModel pg)
        {
            string functionCode = "dmvattu";
            var res = new APIResponseData();
            try
            {
                if (pg == null)
                {
                    res.meta = new Meta(400, "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                if (identity == null)
                {
                    res.meta = new Meta(401, "Unauthorized");
                    return Ok(res);
                }
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
                {
                    res.meta = new Meta(403, "Không được phép truy cập, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    if (pg.BranchId != 0)
                    {
                        Branch branch = context.Branch.Find(Convert.ToInt32(pg.BranchId));
                        if (branch == null)
                        {
                            res.meta = new Meta(400, "Không tìm thấy trạm tương ứng, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }
                        //sinh ID tu dong
                        var id = CustomGuid.NewSequentialId();

                        //Chỉ sinh mã cho các trạm khác Minh Đức
                        if (branch.CompanyId != 3061)
                        {
                            pg.MaVatTu = CommonLib.GetSo("PHUGIA", "Ma", "VL1_", branch.Dataname);
                        }

                        // Check mã vật tư đã tồn tại chưa
                        var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[PHUGIA] WHERE Ma = {pg.MaVatTu}";
                        var vattu = DapperHepper.Query<PhuGiaDTO>(LocalSettings.ConnectString, qrCheck);
                        if (!vattu.Any() && vattu != null)
                        {
                            res.meta = new Meta(400, "Mã vật tư đã tồn tại trên hệ thống, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }

                        command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[PHUGIA] ([ID], [Ma], [TENPG], [NHACUNGCAP], [MALOAIVL], [TENLOAIVL], [HESOQUYDOI], [DONVIQUYDOI], [LASTUPDATED]) ";
                        command.CommandText += "VALUES (@paramID,@paramMa,@paramTENPG,@paramNHACUNGCAP, @paramMaLoaiVL, @paramTenLoaiVL, @paramHeSoQuyDoi, @paramDonViQuyDoi,Getdate())";
                        //command.CommandText += "VALUES ("+ khachhang.ID+","+khachhang.Ma + "," + khachhang.TENKHACHHANG + "," + khachhang.SDT + "," + khachhang.ISSYNC + "," + khachhang.SYSCCHENGE + "," + khachhang.DIACHI + ")";
                        var paramID = command.CreateParameter();
                        paramID.ParameterName = "@paramID";
                        paramID.Value = id;
                        command.Parameters.Add(paramID);

                        var paramMa = command.CreateParameter();
                        paramMa.ParameterName = "@paramMa";
                        paramMa.Value = pg.MaVatTu;
                        command.Parameters.Add(paramMa);

                        var paramTENPG = command.CreateParameter();
                        paramTENPG.ParameterName = "@paramTENPG";
                        paramTENPG.Value = (pg.TenVatTu is null) ? string.Empty : pg.TenVatTu.ToString();
                        command.Parameters.Add(paramTENPG);

                        var paramNHACUNGCAP = command.CreateParameter();
                        paramNHACUNGCAP.ParameterName = "@paramNHACUNGCAP";
                        paramNHACUNGCAP.Value = (pg.NhaCungCap is null) ? string.Empty : pg.NhaCungCap.ToString();
                        command.Parameters.Add(paramNHACUNGCAP);

                        var paramMaLoaiVL = command.CreateParameter();
                        paramMaLoaiVL.ParameterName = "@paramMaLoaiVL";
                        paramMaLoaiVL.Value = pg.MaLoaiVL;
                        command.Parameters.Add(paramMaLoaiVL);

                        var paramTenLoaiVL = command.CreateParameter();
                        paramTenLoaiVL.ParameterName = "@paramTenLoaiVL";
                        paramTenLoaiVL.Value = pg.TenLoaiVL;
                        command.Parameters.Add(paramTenLoaiVL);

                        var paramHeSoQuyDoi = command.CreateParameter();
                        paramHeSoQuyDoi.ParameterName = "@paramHeSoQuyDoi";
                        paramHeSoQuyDoi.Value = pg.HeSoQuyDoi;
                        command.Parameters.Add(paramHeSoQuyDoi);

                        var paramDonViQuyDoi = command.CreateParameter();
                        paramDonViQuyDoi.ParameterName = "@paramDonViQuyDoi";
                        paramDonViQuyDoi.Value = pg.DonViQuyDoi;
                        command.Parameters.Add(paramDonViQuyDoi);

                    }
                    else
                    {
                        res.meta = new Meta(400, "Mã trạm không hợp lệ, vui lòng kiểm tra lại!");
                        return Ok(res);
                    }
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        res.meta = new Meta(200, "Thêm mới thành công");
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                res.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(res);
            }
        }

        [Authorize]
        [HttpPost("dmmacbetong")]
        public IActionResult PostMacBeTong([FromBody] DanhMucMacRequestModel capphoi)
        {
            string functionCode = "dmmacbetong";
            var res = new APIResponseData();
            try
            {
                if (capphoi == null)
                {
                    res.meta = new Meta(400, "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                if (identity == null)
                {
                    res.meta = new Meta(401, "Unauthorized");
                    return Ok(res);
                }
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
                {
                    res.meta = new Meta(403, "Không được phép truy cập, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    if (capphoi.BranchId != 0)
                    {
                        Branch branch = context.Branch.Find(Convert.ToInt32(capphoi.BranchId));
                        if (branch == null)
                        {
                            res.meta = new Meta(400, "Không tìm thấy trạm tương ứng, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }
                        //sinh ID tu dong
                        var id = CustomGuid.NewSequentialId();

                        //Chỉ sinh mã cho các trạm khác Minh Đức
                        if (branch.CompanyId != 3061)
                        {
                            capphoi.MaMac = CommonLib.GetSo("MACBETONG", "Ma", "MAC1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                        }

                        // Check mã vật tư đã tồn tại chưa
                        var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[MACBETONG] WHERE [Ma] = {capphoi.MaMac}";
                        var macBT = DapperHepper.Query<CapPhoiDTO>(LocalSettings.ConnectString, qrCheck);
                        if (macBT.Any() && macBT != null)
                        {
                            res.meta = new Meta(400, "Mã mác bê tông đã tồn tại trên hệ thống, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }

                        command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[MACBETONG]([ID], [Ma], [MaLK], [TENMACBETONG], [CUONGDO], [COTLIEUMAX], [DOSUT],[LASTUPDATED]) ";
                        command.CommandText += "VALUES (@paramMACBETONGID,@paramMa,@paramTenMacBeTong,@paramCUONGDO,@paramCOTLIEUMAX,@paramDOSUT,Getdate());";
                        var paramMACBETONGID = command.CreateParameter();
                        paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                        paramMACBETONGID.Value = id;
                        command.Parameters.Add(paramMACBETONGID);

                        var paramMa = command.CreateParameter();
                        paramMa.ParameterName = "@paramMa";
                        paramMa.Value = capphoi.MaMac;
                        command.Parameters.Add(paramMa);

                        var paramTenMacBeTong = command.CreateParameter();
                        paramTenMacBeTong.ParameterName = "@paramTenMacBeTong";
                        paramTenMacBeTong.Value = capphoi.TenMac;
                        command.Parameters.Add(paramTenMacBeTong);

                        var paramCUONGDO = command.CreateParameter();
                        paramCUONGDO.ParameterName = "@paramCUONGDO";
                        paramCUONGDO.Value = capphoi.CuongDo;
                        command.Parameters.Add(paramCUONGDO);

                        var paramCOTLIEUMAX = command.CreateParameter();
                        paramCOTLIEUMAX.ParameterName = "@paramCOTLIEUMAX";
                        paramCOTLIEUMAX.Value = capphoi.CotLieuMax;
                        command.Parameters.Add(paramCOTLIEUMAX);

                        var paramDOSUT = command.CreateParameter();
                        paramDOSUT.ParameterName = "@paramDOSUT";
                        paramDOSUT.Value = capphoi.DoSut;
                        command.Parameters.Add(paramDOSUT);

                    }
                    else
                    {
                        res.meta = new Meta(400, "Mã trạm không hợp lệ, vui lòng kiểm tra lại!");
                        return Ok(res);
                    }
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        res.meta = new Meta(200, "Thêm mới thành công");
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                res.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(res);
            }
        }

        [Authorize]
        [HttpPost("dmcapphoibetong")]
        public IActionResult PostSoLuongVL([FromBody] DanhMucCapPhoiBeTongRequestModel capphoi)
        {
            string functionCode = "dmmacbetong";
            var res = new APIResponseData();
            try
            {
                if (capphoi == null)
                {
                    res.meta = new Meta(400, "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                if (identity == null)
                {
                    res.meta = new Meta(401, "Unauthorized");
                    return Ok(res);
                }
                string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
                {
                    res.meta = new Meta(403, "Không được phép truy cập, vui lòng kiểm tra lại!");
                    return Ok(res);
                }
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    if (capphoi.BranchId != 0)
                    {
                        Branch branch = context.Branch.Find(Convert.ToInt32(capphoi.BranchId));
                        if (branch == null)
                        {
                            res.meta = new Meta(400, "Không tìm thấy trạm tương ứng, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }
                        //sinh ID tu dong
                        var id = CustomGuid.NewSequentialId();

                        // Lấy thông tin mác bê tông theo mã Mác
                        var qrMac = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[MACBETONG] WHERE [Ma] = {capphoi.MaMac}";
                        var macBT = DapperHepper.Query<CapPhoiDTO>(LocalSettings.ConnectString, qrMac);
                        if (!macBT.Any() && macBT == null)
                        {
                            res.meta = new Meta(400, "Mã mác bê tông không tồn tại, vui lòng kiểm tra lại!");
                            return Ok(res);
                        }

                        command.CommandText += "INSERT INTO [" + branch.Dataname + "].[dbo].[SOLUONGVL] ([MACBETONGID], [MACUAVL], [SOLUONG], [ID], [Ma],[MAMAC], [MAVL], [TENVL],[LASTUPDATED]) VALUES";
                        command.CommandText += "(@paramMACBETONGID, @paramMaCuaVL, @paramMaSOLUONGVL, @paramID, @paramMa, @paramMaMac, @paramMaVL, @paramTenVL,Getdate()),";

                        var paramMACBETONGID = command.CreateParameter();
                        paramMACBETONGID.ParameterName = "@paramMACBETONGID";
                        paramMACBETONGID.Value = macBT.FirstOrDefault().ID;
                        command.Parameters.Add(paramMACBETONGID);

                        var paramMaCuaVL = command.CreateParameter();
                        paramMaCuaVL.ParameterName = "@paramMaCuaVL";
                        paramMaCuaVL.Value = capphoi.MaCuaVL;
                        command.Parameters.Add(paramMaCuaVL);

                        var paramMaSOLUONGVL = command.CreateParameter();
                        paramMaSOLUONGVL.ParameterName = "@paramMaSOLUONGVL";
                        paramMaSOLUONGVL.Value = capphoi.SoLuong;
                        command.Parameters.Add(paramMaSOLUONGVL);

                        var paramID = command.CreateParameter();
                        paramID.ParameterName = "@paramID";
                        paramID.Value = id;
                        command.Parameters.Add(paramID);

                        var paramMa = command.CreateParameter();
                        paramMa.ParameterName = "@paramMa";
                        paramMa.Value = capphoi.Ma;
                        command.Parameters.Add(paramMa);

                        var paramMaMac = command.CreateParameter();
                        paramMaMac.ParameterName = "@paramMaMac";
                        paramMaMac.Value = macBT.FirstOrDefault().Ma;
                        command.Parameters.Add(paramMaMac);

                        var paramMaVL = command.CreateParameter();
                        paramMaVL.ParameterName = "@paramMaVL";
                        paramMaVL.Value = capphoi.MaVatLieu;
                        command.Parameters.Add(paramMaVL);

                        var paramTenVL = command.CreateParameter();
                        paramTenVL.ParameterName = "@paramTenVL";
                        paramTenVL.Value = capphoi.TenVatLieu;
                        command.Parameters.Add(paramTenVL);

                    }
                    else
                    {
                        res.meta = new Meta(400, "Mã trạm không hợp lệ, vui lòng kiểm tra lại!");
                        return Ok(res);
                    }
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        res.meta = new Meta(200, "Thêm mới thành công");
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                res.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(res);
            }
        }
        private string plusActiveKey(string key1, string key2)
        {
            string str = "";
            char[] str1 = key1.ToCharArray();
            char[] str2 = key2.ToCharArray();
            for (int i = 0; i < str1.Length; i++)
            {
                int k = int.Parse(str1[i].ToString()) + int.Parse(str2[i].ToString());
                if (k > 1) k = 1;
                str += k;
            }
            return str;
        }
    }
}
