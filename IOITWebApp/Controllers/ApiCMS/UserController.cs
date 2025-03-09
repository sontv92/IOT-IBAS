using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("user", "user");
        private static string functionCode = "QLND";
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginModel loginModel)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (loginModel != null)
                {
                    string username = loginModel.email;

                    using (var db = new CNTTVNWebContext())
                    {
                        var userRole = new List<UserRole>();
                        var user = db.User.Where(e => e.UserName == username && e.Status != (int)Const.Status.DELETED).ToList();
                        //if (user.Count > 0)
                        //{
                            string password = user.FirstOrDefault().KeyLock.Trim() + user.FirstOrDefault().RegEmail.Trim() + user.FirstOrDefault().UserId + loginModel.password.Trim();
                            password = Utils.GetMD5Hash(password);
                            var userLogin1 = from person in db.User
                                             where person.UserName == username && person.Password == password && person.Status != (int)Const.Status.DELETED
                                             select new UserLogin()
                                             {
                                                 userId = person.UserId,
                                                 userName = person.UserName,
                                                 email = person.Email,
                                                 fullName = person.FullName,
                                                 avata = person.Avata,
                                                 address = person.Address,
                                                 companyId = person.CompanyId,
                                                 password = person.Password,
                                                 phone = person.Phone,
                                                 roleMax = person.RoleMax,
                                                 roleLevel = person.RoleLevel,
                                                 isRoleGroup = person.IsRoleGroup != null ? (bool)person.IsRoleGroup : true,
                                                 status = person.Status,
                                                 BranchId = person.BranchId

                                             }
                                         ;
                            var userLogin = userLogin1.FirstOrDefault();
                            // lấy  logo
                            var company = db.Company.Where(e => e.CompanyId == userLogin.companyId).FirstOrDefault();
                            if(company !=null)
                            {
                                userLogin.Logo = company.Logo != null ? company.Logo : null;
                            }
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
                                                Function function = db.Function.Where(e => e.FunctionId == itemFR.FunctionId).FirstOrDefault();
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
                                userLogin.listMenus = CreateMenu(listFunctionRole, 0);

                                //
                                //check nếu có 1 ngôn ngữ thì lấy ngôn ngữ mạc định là tiếng việt
                                int languageId = 0;
                                var language = db.Language.Where(e => e.CompanyId == userLogin.companyId).ToList();
                                if (language.Count == 1)
                                {
                                    languageId = language.FirstOrDefault().LanguageId;
                                }
                                else if (language.Count > 1)
                                {
                                    languageId = language.Where(e => e.IsMain == true).FirstOrDefault().LanguageId;
                                }
                                userLogin.languageId = languageId;
                                //
                                //check nếu có 1 website thì lấy website mạc định là website chính
                                int websiteId = 0;
                                string logoWebsite = "";
                                var websites = db.Website.Where(e => e.CompanyId == userLogin.companyId).ToList();
                                if (websites.Count == 1)
                                {
                                    websiteId = websites.FirstOrDefault().WebsiteId;
                                    logoWebsite = websites.FirstOrDefault().LogoHeader;
                                }
                                else if (websites.Count > 1)
                                {
                                    websiteId = websites.Where(e => e.WebsiteParentId == 0).FirstOrDefault().WebsiteId;
                                    logoWebsite = websites.Where(e => e.WebsiteParentId == 0).FirstOrDefault().LogoHeader;
                                }
                                userLogin.websiteId = websiteId;
                                userLogin.logoWebsite = logoWebsite;

                                var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Email, userLogin.email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.NameIdentifier, userLogin.userId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.fullName),
                                        new Claim("UserId", userLogin.userId != null ? userLogin.userId.ToString() : ""),
                                        new Claim("CompanyId", userLogin.companyId != null ? userLogin.companyId.ToString() : ""),
                                        new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
                                        new Claim("RoleLevel", userLogin.roleLevel != null ? userLogin.roleLevel.ToString() : ""),
                                        new Claim("AccessKey", access_key != null ? access_key : ""),
                                        new Claim("LanguageId", access_key != null ? languageId.ToString() : ""),
                                        new Claim("WebsiteId", access_key != null ? websiteId.ToString() : ""),
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
                                var roles = db.Role.Where(e => userRole.Select(x => x.RoleId).Contains(e.RoleId)).ToList();
                                userLogin.listRoles = roles;
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
                        //}
                        //else
                        //{
                        //    def.meta = new Meta(404, "Not found");
                        //    return Ok(def);
                        //}
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

        //plus role
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

        //create menu
        private List<MenuDTO> CreateMenu(List<MenuDTO> list, int k)
        {
            var listMenu = list.Where(e => e.MenuParent == k).ToList();
            if (listMenu.Count > 0)
            {
                List<MenuDTO> menus = new List<MenuDTO>();
                foreach (var item in listMenu)
                {
                    char[] str = item.ActiveKey.ToCharArray();
                    if (int.Parse(str[8].ToString()) == 1)
                    {
                        MenuDTO menu = new MenuDTO();
                        menu.MenuId = item.MenuId;
                        menu.Code = item.Code;
                        menu.Name = item.Name;
                        menu.Url = item.Url;
                        menu.Icon = item.Icon;
                        menu.MenuParent = item.MenuParent;
                        menu.ActiveKey = item.ActiveKey;
                        menu.listMenus = CreateMenu(list, item.MenuId);
                        menus.Add(menu);
                    }
                }
                return menus;
            }
            return new List<MenuDTO>();
        }

        [Authorize]
        [HttpPut("changePass/{id}")]
        public async Task<ActionResult> ChangePass(int id, [FromBody] UserChangePass data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (id != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    var users = db.User.Where(x=>x.UserId == id && x.Status != (int)Const.Status.DELETED).ToList();
                    var user = users.FirstOrDefault();
                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    //check password old
                    string password = user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + data.PasswordOld.Trim();
                    password = Utils.GetMD5Hash(password);
                    if (user.Password.Trim() != password)
                    {
                        def.meta = new Meta(213, "Not Exist Password Old");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //update user
                        string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + data.PasswordNew.Trim();
                        passwordNew = Utils.GetMD5Hash(passwordNew);

                        user.Password = passwordNew;
                        user.UpdatedAt = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.UserId > 0)
                            {
                                transaction.Commit();
                                //create action
                                //Models.EF.Action action = new Models.EF.Action();
                                //action.ActionName = "Bạn đã đổi mật khẩu";
                                //action.ActionType = "UPDATE";
                                //action.TargetId = user.UserId;
                                //action.TargetType = "thành công";
                                //action.Logs = action.ActionName + " " + user.FullName;
                                //action.Time = 0;
                                //action.Type = (int)Const.TypeAction.WARNING;
                                //action.CreatedAt = DateTime.Now;
                                //action.UserPushId = id;
                                //action.UserId = id;
                                //action.Status = (int)Const.Status.NORMAL;
                                //db.Actions.Add(action);
                                //await db.SaveChangesAsync();

                                ////push action firebase
                                //Models.Data.Firebase.pushAction(action);

                                //push user firebase
                                //var tasks = new[]
                                //{
                                //    Task.Run(() => Models.Data.Firebase.updateUser(user))
                                //};
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserExists(id))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPut("adminChangePass/{id}")]
        public async Task<ActionResult> AdminChangePass(int id, UserChangePass data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (id != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    User user = await db.User.FindAsync(id);
                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //update user
                        string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + data.PasswordNew.Trim();
                        passwordNew = Utils.GetMD5Hash(passwordNew);

                        user.Password = passwordNew;
                        user.UpdatedAt = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.UserId > 0)
                            {
                                transaction.Commit();
                                //create action
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Cấp lại mật khẩu";
                                action.ActionType = "UPDATE";
                                action.TargetId = user.UserId;
                                action.TargetType = "cho tài khoản của bạn";
                                action.Logs = action.ActionName + " cho tài khoản " + user.FullName;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserId = id;
                                action.UserPushId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                ////push action firebase
                                //Models.Data.Firebase.pushAction(action);

                                ////push user firebase
                                //var tasks = new[]
                                //{
                                //Task.Run(() => Models.Data.Firebase.updateUser(user))
                                //};
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserExists(id))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPut("api/user/lockUser/{id}/{k}")]
        public async Task<ActionResult> LockUser(int id, byte k)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    User user = await db.User.FindAsync(id);
                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                    if (user.UserId != id)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        user.UpdatedAt = DateTime.Now;
                        user.UserEditId = userId;
                        user.Status = k;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.UserId > 0)
                            {
                                transaction.Commit();

                                //create action
                                Models.EF.Action action = new Models.EF.Action();
                                action.ActionName = "Khóa tài khoản";
                                action.ActionType = "UPDATE";
                                action.TargetId = user.UserId;
                                action.TargetType = "của bạn";
                                action.Logs = "Khóa tài khoản " + user.FullName;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserId = id;
                                action.UserPushId = userId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //    //push action firebase
                                //    Models.Data.Firebase.pushAction(action);

                                //    //push user firebase
                                //    var tasks = new[]
                                //    {
                                //    Task.Run(() => Models.Data.Firebase.updateUser(user))
                                //};
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserExists(id))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpGet("infoUser/{id}")]
        public ActionResult InfoUser(int id)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            try
            {
                if (id != userId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {

                    IQueryable<User> user = db.User.Where(e => e.UserId == id && e.Status != (int)Const.Status.DELETED);

                    if (user == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    def.data = user.Select(c => new
                    {
                        c.UserId,
                        c.Code,
                        c.FullName,
                        c.UserName,
                        c.Avata,
                        c.Address,
                        c.Email,
                        c.Phone,
                        c.CreatedAt,
                        c.Status,
                        c.UnitId,
                        c.DepartmentId,
                        c.PositionId,
                        c.RoleMax,
                        c.RoleLevel,
                        c.IsRoleGroup,
                        //unit = db.Units.Where(e => e.UnitId == c.UnitId && e.Status != (int)Const.Status.DELETED).Select(e => new
                        //{
                        //    e.UnitId,
                        //    e.Name
                        //}).FirstOrDefault(),
                        department = db.Department.Where(e => e.DepartmentId == c.DepartmentId && e.Status != (int)Const.Status.DELETED).Select(e => new
                        {
                            e.DepartmentId,
                            e.Name
                        }).FirstOrDefault(),
                        position = db.Position.Where(e => e.PositionId == c.PositionId && e.Status != (int)Const.Status.DELETED).Select(e => new
                        {
                            e.PositionId,
                            e.Name
                        }).FirstOrDefault(),
                    }).FirstOrDefault();
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [Authorize]
        [HttpPut("changeInfoUser")]
        public async Task<ActionResult> changeInfoUser(UserInfo data)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.UserId != userId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    User user = await db.User.FindAsync(data.UserId);
                    if (user == null)
                    {
                        def.meta = new Meta(404, "Not found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        user.FullName = data.FullName;
                        user.Email = data.Email;
                        user.Code = data.Code;
                        user.Avata = data.Avata != null && data.Avata != "" ? data.Avata : user.Avata;
                        user.Address = data.Address;
                        user.Phone = data.Phone;
                        user.UpdatedAt = DateTime.Now;
                        user.UserEditId = userId;
                        db.Entry(user).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();
                            if (user.UserId > 0)
                            {
                                transaction.Commit();
                            }
                            else
                                transaction.Rollback();
                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserExists(user.UserId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool UserExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.User.Count(e => e.UserId == id) > 0;
            }
        }

    }
}