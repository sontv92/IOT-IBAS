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
using System.Collections.Generic;
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
    public class FunctionController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("function", "function");
        private static string functionCode = "QLCN";

        // GET: api/Function
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
                    IQueryable<Function> data = db.Function.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("FunctionId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("FunctionId desc");
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
                            e.FunctionId,
                            e.Code,
                            e.Name,
                            e.Note,
                            e.Status,
                            e.Url,
                            e.Icon,
                            e.FunctionParentId,
                            e.Location,
                            functionParent = db.Function.Where(f => f.FunctionId == e.FunctionParentId).Select(f => new
                            {
                                f.FunctionId,
                                f.Name,
                                f.Code,
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

        [HttpGet("listFunction")]
        public IActionResult listFunction()
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }
            try
            {
                List<SmallFunctionDTO> functions = new List<SmallFunctionDTO>();
                def.data = listFunction(functions, 0, 0, userId);
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch(Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("listFunctionRole")]
        public IActionResult listFunctionRole()
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
                List<FunctionDT> functions = new List<FunctionDT>();
                def.data = listFunction(0);
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // GET: api/Function/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFunction(int id)
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
                Function data = await db.Function.FindAsync(id);

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

        // PUT: api/Function/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFunction(int id, [FromBody] Function data)
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

                if(id == data.FunctionParentId)
                {
                    def.meta = new Meta(215, "ParentId Invalid!");
                    return Ok(def);
                }

                if (id != data.FunctionId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    Function checkExist = db.Function.Where(f => f.Code == data.Code && f.Status != (int)Const.Status.DELETED && f.FunctionId != id).FirstOrDefault();
                if (checkExist != null)
                {
                    def.meta = new Meta(211, "Code Exist!");
                    return Ok(def);
                }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.FunctionParentId = data.FunctionParentId != null ? data.FunctionParentId : 0;
                        data.UpdatedAt = DateTime.Now;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.FunctionId > 0)
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
                            if (FunctionExists(data.FunctionId))
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
                log.Error("Exception:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        // POST: api/Function
        [HttpPost]
        public async Task<IActionResult> PostFunction(Function data)
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
                    Function checkItemExist = db.Function.Where( f => f.Code == data.Code && f.Status != (int)Const.Status.DELETED).FirstOrDefault();
                if(checkItemExist != null)
                {
                    def.meta = new Meta(211, "Code Exist!");
                    return Ok(def);
                }

                data.Status = (int)Const.Status.NORMAL;

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.FunctionParentId = data.FunctionParentId != null ? data.FunctionParentId : 0;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        db.Function.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.FunctionId > 0)
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
                            if (FunctionExists(data.FunctionId))
                            {
                                def.meta = new Meta(212, "Exist");
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
        public async Task<IActionResult> DeleteFunction(int id)
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
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    Function data = await db.Function.FindAsync(id);
                    data.Status = (int)Const.Status.DELETED;

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Entry(data).State = EntityState.Modified;

                        var fr = db.FunctionRole.Where(e => e.FunctionId == data.FunctionId).ToList();
                        db.FunctionRole.RemoveRange(fr);
                        //fr.ForEach(f => f.Status = (int)Const.Status.DELETED);

                        var listChild = db.Function.Where(f => f.FunctionParentId == data.FunctionId && f.Status != (int)Const.Status.DELETED).ToList();
                        listChild.ForEach(c => c.FunctionParentId = 0);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.FunctionId > 0)
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
                            if (FunctionExists(data.FunctionId))
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

        private List<FunctionDT> listFunction(int functionId)
        {
            using (var db = new CNTTVNWebContext())
            {
                List<FunctionDT> functions = new List<FunctionDT>();
                var data = db.Function.Where(e => e.FunctionParentId == functionId && e.Status != (int)Const.Status.DELETED).ToList();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        FunctionDT function = new FunctionDT();
                        function.id = item.FunctionId;
                        function.label = item.Name;
                        function.icon = item.Icon;
                        function.location = item.Location;
                        function.selected = false;
                        function.children = listFunction(item.FunctionId);
                        function.is_max = functionId == 0 ? true : false;

                        functions.Add(function);
                    }
                }

                return functions;
            }
        }

        private List<SmallFunctionDTO> listFunction(List<SmallFunctionDTO> dt, int functionId, int level, int roleMax)
        {
            var index = level + 1;
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    IEnumerable<Function> data;
                    if (roleMax == 1)
                    {
                        data = db.Function.Where(e => e.FunctionParentId == functionId && e.Status != (int)Const.Status.DELETED).ToList();
                    }
                    else
                    {
                        data = (from fr in db.FunctionRole
                                join f in db.Function on fr.FunctionId equals f.FunctionId
                                where
                                fr.TargetId == roleMax &&
                                fr.Type == (int)Const.TypeFunction.FUNCTION_ROLE
                                && fr.Status != (int)Const.Status.DELETED && fr.Status != (int)Const.Status.DELETED
                                && f.FunctionParentId == functionId
                                select f).ToList();
                    }

                    if (data != null)
                    {
                        if (data.Count() > 0)
                        {
                            foreach (var item in data)
                            {
                                SmallFunctionDTO function = new SmallFunctionDTO();
                                function.FunctionId = item.FunctionId;
                                function.Code = item.Code;
                                function.Name = item.Name;
                                function.Level = level;
                                //function.children = listFunction(item.FunctionId);
                                dt.Add(function);
                                try
                                {
                                    listFunction(dt, item.FunctionId, index, roleMax);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }

            return dt;
        }

        private bool FunctionExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Function.Count(e => e.FunctionId == id) > 0;
            }
        }
    }
}
