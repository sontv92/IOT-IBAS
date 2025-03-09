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
    public class WebsiteController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("website", "website");
        private static string functionCode = "QLWS";

        // GET: api/Website
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
                    IQueryable<Website> data = db.Website.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("WebsiteId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("WebsiteId desc");
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
                        def.data = data.Select(e=>new {
                            e.WebsiteId,
                            e.Name,
                            e.Url,
                            e.LanguageId,
                            e.CompanyId,
                            e.WebsiteParentId,
                            e.LogoHeader,
                            e.LogoFooter,
                            e.Banner,
                            e.Hotline,
                            e.Hotmail,
                            e.Address,
                            e.LinkGooglePlus,
                            e.LinkFacebookPage,
                            e.LinkTwitter,
                            e.LinkYoutube,
                            e.LinkInstagram,
                            e.LinkLinkedIn,
                            e.LinkOther1,
                            e.LinkOther2,
                            e.LinkOther3,
                            e.GoogleAnalitics,
                            e.UserId,
                            e.CreatedAt,
                            e.UpdatedAt,
                            e.MetaTitle,
                            e.MetaKeyword,
                            e.MetaDescription,
                            e.HighlightsNewsId,
                            e.Status,
                            e.TechNiQuePhone,
                            e.GuaRanTeePhone,
                            websiteParent = db.Website.Where(c => c.WebsiteId == e.WebsiteParentId).Select(c => new {
                                c.WebsiteId,
                                c.Name
                            }).FirstOrDefault()
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

        // GET: api/Website/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWebsite(int id)
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
                    Website data = await db.Website.FindAsync(id);

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

        // PUT: api/Website/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWebsite(int id,[FromBody] Website data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
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
                if (companyId!=data.CompanyId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = data.Status;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.WebsiteId > 0)
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
                            if (!WebsiteExists(data.WebsiteId))
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

        // POST: api/Website
        [HttpPost]
        public async Task<IActionResult> PostWebsite([FromBody] Website data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //Nếu ko truyền vào ngôn ngữ thì chọn ngôn ngữ mạc định
                        if (data.LanguageId == null)
                        {
                            //Nếu ngôn ngữ mạc định = 0 thì cảnh báo tạo ngôn ngữ 
                            if (languageId == 0)
                            {
                                def.meta = new Meta(210, "Language default is null");
                                return Ok(def);
                            }
                            else
                                data.LanguageId = languageId;
                        }

                        data.CompanyId = companyId;
                        data.UserId = userId;
                        data.CreatedAt = DateTime.Now;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.NORMAL;
                        db.Website.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.WebsiteId > 0)
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
                            if (WebsiteExists(data.WebsiteId))
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

        // DELETE: api/Website/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWebsite(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
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
                    Website data = await db.Website.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    if (companyId != data.CompanyId)
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

                            if (data.WebsiteId > 0)
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
                            if (!WebsiteExists(data.WebsiteId))
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

        private bool WebsiteExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Website.Count(e => e.WebsiteId == id) > 0;
            }
        }
    }
}


