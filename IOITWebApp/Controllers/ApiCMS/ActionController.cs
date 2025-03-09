using IOITWebApp.Models;
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
    public class ActionController : ControllerBase
    {
        //private CNTTVNData db = new CNTTVNData();
        private static readonly ILog log = LogMaster.GetLogger("action", "action");
        private static string functionCode = "QLTB";

        // GET: api/Action
        [Route("user/{id}")]
        [HttpGet]
        public IActionResult GetByPage(int id, [FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (paging != null)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Models.EF.Action> data = db.Action.Where(c => c.UserId == id && c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("ActionId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("ActionId desc");
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
                        def.data = data.Select(e => new
                        {
                            e.ActionId,
                            e.ActionName,
                            e.ActionType,
                            e.TargetId,
                            e.TargetType,
                            e.CreatedAt,
                            e.UserPushId,
                            e.UserId,
                            e.Type,
                            user = db.User.Where(u => u.UserId == e.UserPushId).Select(u => new
                            {
                                u.FullName,
                                u.Avata
                            }).FirstOrDefault(),
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

        // GET: api/Action/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAction(int id)
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
                    Models.EF.Action data = await db.Action.FindAsync(id);

                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    def.meta = new Meta(200, "Success");
                    def.data = data;
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

        // PUT: api/Action/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAction(int id, Models.EF.Action data)
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

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ActionId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!ActionExists(data.ActionId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Action
        [HttpPost]
        public async Task<IActionResult> PostAction(Models.EF.Action data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
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
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Action.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ActionId > 0)
                                transaction.Commit();
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
                            if (ActionExists(data.ActionId))
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
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Action/5
        [HttpDelete]
        public async Task<IActionResult> DeleteAction(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Models.EF.Action data = await db.Action.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Action.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.ActionId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!ActionExists(data.ActionId))
                            {
                                def.meta = new Meta(404, "Not Found");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Internal Server Error");
                                return Ok(def);
                            }
                        }
                    }
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
        //[Route("api/action/getWarningContract/{id}")]
        //public IHttpActionResult getWarningContract(int id, [FromUri] FilteredPagination paging)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (paging != null)
        //    {
        //        def.meta = new Meta(200, "Success");

        //        IQueryable<DashboardNote> data = from a in db.Actions
        //                                         join c in db.Contracts on a.TargetId equals c.ContractId
        //                                         where a.UserId == id && a.ActionType == "WARNING_CONTRACT" && a.Type == (int)Const.TypeAction.WARNING
        //                                         && a.Status != (int)Const.Status.DELETED && c.Status != (int)Const.Status.DELETED
        //                                         select new DashboardNote
        //                                         {
        //                                             Id = a.ActionId,
        //                                             //MonitoringName = db.Users.Where(u => u.UserId == a.UserId).FirstOrDefault().FullName,
        //                                             //Avatar = db.Users.Where(u => u.UserId == a.UserId).FirstOrDefault().Avata,
        //                                             ContractName = c.Name,
        //                                             ContractCode = c.Code,
        //                                             //Note = a.ActionName + " đã " + a.ActionType,
        //                                             //MonitoringPhone = db.Users.Where(u => u.UserId == a.UserId).FirstOrDefault().Phone,
        //                                             Status = a.Status,
        //                                             //AvatarUpdate = db.Users.Where(u => u.UserId == a.Time).FirstOrDefault().Avata,
        //                                             //UserUpdate = db.Users.Where(u => u.UserId == a.Time).FirstOrDefault().FullName,
        //                                             //DateUpdate = a.CreatedAt,
        //                                             //NoteSolution = a.Logs
        //                                         };


        //        if (paging.query != null)
        //        {
        //            paging.query = HttpUtility.UrlDecode(paging.query);
        //        }

        //        data = data.Where(paging.query);
        //        def.metadata = data.Count();

        //        if (paging.page_size > 0)
        //        {
        //            if (paging.order_by != null)
        //            {
        //                data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //            }
        //            else
        //            {
        //                data = data.OrderBy("Id desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
        //            }
        //        }
        //        else
        //        {
        //            if (paging.order_by != null)
        //            {
        //                data = data.OrderBy(paging.order_by);
        //            }
        //            else
        //            {
        //                data = data.OrderBy("Id desc");
        //            }
        //        }

        //        if (paging.select != null && paging.select != "")
        //        {
        //            paging.select = "new(" + paging.select + ")";
        //            paging.select = HttpUtility.UrlDecode(paging.select);
        //            def.data = data.Select(paging.select);
        //        }
        //        else
        //        {
        //            def.data = data;
        //        }

        //        return Ok(def);
        //    }
        //    else
        //    {
        //        def.meta = new Meta(400, "Bad Request");
        //        return Ok(def);
        //    }
        //}

        //[Route("api/action/{id}/UpdateWarning/{status}")]
        //[HttpPut]
        //public async Task<IHttpActionResult> UpdateWarning(int id, byte status, MonitoringHistory model)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    var identity = (ClaimsIdentity)User.Identity;
        //    int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
        //    int roleLevel = int.Parse(identity.Claims.Where(c => c.Type == "RoleLevel").Select(c => c.Value).SingleOrDefault());
        //    //check role
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }

        //        var data = await db.Actions.FindAsync(id);

        //        if (data == null)
        //        {
        //            def.meta = new Meta(400, "Bad Request");
        //            return Ok(def);
        //        }

        //        using (var transaction = db.Database.BeginTransaction())
        //        {
        //            if (roleLevel != (int)Const.RoleLevel.MONITORING)
        //            {
        //                //data.Logs = model != null ? model.NoteSolution : "";
        //                data.Time = userId;
        //                data.CreatedAt = DateTime.Now;
        //                data.Status = status;
        //            }
        //            else
        //            {
        //                def.meta = new Meta(222, "No permission");
        //                return Ok(def);
        //            }
        //            db.Entry(data).State = EntityState.Modified;
        //            try
        //            {
        //                await db.SaveChangesAsync();

        //                if (data.ActionId > 0)
        //                {
        //                    transaction.Commit();
        //                    try
        //                    {
        //                        //Lấy thông tin người gửi
        //                        var userSent = db.Users.Where(e => e.UserId == userId).FirstOrDefault();

        //                        //Lấy danh sách người nhận action + email - người giải quyết
        //                        var monitoringUsers = await db.MonitoringUsers.Where(e => e.ContractId == data.TargetId
        //                        && e.Type == (int)Const.TypeMonitoringUser.MANAGER && e.UserMonitoringId != userId
        //                        && e.Status == (int)Const.Status.NORMAL).ToListAsync();
        //                        foreach (var itemUP in monitoringUsers)
        //                        {
        //                            //Lấy tên hợp đồng
        //                            var contract = db.Contracts.Where(e => e.ContractId == data.TargetId).FirstOrDefault();
        //                            //create action
        //                            Models.EF.Action action = new Models.EF.Action();
        //                            if (status == (int)Const.Status.NORMAL_MANAGER)
        //                                action.ActionName = "chuyển cấp cao giải quyết chậm tiến độ hợp đồng";
        //                            else
        //                                action.ActionName = "giải quyết chậm tiến độ hợp đồng";
        //                            action.ActionType = "UPDATE";
        //                            action.TargetType = "của hợp đồng " + contract.Name + " - " + contract.Code;
        //                            action.TargetId = data.TargetId;
        //                            action.Logs = action.ActionName + " " + action.TargetType;
        //                            action.Time = 0;
        //                            action.Type = (int)Const.TypeAction.ACTION;
        //                            action.CreatedAt = DateTime.Now;
        //                            action.UserPushId = userId;
        //                            action.UserId = itemUP.UserMonitoringId;
        //                            action.Status = (int)Const.Status.NORMAL;

        //                            db.Actions.Add(action);
        //                            await db.SaveChangesAsync();

        //                            //push action
        //                            Models.Data.Firebase.pushAction(action);

        //                            //push email
        //                            //Lây thông tin người nhận
        //                            var user = await db.Users.Where(e => e.UserId == itemUP.UserMonitoringId).FirstOrDefaultAsync();
        //                            string mes = action.ActionName + " cho " + action.TargetType;
        //                            string content = "Nội dung chậm tiến độ:" + data.ActionName + " đã " + data.ActionType;
        //                            //    + "Giải pháp: " + data.Solution;
        //                            //sendEmail(user.FullName, user.Email, userSent.FullName, mes, HttpContext.Current);
        //                        }


        //                    }
        //                    catch { }
        //                }
        //                else
        //                    transaction.Rollback();

        //                def.meta = new Meta(200, "Success");
        //                def.data = data;
        //                return Ok(def);
        //            }
        //            catch (DbUpdateException e)
        //            {
        //                transaction.Rollback();
        //                log.Error("DbUpdateException:" + e);
        //                if (!ActionExists(data.ActionId))
        //                {
        //                    def.meta = new Meta(404, "Not Found");
        //                    return Ok(def);
        //                }
        //                else
        //                {
        //                    def.meta = new Meta(500, "Internal Server Error");
        //                    return Ok(def);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Internal Server Error");
        //        return Ok(def);
        //    }
        //}

        // delete action Firebase   
        [HttpPost("api/action/{id}/del-action-firebase")]
        public IActionResult DeleteActionFireBase(int id)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (id < 0 || id != userId)
            {
                def.meta = new Meta(404, "Not Found");
                return Ok(def);
            }
            var tasks = new[]
                {
                    Task.Run(() => Models.Data.Firebase.deleteAction(id))
                };

            def.meta = new Meta(200, "Success");
            def.data = id;
            return Ok(def);
        }

        [HttpPost("api/action/{id}/del-warning-firebase")]
        public IActionResult DeleteWarningFireBase(int id)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (id < 0 || id != userId)
            {
                def.meta = new Meta(404, "Not Found");
                return Ok(def);
            }
            var tasks = new[]
                {
                    Task.Run(() => Models.Data.Firebase.deleteWarning(id))
                };

            def.meta = new Meta(200, "Success");
            def.data = id;
            return Ok(def);
        }

        private bool ActionExists(long id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Action.Count(e => e.ActionId == id) > 0;
            }
        }

    }
}
