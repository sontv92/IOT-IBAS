using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
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
    public class RoleController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("role", "role");

        // GET: api/Role
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            var identity = (ClaimsIdentity)User.Identity;
            int roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault());
            int roleLevel = int.Parse(identity.Claims.Where(c => c.Type == "RoleLevel").Select(c => c.Value).SingleOrDefault());
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            if (paging != null)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Role> data = db.Role.Where(c => c.Status != (int)Const.Status.DELETED);
                    //if (roleMax != 1)
                    //{
                    //    paging.query = "LevelRole > " + roleLevel;
                    //}
                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    var userrole = db.UserRole.Where(c => c.UserId == userId).Where(c => c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (userrole.RoleId != 1)
                    {
                        data = data.Where(c=>c.RoleId != 1);
                    }
                    def.metadata = data.Count();

                    if (paging.page_size > 0)
                    {
                        if (paging.order_by != null)
                        {
                            data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                        }
                        else
                        {
                            data = data.OrderBy("RoleId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("RoleId desc");
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
                            e.RoleId,
                            e.Name,
                            e.Code,
                            e.Note,
                            e.LevelRole,
                            e.Status,
                            listFunction = db.FunctionRole.Where(fr => fr.TargetId == e.RoleId).Select(fr => new
                            {
                                fr.FunctionId,
                                fr.ActiveKey
                            })
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

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
            using (var db = new CNTTVNWebContext())
            {
                Role role = await db.Role.FindAsync(id);

                if (role == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }

                def.meta = new Meta(200, "Success");
                def.data = role;
                return Ok(def);
            }
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO role)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (id != role.RoleId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    Role checkItemExist = db.Role.Where(f => f.RoleId != role.RoleId && f.Code == role.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkItemExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Role data = await db.Role.FindAsync(id);
                        data.Code = role.Code;
                        data.Name = role.Name;
                        data.Note = role.Note;

                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
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
                            if (RoleExists(data.RoleId))
                            {
                                def.meta = new Meta(212, "Exist");
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Role
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleDTO role)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    Role checkItemExist = db.Role.Where(f => f.Code == role.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkItemExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {

                        Role roles = new Role();
                        roles.Code = role.Code;
                        roles.Note = role.Note;
                        roles.Name = role.Name;
                        roles.Status = (int)Const.Status.NORMAL;

                        db.Role.Add(roles);
                        role.RoleId = roles.RoleId;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (roles.RoleId > 0)
                                transaction.Commit();
                            else
                                transaction.Rollback();

                            def.meta = new Meta(200, "Success");
                            def.data = role;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (RoleExists(roles.RoleId))
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            DefaultResponse def = new DefaultResponse();
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
                        Role data = await db.Role.FindAsync(id);
                        data.Status = (int)Const.Status.DELETED;

                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.RoleId > 0)
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
                            if (RoleExists(data.RoleId))
                            {
                                def.meta = new Meta(212, "Exist");
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
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private bool RoleExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Role.Count(e => e.RoleId == id) > 0;
            }
        }
    }
}
