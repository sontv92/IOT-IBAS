using DocumentFormat.OpenXml.Drawing.Charts;
using IOITWebApp;
using IOITWebApp.Helper;
using IOITWebApp.Models;
using IOITWebApp.Models.Bravo;
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
                                             email = person.Email,
                                             fullName = person.FullName,
                                             password = person.Password,
                                             phone = person.Phone,
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

                            userLogin.access_key = access_key;
                            var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Email, userLogin.email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.NameIdentifier, userLogin.userId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.fullName),
                                        new Claim("UserId", userLogin.userId != null ? userLogin.userId.ToString() : ""),
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
                            userLogin.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                            def.data = userLogin;
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
        [HttpGet("dmvattucapphoi")]
        public IActionResult GetDmVatTuCapPhoi(int? branchId)
        {
            string functionCode = "dmvattucapphoi";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            List<VatuCapPhoiResponseModel> data = new List<VatuCapPhoiResponseModel>();
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            // Lấy danh sách vật liệu
            var qrVL = $"SELECT * FROM [{branch.FirstOrDefault().Dataname}].[dbo].[CUAVL]";
            var lstVL = DapperHepper.Query<CuaVL>(LocalSettings.ConnectString, qrVL);

            var qrLoaiVL = $"SELECT * FROM [{branch.FirstOrDefault().Dataname}].[dbo].[LOAIVL]";
            var lstLoaiVL = DapperHepper.Query<LoaiVL>(LocalSettings.ConnectString, qrLoaiVL);
            if (lstVL != null && lstVL.Any())
            {
                foreach (var vt in lstVL)
                {
                    data.Add(new VatuCapPhoiResponseModel()
                    {
                        MaVatTu = vt.MACUAVL,
                        TenVatu = vt.TENCUAVL,
                        NhomVatTu = lstLoaiVL?.FirstOrDefault(x => x.ID == vt.MALOAIVL).TENLOAIVL
                    });
                }
            }
            res.data = data;
            res.meta = new Meta(200, "Success");
            return Ok(res);
        }
        [Authorize]
        [HttpGet("dmduan")]
        public IActionResult GetDmDuAn(int? branchId)
        {
            string functionCode = "dmduan";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            // Lấy danh sách dự án
            var qrDa = $"SELECT Ma, TENDUAN FROM [{branch.FirstOrDefault().Dataname}].[dbo].[DUAN]";
            var lstVL = DapperHepper.Query<DuAnResponseModel>(LocalSettings.ConnectString, qrDa);
            res.data = lstVL;
            res.meta = new Meta(200, "Success");
            return Ok(res);
        }
        [Authorize]
        [HttpGet("dmkhachhang")]
        public IActionResult GetDmKhachHang(int? branchId)
        {
            string functionCode = "dmkhachhang";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            // Lấy danh sách dự án
            var qrDa = $"SELECT Ma, TENKHACHHANG FROM [{branch.FirstOrDefault().Dataname}].[dbo].[KHACHHANG]";
            var lstVL = DapperHepper.Query<KhachHangResponseModel>(LocalSettings.ConnectString, qrDa);
            res.data = lstVL;
            res.meta = new Meta(200, "Success");
            return Ok(res);
        }
        [Authorize]
        [HttpGet("dmsanpham")]
        public IActionResult GetDmSanPham(int? branchId)
        {
            string functionCode = "dmsanpham";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            List<SanPhamResponseModel> data = new List<SanPhamResponseModel>();
            var qrMacBetong = $"select Ma, TENMACBETONG FROM [{branch.FirstOrDefault().Dataname}].[dbo].MACBETONG";
            var lstSp = DapperHepper.Query<MacBeTongDTO>(LocalSettings.ConnectString, qrMacBetong);
            foreach (var item in lstSp)
            {
                data.Add(new SanPhamResponseModel()
                {
                    Ma = item.Ma,
                    Ten = item.TENMACBETONG
                });
            }
            res.data = data;
            res.meta = new Meta(200, "Success");
            return Ok(res);
        }
        [Authorize]
        [HttpGet("dinhmuccapphoibt")]
        public IActionResult GetDinhMucCapPhoiBeTong(int? branchId)
        {
            string functionCode = "dinhmuccapphoibt";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            List<VatuCapPhoiResponseModel> data = new List<VatuCapPhoiResponseModel>();
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<DinhMucCapPhoiBeTongResponseModel> capphoi = new List<DinhMucCapPhoiBeTongResponseModel>();
                var count = 0;

                command.CommandText = "SELECT COUNT(*) as COUNT FROM [" + branch.FirstOrDefault().Dataname + "].[dbo].CUAVL";
                context.Database.OpenConnection();
                var resultCount = command.ExecuteReader();
                resultCount.Read();
                count = resultCount.GetInt32(0);
                context.Database.CloseConnection();

                command.CommandText = string.Empty;
                command.CommandText += "IF(OBJECT_ID('tempdb..#TempTable') IS NOT NULL) BEGIN DROP TABLE  [" + branch.FirstOrDefault().Dataname + "].[dbo].#TempTable END; ";
                command.CommandText += "SELECT * INTO [" + branch.FirstOrDefault().Dataname + "].[dbo].#TempTable FROM ( SELECT [MACBETONGID], [MACUAVL], [SOLUONG] FROM [" + branch.FirstOrDefault().Dataname + "].[dbo].SOLUONGVL ) SOLUONGVLResults ";
                command.CommandText += "PIVOT (SUM([SOLUONG]) FOR [MACUAVL] IN (";
                for (int k = 1; k <= count; k++)
                {
                    if (k < count)
                        command.CommandText += "[" + k + "],";
                    else
                        command.CommandText += "[" + k + "]))";
                }
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
                command.CommandText += "from  [" + branch.FirstOrDefault().Dataname + "].[dbo].#TempTable tmp INNER JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].MACBETONG mbt ON tmp.MACBETONGID = mbt.ID";
                command.CommandText += " ORDER BY Ma DESC";
                command.CommandText += " DROP TABLE #TempTable; ";


                List<VatLieuDTO> TenCuaVL = new List<VatLieuDTO>();
                using (var command2 = context.Database.GetDbConnection().CreateCommand())
                {
                    for (int i = 1; i <= count; i++)
                    {
                        context.Database.OpenConnection();
                        command2.CommandText = "SELECT cuavl.TENCUAVL, cuavl.MACUAVL, cuavl.TRANGTHAI FROM [" + branch.FirstOrDefault().Dataname + "].[dbo].CUAVL as cuavl WHERE MACUAVL = " + i;
                        var resultCount1 = command2.ExecuteReader();
                        resultCount1.Read();
                        TenCuaVL.Add(new VatLieuDTO()
                        {
                            TENCUAVL = resultCount1.GetString(0),
                            MACUAVL = resultCount1.GetInt32(1),
                            TRANGTHAI = resultCount1.GetBoolean(2)
                        });
                        context.Database.CloseConnection();
                    }
                }
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    result.Read();
                    result.NextResult();
                    while (result.Read())
                    {
                        DinhMucCapPhoiBeTongResponseModel item = new DinhMucCapPhoiBeTongResponseModel();
                        item.MaCapPhoi = result["Ma"] == null ? string.Empty : (string)result["Ma"];
                        item.TenCapPhoi = (result["TENMACBETONG"] is DBNull) ? String.Empty : (string)result["TENMACBETONG"];

                        item.DoSut = (result["DOSUT"] is DBNull) ? String.Empty : (string)result["DOSUT"];

                        item.Details = new List<Detail>();
                        for (int i = 1; i <= count; i++)
                        {
                            var itemCuaVL = i.ToString();
                            Detail vatLieu = new Detail()
                            {
                                KhoiLuong = (decimal)((result[itemCuaVL] is DBNull) ? 0 : Math.Round((Double)result[itemCuaVL], 2)),
                                MaVatTu = TenCuaVL[i - 1].MACUAVL,
                                TenVatTu = TenCuaVL[i - 1].TENCUAVL
                            };
                            item.Details.Add(vatLieu);
                        }

                        capphoi.Add(item);
                    }
                    res.data = capphoi;
                }
                res.meta = new Meta(200, "Success");
                return Ok(res);
            }
        }
        [Authorize]
        [HttpGet("lenhsanxuat")]
        public IActionResult GetLenhSanXuat(int? branchId)
        {
            string functionCode = "lenhsanxuat";
            var res = new APIResponseData();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                res.meta = new Meta(401, "No permission");
                return Ok(res);
            }

            if (branchId == null)
            {
                res.meta = new Meta(400, "Mã trạm là bắt buộc");
                return Ok(res);
            }
            List<LenhSanXuatResponseModel> data = new List<LenhSanXuatResponseModel>();
            var qrBranch = $"SELECT * FROM Branch WHERE BranchId = {branchId}";

            var branch = DapperHepper.Query<Branch>(LocalSettings.ConnectString, qrBranch);
            if (branch == null)
            {
                res.meta = new Meta(404, "Không tìm thấy trạm tương ứng");
                return Ok(res);
            }
            try
            {
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    command.CommandText = " SELECT KHACHHANGID,NHANVIENID,DUANID,MACBETONGID, ID, Ma,MAKHACHHANG, TENKHACHHANG,MADUAN,TENDUAN,TONGSOPHIEU,METKHOIDATHANG,METKHOITICHLUY,NGAYDATHANG,TENMACBETONG, MAMACBETONG,NGAYDATHANGTITLE,TENNV,LASTUPDATED INTO #Result FROM ";
                    command.CommandText += "(";
                    command.CommandText += "SELECT sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, sa.MAKHACHHANG as MAKHACHHANG, kh.TENKHACHHANG as TENKHACHHANG,sa.MADUAN as MADUAN, da.TENDUAN as TENDUAN ,sa.ID,sa.Ma, CASE WHEN sa.TONGSOPHIEU > ISNULL(sa.TONGSOPHIEU_TEMP, 0) THEN sa.TONGSOPHIEU ELSE ISNULL(sa.TONGSOPHIEU_TEMP, 0) END AS TONGSOPHIEU, sa.[METKHOIDATHANG], ROUND((ISNULL(sa.METKHOITICHLUY, 0) + ISNULL(sa.METKHOITICHLUY_TEMP, 0) + SUM(ISNULL(te.METKHOITICHLUY_BUTRU,0))),2) AS METKHOITICHLUY, sa.[NGAYDATHANG],mac.TENMACBETONG as TENMACBETONG, mac.Ma as MAMACBETONG, FORMAT(sa.[NGAYDATHANG], 'dd/MM/yyyy HH:mm:ss') as NGAYDATHANGTITLE, nv1.TENNV as TENNV, sa.LASTUPDATED FROM [" + branch.FirstOrDefault().Dataname + "].[dbo].[DATHANG] sa LEFT JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].[NHANVIEN] nv1 ON nv1.ID = sa.NHANVIENID LEFT JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].[KHACHHANG] kh ON kh.ID = sa.KHACHHANGID LEFT JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].[DUAN] da ON da.ID = sa.DUANID  LEFT JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].[MACBETONG] mac ON mac.ID = sa.MACBETONGID LEFT JOIN [" + branch.FirstOrDefault().Dataname + "].[dbo].[DATHANG_TEMP] te ON sa.ID = te.DATHANGID ";
                    command.CommandText += "GROUP BY sa.KHACHHANGID,sa.NHANVIENID,sa.DUANID,sa.MACBETONGID, kh.TENKHACHHANG, \r\n da.TENDUAN,sa.ID,sa.Ma,sa.METKHOITICHLUY,sa.METKHOITICHLUY_TEMP,sa.TONGSOPHIEU,\r\n sa.TONGSOPHIEU_TEMP,sa.[METKHOIDATHANG], sa.[NGAYDATHANG],mac.TENMACBETONG,nv1.TENNV, sa.MAKHACHHANG, sa.MADUAN,\r\n sa.LASTUPDATED, mac.Ma";
                    command.CommandText += ") nv";
                    command.CommandText += " WHERE Ma NOT LIKE N'%DH2%'";
                    command.CommandText += " SELECT COUNT(*) AS COUNTS FROM #Result ;";
                    command.CommandText += " SELECT * FROM #Result ";
                    command.CommandText += " ORDER BY NGAYDATHANG DESC";

                    command.CommandText += " DROP TABLE #Result; ";
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        result.Read();
                        result.NextResult();
                        while (result.Read())
                        {

                            data.Add(new LenhSanXuatResponseModel()
                            {
                                KhoiLuong = Math.Round((Single)result["METKHOIDATHANG"], 1),
                                MaCapPhoi = (result["MAMACBETONG"] is DBNull) ? String.Empty : (string)result["MAMACBETONG"],
                                MaDA = (result["MADUAN"] is DBNull) ? String.Empty : (string)result["MADUAN"],
                                MaKH = (result["MAKHACHHANG"] is DBNull) ? String.Empty : (string)result["MAKHACHHANG"],
                                MaLenhSanXuat = (result["Ma"] is DBNull) ? String.Empty : (string)result["Ma"],
                                ThoiGianBatDau = (DateTime)result["NGAYDATHANG"],
                                MetKhoiDaTron = (double)result["METKHOITICHLUY"],
                            });
                        }

                        res.data = data;
                    }
                    res.meta = new Meta(200, "Success");
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.meta = new Meta(500, "Đã có lỗi xảy ra");
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
