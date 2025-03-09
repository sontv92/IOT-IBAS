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
    public class CategoryController : ControllerBase
    {
        //private CNTTVNWebContext db = new CNTTVNWebContext();
        private static readonly ILog log = LogMaster.GetLogger("category", "category");
        private static string functionCode = "QLDM";

        // GET: api/Category
        //[AllowAnonymous]
        [HttpGet("GetByPage")]
        public IActionResult GetByPage([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                //check role
                var identity = (ClaimsIdentity)User.Identity;
                //string Email = identity.Claims.Where(c => c.Type == "email").Select(c => c.Value).SingleOrDefault();
                //string Jti = identity.Claims.Where(c => c.Type == "jti").Select(c => c.Value).SingleOrDefault();
                //string UserId = identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault();
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
                        IQueryable<Category> data = db.Category.Where(c => c.Status != (int)Const.Status.DELETED);
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
                                data = data.OrderBy("CategoryId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                                data = data.OrderBy("CategoryId desc");
                            }
                        }

                        if (paging.select != null && paging.select != "")
                        {
                            paging.select = "new(" + paging.select + ")";
                            paging.select = HttpUtility.UrlDecode(paging.select);
                            def.data = data.Select(paging.select);
                        }
                        else
                            def.data = data.Select(e => new
                            {
                                e.CategoryId,
                                e.Name,
                                e.Code,
                                e.CategoryParentId,
                                e.Description,
                                e.Contents,
                                e.Url,
                                e.Image,
                                e.Icon,
                                e.IconFa,
                                e.IconText,
                                e.Location,
                                e.TypeCategoryId,
                                e.LanguageId,
                                e.CreatedAt,
                                e.UpdatedAt,
                                e.UserId,
                                e.MetaTitle,
                                e.MetaKeyword,
                                e.MetaDescription,
                                e.Status,
                                e.NumberDisplayMobile,
                                categoryParent = db.Category.Where(c => c.CategoryId == e.CategoryParentId).Select(c => new {
                                    c.CategoryId,
                                    c.Name
                                }).FirstOrDefault()
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
            catch (Exception ex)
            {
                log.Error("Exception:" + ex);
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(int id)
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
                    Category data = await db.Category.FindAsync(id);

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

        [HttpGet("listNews/{idc}")]
        public async Task<ActionResult> ListNews([FromRoute] int idc)
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
                    var data = await (from cm in db.CategoryMapping
                                      join n in db.News on cm.TargetId equals n.NewsId
                                      where cm.CategoryId == idc
                                         && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_NEWS
                                         && cm.Status != (int)Const.Status.DELETED
                                      select new
                                      {
                                          cm.CategoryMappingId,
                                          cm.CategoryId,
                                          cm.TargetId,
                                          cm.TargetType,
                                          cm.Location,
                                          cm.CreatedAt,
                                          cm.Status,
                                          n.Title
                                      }).OrderByDescending(e => e.Location).ToListAsync();

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
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("listProduct/{idc}")]
        public async Task<ActionResult> ListProduct([FromRoute] int idc)
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
                    var data = await (from cm in db.CategoryMapping
                                      join p in db.Product on cm.TargetId equals p.ProductId
                                      where cm.CategoryId == idc
                                         && cm.TargetType == (int)Const.TypeCategoryMapping.CATEGORY_PRODUCT
                                         && cm.Status != (int)Const.Status.DELETED
                                      select new
                                      {
                                          cm.CategoryMappingId,
                                          cm.CategoryId,
                                          cm.TargetId,
                                          cm.TargetType,
                                          cm.Location,
                                          cm.CreatedAt,
                                          cm.Status,
                                          p.Name
                                      }).OrderByDescending(e => e.Location).ToListAsync();

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
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpPut("sortCategoryMapping/{idc}")]
        public async Task<ActionResult> SortCategoryMapping([FromRoute] int idc, [FromBody] List<CategoryMapping> data)
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
                if (!ModelState.IsValid || data.Count <= 0 || data == null)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var cate = await db.Category.FindAsync(idc);
                        if (cate == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        try
                        {
                            db.UpdateRange(data);
                            await db.SaveChangesAsync();

                            transaction.Commit();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!CategoryExists(cate.CategoryId))
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

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCategory(int id, [FromBody] CategoryDTO data)
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

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                using (var db = new CNTTVNWebContext())
                {
                    //var checkExist = db.Category.Where(c => c.Code == data.Name && c.CategoryParentId == 0 && c.Status != (int)Const.Status.DELETED && c.CategoryId != id).FirstOrDefault();
                    //if (checkExist != null)
                    //{
                    //    def.meta = new Meta(213, "Code Exist!");
                    //    return Ok(def);
                    //}

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        var cate = db.Category.Find(id);
                        if (cate == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        cate.Name = data.Name;
                        cate.Code = data.Code;
                        cate.CategoryParentId = data.CategoryParentId != null ? (int)data.CategoryParentId : 0;
                        cate.Description = data.Description;
                        cate.Contents = data.Contents;
                        cate.Url = data.Url;
                        cate.Image = data.Image;
                        cate.Icon = data.Icon;
                        cate.IconFa = data.IconFa;
                        cate.IconText = data.IconText;
                        cate.TypeCategoryId = data.TypeCategoryId;
                        cate.LanguageId = data.LanguageId;
                        cate.MetaTitle = data.MetaTitle;
                        cate.MetaKeyword = data.MetaKeyword;
                        cate.MetaDescription = data.MetaDescription;
                        cate.Location = data.Location;
                        cate.UserId = data.UserId;
                        cate.UpdatedAt = DateTime.Now;
                        cate.NumberDisplayMobile = data.NumberDisplayMobile;
                        db.Entry(cate).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            transaction.Commit();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!CategoryExists(cate.CategoryId))
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

        //PUT Status
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
                    Category data = await db.Category.FindAsync(id);
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

                            if (data.CategoryId > 0)
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
                            if (!CategoryExists(data.CategoryId))
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

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            int websiteId = int.Parse(identity.Claims.Where(c => c.Type == "WebsiteId").Select(c => c.Value).SingleOrDefault());
            int languageId = int.Parse(identity.Claims.Where(c => c.Type == "LanguageId").Select(c => c.Value).SingleOrDefault());
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

                if (companyId != data.CompanyId || userId != data.UserId)
                {
                    def.meta = new Meta(400, "Bad Request");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }

                if (data.TypeCategoryId == null || data.TypeCategoryId < 0)
                {
                    def.meta = new Meta(211, "Name Null!");
                    return Ok(def);
                }


                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Category cate = new Category();
                        cate.Name = data.Name;
                        cate.Code = data.Code;
                        cate.CategoryParentId = data.CategoryParentId != null ? (int)data.CategoryParentId : 0;
                        cate.Description = data.Description;
                        cate.Contents = data.Contents;
                        cate.Url = data.Url == null ? Utils.NonUnicode(data.Name) : data.Url;
                        cate.Image = data.Image;
                        cate.Icon = data.Icon;
                        cate.IconFa = data.IconFa;
                        cate.IconText = data.IconText;
                        cate.Location = data.Location == null ? db.Category.ToList().Count : data.Location;
                        cate.TypeCategoryId = data.TypeCategoryId;
                        cate.LanguageId = data.LanguageId == null ? languageId : data.LanguageId;
                        cate.WebsiteId = data.WebsiteId == null ? websiteId : data.WebsiteId;
                        cate.CompanyId = data.CompanyId == null ? companyId : data.CompanyId;
                        cate.MetaTitle = data.MetaTitle;
                        cate.MetaKeyword = data.MetaKeyword;
                        cate.MetaDescription = data.MetaDescription;
                        cate.CreatedAt = DateTime.Now;
                        cate.UpdatedAt = DateTime.Now;
                        cate.UserId = data.UserId;
                        cate.NumberDisplayMobile = data.NumberDisplayMobile;
                        cate.Status = (int)Const.Status.NORMAL;
                        db.Category.Add(cate);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.CategoryId = cate.CategoryId;

                            if (data.CategoryId > 0)
                            {
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
                            if (CategoryExists((int)data.CategoryId))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
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
                bool check = DeleteCategoryFunc(id);
                if (check == false)
                {
                    def.meta = new Meta(404, "Not Found");
                    def.data = check;
                    return Ok(def);
                }
                else
                {
                    def.meta = new Meta(200, "Success");
                    def.data = check;
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

        private bool DeleteCategoryFunc(int CategoryId)
        {
            using (var db = new CNTTVNWebContext())
            {
                Category data = db.Category.Find(CategoryId);
                if (data == null)
                {
                    return false;
                }

                try
                {
                    //Xóa danh mục hiện tại
                    data.UpdatedAt = DateTime.Now;
                    data.Status = (int)Const.Status.DELETED;

                    //Xóa các mapping của danh mục
                    var listMapping = db.CategoryMapping.Where(cm => cm.CategoryId == CategoryId && cm.Status != (int)Const.Status.DELETED).ToList();
                    listMapping.ForEach(cm => cm.Status = (int)Const.Status.DELETED);

                    //xóa danh mục con của danh mục hiện tại
                    var listChild = db.Category.Where(c => c.CategoryParentId == CategoryId && c.Status != (int)Const.Status.DELETED).ToList();
                    if (listChild.Count() > 0)
                    {
                        foreach (var item in listChild)
                        {
                            DeleteCategoryFunc(item.CategoryId);
                        }
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }
        }

        // GET by Tree
        [HttpGet("GetByTree")]
        public IActionResult GetByTree([FromQuery] int[] arr)
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
                List<SmallCategoryDTO> list = new List<SmallCategoryDTO>();
                var query = "";
                foreach (var type in arr)
                {
                    query += "TypeCategoryId=" + type + " OR ";
                }

                var data = GetByTreeFunction(list, 0, 1, query, "");
                def.data = data;
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

        private List<SmallCategoryDTO> GetByTreeFunction(List<SmallCategoryDTO> list, int CategoryParentId, int level, string query, string genealogy)
        {
            var index = level + 1;
            var q = "";
            if (query != "" && CategoryParentId == 0)
            {
                int lastIndexOf = query.LastIndexOf(" OR ");
                q = query.Substring(0, lastIndexOf);
                q = "CategoryParentId=" + CategoryParentId + " AND Status!=99 AND (" + q + ")";
            }
            else
            {
                q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
            }

            using (var db = new CNTTVNWebContext())
            {
                var data = db.Category.Where(q).Select(e => new SmallCategoryDTO
                {
                    CategoryId = e.CategoryId,
                    Code = e.Code,
                    Name = e.Name,
                    CategoryParentId = e.CategoryParentId,
                    Status = e.Status,
                    Level = level,
                    Check = false
                }).ToList();

                foreach (SmallCategoryDTO dt in data)
                {
                    String strg = genealogy;
                    strg += dt.CategoryParentId.ToString() + "_";
                    dt.Genealogy = strg;
                    list.Add(dt);
                    GetByTreeFunction(list, dt.CategoryId, index, query, strg);
                }
            }

            return list;
        }

        private bool CategoryExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Category.Count(e => e.CategoryId == id) > 0;
            }
        }

        #region Sắp xếp danh mục menu bằng cách kéo thả

        [HttpGet("GetCategorySort")]
        public IActionResult GetCategorySort([FromQuery] int[] arr, string txtSearch)
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
                var query = "";
                int cs = 1;
                int idx = 0;
                foreach (var type in arr)
                {
                    query += "TypeCategoryId=" + type + " OR ";
                }


                if(txtSearch != null && txtSearch != "")
                {
                    int lastIndexOf = query.LastIndexOf(" OR ");
                    query = query.Substring(0, lastIndexOf);
                    query = "Name.Contains(\"" + txtSearch + "\") AND (" + query + ")";
                    cs = 2;
                }

                var data = GetCategorySortFunction(0, query, 0, "—", cs, idx);
                def.data = data.categorySorts;
                def.metadata = data.Sum;
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

        private FullCategorySort GetCategorySortFunction(int CategoryParentId, string query, int Level, string CategoryParentName, int cs, int index)
        {
            var q = "";
            if(cs == 1)
            {
                if (query != "" && CategoryParentId == 0)
                {
                    int lastIndexOf = query.LastIndexOf(" OR ");
                    q = query.Substring(0, lastIndexOf);
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99 AND (" + q + ")";
                }
                else
                {
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
                }
            }
            else
            {
                if(query != "" && CategoryParentId == 0)
                {
                    q = query + " AND Status!=99";
                }
                else
                {
                    q = "CategoryParentId=" + CategoryParentId + " AND Status!=99";
                }
            }

            using (var db = new CNTTVNWebContext())
            {
                FullCategorySort obj = new FullCategorySort();
                var data = db.Category.Where(q).Select(e => new CategorySort
                {
                    CategoryId = e.CategoryId,
                    Name = e.Name,
                    Location = e.Location,
                    Level = Level,
                    CategoryParentName = CategoryParentName,
                    Image = e.Image,
                    Url = e.Url,
                    Descriptions = e.Description,
                }).OrderBy(e => e.Location).ToList();

                foreach (var item in data)
                {
                    index = index + 1;
                    var child = GetCategorySortFunction(item.CategoryId, query, Level + 1, item.Name, cs, index);
                    index = (int)child.Sum;
                    item.categorySorts = child.categorySorts;
                }

                log.Error("Index:" + index);
                obj.categorySorts = data;
                obj.Sum = index;
                return obj;
            }
        }

        [HttpPost("SaveCategorySort")]
        public async Task<IActionResult> SaveCategorySort([FromBody] List<CategorySort> data)
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

            if (data == null)
            {
                def.meta = new Meta(400, "Bad request");
                return Ok(def);
            }

            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            Category category = db.Category.Where(c => c.CategoryId == item.CategoryId && c.Status != (int)Const.Status.DELETED).FirstOrDefault();
                            if (category != null)
                            {
                                category.CategoryParentId = item.CategoryParentId != null ? (int)item.CategoryParentId : 0;
                                category.Location = item.Location;
                                db.Update(category);
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        def.meta = new Meta(200, "Sắp xếp thành công!");
                        def.data = "Success";
                        return Ok(def);
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

        #endregion

    }
}