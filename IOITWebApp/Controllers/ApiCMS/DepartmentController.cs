using IOITWebApp;
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
    public class DepartmentController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "QLPB";

        // GET: api/Department
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
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    IQueryable<Department> data = db.Department.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("DepartmentId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("DepartmentId desc");
                        }
                    }

                    if (paging.select != null && paging.select != "")
                    {
                        paging.select = "new(" + paging.select + ")";
                        paging.select = HttpUtility.UrlDecode(paging.select);
                        def.data = data.Select(paging.select);
                    }
                    else
                        def.data = data.Select(d => new
                        {
                            d.DepartmentId,
                            d.Code,
                            d.Name,
                            d.Phone,
                            d.Email,
                            d.Location,
                            d.CreatedAt,
                            d.UpdatedAt,
                            d.UserId,
                            //unit = db.Units.Where(u => u.UnitId == d.UnitId && u.Status != (int)Const.Status.DELETED).Select(u => new {
                            //    u.UnitId,
                            //    u.Code,
                            //    u.Name
                            //}).FirstOrDefault()
                        }).ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Department/5
        [HttpGet]
        public async Task<IActionResult> GetDepartment(int id)
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
                    Department data = await db.Department.FindAsync(id);

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

        // PUT: api/Department/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id,[FromBody] Department data)
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
                    Department checkExist = db.Department.Where(f => f.Code == data.Code && f.Status != (int)Const.Status.DELETED && f.DepartmentId != id).FirstOrDefault();
                    if (checkExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UpdatedAt = DateTime.Now;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DepartmentId > 0)
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
                            if (!DepartmentExists(data.DepartmentId))
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

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> PostDepartment([FromBody] Department data)
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
                    Department checkExist = db.Department.Where(f => f.Code == data.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (checkExist != null)
                    {
                        def.meta = new Meta(211, "Code Exist!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        db.Department.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.DepartmentId > 0)
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
                            if (DepartmentExists(data.DepartmentId))
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

        // DELETE: api/Department/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
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
                    Department data = await db.Department.FindAsync(id);
                if (data == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }
                        using (var transaction = db.Database.BeginTransaction())
                        {

                            data.UpdatedAt = DateTime.Now;
                            data.Status = (int)Const.Status.DELETED;
                            db.Entry(data).State = EntityState.Modified;
                            try
                            {
                                await db.SaveChangesAsync();

                                if (data.DepartmentId > 0)
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
                                if (!DepartmentExists(data.DepartmentId))
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

        private bool DepartmentExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Department.Count(e => e.DepartmentId == id) > 0;
            }
        }
    }
}
