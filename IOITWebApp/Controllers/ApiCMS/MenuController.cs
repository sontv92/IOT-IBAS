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
    public class MenuController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("menu", "menu");
        private static string functionCode = "QLMN";

        // GET: api/Menu
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
                    IQueryable<Menu> data = db.Menu.Where(c => c.Status != (int)Const.Status.DELETED);
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
                            data = data.OrderBy("MenuId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("MenuId desc");
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
                        //trả về kiểu cha con
                        def.data = data.ToList();
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

        // GET: api/Menu/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
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
                    Menu data = await db.Menu.FindAsync(id);

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

        // PUT: api/Menu/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu(int id, MenuDT data)
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

                if (data.Code == null || data.Code == "")
                {
                    def.meta = new Meta(211, "Code Null!");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(212, "Name Null!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Entry(data).State = EntityState.Modified;
                        var menu = db.Menu.Find(id);

                        if (menu == null)
                        {
                            def.meta = new Meta(404, "Not found!");
                            return Ok(def);
                        }

                        menu.Code = data.Code;
                        menu.Name = data.Name;
                        menu.Note = data.Note;
                        menu.LanguageId = data.LanguageId;
                        menu.WebsiteId = data.WebsiteId;
                        menu.CompanyId = data.CompanyId;
                        menu.UserId = data.UserId;
                        menu.UpdatedAt = DateTime.Now;

                        db.Entry(menu).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            var list = db.MenuItem.Where(mi => mi.MenuId == data.MenuId && mi.Status != (int)Const.Status.DELETED).ToList();
                            list.ForEach(l => l.Status = (int)Const.Status.DELETED);
                            await db.SaveChangesAsync();
                            CreateMenu(data.listMenuItem, data.MenuId, 0);

                            transaction.Commit();
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!MenuExists(data.MenuId))
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

        // POST: api/Menu
        [HttpPost]
        public async Task<IActionResult> PostMenu([FromBody] MenuDT data)
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

                if (data.Code == null || data.Code == "")
                {
                    def.meta = new Meta(211, "Code Null!");
                    return Ok(def);
                }

                if (data.Name == null || data.Name == "")
                {
                    def.meta = new Meta(212, "Name Null!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Menu.Add(data);
                        Menu menu = new Menu();
                        menu.Code = data.Code;
                        menu.Name = data.Name;
                        menu.Note = data.Note;
                        menu.LanguageId = data.LanguageId;
                        menu.WebsiteId = data.WebsiteId;
                        menu.CompanyId = data.CompanyId;
                        menu.UserId = data.UserId;
                        menu.CreatedAt = DateTime.Now;
                        menu.UpdatedAt = DateTime.Now;
                        menu.Status = (int)Const.Status.NORMAL;
                        db.Menu.Add(menu);
                        try
                        {
                            await db.SaveChangesAsync();
                            data.MenuId = menu.MenuId;

                            CreateMenu(data.listMenuItem, data.MenuId, 0);

                            //foreach(var item in data.listMenuItem)
                            //{
                            //    MenuItem menuItem = new MenuItem();
                            //    menuItem.CategoryId = item.CategoryId;
                            //    menuItem.MenuId = data.MenuId;
                            //    menuItem.MenuParentId = item.MenuParentId;
                            //    menuItem.CreatedAt = DateTime.Now;
                            //    menuItem.UpdatedAt = DateTime.Now;
                            //    menuItem.UserId = data.UserId;
                            //    menuItem.Status = (int)Const.Status.NORMAL;

                            //    db.MenuItem.Add(menuItem);
                            //}
                            //await db.SaveChangesAsync();

                            if (data.MenuId > 0)
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
                            if (MenuExists(data.MenuId))
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

        private void CreateMenu(List<MenuItemDT> list, int MenuId, int MenuParentId)
        {
            using (var db = new CNTTVNWebContext())
            {
                //List<MenuChildren> menus = new List<MenuChildren>();
                for(var i = 0; i < list.Count; i ++)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.CategoryId = list[i].id;
                    menuItem.MenuId = MenuId;
                    menuItem.MenuParentId = MenuParentId;
                    menuItem.Location = i + 1;
                    menuItem.CreatedAt = DateTime.Now;
                    menuItem.UpdatedAt = DateTime.Now;
                    menuItem.Status = (int)Const.Status.NORMAL;

                    db.MenuItem.Add(menuItem);
                    db.SaveChanges();

                    if(list[i].children != null)
                    {
                        int ParentId = menuItem.MenuItemId;
                        CreateMenu(list[i].children, MenuId, ParentId);
                    }
                }
            }
        }

        //private List<MenuChildren> CreateMenuItem(List<MenuChildren> list, int CategoryId, int MenuId, int MenuParentId)
        //{
        //    using (var db = new CNTTVNWebContext())
        //    {
        //        var listMenu = db.MenuItem.Where(mi => mi.CategoryId == CategoryId && mi.MenuId == MenuId && mi.MenuParentId == MenuParentId && mi.Status != (int)Const.Status.DELETED).ToList();
        //        if (listMenu.Count > 0)
        //        {
        //            //List<MenuChildren> menus = new List<MenuChildren>();
        //            foreach (var item in listMenu)
        //            {
        //                MenuChildren menu = new MenuChildren();
        //                menu.MenuItemId = item.MenuItemId;
        //                menu.CategoryId = item.CategoryId;
        //                menu.MenuId = item.MenuId;
        //                menu.MenuParentId = item.MenuParentId;
        //                menu.Children = CreateMenuItem(menu.Children, (int)menu.MenuId, (int)menu.CategoryId, (int)menu.MenuParentId);
        //                list.Add(menu);
        //            }
        //        }

        //        return list;
        //    }
        //}


        // GET by Tree
        [HttpGet("GetCategoryMenu")]
        public async Task<IActionResult> GetCategoryMenu()
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
                    List<CategoryMenu> listOut = new List<CategoryMenu>();
                    //get all category
                    List<CategoryMenu> listIn = await db.Category.Where(e => e.Status == (int)Const.Status.NORMAL)
                        .Select(e => new CategoryMenu
                        {
                            CategoryId = e.CategoryId,
                            CategoryParentId = e.CategoryParentId,
                            Name = e.Name,
                            IsParent = true

                        }).ToListAsync();

                    var data = CreateCategoryMenuTree(listIn, listOut, 0);
                    def.data = data;
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("GetCategoryMenuLeft/{id}")]
        public async Task<IActionResult> GetCategoryMenuLeft([FromRoute] int id)
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
                List<CategoryMenu> listIn = new List<CategoryMenu>();
                List<CategoryMenu> listOut = new List<CategoryMenu>();
                using (var db = new CNTTVNWebContext())
                {
                    //get all category
                    var cate = await db.Category.Where(e => e.Status == (int)Const.Status.NORMAL).ToListAsync();
                    //get all menu item
                    var menu = await db.MenuItem.Where(e => e.MenuId == id && e.Status != (int)Const.Status.DELETED).ToListAsync();

                    foreach (var item in cate)
                    {
                        int categoryId = item.CategoryId;
                        CategoryMenu categoryMenu = new CategoryMenu();
                        //Check nếu chưa có ở menu bên phải thì lấy
                        if (menu.Where(e => e.CategoryId == categoryId).Count() <= 0)
                        {
                            categoryMenu.CategoryId = item.CategoryId;
                            categoryMenu.CategoryParentId = item.CategoryParentId;
                            categoryMenu.Name = item.Name;
                            categoryMenu.IsParent = false;
                            listIn.Add(categoryMenu);
                        }
                        else
                        {
                            //Nếu có ở menu bên phải nhưng là danh mục cha thì check xem có danh mục con nào mà ko
                            //lằm bên menu bên phải ko, nếu có thì giữ lại danh mục cha
                            var cateChild = cate.Where(e => e.CategoryParentId == categoryId).ToList();
                            if(cateChild.Count > 0)
                            {
                                bool check = false;
                                foreach (var itemChild in cateChild)
                                {
                                    if (menu.Where(e => e.CategoryId == itemChild.CategoryId).Count() <= 0)
                                    {
                                        check = true;
                                        break;
                                    }
                                }
                                if (check)
                                {
                                    categoryMenu.CategoryId = item.CategoryId;
                                    categoryMenu.CategoryParentId = item.CategoryParentId;
                                    categoryMenu.Name = item.Name;
                                    categoryMenu.IsParent = true;
                                    listIn.Add(categoryMenu);
                                }
                            }
                        }
                    }


                    var data = CreateCategoryMenuTree(listIn, listOut, 0);
                    def.data = data;
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        [HttpGet("GetCategoryMenuRight/{id}")]
        public IActionResult GetCategoryMenuRight([FromRoute] int id)
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
                List<CategoryMenu> listOut = new List<CategoryMenu>();
                using (var db = new CNTTVNWebContext())
                {
                    List<CategoryMenu> listIn = (from mi in db.MenuItem
                                   join c in db.Category on mi.CategoryId equals c.CategoryId
                                   where c.Status==(int)Const.Status.NORMAL && mi.Status!=(int)Const.Status.DELETED
                                   && mi.MenuId==id
                                   select new CategoryMenu
                                   {
                                       CategoryId=c.CategoryId,
                                       CategoryParentId= (int)mi.MenuParentId,
                                       Name=c.Name,
                                       MenuItemId=mi.MenuItemId,
                                   }).ToList();


                    CategoryMenu categoryMenu = new CategoryMenu();
                    categoryMenu.CategoryId = 0;
                    categoryMenu.Name = "Root";
                    var data = CreateCategoryMenuTreeRight(listIn, listOut, 0);
                    categoryMenu.Children = data;

                    List<CategoryMenu> res = new List<CategoryMenu>();
                    res.Add(categoryMenu);

                    def.data = res;
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, "Internal Server Error");
                return Ok(def);
            }
        }

        private List<CategoryMenu> CreateCategoryMenuTree(List<CategoryMenu> listIn, List<CategoryMenu> listOut, int CategoryParentId)
        {
            using (var db = new CNTTVNWebContext())
            {
                var listMenu = listIn.Where(c => c.CategoryParentId == CategoryParentId).ToList();
                if (listMenu.Count > 0)
                {
                    foreach (var item in listMenu)
                    {
                        CategoryMenu menu = new CategoryMenu();
                        menu.CategoryId = item.CategoryId;
                        menu.Name = item.Name;
                        menu.CategoryParentId = item.CategoryParentId;
                        List<CategoryMenu> listChildren = new List<CategoryMenu>();
                        menu.Children = CreateCategoryMenuTree(listIn, listChildren, item.CategoryId);
                        listOut.Add(menu);
                    }
                }

                return listOut;
            }
        }

        private List<CategoryMenu> CreateCategoryMenuTreeRight(List<CategoryMenu> listIn, List<CategoryMenu> listOut, int CategoryParentId)
        {
            using (var db = new CNTTVNWebContext())
            {
                var listMenu = listIn.Where(c => c.CategoryParentId == CategoryParentId).ToList();
                if (listMenu.Count > 0)
                {
                    foreach (var item in listMenu)
                    {
                        CategoryMenu menu = new CategoryMenu();
                        menu.CategoryId = item.CategoryId;
                        menu.Name = item.Name;
                        menu.CategoryParentId = item.CategoryParentId;
                        List<CategoryMenu> listChildren = new List<CategoryMenu>();
                        menu.Children = CreateCategoryMenuTreeRight(listIn, listChildren, item.MenuItemId);
                        listOut.Add(menu);
                    }
                }

                return listOut;
            }
        }

        //private List<CategoryMenu> CreateCategoryMenu(List<CategoryMenu> list, int CategoryParentId)
        //{
        //    using (var db = new CNTTVNWebContext())
        //    {
        //        var listMenu = db.Category.Where(c => c.CategoryParentId == CategoryParentId && c.Status != (int)Const.Status.DELETED).ToList();
        //        if (listMenu.Count > 0)
        //        {
        //            //List<MenuChildren> menus = new List<MenuChildren>();
        //            foreach (var item in listMenu)
        //            {
        //                CategoryMenu menu = new CategoryMenu();
        //                menu.CategoryId = item.CategoryId;
        //                menu.Name = item.Name;
        //                menu.CategoryParentId = item.CategoryParentId;
        //                List<CategoryMenu> listChildren = new List<CategoryMenu>();
        //                menu.Children = CreateCategoryMenu(listChildren, menu.CategoryId);
        //                list.Add(menu);
        //            }
        //        }

        //        return list;
        //    }
        //}

        // DELETE: api/Menu/5
        //[ResponseType(typeof(Menu))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
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
                    Menu data = await db.Menu.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        //db.Menu.Remove(data);
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.MenuId > 0)
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
                            if (!MenuExists(data.MenuId))
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

        // Get List Cate By Tree

        private bool MenuExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Menu.Count(e => e.MenuId == id) > 0;
            }
        }
    }
}


