using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("userRole", "userRole");
        private static string functionCode = "QLND";

        // GET: api/UserRole
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault());
            int roleLevel = int.Parse(identity.Claims.Where(c => c.Type == "RoleLevel").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            if (paging != null)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<User> data = db.User.Where(c => c.Status != (int)Const.Status.DELETED);
                    //if (roleMax != 1)
                    //{
                    //    paging.query = "RoleLevel > " + roleLevel;
                    //}
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                    {
                        def.data = data.Select(c => new
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
                            c.CompanyId,
                            c.BranchId,
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
                            listRole = db.UserRole.Where(e => e.UserId == c.UserId && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.RoleId,
                                RoleName = db.Role.Where(r => r.RoleId == e.RoleId).FirstOrDefault().Name,
                            }).ToList(),
                            listFunction = db.FunctionRole.Where(e => e.TargetId == c.UserId && e.Type == (int)Const.TypeFunction.FUNCTION_USER && e.Status != (int)Const.Status.DELETED).Select(e => new
                            {
                                e.FunctionId,
                                e.ActiveKey
                            }).ToList(),
                            //listUnit = db.UserProjects.Where(e => e.UserId == c.UserId && e.Type == (int)Const.TypeUserProject.USER_UNIT && e.Status != (int)Const.Status.DELETED).Select(e => new
                            //{
                            //    e.TargetId,
                            //}).ToList(),
                            //listProject = db.UserProjects.Where(e => e.UserId == c.UserId && e.Type == (int)Const.TypeUserProject.USER_PROJECT && e.Status != (int)Const.Status.DELETED).Select(e => new
                            //{
                            //    e.TargetId,
                            //}).ToList()
                        }).ToList();
                    }
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        [HttpGet("GetByCompany")]
        public IActionResult GetByCompany([FromQuery] FilteredPagination paging)
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
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Company> data = db.Company.Where(c => c.Status != (int)Const.Status.DELETED);
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("CompanyId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("CompanyId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                        def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }
        [HttpGet("GetByPageNotRole")]
        public IActionResult GetByPageNotRole([FromQuery] FilteredPagination paging)
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
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<User> data = from c in db.User.Where(e => e.Status == (int)Const.Status.NORMAL)
                                            where !db.UserRole.Any(m => m.UserId == c.UserId && m.Status != (int)Const.Status.DELETED)
                                            select c;
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                    }
                    else
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by);
                        }
                        else
                        {
                            data = data.OrderBy("CreatedAt desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                    {
                        def.data = data;
                    }

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/UserRole/5
        [HttpGet("{id}")]
        public IActionResult GetUserRole(int id)
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

            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    IQueryable<User> data = db.User.Where(c => c.UserId == id && c.Status != (int)Const.Status.DELETED);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    var dataS = data.Select(c => new
                    {
                        c.UserId,
                        EmployeeCode = c.Code,
                        c.FullName,
                        listRole = db.UserRole.Where(e => e.UserId == c.UserId && e.Status != (int)Const.Status.DELETED).Select(e => new
                        {
                            e.RoleId,
                            RoleName = db.Role.Where(r => r.RoleId == e.RoleId).FirstOrDefault().Name,
                        }).ToList(),

                    });

                    def.data = dataS.Where(e => e.listRole.Count() > 0).ToList();
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        //[HttpGet]
        //[Route("api/userRole/ListUserMonitor")]
        //public IHttpActionResult ListUserMonitor()
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "No permission");
        //        return Ok(def);
        //    }
        //    try
        //    {
        //        //IQueryable<User> data = db.Users.Where(c => c.UserId == id && c. && c.Status != (int)Const.Status.DELETED);
        //        IQueryable<User> data = from user in db.Users
        //                                join ur in db.UserRoles on user.UserId equals ur.UserId
        //                                where user.Status == (int)Const.Status.NORMAL && ur.Status != (int)Const.Status.DELETED
        //                                && ur.Role.Code == "MONITORING"
        //                                select user;
        //        if (data == null)
        //        {
        //            def.meta = new Meta(404, "Not Found");
        //            return Ok(def);
        //        }

        //        def.meta = new Meta(200, "Success");
        //        def.data = data.Select(c => new
        //        {
        //            c.UserId,
        //            c.FullName,
        //        });

        //        return Ok(def);
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Internal Server Error");
        //        return Ok(def);
        //    }
        //}

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRole(int id, [FromBody] UserRoleDT data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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
                    int countUser = db.User.Where(f => f.CompanyId == data.CompanyId && f.Status != (int)Const.Status.DELETED).Count();
                    int CountUserActive = db.Company.Where(c => c.CompanyId == data.CompanyId).FirstOrDefault().CountUser;
                    if ((countUser + 1) > CountUserActive)
                    {
                        def.meta = new Meta(21111, "Account limit exceeded!");
                        return Ok(def);
                    }
                    User current = await db.User.FindAsync(id);
                    if (current == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    User checkUserNameExist = db.User.Where(f => f.UserId != data.UserId && f.UserName == data.UserName && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkUserNameExist != null)
                    {
                        def.meta = new Meta(211, "UserName Exist!");
                        return Ok(def);
                    }

                    User checkEmailExist = db.User.Where(f => f.UserId != data.UserId && f.Email == data.Email && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmailExist != null)
                    {
                        def.meta = new Meta(2111, "Email Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //update user
                        current.FullName = data.FullName;
                        current.Code = data.Code;
                        current.Phone = data.Phone;
                        current.Email = data.Email;
                        current.Address = data.Address;
                        current.Avata = data.Avata;
                        current.UnitId = data.UnitId;
                        current.DepartmentId = data.DepartmentId;
                        current.PositionId = data.PositionId;
                        current.CompanyId = data.CompanyId;
                        current.BranchId = data.BranchId;
                        current.IsRoleGroup = data.IsRoleGroup != null ? data.IsRoleGroup : true;
                        current.UpdatedAt = DateTime.Now;
                        db.Entry(current).State = EntityState.Modified;

                        try
                        {
                            //role old
                            byte levelOld = (byte)current.RoleLevel;
                            // role
                            var checkRole = false;
                            byte level = 99;
                            int max = 9999;
                            //update list role
                            //add new
                            var role = db.Role.Find(data.Roleid);
                            if (role != null)
                            {
                                UserRole userRole = new UserRole();
                                userRole.RoleId = role.RoleId;
                                userRole.UserId = data.UserId;
                                userRole.Status = (int)Const.Status.NORMAL;
                                db.UserRole.Add(userRole);
                                //
                                if (role.LevelRole < level)
                                {
                                    level = (byte)role.LevelRole;
                                    max = role.RoleId;
                                }
                            }
                            //foreach (var item in data.listRole)
                            //{
                            //    var role = db.Role.Find(item.RoleId);
                            //    if (role != null)
                            //    {
                            //        var userRoleNew = db.UserRole.Where(e => e.UserId == data.UserId && e.RoleId == item.RoleId && e.Status != (int)Const.Status.DELETED).ToList();
                            //        if (userRoleNew.Count <= 0)
                            //        {
                            //            UserRole userRole = new UserRole();
                            //            userRole.RoleId = item.RoleId;
                            //            userRole.UserId = data.UserId;
                            //            userRole.Status = (int)Const.Status.NORMAL;
                            //            db.UserRole.Add(userRole);
                            //        }
                            //        //check role
                            //        if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                            //            checkRole = true;
                            //        //
                            //        if (role.LevelRole < level)
                            //        {
                            //            level = (byte)role.LevelRole;
                            //            max = role.RoleId;
                            //        }
                            //    }
                            //}
                            //delete old
                            var listUserRole = db.UserRole.Where(e => e.UserId == data.UserId && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item in listUserRole)
                            {
                                var listNew = data.listRole.Where(e => e.RoleId == item.RoleId).ToList();
                                if (listNew.Count <= 0)
                                {
                                    UserRole userRoleExit = await db.UserRole.FindAsync(item.UserRoleId);
                                    userRoleExit.Status = (int)Const.Status.DELETED;
                                    db.Entry(userRoleExit).State = EntityState.Modified;
                                }
                                //else
                                //{
                                //    //Check xem có phải quyền giám sát ko
                                //    var role = db.Role.Find(item.RoleId);
                                //    if (role != null)
                                //    {
                                //        //check role
                                //        if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                //            checkRole = true;
                                //    }
                                //}
                            }

                            //update quyền cao nhất và cấp cao nhất của user
                            current.RoleLevel = level;
                            current.RoleMax = max;
                            db.Entry(current).State = EntityState.Modified;

                            //update list function
                            foreach (var item in data.listFunction)
                            {
                                var functionNew = db.FunctionRole.Where(e => e.TargetId == data.UserId
                                && e.FunctionId == item.FunctionId
                                && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                                && e.Status != (int)Const.Status.DELETED).ToList();
                                //add new
                                if (functionNew.Count <= 0)
                                {
                                    FunctionRole functionRole = new FunctionRole();
                                    functionRole.TargetId = data.UserId;
                                    functionRole.FunctionId = item.FunctionId;
                                    functionRole.ActiveKey = item.ActiveKey;
                                    functionRole.Type = (int)Const.TypeFunction.FUNCTION_USER;
                                    functionRole.CreatedAt = DateTime.Now;
                                    functionRole.UpdatedAt = DateTime.Now;
                                    functionRole.UserId = data.UserCreateId;
                                    functionRole.Status = (int)Const.Status.NORMAL;
                                    db.FunctionRole.Add(functionRole);
                                }
                                else
                                {
                                    //update
                                    var functionRoleExit = functionNew.FirstOrDefault();
                                    functionRoleExit.ActiveKey = item.ActiveKey;
                                    functionRoleExit.UpdatedAt = DateTime.Now;
                                    functionRoleExit.UserId = data.UserCreateId;
                                    db.Entry(functionRoleExit).State = EntityState.Modified;
                                }
                            }

                            //update list user unit
                            //add new
                            //foreach (var item in data.listUnit)
                            //{
                            //    var unitNew = db.UserProjects.Where(e => e.UserId == data.UserId && e.TargetId == item.TargetId
                            //    && e.Type == (int)Const.TypeUserProject.USER_UNIT
                            //    && e.Status != (int)Const.Status.DELETED).ToList();
                            //    if (unitNew.Count <= 0)
                            //    {
                            //        UserProject userProject = new UserProject();
                            //        userProject.TargetId = item.TargetId;
                            //        userProject.UserId = data.UserId;
                            //        userProject.Type = (int)Const.TypeUserProject.USER_UNIT;
                            //        userProject.CreatedAt = DateTime.Now;
                            //        userProject.UpdatedAt = DateTime.Now;
                            //        userProject.UserCreateId = data.UserCreateId;
                            //        userProject.Status = (int)Const.Status.NORMAL;
                            //        db.UserProjects.Add(userProject);
                            //    }
                            //}
                            //delete old
                            //var listUserUnit = db.UserProjects.Where(e => e.UserId == data.UserId
                            //&& e.Type == (int)Const.TypeUserProject.USER_UNIT
                            //&& e.Status != (int)Const.Status.DELETED).ToList();
                            //foreach (var item in listUserUnit)
                            //{
                            //    var listNew = data.listUnit.Where(e => e.TargetId == item.TargetId).ToList();
                            //    if (listNew.Count <= 0)
                            //    {
                            //        UserProject userUnitExit = await db.UserProjects.FindAsync(item.UserProjectId);
                            //        userUnitExit.UpdatedAt = DateTime.Now;
                            //        userUnitExit.Status = (int)Const.Status.DELETED;
                            //        db.Entry(userUnitExit).State = EntityState.Modified;
                            //    }
                            //}

                            //check role hạ quyền hay tăng quyển với quản lý và giám sát
                            //var checkLevel = true; //=true là tăng quyền, =false là hạ quyền
                            //if (level != levelOld)
                            //{
                            //    if (level == (int)Const.RoleLevel.MONITORING)
                            //        checkLevel = false;
                            //}
                            //Nếu từ quản lý xuống giám sát thì xóa hết những hợp đồng mà ng đó có quyền giám sát
                            //if (!checkLevel)
                            //{
                            //    var monitoringUsers = db.MonitoringUsers.Where(e => e.UserMonitoringId == id
                            //    && e.Type == (int)Const.TypeMonitoringUser.MANAGER).ToList();
                            //    foreach (var item in monitoringUsers)
                            //    {
                            //        item.Status = (int)Const.Status.DELETED;
                            //        db.Entry(item).State = EntityState.Modified;
                            //    }
                            //}

                            //Nếu từ giám sát nên quản lý thì check xem họ quản lý những dự án nào thì cấp quyền giám sát các hợp đồng trong các dự án đó

                            //update list user project
                            //add new
                            //foreach (var item in data.listProject)
                            //{
                            //    var projectNew = db.UserProjects.Where(e => e.UserId == data.UserId && e.TargetId == item.TargetId
                            //    && e.Type == (int)Const.TypeUserProject.USER_PROJECT
                            //    && e.Status != (int)Const.Status.DELETED).ToList();
                            //    if (projectNew.Count <= 0)
                            //    {
                            //        UserProject userProject = new UserProject();
                            //        userProject.TargetId = item.TargetId;
                            //        userProject.UserId = data.UserId;
                            //        userProject.Type = (int)Const.TypeUserProject.USER_PROJECT;
                            //        userProject.CreatedAt = DateTime.Now;
                            //        userProject.UpdatedAt = DateTime.Now;
                            //        userProject.UserCreateId = data.UserCreateId;
                            //        userProject.Status = (int)Const.Status.NORMAL;
                            //        db.UserProjects.Add(userProject);

                            //        //add role monitoring
                            //        //Nếu user có quyền quán lý dự án x sẽ có quyền giám sát tất cả các hợp đồng trong dự án x (nâng quyền)
                            //        if (checkRole)
                            //        {
                            //            var contract = db.Contracts.Where(e => e.ProjectId == item.TargetId && e.Status != (int)Const.Status.DELETED
                            //            && (e.Type == (int)Const.TypeContract.CONTRACT || (e.Type == (int)Const.TypeContract.CONTRACT_CHILD && e.TypeContractChild == (int)Const.TypeChangeContract.CHANGE_CONTRACT_NEW))).ToList();
                            //            //Thêm mới
                            //            foreach (var itemC in contract)
                            //            {
                            //                var mu = db.MonitoringUsers.Where(e => e.ContractId == itemC.ContractId
                            //                && e.UserMonitoringId == data.UserId
                            //                && e.Type == (int)Const.TypeMonitoringUser.MANAGER
                            //                && e.Status != (int)Const.Status.DELETED).ToList();

                            //                if (mu.Count <= 0)
                            //                {
                            //                    MonitoringUser monitoringUser = new MonitoringUser();
                            //                    monitoringUser.ContractId = itemC.ContractId;
                            //                    monitoringUser.ProjectId = itemC.ProjectId;
                            //                    monitoringUser.DateStart = itemC.DateStart;
                            //                    monitoringUser.DateEnd = itemC.DateEnd;
                            //                    monitoringUser.UserMonitoringId = data.UserId;
                            //                    monitoringUser.Type = (int)Const.TypeMonitoringUser.MANAGER;
                            //                    monitoringUser.CreatedAt = DateTime.Now;
                            //                    monitoringUser.UserCreateId = data.UserCreateId;
                            //                    monitoringUser.Status = (int)Const.Status.NORMAL;
                            //                    db.MonitoringUsers.Add(monitoringUser);
                            //                }
                            //                else
                            //                {
                            //                    MonitoringUser monitoringUser = mu.FirstOrDefault();
                            //                    monitoringUser.DateStart = itemC.DateStart;
                            //                    monitoringUser.DateEnd = itemC.DateEnd;
                            //                    monitoringUser.Type = (int)Const.TypeMonitoringUser.MANAGER;
                            //                    monitoringUser.UserCreateId = data.UserCreateId;
                            //                    db.Entry(monitoringUser).State = EntityState.Modified;
                            //                }
                            //            }
                            //            //Xóa cũ
                            //            var listMU = db.MonitoringUsers.Where(e => e.UserMonitoringId == data.UserId && e.Type == (int)Const.TypeMonitoringUser.MANAGER).ToList();
                            //            foreach (var itemLMU in listMU)
                            //            {
                            //                var lmu = contract.Where(e => e.ContractId == itemLMU.ContractId).ToList();
                            //                if (lmu.Count <= 0)
                            //                {
                            //                    MonitoringUser monitoringUser = itemLMU;
                            //                    monitoringUser.UserCreateId = data.UserId;
                            //                    monitoringUser.Status = (int)Const.Status.DELETED;
                            //                    db.Entry(monitoringUser).State = EntityState.Modified;
                            //                }
                            //            }

                            //        }
                            //    }
                            //}
                            //delete old
                            //var listUserProject = db.UserProjects.Where(e => e.UserId == data.UserId
                            //&& e.Type == (int)Const.TypeUserProject.USER_PROJECT
                            //&& e.Status != (int)Const.Status.DELETED).ToList();
                            //foreach (var item in listUserProject)
                            //{
                            //    var listNew = data.listProject.Where(e => e.TargetId == item.TargetId).ToList();
                            //    if (listNew.Count <= 0)
                            //    {
                            //        UserProject userProjectExit = await db.UserProjects.FindAsync(item.UserProjectId);
                            //        userProjectExit.UpdatedAt = DateTime.Now;
                            //        userProjectExit.Status = (int)Const.Status.DELETED;
                            //        db.Entry(userProjectExit).State = EntityState.Modified;

                            //        //add role monitoring
                            //        //Nếu user ko có quyền quán lý dự án x sẽ ko có quyền giám sát tất cả các hợp đồng trong dự án x nữa
                            //        //if (checkRole)
                            //        //{
                            //        var contract = db.Contracts.Where(e => e.ProjectId == item.TargetId && e.Status != (int)Const.Status.DELETED).ToList();
                            //        //Xóa quyền giám sát
                            //        foreach (var itemC in contract)
                            //        {
                            //            var listMU = db.MonitoringUsers.Where(e => e.ContractId == itemC.ContractId && e.UserMonitoringId == data.UserId && e.Type == (int)Const.TypeMonitoringUser.MANAGER).FirstOrDefault();
                            //            if (listMU != null)
                            //            {
                            //                MonitoringUser monitoringUser = listMU;
                            //                monitoringUser.UserCreateId = data.UserCreateId;
                            //                monitoringUser.Status = (int)Const.Status.DELETED;
                            //                db.Entry(monitoringUser).State = EntityState.Modified;
                            //            }
                            //        }
                            //        //}
                            //    }
                            //    else
                            //    {
                            //        //Nếu là quyền giám sát
                            //        if (!checkRole)
                            //        {
                            //            var contract = db.Contracts.Where(e => e.ProjectId == item.TargetId && e.Status != (int)Const.Status.DELETED).ToList();
                            //            //Xóa quyền giám sát
                            //            foreach (var itemC in contract)
                            //            {
                            //                var listMU = db.MonitoringUsers.Where(e => e.ContractId == itemC.ContractId && e.UserMonitoringId == data.UserId && e.Type == (int)Const.TypeMonitoringUser.MANAGER).FirstOrDefault();
                            //                if (listMU != null)
                            //                {
                            //                    MonitoringUser monitoringUser = listMU;
                            //                    monitoringUser.UserCreateId = data.UserCreateId;
                            //                    monitoringUser.Status = (int)Const.Status.DELETED;
                            //                    db.Entry(monitoringUser).State = EntityState.Modified;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            await db.SaveChangesAsync();

                            transaction.Commit();
                            ////create action
                            //Models.EF.Action action = new Models.EF.Action();
                            //action.ActionName = "Sửa tài khoản";
                            //action.ActionType = "UPDATE";
                            //action.TargetId = data.UserId;
                            //action.TargetType = "USER";
                            //action.Logs = JsonConvert.SerializeObject(data);
                            //action.Time = 0;
                            //action.Type = (int)Const.TypeAction.ACTION;
                            //action.CreatedAt = DateTime.Now;
                            //action.UserId = data.UserCreateId;
                            //action.Status = (int)Const.Status.NORMAL;
                            //db.Actions.Add(action);
                            //await db.SaveChangesAsync();

                            ////push action firebase
                            //Models.Data.Firebase.pushAction(action);

                            //push user firebase
                            var tasks = new[]
                            {
                            Task.Run(() => IOITWebApp.Models.Data.Firebase.updateUser(current))
                        };

                            def.meta = new Meta(200, "Success");
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserRoleExists(id))
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
                log.Error("Exception: " + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Function
        [HttpPost]
        public async Task<IActionResult> PostUserRole([FromBody] UserRoleDT data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
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
                    int countUser = db.User.Where(f => f.CompanyId == data.CompanyId && f.Status != (int)Const.Status.DELETED).Count();
                    int CountUserActive = db.Company.Where(c => c.CompanyId == data.CompanyId).FirstOrDefault().CountUser;
                    if ((countUser + 1) > CountUserActive)
                    {
                        def.meta = new Meta(21111, "Account limit exceeded!");
                        return Ok(def);
                    }

                    User checkUserNameExist = db.User.Where(f => f.UserName == data.UserName && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkUserNameExist != null)
                    {
                        def.meta = new Meta(211, "UserName Exist!");
                        return Ok(def);
                    }

                    User checkEmailExist = db.User.Where(f => f.Email == data.Email && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkEmailExist != null)
                    {
                        def.meta = new Meta(2111, "Email Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        User user = new User();
                        user.Address = data.Address;
                        user.FullName = data.FullName;
                        user.UserName = data.UserName;
                        user.Code = data.Code;
                        user.Email = data.Email;
                        user.Avata = data.Avata;
                        user.Password = Utils.GetMD5Hash(data.Password);
                        user.Phone = data.Phone;
                        user.UnitId = data.UnitId;
                        user.DepartmentId = data.DepartmentId;
                        user.PositionId = data.PositionId;
                        user.KeyLock = Utils.RandomString(20);
                        user.RegEmail = Utils.RandomString(8);
                        user.RoleMax = 9999;
                        user.RoleLevel = 99;
                        user.IsRoleGroup = data.IsRoleGroup != null ? data.IsRoleGroup : true;
                        user.CreatedAt = DateTime.Now;
                        user.UpdatedAt = DateTime.Now;
                        user.Status = (int)Const.Status.NORMAL;
                        user.UserCreateId = userId;
                        user.UserEditId = userId;
                        user.CompanyId = data.CompanyId;
                        user.BranchId = data.BranchId;
                        db.User.Add(user);
                        await db.SaveChangesAsync();
                        data.UserId = user.UserId;

                        //update pass
                        string pass = user.KeyLock.Trim() + user.RegEmail.Trim() + user.UserId + user.Password.Trim();
                        user.Password = Utils.GetMD5Hash(pass);

                        // role
                        byte level = 99;
                        int max = 9999;
                        //add role 
                        //foreach (var item in data.listRole)
                        //{
                        var role = db.Role.Find(data.Roleid);
                        if (role != null)
                        {
                            UserRole userRole = new UserRole();
                            userRole.RoleId = role.RoleId;
                            userRole.UserId = user.UserId;
                            userRole.Status = (int)Const.Status.NORMAL;
                            db.UserRole.Add(userRole);
                            //
                            if (role.LevelRole < level)
                            {
                                level = (byte)role.LevelRole;
                                max = role.RoleId;
                            }
                        }
                        //}
                        //update cấp độ user và quyền cao nhất của user đó
                        user.RoleLevel = level;
                        user.RoleMax = max;
                        db.Entry(user).State = EntityState.Modified;

                        //add function
                        //foreach (var item in data.listFunction)
                        //{
                        //    FunctionRole functionRole = new FunctionRole();
                        //    functionRole.TargetId = data.UserId;
                        //    functionRole.FunctionId = item.FunctionId;
                        //    functionRole.ActiveKey = item.ActiveKey;
                        //    functionRole.Type = (int)Const.TypeFunction.FUNCTION_USER;
                        //    functionRole.CreatedAt = DateTime.Now;
                        //    functionRole.UpdatedAt = DateTime.Now;
                        //    functionRole.UserId = data.UserCreateId;
                        //    functionRole.Status = (int)Const.Status.NORMAL;
                        //    db.FunctionRole.Add(functionRole);
                        //}

                        //add unit
                        //foreach (var item in data.listUnit)
                        //{
                        //    UserProject userProject = new UserProject();
                        //    userProject.TargetId = item.TargetId;
                        //    userProject.UserId = data.UserId;
                        //    userProject.Type = (int)Const.TypeUserProject.USER_UNIT;
                        //    userProject.CreatedAt = DateTime.Now;
                        //    userProject.UpdatedAt = DateTime.Now;
                        //    userProject.UserCreateId = data.UserCreateId;
                        //    userProject.Status = (int)Const.Status.NORMAL;
                        //    db.UserProjects.Add(userProject);
                        //}

                        //add project
                        //foreach (var item in data.listProject)
                        //{
                        //    UserProject userProject = new UserProject();
                        //    userProject.TargetId = item.TargetId;
                        //    userProject.UserId = data.UserId;
                        //    userProject.Type = (int)Const.TypeUserProject.USER_PROJECT;
                        //    userProject.CreatedAt = DateTime.Now;
                        //    userProject.UpdatedAt = DateTime.Now;
                        //    userProject.UserCreateId = data.UserCreateId;
                        //    userProject.Status = (int)Const.Status.NORMAL;
                        //    db.UserProjects.Add(userProject);

                        //    //add role monitoring
                        //    //Nếu user có quyền quán lý dự án x sẽ có quyền giám sát tất cả các hợp đồng trong dự án x
                        //    if (checkRole)
                        //    {
                        //        var contract = db.Contracts.Where(e => e.ProjectId == item.TargetId && e.Status != (int)Const.Status.DELETED).ToList();
                        //        //Thêm mới
                        //        foreach (var itemC in contract)
                        //        {
                        //            var mu = db.MonitoringUsers.Where(e => e.ContractId == itemC.ContractId && e.UserMonitoringId == data.UserId && e.Type == (int)Const.TypeMonitoringUser.MANAGER).ToList();
                        //            if (mu.Count <= 0)
                        //            {
                        //                MonitoringUser monitoringUser = new MonitoringUser();
                        //                monitoringUser.ContractId = itemC.ContractId;
                        //                monitoringUser.ProjectId = itemC.ProjectId;
                        //                monitoringUser.DateStart = itemC.DateStart;
                        //                monitoringUser.DateEnd = itemC.DateEnd;
                        //                monitoringUser.UserMonitoringId = data.UserId;
                        //                monitoringUser.Type = (int)Const.TypeMonitoringUser.MANAGER;
                        //                monitoringUser.CreatedAt = DateTime.Now;
                        //                monitoringUser.UserCreateId = data.UserCreateId;
                        //                monitoringUser.Status = (int)Const.Status.NORMAL;
                        //                db.MonitoringUsers.Add(monitoringUser);
                        //            }
                        //            else
                        //            {
                        //                MonitoringUser monitoringUser = mu.FirstOrDefault();
                        //                monitoringUser.DateStart = itemC.DateStart;
                        //                monitoringUser.DateEnd = itemC.DateEnd;
                        //                monitoringUser.Type = (int)Const.TypeMonitoringUser.MANAGER;
                        //                monitoringUser.UserCreateId = data.UserCreateId;
                        //                db.Entry(monitoringUser).State = EntityState.Modified;
                        //            }
                        //        }
                        //        //Xóa cũ
                        //        var listMU = db.MonitoringUsers.Where(e => e.UserMonitoringId == data.UserId && e.Type == (int)Const.TypeMonitoringUser.MANAGER).ToList();
                        //        foreach (var itemLMU in listMU)
                        //        {
                        //            var lmu = contract.Where(e => e.ContractId == itemLMU.ContractId).ToList();
                        //            if (lmu.Count <= 0)
                        //            {
                        //                MonitoringUser monitoringUser = itemLMU;
                        //                monitoringUser.UserCreateId = data.UserId;
                        //                monitoringUser.Status = (int)Const.Status.DELETED;
                        //                db.Entry(monitoringUser).State = EntityState.Modified;
                        //            }
                        //        }

                        //    }
                        //}

                        try
                        {
                            await db.SaveChangesAsync();

                            if (user.UserId > 0)
                            {
                                transaction.Commit();
                                ////create action
                                //Models.EF.Action action = new Models.EF.Action();
                                //action.ActionName = "Tạo tài khoản";
                                //action.ActionType = "CREATE";
                                //action.TargetId = data.UserId;
                                //action.TargetType = "USER";
                                //action.Logs = JsonConvert.SerializeObject(data);
                                //action.Time = 0;
                                //action.Type = (int)Const.TypeAction.ACTION;
                                //action.CreatedAt = DateTime.Now;
                                //action.UserId = data.UserCreateId;
                                //action.Status = (int)Const.Status.NORMAL;
                                //db.Actions.Add(action);
                                //await db.SaveChangesAsync();

                                ////push action firebase
                                //Models.Data.Firebase.pushAction(action);
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (UserRoleExists(data.UserId))
                            {
                                def.meta = new Meta(211, "Exist");
                                return Ok(def);
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Function/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                    User data = await db.User.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }

                    if (id == 1)
                    {
                        def.meta = new Meta(210, "Not delete Admin Super");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //delete user
                        data.UserEditId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        //delete user role
                        var userRoles = db.UserRole.Where(e => e.UserId == id && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in userRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //delete function role
                        var functionRoles = db.FunctionRole.Where(e => e.TargetId == id
                        && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                        && e.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in functionRoles)
                        {
                            item.Status = (int)Const.Status.DELETED;
                            item.UpdatedAt = DateTime.Now;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        //delete user unit
                        //var userUnit = db.UserProjects.Where(e => e.UserId == id
                        //&& e.Type == (int)Const.TypeUserProject.USER_UNIT
                        //&& e.Status != (int)Const.Status.DELETED).ToList();
                        //foreach (var item in userUnit)
                        //{
                        //    item.Status = (int)Const.Status.DELETED;
                        //    item.UpdatedAt = DateTime.Now;
                        //    db.Entry(item).State = EntityState.Modified;
                        //}

                        //delete user project
                        //var userProject = db.UserProjects.Where(e => e.UserId == id
                        //&& e.Type == (int)Const.TypeUserProject.USER_PROJECT
                        //&& e.Status != (int)Const.Status.DELETED).ToList();
                        //foreach (var item in userProject)
                        //{
                        //    item.Status = (int)Const.Status.DELETED;
                        //    item.UpdatedAt = DateTime.Now;
                        //    db.Entry(item).State = EntityState.Modified;
                        //}

                        try
                        {
                            await db.SaveChangesAsync();
                            if (data.UserId > 0)
                            {
                                transaction.Commit();
                                //create action
                                IOITWebApp.Models.EF.Action action = new IOITWebApp.Models.EF.Action();
                                action.ActionName = "Xóa tài khoản";
                                action.ActionType = "DELETE";
                                action.TargetId = data.UserId;
                                action.TargetType = data.FullName + " - " + data.UserName;
                                action.Logs = action.ActionName + " " + action.TargetType;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = data.UserId;
                                action.UserId = data.UserId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //push action
                                IOITWebApp.Models.Data.Firebase.pushAction(action);
                                //push user firebase
                                var tasks = new[]
                                {
                                Task.Run(() => IOITWebApp.Models.Data.Firebase.updateUser(data))
                                };
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = id;
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserRoleExists(id))
                            {
                                def.meta = new Meta(500, "Not Found");
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
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("ResetPassUser")]
        public async Task<IActionResult> ResetPassUser(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
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
                    User data = await db.User.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(400, "Bad Request");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //delete user
                        data.UserEditId = userId;
                        data.UpdatedAt = DateTime.Now;
                        //string pass = data.KeyLock.Trim() + data.RegEmail.Trim() + "1" + Utils.GetMD5Hash("123456").Trim();
                        string pass = data.KeyLock.Trim() + data.RegEmail.Trim() + data.UserId + Utils.GetMD5Hash("Abc123").Trim();
                        data.Password = Utils.GetMD5Hash(pass);
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();
                            if (data.UserId > 0)
                            {
                                transaction.Commit();
                                //create action
                                IOITWebApp.Models.EF.Action action = new IOITWebApp.Models.EF.Action();
                                action.ActionName = "Reset tài khoản";
                                action.ActionType = "PUT";
                                action.TargetId = data.UserId;
                                action.TargetType = data.FullName + " - " + data.UserName;
                                action.Logs = action.ActionName + " " + action.TargetType;
                                action.Time = 0;
                                action.Type = (int)Const.TypeAction.ACTION;
                                action.CreatedAt = DateTime.Now;
                                action.UserPushId = data.UserId;
                                action.UserId = data.UserId;
                                action.Status = (int)Const.Status.NORMAL;
                                db.Action.Add(action);
                                await db.SaveChangesAsync();

                                //push action
                                IOITWebApp.Models.Data.Firebase.pushAction(action);
                                //push user firebase
                                var tasks = new[]
                                {
                                Task.Run(() => IOITWebApp.Models.Data.Firebase.updateUser(data))
                                };
                            }
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = id;
                            return Ok(def);
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateConcurrencyException:" + e);
                            if (!UserRoleExists(id))
                            {
                                def.meta = new Meta(500, "Not Found");
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
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool UserRoleExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.User.Count(e => e.UserId == id) > 0;
            }
        }

        [HttpGet("GetByUser")]
        public IActionResult GetByUserAsync(int id)
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
            using (var db = new CNTTVNWebContext())
            {
                IOITWebApp.Models.EF.User user = db.User.Find(id);
                var BranchId = user.BranchId;
                string[] arrListStr = BranchId.Split(',');
                int[] convertedItems = Array.ConvertAll<string, int>(arrListStr, int.Parse);
                IQueryable<Branch> data = db.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => convertedItems.Contains(x.BranchId));
                if (data == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }

                def.meta = new Meta(200, "Success");
                def.data = data.ToList();
                return Ok(def);
            }
        }
    }
}
