using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
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
using System.Data.Common;
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
                if (loginModel == null || string.IsNullOrEmpty(loginModel.username) || string.IsNullOrEmpty(loginModel.password))
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                string username = loginModel.username.Trim();
                string password = loginModel.password.Trim();

                using (var db = new CNTTVNWebContext())
                {
                    var user = db.User
                                 .Where(u => u.UserName == username && u.Status != (int)Const.Status.DELETED)
                                 .FirstOrDefault();

                    if (user == null)
                    {
                        def.meta = new Meta(404, "Not found");
                        return Ok(def);
                    }

                    string hashedPassword = Utils.GetMD5Hash(user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + Utils.GetMD5Hash(password));
                    if (user.Password != hashedPassword)
                    {
                        def.meta = new Meta(213, "Invalid data");
                        return Ok(def);
                    }

                    // Check if user is locked
                    if (user.Status == (int)Const.Status.LOCK)
                    {
                        def.meta = new Meta(223, "User Locked");
                        return Ok(def);
                    }

                    // Retrieve function roles
                    var listFunctionRole = GetUserFunctionRoles(user.UserId, db, user.IsRoleGroup);

                    string accessKey = GenerateAccessKey(listFunctionRole);

                    // Create JWT token
                    var token = GenerateJwtToken(user.FullName, accessKey);

                    var response = new LoginResponseModel
                    {
                        access_token = new JwtSecurityTokenHandler().WriteToken(token),
                        access_key = accessKey
                    };

                    def.data = response;
                    def.meta = new Meta(200, "success");
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
        public async Task<IActionResult> PostVatTu([FromBody] DanhMucVatTuRequestModel pg)
        {
            string functionCode = "dmvattu";
            var res = new APIResponseData();
            try
            {
                if (pg == null)
                {
                    return BadRequestResponse("Dữ liệu không hợp lệ, vui lòng kiểm tra lại!");
                }

                // Kiểm tra quyền truy cập
                var identity = (ClaimsIdentity)User.Identity;
                if (identity == null || !HasAccess(identity, functionCode))
                {
                    return UnauthorizedResponse();
                }

                using (var context = new CNTTVNWebContext())
                {
                    if (pg.BranchId == 0)
                    {
                        return BadRequestResponse("Mã trạm không hợp lệ, vui lòng kiểm tra lại!");
                    }

                    Branch branch = context.Branch.Find(Convert.ToInt32(pg.BranchId));
                    if (branch == null)
                    {
                        return BadRequestResponse("Không tìm thấy trạm tương ứng, vui lòng kiểm tra lại!");
                    }

                    var id = CustomGuid.NewSequentialId();
                    var maVatTu = CommonLib.GetSo("PHUGIA", "Ma", "VL1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);

                    // Check mã vật tư đã tồn tại chưa
                    var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[PHUGIA] WHERE MaLK = @MaVatTu";

                    var connection = context.Database.GetDbConnection();

                    var vattu = await connection.QueryFirstOrDefaultAsync<PhuGiaDTO>(qrCheck, new { MaVatTu = pg.MaVatTu });

                    if (vattu != null)
                    {
                        await UpdateVatTu(context, branch.Dataname, vattu.ID, pg);
                        res.meta = new Meta(200, "Cập nhật thành công");
                    }
                    else
                    {
                        await InsertVatTu(context, branch.Dataname, id, maVatTu, pg);
                        res.meta = new Meta(200, "Thêm mới thành công");
                    }

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                return InternalServerErrorResponse();
            }
        }
        [Authorize]
        [HttpPost("dmmacbetong")]
        public async Task<IActionResult> PostMacBeTong([FromBody] DanhMucMacRequestModel capphoi)
        {
            string functionCode = "dmmacbetong";
            var res = new APIResponseData();

            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (capphoi == null)
                {
                    return BadRequestResponse("Dữ liệu không hợp lệ, vui lòng kiểm tra lại!");
                }

                // Kiểm tra quyền truy cập
                var identity = (ClaimsIdentity)User.Identity;
                if (identity == null || !HasAccess(identity, functionCode))
                {
                    return UnauthorizedResponse();
                }

                // Lấy thông tin trạm
                if (capphoi.BranchId == 0)
                {
                    return BadRequestResponse("Mã trạm không hợp lệ, vui lòng kiểm tra lại!");
                }

                // Xử lý kết nối cơ sở dữ liệu
                using (var context = new CNTTVNWebContext())
                using (var command = CreateDbCommand(context))
                {
                    var branch = context.Branch.Find(Convert.ToInt32(capphoi.BranchId));
                    if (branch == null)
                    {
                        return BadRequestResponse("Không tìm thấy trạm tương ứng, vui lòng kiểm tra lại!");
                    }

                    // Sinh mã ID tự động
                    var id = CustomGuid.NewSequentialId();
                    var maMac = CommonLib.GetSo("MACBETONG", "Ma", "MAC1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);

                    // Kiểm tra mã mác bê tông đã tồn tại
                    var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[MACBETONG] WHERE [MaLK] = {capphoi.MaMac}";
                    var macBT = DapperHepper.Query<CapPhoiDTO>(LocalSettings.ConnectString, qrCheck);

                    if (macBT != null && macBT.Any())
                    {
                        // Cập nhật mác bê tông nếu đã tồn tại
                        UpdateMacBetong(command, branch, macBT.FirstOrDefault(), capphoi);
                        res.meta = new Meta(200, "Cập nhật thành công");
                    }
                    else
                    {
                        // Thêm mới mác bê tông
                        InsertMacBetong(command, branch, id, capphoi, maMac);
                        res.meta = new Meta(200, "Thêm mới thành công");
                    }

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                return InternalServerErrorResponse();
            }
        }

        [Authorize]
        [HttpPost("dmcapphoibetong")]
        public async Task<IActionResult> PostSoLuongVL([FromBody] DanhMucCapPhoiBeTongRequestModel capphoi)
        {
            string functionCode = "dmmacbetong";
            var res = new APIResponseData();

            try
            {
                if (capphoi == null)
                {
                    return Ok(new APIResponseData { meta = new Meta(400, "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!") });
                }

                // Check role
                var identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    return UnauthorizedResponse();
                }

                string access_key = identity.Claims.FirstOrDefault(c => c.Type == "AccessKey")?.Value;
                if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
                {
                    return UnauthorizedResponse();
                }

                using (var context = new CNTTVNWebContext())
                {
                    if (capphoi.BranchId == 0)
                    {
                        return BadRequestResponse("Mã trạm không hợp lệ!");
                    }

                    var branch = await context.Branch.FindAsync(capphoi.BranchId);
                    if (branch == null)
                    {
                        return BadRequestResponse("Không tìm thấy trạm tương ứng!");
                    }

                    var query = "SELECT TOP 1 * FROM [" + branch.Dataname + "].[dbo].[MACBETONG] WHERE [Ma] = @MaMac";
                    var parameters = new { MaMac = capphoi.Items.Select(i => i.MaMac).Distinct() };

                    var macBTs = (await DapperHepper.QueryAsync<CapPhoiDTO>(LocalSettings.ConnectString, query, parameters)).ToDictionary(m => m.Ma);

                    if (!macBTs.Any())
                    {
                        return BadRequestResponse("Mã mác bê tông không tồn tại!");
                    }

                    var insertData = new List<object>();

                    foreach (var item in capphoi.Items)
                    {
                        if (!macBTs.TryGetValue(item.MaMac, out var macBT))
                        {
                            return Ok(new APIResponseData { meta = new Meta(400, $"Mã mác {item.MaMac} không tồn tại!") });
                        }

                        insertData.Add(new
                        {
                            MACBETONGID = macBT.ID,
                            MaCuaVL = item.MaCuaVL,
                            SoLuong = item.SoLuong,
                            ID = CustomGuid.NewSequentialId(),
                            MaMac = macBT.Ma,
                            MaVL = item.MaVatLieu,
                            TenVL = item.TenVatLieu,
                            MaBravo = item.Ma,
                            TimeChange = item.TimeChange
                        });
                    }

                    if (insertData.Any())
                    {
                        var sqlInsert = $"INSERT INTO [{branch.Dataname}].[dbo].[SOLUONGVL] " +
                                        "([MACBETONGID], [MACUAVL], [SOLUONG], [ID], [MAMAC], [MAVL], [TENVL], [MaLK], [LASTUPDATED]) " +
                                        "VALUES (@MACBETONGID, @MaCuaVL, @SoLuong, @ID, @MaMac, @MaVL, @TenVL, @MaBravo, @TimeChange)";

                        DapperHepper.ExecuteNew(LocalSettings.ConnectString, sqlInsert, insertData);

                    }

                    return Ok(new APIResponseData { meta = new Meta(200, "Thêm mới thành công!") });
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return InternalServerErrorResponse();
            }

        }

        /// <summary>
        /// Phương thức giúp trả về response lỗi 400
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IActionResult BadRequestResponse(string message)
        {
            var res = new APIResponseData
            {
                meta = new Meta(400, message)
            };
            return Ok(res);
        }

        /// <summary>
        /// Phương thức giúp trả về response lỗi 401 (Unauthorized)
        /// </summary>
        /// <returns></returns>
        private IActionResult UnauthorizedResponse()
        {
            var res = new APIResponseData
            {
                meta = new Meta(401, "Unauthorized")
            };
            return Ok(res);
        }

        // Phương thức giúp trả về response lỗi 500
        private IActionResult InternalServerErrorResponse()
        {
            var res = new APIResponseData
            {
                meta = new Meta(500, "Lỗi máy chủ!")
            };
            return Ok(res);
        }

        /// <summary>
        /// Phương thức kiểm tra quyền truy cập
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        private bool HasAccess(ClaimsIdentity identity, string functionCode)
        {
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            return CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE);
        }

        /// <summary>
        /// Tạo DbCommand
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DbCommand CreateDbCommand(CNTTVNWebContext context)
        {
            return context.Database.GetDbConnection().CreateCommand();
        }

        /// <summary>
        /// Thêm mới vật tư
        /// </summary>
        /// <param name="context"></param>
        /// <param name="branchDataname"></param>
        /// <param name="id"></param>
        /// <param name="maVatTu"></param>
        /// <param name="pg"></param>
        /// <returns></returns>
        private async Task InsertVatTu(CNTTVNWebContext context, string branchDataname, Guid id, string maVatTu, DanhMucVatTuRequestModel pg)
        {
            var connection = context.Database.GetDbConnection();
            string insertSql = $@"
                                    INSERT INTO [{branchDataname}].[dbo].[PHUGIA] 
                                    ([ID], [Ma], [TENPG], [NHACUNGCAP], [MALOAIVL], [TENLOAIVL], [HESOQUYDOI], [DONVIQUYDOI], [MaLK], [LASTUPDATED]) 
                                    VALUES (@ID, @Ma, @TENPG, @NHACUNGCAP, @MALOAIVL, @TENLOAIVL, @HESOQUYDOI, @DONVIQUYDOI, @MaBravo, Getdate())";

            await connection.ExecuteAsync(insertSql, new
            {
                ID = id, // Đảm bảo rằng id là Guid
                Ma = maVatTu,
                TENPG = pg.TenVatTu ?? string.Empty,
                NHACUNGCAP = pg.NhaCungCap ?? string.Empty,
                MALOAIVL = pg.MaLoaiVL,
                TENLOAIVL = pg.TenLoaiVL,
                HESOQUYDOI = pg.HeSoQuyDoi,
                DONVIQUYDOI = pg.DonViQuyDoi,
                MaBravo = pg.MaVatTu
            });
        }

        /// <summary>
        /// Cập nhật vật tư
        /// </summary>
        /// <param name="context"></param>
        /// <param name="branchDataname"></param>
        /// <param name="id"></param>
        /// <param name="pg"></param>
        /// <returns></returns>
        private async Task UpdateVatTu(CNTTVNWebContext context, string branchDataname, Guid id, DanhMucVatTuRequestModel pg)
        {
            var connection = context.Database.GetDbConnection();
            string updateSql = $@"
                                    UPDATE TOP(1) [{branchDataname}].[dbo].[PHUGIA]
                                    SET [TENPG] = @TENPG, [NHACUNGCAP] = @NHACUNGCAP, [MALOAIVL] = @MALOAIVL, 
                                        [TENLOAIVL] = @TENLOAIVL, [HESOQUYDOI] = @HESOQUYDOI, [DONVIQUYDOI] = @DONVIQUYDOI, 
                                        [LASTUPDATED] = Getdate()
                                    WHERE [ID] = @ID";

            await connection.ExecuteAsync(updateSql, new
            {
                ID = id, // Đảm bảo rằng id là Guid
                TENPG = pg.TenVatTu ?? string.Empty,
                NHACUNGCAP = pg.NhaCungCap ?? string.Empty,
                MALOAIVL = pg.MaLoaiVL,
                TENLOAIVL = pg.TenLoaiVL,
                HESOQUYDOI = pg.HeSoQuyDoi,
                DONVIQUYDOI = pg.DonViQuyDoi
            });
        }

        /// <summary>
        /// Cập nhật mác bê tông
        /// </summary>
        /// <param name="command"></param>
        /// <param name="branch"></param>
        /// <param name="macBetongId"></param>
        /// <param name="capphoi"></param>
        private void UpdateMacBetong(DbCommand command, Branch branch, CapPhoiDTO capPhoiDTO, DanhMucMacRequestModel capphoi)
        {
            command.CommandText = $"UPDATE TOP(1) [{branch.Dataname}].[dbo].[MACBETONG] SET " +
                                  "[TENMACBETONG]= @paramTenMacBeTong, [CUONGDO] = @paramCUONGDO, " +
                                  "[COTLIEUMAX]= @paramCOTLIEUMAX, [DOSUT]=@paramDOSUT, " +
                                  "[GhiChu]=@paramGhiChu, [DONVIQUYDOI]=@paramDONVIQUYDOI, " +
                                  "[LASTUPDATED]= Getdate() WHERE ID = @paramMACBETONGID;";
            AddParameters(command, capPhoiDTO.ID, capphoi, capPhoiDTO.Ma);
        }

        /// <summary>
        /// Thêm mới mác bê tông
        /// </summary>
        /// <param name="command"></param>
        /// <param name="branch"></param>
        /// <param name="id"></param>
        /// <param name="capphoi"></param>
        private void InsertMacBetong(DbCommand command, Branch branch, Guid id, DanhMucMacRequestModel capphoi, string maMac)
        {
            command.CommandText = $"INSERT INTO [{branch.Dataname}].[dbo].[MACBETONG] " +
                                  "([ID], [Ma], [TENMACBETONG], [CUONGDO], [COTLIEUMAX], [DOSUT], " +
                                  "[GhiChu], [DONVIQUYDOI], [MaLK], [LASTUPDATED]) " +
                                  "VALUES (@paramMACBETONGID, @paramMa, @paramTenMacBeTong, @paramCUONGDO, " +
                                  "@paramCOTLIEUMAX, @paramDOSUT, @paramGhiChu, @paramDONVIQUYDOI, @paramMaBravo, Getdate());";
            AddParameters(command, id, capphoi, maMac);
        }

        /// <summary>
        /// Thêm các tham số vào DbCommand
        /// </summary>
        /// <param name="command"></param>
        /// <param name="macBetongId"></param>
        /// <param name="capphoi"></param>
        private void AddParameters(DbCommand command, object macBetongId, DanhMucMacRequestModel capphoi, string maMac)
        {
            command.Parameters.Clear();
            command.Parameters.Add(CreateParameter(command, "@paramMACBETONGID", macBetongId));
            command.Parameters.Add(CreateParameter(command, "@paramMa", maMac));
            command.Parameters.Add(CreateParameter(command, "@paramTenMacBeTong", capphoi.TenMac));
            command.Parameters.Add(CreateParameter(command, "@paramCUONGDO", capphoi.CuongDo ?? (object)DBNull.Value));
            command.Parameters.Add(CreateParameter(command, "@paramCOTLIEUMAX", capphoi.CotLieuMax));
            command.Parameters.Add(CreateParameter(command, "@paramDOSUT", capphoi.DoSut ?? (object)DBNull.Value));
            command.Parameters.Add(CreateParameter(command, "@paramGhiChu", capphoi.GhiChu ?? (object)DBNull.Value));
            command.Parameters.Add(CreateParameter(command, "@paramDONVIQUYDOI", capphoi.DonViTinh ?? (object)DBNull.Value));
            command.Parameters.Add(CreateParameter(command, "@paramMaBravo", capphoi.MaMac));
        }

        /// <summary>
        /// Tạo Parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private DbParameter CreateParameter(DbCommand command, string paramName, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// Helper method to get function roles for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="db"></param>
        /// <param name="isRoleGroup"></param>
        /// <returns></returns>
        private List<MenuDTO> GetUserFunctionRoles(int userId, CNTTVNWebContext db, bool? isRoleGroup)
        {
            var listFunctionRole = new List<MenuDTO>();

            if ((bool)!isRoleGroup)
            {
                var userFunctionRoles = db.FunctionRole
                                          .Where(fr => fr.TargetId == userId && fr.Type == (int)Const.TypeFunction.FUNCTION_USER && fr.Status == (int)Const.Status.NORMAL)
                                          .OrderBy(fr => fr.Function.Location)
                                          .ToList();

                foreach (var itemFR in userFunctionRoles)
                {
                    AddFunctionRoleToMenu(itemFR, listFunctionRole);
                }
            }
            else
            {
                var userRoles = db.UserRole.Where(ur => ur.UserId == userId && ur.Status == (int)Const.Status.NORMAL).ToList();

                foreach (var item in userRoles)
                {
                    var roleFunctionRoles = db.FunctionRole
                                              .Where(fr => fr.TargetId == item.RoleId && fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE && fr.Status == (int)Const.Status.NORMAL)
                                              .OrderBy(fr => fr.Function.Location)
                                              .ToList();

                    foreach (var itemFR in roleFunctionRoles)
                    {
                        AddFunctionRoleToMenu(itemFR, listFunctionRole);
                    }
                }
            }

            return listFunctionRole;
        }

        /// <summary>
        /// Helper method to add function role to the menu list
        /// </summary>
        /// <param name="itemFR"></param>
        /// <param name="listFunctionRole"></param>
        private void AddFunctionRoleToMenu(FunctionRole itemFR, List<MenuDTO> listFunctionRole)
        {
            // Kiểm tra null cho itemFR.Function trước khi truy cập thuộc tính
            if (itemFR.Function == null)
            {
                return; // Nếu Function là null, không làm gì và thoát khỏi hàm
            }

            var existingMenu = listFunctionRole.FirstOrDefault(e => e.MenuId == itemFR.FunctionId);

            if (existingMenu != null)
            {
                string updatedKey = existingMenu.ActiveKey != itemFR.ActiveKey ? plusActiveKey(existingMenu.ActiveKey, itemFR.ActiveKey) : existingMenu.ActiveKey;
                existingMenu.ActiveKey = updatedKey;
            }
            else
            {
                var function = itemFR.Function; // Đảm bảo function không null tại đây
                MenuDTO menu = new MenuDTO
                {
                    MenuId = itemFR.FunctionId,
                    Code = function.Code,
                    Name = function.Name,
                    Url = function.Url,
                    Icon = function.Icon,
                    MenuParent = (int)function.FunctionParentId,
                    ActiveKey = itemFR.ActiveKey
                };
                listFunctionRole.Add(menu);
            }
        }


        /// <summary>
        /// Helper method to generate access key from function roles
        /// </summary>
        /// <param name="listFunctionRole"></param>
        /// <returns></returns>
        private string GenerateAccessKey(List<MenuDTO> listFunctionRole)
        {
            string accessKey = string.Join("-", listFunctionRole.Where(fr => fr.ActiveKey != "000000000")
                                                                .Select(fr => $"{fr.Code}:{fr.ActiveKey}"));

            return accessKey;
        }

        /// <summary>
        /// Helper method to generate JWT token
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateJwtToken(string fullName, string accessKey)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim("AccessKey", accessKey)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));

            return new JwtSecurityToken(
                _configuration["AppSettings:JwtIssuer"],
                _configuration["AppSettings:JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
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
