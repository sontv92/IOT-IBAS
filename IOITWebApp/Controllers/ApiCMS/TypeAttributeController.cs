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
    public class TypeAttributeController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("typeattribute", "typeattribute");
        private static string functionCode = "QLLH";

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
                    IQueryable<TypeAttribute> data = db.TypeAttribute.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("TypeAttributeId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("TypeAttributeId desc");
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
                            e.TypeAttributeId,
                            e.Name,
                            e.IsUpdate,
                            e.IsDelete,
                            e.TypeAttribuiteParentId,
                            e.UserId,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.Status,
                            listAttributeItem = db.TypeAttributeItem.Where(c => c.TypeAttributeId == e.TypeAttributeId && c.Status != (int)Const.Status.DELETED).Select(c => new
                            {
                                c.TypeAttributeItemId,
                                c.Name,
                                c.TypeAttributeId,
                                c.Location,
                                c.Code,
                                c.CreatedAt,
                                c.UpdatedAt,
                                c.Status
                            }).ToList()
                        }).ToList();
                    }
                        //def.data = data.ToList();

                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/TypeAttribute/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeAttribute(int id)
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
                    TypeAttribute data = await db.TypeAttribute.FindAsync(id);

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

        // PUT: api/TypeAttribute/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeAttribute(int id,[FromBody] TypeAttributeDTO data)
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
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        TypeAttribute typeAttribute = db.TypeAttribute.Where(e => e.TypeAttributeId == id).FirstOrDefault();
                        typeAttribute.Name = data.Name;
                        typeAttribute.TypeAttribuiteParentId = data.TypeAttribuiteParentId;
                        typeAttribute.IsUpdate = data.IsUpdate;
                        typeAttribute.IsDelete = data.IsDelete;
                        typeAttribute.UpdatedAt = DateTime.Now;
                        typeAttribute.UserId = userId;
                        typeAttribute.Status = data.Status;
                        db.Entry(typeAttribute).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeId > 0)
                            {
                                ////Xóa cũ
                                //var listTypeAttributeItem = db.TypeAttributeItem.Where(e => e.TypeAttributeId == id).ToList();
                                //foreach(var item in listTypeAttributeItem)
                                //{
                                //    item.Status = (int)Const.Status.DELETED;
                                //    db.Entry(item).State = EntityState.Modified;
                                //}
                                //await db.SaveChangesAsync();
                                //Thêm mới
                                foreach (var item in data.listAttributeItem)
                                {

                                    if(item.Status != (int)Const.Status.DELETED && item.TypeAttributeItemId == null)
                                    {
                                        TypeAttributeItem typeAttributeItem = new TypeAttributeItem();
                                        typeAttributeItem.Name = item.Name;
                                        typeAttributeItem.TypeAttributeId = typeAttribute.TypeAttributeId;
                                        typeAttributeItem.Code = item.Code;
                                        typeAttributeItem.Location = item.Location;
                                        typeAttributeItem.UserId = userId;
                                        typeAttributeItem.CreatedAt = DateTime.Now;
                                        typeAttributeItem.UpdatedAt = DateTime.Now;
                                        typeAttributeItem.Status = (int)Const.Status.NORMAL;
                                        await db.TypeAttributeItem.AddAsync(typeAttributeItem);
                                    }
                                    else if(item.TypeAttributeItemId != null)
                                    {
                                        TypeAttributeItem exist = db.TypeAttributeItem.Find(item.TypeAttributeItemId);
                                        if (exist != null)
                                        {
                                            if (item.Status == (int)Const.Status.DELETED)
                                            {
                                                exist.Status = (int)Const.Status.DELETED;
                                            }
                                            else
                                            {
                                                exist.Code = item.Code;
                                                exist.Name = item.Name;
                                                exist.Location = item.Location;
                                            }
                                            db.Entry(exist).State = EntityState.Modified;
                                        }
                                    }
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
                            }
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
                            if (!TypeAttributeExists(data.TypeAttributeId))
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

        // POST: api/TypeAttribute
        [HttpPost]
        public async Task<IActionResult> PostTypeAttribute([FromBody] TypeAttributeDTO data)
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
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        TypeAttribute typeAttribute = new TypeAttribute();
                        typeAttribute.Name = data.Name;
                        typeAttribute.TypeAttribuiteParentId = data.TypeAttribuiteParentId;
                        typeAttribute.IsUpdate = data.IsUpdate;
                        typeAttribute.IsDelete = data.IsDelete;
                        typeAttribute.CreatedAt = DateTime.Now;
                        typeAttribute.UpdatedAt = DateTime.Now;
                        typeAttribute.UserId = userId;

                        db.TypeAttribute.Add(typeAttribute);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (typeAttribute.TypeAttributeId > 0)
                            {
                                foreach (var item in data.listAttributeItem)
                                {
                                    TypeAttributeItem typeAttributeItem = new TypeAttributeItem();
                                    typeAttributeItem.Name = item.Name;
                                    typeAttributeItem.TypeAttributeId = typeAttribute.TypeAttributeId;
                                    typeAttributeItem.Code = item.Code;
                                    typeAttributeItem.Location = item.Location;
                                    typeAttributeItem.UserId = userId;
                                    typeAttributeItem.CreatedAt = DateTime.Now;
                                    typeAttributeItem.UpdatedAt = DateTime.Now;
                                    typeAttributeItem.Status = (int)Const.Status.NORMAL;
                                    await db.TypeAttributeItem.AddAsync(typeAttributeItem);
                                }
                                await db.SaveChangesAsync();
                                transaction.Commit();
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
                            if (TypeAttributeExists(data.TypeAttributeId))
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

        // DELETE: api/TypeAttribute/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeAttribute(int id)
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
                using (var db = new CNTTVNWebContext())
                {
                    TypeAttribute data = await db.TypeAttribute.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.TypeAttributeId > 0)
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
                            if (!TypeAttributeExists(data.TypeAttributeId))
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

        private bool TypeAttributeExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.TypeAttribute.Count(e => e.TypeAttributeId == id) > 0;
            }
        }

    }
}
