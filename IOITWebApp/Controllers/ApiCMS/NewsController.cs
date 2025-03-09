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
    public class NewsController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "QLTT";

        // GET: api/News
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
                try
                {
                    using (var db = new CNTTVNWebContext())
                    {
                        def.meta = new Meta(200, "Success");
                        IQueryable<News> data = db.News.Where(c => c.Status != (int)Const.Status.DELETED);
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
                                data = data.OrderBy("NewsId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("NewsId desc");
                            }
                        }

                        if (paging.select != null && paging.select != "")
                        {
                            paging.select = "new(" + paging.select + ")";
                            paging.select = HttpUtility.UrlDecode(paging.select);
                            def.data = data.Select(paging.select).ToDynamicList();
                        }
                        else
                        {
                            def.data = data.Select(e => new
                            {
                                e.NewsId,
                                e.Title,
                                e.Description,
                                e.Contents,
                                e.Image,
                                e.Url,
                                e.DateStartActive,
                                e.DateStartOn,
                                e.DateEndOn,
                                e.IsHome,
                                e.IsHot,
                                e.CompanyId,
                                e.WebsiteId,
                                e.ViewNumber,
                                e.Location,
                                e.TypeNewsId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.Status,
                                e.IsService,
                                e.LinkVideo,
                                e.Author,
                                listCategory = db.CategoryMapping.Where(cp => cp.TargetId == e.NewsId && cp.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS && cp.Status != (int)Const.Status.DELETED).Select(p => new {
                                    p.CategoryId,
                                    Name = db.Category.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault().Name,
                                    Check = true
                                }).ToList(),
                                listTag = db.Tag.Where(t => t.TargetId == e.NewsId && t.Status != (int)Const.Status.DELETED).Select(p => new {
                                    p.TagId,
                                    p.Name,
                                    Check = true
                                }).ToList(),
                                listAttachment = db.Attactment.Where(a => a.TargetId == e.NewsId && a.TargetType == (int)Const.TypeAttachment.NEWS_IMAGE && a.Status != (int)Const.Status.DELETED).ToList(),
                                listRelated = db.Related.Where(r => r.TargetId == e.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).Select(r => new {
                                    r.TargetRelatedId
                                }).ToList()
                            }).ToList();
                        }

                        return Ok(def);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception:" + e);
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNews(int id)
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
                    News data = await db.News.FindAsync(id);

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

        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNews(int id,[FromBody] NewsDTO data)
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

                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }

                //if (data.Contents == null || data.Contents == "")
                //{
                //    def.meta = new Meta(2111, "Contents Null!");
                //    return Ok(def);
                //}

                if (data.TypeNewsId == null || data.TypeNewsId == -1)
                {
                    def.meta = new Meta(2112, "TypeNewsId Null!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        News news = await db.News.FindAsync(id);
                        news.Title = data.Title;
                        news.Description = data.Description;
                        news.Contents = data.Contents;
                        news.Image = data.Image;
                        news.Url = data.Url;
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn != null ? data.DateStartOn : news.DateStartOn;
                        news.DateEndOn = data.DateEndOn != null ? data.DateEndOn : news.DateEndOn;
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.Location = data.Location;
                        news.TypeNewsId = data.TypeNewsId;
                        news.MetaTitle = data.MetaTitle;
                        news.MetaKeyword = data.MetaKeyword;
                        news.MetaDescription = data.MetaDescription;
                        //news.CreatedAt = DateTime.Now;
                        news.UpdatedAt = DateTime.Now;
                        news.UserId = data.UserId;
                        news.Status = data.Status;
                        news.IsService = data.IsService;
                        news.LinkVideo = data.LinkVideo;
                        news.Author = data.Author;

                        db.Entry(news).State = EntityState.Modified;

                        //remove category mapping

                        //add category mapping
                        if (data.listCategory != null)
                        {
                            foreach (var item in data.listCategory)
                            {
                                CategoryMapping exist = db.CategoryMapping.Where(cm => cm.CategoryId == item.CategoryId && cm.TargetId == id && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                if(exist == null)
                                {
                                    if(item.Check == true)
                                    {
                                        CategoryMapping categoryNewsMapping = new CategoryMapping();
                                        categoryNewsMapping.CategoryId = item.CategoryId;
                                        categoryNewsMapping.TargetId = news.NewsId;
                                        categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_NEWS;
                                        categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                        categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                        db.CategoryMapping.Add(categoryNewsMapping);
                                    }
                                }
                                else
                                {
                                    if(item.Check != true)
                                    {
                                        exist.Status = (int)Const.Status.DELETED;
                                        db.Entry(exist).State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        //remove tag

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                if(item.TagId == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TargetId = news.NewsId;
                                    tag.TargetType = (int)Const.TypeTag.TAG_NEWS;
                                    tag.Url = Utils.NonUnicode(item.Name);
                                    tag.UserId = data.UserId;
                                    tag.CreatedAt = DateTime.Now;
                                    tag.Status = (int)Const.Status.NORMAL;
                                    db.Tag.Add(tag);
                                }
                                else
                                {
                                    Tag exist = db.Tag.Where(t => t.TagId == item.TagId && t.Status != (int)Const.Status.DELETED).FirstOrDefault();
                                    if (exist != null)
                                    {
                                        if(item.Check == false)
                                        {
                                            exist.Status = (int)Const.Status.DELETED;
                                            db.Entry(exist).State = EntityState.Modified;
                                        }
                                    }
                                }
                            }
                        }

                        if(data.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE)
                        {
                            if (data.listAttachment != null)
                            {
                                foreach (var item in data.listAttachment)
                                {
                                    if (item.AttactmentId != null)
                                    {
                                        var attachmentExist = db.Attactment.Find(item.AttactmentId);
                                        if (attachmentExist != null)
                                        {
                                            if (item.Status == (int)Const.Status.DELETED)
                                            {
                                                attachmentExist.Status = (int)Const.Status.DELETED;
                                            }
                                            else
                                            {
                                                attachmentExist.IsImageMain = item.IsImageMain;
                                            }
                                        }
                                        db.Entry(attachmentExist).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        Attactment attactment = new Attactment();
                                        attactment.Name = item.Name;
                                        attactment.TargetId = news.NewsId;
                                        attactment.IsImageMain = item.IsImageMain;
                                        attactment.TargetType = (int)Const.TypeAttachment.NEWS_IMAGE;
                                        attactment.Url = item.Url;
                                        attactment.Thumb = item.Thumb;
                                        attactment.CreatedAt = DateTime.Now;
                                        attactment.UserId = data.UserId;
                                        attactment.Status = (int)Const.Status.NORMAL;
                                        db.Attactment.Add(attactment);

                                        if (item.IsImageMain == true)
                                        {
                                            news.Image = item.Url;
                                            db.Entry(news).State = EntityState.Modified;
                                        }
                                    }

                                    if (item.IsImageMain == true && item.Status != (int)Const.Status.DELETED)
                                    {
                                        news.Image = item.Url;
                                    }
                                    db.Entry(news).State = EntityState.Modified;
                                }
                            }
                        }

                        //Sản phẩm gợi ý
                        List<Related> listRelated = db.Related.Where(r => r.TargetId == news.NewsId && r.TargetType == (int)Const.TypeRelated.NEWS_NEWS && r.Status != (int)Const.Status.DELETED).ToList();
                        if (listRelated != null)
                        {
                            listRelated.ForEach(lr => lr.Status = (int)Const.Status.DELETED);
                        }

                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_NEWS;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = data.UserId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        CategoryMapping categoryMapping = db.CategoryMapping.Where(cm => cm.CategoryId == -1 && cm.TargetId == id && cm.TargetType == (int)Const.TypeOrderBy.NEWS_IS_HOME && cm.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (news.IsHome == true)
                        {
                            if (categoryMapping == null)
                            {
                                CategoryMapping cm = new CategoryMapping();
                                cm.CategoryId = -1;
                                cm.TargetId = news.NewsId;
                                cm.TargetType = (int)Const.TypeOrderBy.NEWS_IS_HOME;
                                cm.Location = 99;
                                cm.CreatedAt = DateTime.Now;
                                cm.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(cm);
                            }
                        }
                        else
                        {
                            if (categoryMapping != null)
                            {
                                categoryMapping.Status = (int)Const.Status.DELETED;
                                db.Update(categoryMapping);
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                            if (!NewsExists(data.NewsId))
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

        // POST: api/News
        [HttpPost]
        public async Task<IActionResult> PostNews(NewsDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
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

                if (data.UserId == null || data.UserId != userId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.CompanyId == null || data.CompanyId != companyId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Title == null || data.Title == "")
                {
                    def.meta = new Meta(211, "Title Null!");
                    return Ok(def);
                }

                //if (data.Contents == null || data.Contents == "")
                //{
                //    def.meta = new Meta(2111, "Contents Null!");
                //    return Ok(def);
                //}

                if (data.TypeNewsId == null || data.TypeNewsId == -1)
                {
                    def.meta = new Meta(2112, "TypeNewsId Null!");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //add news
                        News news = new News();
                        news.Title = data.Title;
                        news.Description = data.Description==null ? "" : data.Description;
                        news.Contents = data.Contents;
                        news.Image = data.Image;
                        news.Url = data.Url !=null ? data.Url : Utils.NonUnicode(news.Title);
                        news.DateStartActive = data.DateStartActive == null ? DateTime.Now : data.DateStartActive;
                        news.DateStartOn = data.DateStartOn == null ? DateTime.Now : data.DateStartOn;
                        news.DateEndOn = data.DateEndOn == null ? DateTime.Now.AddYears(100) : data.DateEndOn;
                        news.ViewNumber = data.ViewNumber != null ? data.ViewNumber : 1;
                        news.IsHome = data.IsHome != null ? data.IsHome : false;
                        news.IsHot = data.IsHot != null ? data.IsHot : false;
                        news.Location = data.Location;
                        news.TypeNewsId = data.TypeNewsId != null ? data.TypeNewsId : (int)Const.TypeNews.NEWS_TEXT;
                        news.MetaTitle = data.MetaTitle != null ? data.MetaTitle : data.Title;
                        news.MetaKeyword = data.MetaKeyword != null ? data.MetaKeyword : data.Title;
                        news.MetaDescription = data.MetaDescription != null ? data.MetaDescription : data.Description;
                        news.WebsiteId = data.WebsiteId != null ? data.WebsiteId : websiteId;
                        news.CompanyId = data.CompanyId != null ? data.CompanyId : companyId;
                        news.CreatedAt = DateTime.Now;
                        news.UpdatedAt = DateTime.Now;
                        news.UserId = data.UserId;
                        news.Status = data.Status;
                        news.IsService = data.IsService;
                        news.LinkVideo = data.LinkVideo;
                        news.Author = data.Author;

                        db.News.Add(news);
                        await db.SaveChangesAsync();

                        data.NewsId = news.NewsId;

                        if(news.IsHome == true)
                        {
                            CategoryMapping categoryMapping = new CategoryMapping();
                            categoryMapping.CategoryId = -1;
                            categoryMapping.TargetId = news.NewsId;
                            categoryMapping.TargetType = (int)Const.TypeOrderBy.NEWS_IS_HOME;
                            categoryMapping.Location = 99;
                            categoryMapping.CreatedAt = DateTime.Now;
                            categoryMapping.Status = (int)Const.Status.NORMAL;
                            db.CategoryMapping.Add(categoryMapping);
                        }

                        //add category mapping
                        if (data.listCategory != null)
                        {
                            foreach (var item in data.listCategory)
                            {
                                CategoryMapping categoryNewsMapping = new CategoryMapping();
                                categoryNewsMapping.CategoryId = item.CategoryId;
                                categoryNewsMapping.TargetId = news.NewsId;
                                categoryNewsMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_NEWS;
                                categoryNewsMapping.Location = db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList() != null ? db.CategoryMapping.Where(e => e.CategoryId == item.CategoryId).ToList().Count : 1;
                                categoryNewsMapping.CreatedAt = DateTime.Now;
                                categoryNewsMapping.Status = (int)Const.Status.NORMAL;
                                db.CategoryMapping.Add(categoryNewsMapping);
                            }
                        }

                        //add tag
                        if (data.listTag != null)
                        {
                            foreach (var item in data.listTag)
                            {
                                Tag tag = new Tag();
                                tag.Name = item.Name;
                                tag.TargetId = news.NewsId;
                                tag.TargetType = (int)Const.TypeTag.TAG_NEWS;
                                tag.Url = Utils.NonUnicode(item.Name);
                                tag.WebsiteId = news.WebsiteId;
                                tag.CompanyId = news.CompanyId;
                                tag.UserId = data.UserId;
                                tag.CreatedAt = DateTime.Now;
                                tag.Status = (int)Const.Status.NORMAL;
                                db.Tag.Add(tag);
                            }
                        }

                        //add list Image Product
                        if (data.TypeNewsId == (int)Const.TypeNews.NEWS_IMAGE)
                        {
                            if (data.listAttachment != null)
                            {
                                foreach (var item in data.listAttachment)
                                {
                                    Attactment attactment = new Attactment();
                                    attactment.Name = item.Name;
                                    attactment.TargetId = news.NewsId;
                                    attactment.IsImageMain = item.IsImageMain;
                                    attactment.TargetType = (int)Const.TypeAttachment.NEWS_IMAGE;
                                    attactment.Url = item.Url;
                                    attactment.Thumb = item.Thumb;
                                    attactment.CreatedAt = DateTime.Now;
                                    attactment.UserId = userId;
                                    attactment.Status = (int)Const.Status.NORMAL;
                                    db.Attactment.Add(attactment);

                                    if (item.IsImageMain == true)
                                    {
                                        news.Image = item.Url;
                                    }
                                }
                                db.Entry(news).State = EntityState.Modified;
                            }
                        }

                        //add product related
                        if (data.listRelated != null)
                        {
                            foreach (var item in data.listRelated)
                            {
                                Related related = new Related();
                                related.TargetId = news.NewsId;
                                related.TargetRelatedId = item.TargetRelatedId;
                                related.TargetType = (int)Const.TypeRelated.NEWS_NEWS;
                                related.Location = item.Location;
                                related.CreatedAt = DateTime.Now;
                                related.UserId = userId;
                                related.Status = (int)Const.Status.NORMAL;
                                db.Related.Add(related);
                            }
                        }

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                            if (NewsExists(data.NewsId))
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

        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
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
                    News data = await db.News.FindAsync(id);
                if (data == null)
                {
                    def.meta = new Meta(404, "Not Found");
                    return Ok(def);
                }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.News.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                            if (!NewsExists(data.NewsId))
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

        [HttpPut("ShowHide/{id}/{stt}")]
        public async Task<ActionResult> ShowHide(int id, int stt)
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
                using (var db = new CNTTVNWebContext())
                {
                    News data = await db.News.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Comment.Remove(data);
                        data.Status = (byte)stt;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.NewsId > 0)
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
                            if (!NewsExists(data.NewsId))
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

        //lấy bài viết nổi bật trên trang chủ theo HighlightsNewsId trong bảng Website
        //[HttpGet("GetHighlightsNews")]
        //public IActionResult GetHighlightsNews()
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
        //    {
        //        def.meta = new Meta(222, "Bạn không có quyền truy cập mục này!");
        //        return Ok(def);
        //    }
        //    try
        //    {
        //        using (var db = new CNTTVNWebContext())
        //        {
        //            var data = (from cm in db.CategoryMapping
        //                        join s in db.SessionAution on cm.TargetId equals s.SessionAutionId
        //                        where cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_SESSION
        //                        && cm.CategoryId == -1
        //                        && cm.Status != (int)Const.Status.DELETED
        //                        && s.Status == (int)Const.Status.NORMAL
        //                        select new
        //                        {
        //                            s.SessionAutionId,
        //                            s.Url,
        //                            s.Name,
        //                            cm.Location
        //                        }).OrderBy(e => e.Location).ToList();

        //            var result = new List<int>();
        //            foreach(var item in data)
        //            {
        //                result.Add(item.SessionAutionId);
        //            }


        //            def.meta = new Meta(200, "Success");
        //            def.data = result;
        //            return Ok(def);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Xin vui lòng thừ lại sau!");
        //        return Ok(def);
        //    }
        //}

        //[HttpPut("SaveHighlightNews")]
        //public async Task<IActionResult> SaveHighlightNews([FromBody] List<int> data)
        //{
        //    DefaultResponse def = new DefaultResponse();
        //    //check role
        //    var identity = (ClaimsIdentity)User.Identity;
        //    string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
        //    if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
        //    {
        //        def.meta = new Meta(222, "Bạn không có quyền truy cập tới mục này!");
        //        return Ok(def);
        //    }
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            def.meta = new Meta(400, "Lỗi sai dữ liệu!");
        //            return Ok(def);
        //        }

        //        using (var db = new CNTTVNWebContext())
        //        {
        //            using (var transaction = db.Database.BeginTransaction())
        //            {
        //                var list = db.CategoryMapping.Where(cm => cm.CategoryId == -1 && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_SESSION && cm.Status != (int)Const.Status.DELETED).ToList();
        //                list.ForEach(a => a.Status = (int)Const.Status.DELETED);
        //                if(data != null)
        //                {
        //                    int k = 1;
        //                    foreach(var item in data)
        //                    {
        //                        CategoryMapping categoryMapping = new CategoryMapping();
        //                        categoryMapping.CategoryId = -1;
        //                        categoryMapping.TargetId = item;
        //                        categoryMapping.Location = k;
        //                        categoryMapping.TargetType = (int)Const.TypeCategoryMapping.CATEGORY_SESSION;
        //                        categoryMapping.CreatedAt = DateTime.Now;
        //                        categoryMapping.Status = (int)Const.Status.NORMAL;
        //                        k++;
        //                        db.CategoryMapping.Add(categoryMapping);
        //                    }
        //                }

        //                try
        //                {
        //                    await db.SaveChangesAsync();
        //                    transaction.Commit();
        //                    def.meta = new Meta(200, "Lưu tin tức nổi bật thành công!");
        //                    def.data = data;
        //                    return Ok(def);

        //                }
        //                catch (DbUpdateException e)
        //                {
        //                    log.Error("DbUpdateException:" + e);
        //                    transaction.Rollback();
        //                    def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
        //                    return Ok(def);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error:" + e);
        //        def.meta = new Meta(500, "Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
        //        return Ok(def);
        //    }
        //}

        private bool NewsExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.News.Count(e => e.NewsId == id) > 0;
            }
        }
    }
}


