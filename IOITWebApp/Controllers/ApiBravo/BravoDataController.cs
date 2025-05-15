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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NPOI.HSSF.Util.HSSFColor;

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
                            var expires = DateTime.Now.AddSeconds(Convert.ToDouble(_configuration["AppSettings:JwtExpireBravoSecond"]));

                            var token = new JwtSecurityToken(
                                _configuration["AppSettings:JwtIssuer"],
                                _configuration["AppSettings:JwtIssuer"],
                                claims,
                                expires: expires,
                                signingCredentials: creds
                            );
                            var response = new LoginResponseModel()
                            {
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
                    var maVatTu = string.Empty;

                    // Check mã vật tư đã tồn tại chưa
                    var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[PHUGIA] WHERE MaLK = @MaVatTu";

                    var connection = context.Database.GetDbConnection();

                    var vattu = await connection.QueryFirstOrDefaultAsync<PhuGiaDTO>(qrCheck, new { MaVatTu = pg.MaVatTu });

                    if (vattu != null)
                    {
                        maVatTu = vattu.Ma;
                        await UpdateVatTu(context, branch.Dataname, vattu.ID, pg);
                        res.meta = new Meta(200, "Cập nhật thành công");
                        res.data = new ResponseDetail() { MaThamChieu = maVatTu };
                    }
                    else
                    {
                        maVatTu = CommonLib.GetSo("PHUGIA", "Ma", "VL1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                        await InsertVatTu(context, branch.Dataname, id, maVatTu, pg);
                        res.meta = new Meta(200, "Thêm mới thành công");
                        res.data = new ResponseDetail() { MaThamChieu = maVatTu };
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
                    var maMac = string.Empty;
                    // Kiểm tra mã mác bê tông đã tồn tại
                    var qrCheck = $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[MACBETONG] WHERE [MaLK] = '{capphoi.MaMac}'";
                    var macBT = DapperHepper.Query<CapPhoiDTO>(LocalSettings.ConnectString, qrCheck);

                    if (macBT != null && macBT.Any())
                    {
                        maMac = macBT.FirstOrDefault().Ma;
                        // Cập nhật mác bê tông nếu đã tồn tại
                        UpdateMacBetong(command, branch, macBT.FirstOrDefault(), capphoi);
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            res.meta = new Meta(200, "Cập nhật thành công");
                            res.data = new ResponseDetail() { MaThamChieu = maMac };
                        }
                    }
                    else
                    {
                        maMac = CommonLib.GetSo("MACBETONG", "Ma", "MAC1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);
                        // Thêm mới mác bê tông
                        InsertMacBetong(command, branch, id, capphoi, maMac);
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            result.Read();
                            res.meta = new Meta(200, "Thêm mới thành công");
                            res.data = new ResponseDetail() { MaThamChieu = maMac };
                        }
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
            try
            {
                const string functionCode = "dmmacbetong";

                if (capphoi == null)
                    return Ok(new APIResponseData { meta = new Meta(400, "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!") });

                var identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                    return UnauthorizedResponse();

                string accessKey = identity.Claims.FirstOrDefault(c => c.Type == "AccessKey")?.Value;
                if (!CheckRole.CheckRoleByCode(accessKey, functionCode, (int)Const.Action.CREATE))
                    return UnauthorizedResponse();

                using (var context = new CNTTVNWebContext())
                {

                    if (capphoi.BranchId == 0)
                        return BadRequestResponse("Mã trạm không hợp lệ!");

                    var branch = await context.Branch.FindAsync(capphoi.BranchId);
                    if (branch == null)
                        return BadRequestResponse("Không tìm thấy trạm tương ứng!");

                    string databaseName = branch.Dataname;

                    if (capphoi.Items == null || !capphoi.Items.Any() || capphoi.Items.Count > 20)
                        return Ok(new APIResponseData { meta = new Meta(400, "Danh sách vật liệu không hợp lệ!") });

                    string maMac = capphoi.Items.FirstOrDefault()?.MaMac;
                    var macBTs = await DapperHepper.QueryAsync<MacBeTongDTO>(
                        LocalSettings.ConnectString,
                        $"SELECT TOP 1 * FROM {databaseName}.[dbo].[MACBETONG] WHERE [MaLK] = @MaMac",
                        new { MaMac = maMac }
                    );

                    if (macBTs == null || !macBTs.Any())
                        return Ok(new APIResponseData { meta = new Meta(400, $"Mã mác {maMac} không tồn tại!") });

                    var macBT = macBTs.First();
                    var allMaCuaVL = Enumerable.Range(1, 20).ToList();
                    var existingMaCuaVL = capphoi.Items.Select(x => x.MaCuaVL).Distinct().ToList();
                    var missingMaCuaVL = allMaCuaVL.Except(existingMaCuaVL);

                    foreach (var maCuaVL in missingMaCuaVL)
                    {
                        capphoi.Items.Add(new DanhMucCapPhoiBeTongItemRequestModel
                        {
                            MaMac = macBT.Ma,
                            MaCuaVL = maCuaVL,
                            SoLuong = 0,
                            MaVatLieu = "",
                            TenVatLieu = "",
                            Ma = capphoi.Items.FirstOrDefault()?.Ma,
                            DonViQuyDoi = "",
                            HeSoQuyDoi = 0
                        });
                    }

                    string maDinhMuc = "";
                    var soluongVLs = (await DapperHepper.QueryAsync<SoLuongVLDTO>(
                        LocalSettings.ConnectString,
                        $"SELECT * FROM {databaseName}.[dbo].[SOLUONGVL] WHERE [MACBETONGID] = @macBeTongID",
                        new { macBeTongID = macBT.ID }
                    ))?.ToList();

                    if (soluongVLs != null && soluongVLs.Any())
                        maDinhMuc = soluongVLs.First().Ma;
                    else
                        maDinhMuc = CommonLib.GetSo("SOLUONGVL", "Ma", "SL1_" + DateTime.Now.ToString("yyMMdd") + "-", branch.Dataname);

                    foreach (var item in capphoi.Items)
                    {
                        if (item.MaCuaVL == 0)
                        {
                            log.Error("Error: Mã cửa vật liệu không hợp lệ!");
                            return Ok(new APIResponseData { meta = new Meta(400, "Mã cửa vật liệu không hợp lệ!") });
                        }
                        if (!string.IsNullOrEmpty(item.MaVatLieu))
                        {
                            var checkPG = await DapperHepper.QueryAsync<PhuGiaDTO>(
                               LocalSettings.ConnectString,
                               $"SELECT TOP(1) * FROM [{branch.Dataname}].[dbo].[PHUGIA] WHERE MaLK = @MaVatTu",
                               new { MaVatTu = item.MaVatLieu }
                           );

                            if (checkPG == null || !checkPG.Any())
                                return Ok(new APIResponseData { meta = new Meta(400, $"Mã vật tư {item.MaVatLieu} không tồn tại!") });
                        }

                        var existing = soluongVLs?.FirstOrDefault(x => x.MACUAVL == item.MaCuaVL);

                        var sql = existing != null
                            ? $"UPDATE [{databaseName}].[dbo].[SOLUONGVL] SET [SOLUONG] = @SoLuong, [MAVATLIEU] = @MaVL, [TENVATLIEU] = @TenVL, [MaLK] = @MaBravo, [DonViQuyDoi] = @DonViQuyDoi, [HeSoQuyDoi] = @HeSoQuyDoi, [LASTUPDATED] = GETDATE() WHERE [MACBETONGID] = @MACBETONGID AND [MACUAVL] = @MaCuaVL"
                            : $"INSERT INTO [{databaseName}].[dbo].[SOLUONGVL] ([MACBETONGID], [MACUAVL], [Ma], [SOLUONG], [ID], [MAMAC], [MAVATLIEU], [TENVATLIEU], [MaLK], [DonViQuyDoi], [HeSoQuyDoi], [LASTUPDATED]) VALUES (@MACBETONGID, @MaCuaVL, @Ma, @SoLuong, @ID, @MaMac, @MaVL, @TenVL, @MaBravo, @DonViQuyDoi, @HeSoQuyDoi, GETDATE())";

                        var parameters = new
                        {
                            MACBETONGID = macBT.ID,
                            MaCuaVL = item.MaCuaVL,
                            Ma = maDinhMuc,
                            SoLuong = item.SoLuong,
                            ID = CustomGuid.NewSequentialId(),
                            MaMac = macBT.Ma,
                            MaVL = item.MaVatLieu,
                            TenVL = item.TenVatLieu,
                            MaBravo = item.Ma,
                            DonViQuyDoi = item.DonViQuyDoi,
                            HeSoQuyDoi = item.HeSoQuyDoi
                        };

                        int result = DapperHepper.ExecuteNew(LocalSettings.ConnectString, sql, parameters);

                        if (result == -1)
                        {
                            log.Error($"Error: {(existing != null ? "Cập nhật" : "Thêm")} không thành công. Mã mác: {macBT.Ma}, MACUAVL: {item.MaCuaVL}");
                            return Ok(new APIResponseData { meta = new Meta(400, $"{(existing != null ? "Cập nhật" : "Thêm")} không thành công!") });
                        }
                    }

                    return Ok(new APIResponseData
                    {
                        meta = new Meta(200, "Cập nhật thành công!"),
                        data = new ResponseDetail { MaThamChieu = maDinhMuc }
                    });
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
                                    ([ID], [Ma], [MaLK], [Ten], [NhaCungCap], [MaLoaiVL], [TenLoaiVL], [HeSoQuyDoi], [DonViQuyDoi], [LASTUPDATED]) 
                                    VALUES (@ID, @Ma, @MaBravo, @TENPG, @NHACUNGCAP, @MALOAIVL, @TENLOAIVL, @HESOQUYDOI, @DONVIQUYDOI, Getdate())";

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
                                    SET [Ten] = @TENPG, [NhaCungCap] = @NHACUNGCAP, [MaLoaiVL] = @MALOAIVL, 
                                        [TenLoaiVL] = @TENLOAIVL, [HeSoQuyDoi] = @HESOQUYDOI, [DonViQuyDoi] = @DONVIQUYDOI, 
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
                                  "([ID], [Ma], [TENMACBETONG], [CUONGDO], [COTLIEUMAX], [DOSUT], [ISSYNC]," +
                                  "[GhiChu], [DONVIQUYDOI], [MaLK], [LASTUPDATED]) " +
                                  "VALUES (@paramMACBETONGID, @paramMa, @paramTenMacBeTong, @paramCUONGDO, " +
                                  "@paramCOTLIEUMAX, @paramDOSUT, 1, @paramGhiChu, @paramDONVIQUYDOI, @paramMaBravo, Getdate());";
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
