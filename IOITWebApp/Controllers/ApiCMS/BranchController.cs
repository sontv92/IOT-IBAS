using IOITWebApp;
using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S7.Net;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("branch", "branch");
        private static string functionCode = "QLTT";
        private readonly IHostingEnvironment _hostingEnvironment;
        public BranchController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/Branch
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
                    IQueryable<Branch> data = db.Branch.Where(c =>
                        c.Status != (int)Const.Status.DELETED &&
                        (
                            // Module = 0: loại TypeTram = 2
                            (paging.Module == 0 || paging.Module == null && (c.TypeTram != 2 || c.TypeTram == null))

                            ||

                            // Module = 1:
                            (paging.Module == 1 &&
                                (
                                    paging.TypeTram == null || paging.TypeTram == 0 || // không lọc
                                    (paging.TypeTram == 1 && (c.TypeTram == 1 || c.TypeTram == null)) || // lọc TypeTram = 1 + null
                                    (paging.TypeTram != 1 && c.TypeTram == paging.TypeTram) // lọc theo các giá trị khác
                                )
                            )
                        )
                    );

                    if (paging.query != null)
                    {
                        paging.query = HttpUtility.UrlDecode(paging.query);
                    }

                    data = data.Where(paging.query);
                    if (!string.IsNullOrEmpty(paging.Branchlist) && paging.Branchlist.Trim() != "null" && paging.Branchlist.Trim() != "undefined")
                    {
                        var branchArr = paging.Branchlist.Split(",");
                        data = data.Where(x => branchArr.Any(y => x.BranchId == int.Parse(y)));
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
                            data = data.OrderBy("BranchId desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("BranchId desc");
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



        // PUT: api/Branch/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, BranchDTO data)
        {

            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.UPDATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền sửa trạm trộn!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if ((userId != data.UserId))
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    Branch branch = db.Branch.Where(b => b.BranchId == id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    string passold = db.Branch.Where(b => b.BranchId == id && b.Status != (int)Const.Status.DELETED).FirstOrDefault().Password;
                    if (branch == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy trạm trộn!");
                        return Ok(def);
                    }


                    Branch exist = db.Branch.Where(b => b.Code == data.Code && b.BranchId != id && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã trạm trộn đã tồn tại!");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        branch.Code = data.Code;
                        branch.Name = data.Name;
                        branch.Avatar = data.Avatar;
                        branch.Email = data.Email;
                        branch.Phone = data.Phone;
                        branch.Address = data.Address;
                        branch.Contents = data.Contents;
                        branch.UserId = userId;
                        branch.Location = data.Location;
                        branch.Lat = data.Lat;
                        branch.Long = data.Long;
                        branch.UpdatedAt = DateTime.Now;
                        branch.CompanyId = data.CompanyId;
                        branch.Username = data.Username;
                        branch.Password = data.Password;
                        branch.PMQLXe = data.PMQLXe;
                        branch.QLCamera = data.QLCamera;

                        db.Entry(branch).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();
                            if (data.Password != passold)
                            {
                                int changepass = changepassuser(data.Username, data.Username, data.Password, passold);
                                if (changepass != -1)
                                {
                                    def.meta = new Meta(400, "Không thể thay đổi mật khẩu !");
                                    return Ok(def);
                                }
                            }
                            if (data.BranchId > 0)
                            {

                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                            def.meta = new Meta(200, "Sửa thành công!");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!BranchExists(branch.BranchId))
                            {
                                def.meta = new Meta(404, "Không tìm thấy trạm trộn!");
                                return Ok(def);
                            }
                            else
                            {
                                def.meta = new Meta(500, "Lỗi máy chủ!");
                                return Ok(def);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        // POST: api/Branch
        [HttpPost]
        public async Task<IActionResult> PostBranch(BranchDTO data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.CREATE))
            {
                def.meta = new Meta(222, "Bạn không có quyền thêm mới trạm trộn!");
                return Ok(def);
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                if (userId != data.UserId)
                {
                    def.meta = new Meta(400, "Lỗi dữ liệu!");
                    return Ok(def);
                }
                using (var db = new CNTTVNWebContext())
                {
                    Branch exist = db.Branch.Where(b => b.Code == data.Code && b.Status != (int)Const.Status.DELETED).FirstOrDefault();
                    if (exist != null)
                    {
                        def.meta = new Meta(212, "Mã trạm trộn đã tồn tại!");
                        return Ok(def);
                    }
                    // Tạo database và user
                    var nameDB = data.Username;
                    if (data.TypeTram == 2)
                        nameDB += "_tramcan_online";
                    else
                        nameDB += "_online";

                    var restore = RestoreDatabase(nameDB, data.TypeTram);
                    if (restore == 1)
                    {
                        def.meta = new Meta(400, "Tên cơ sở dữ liệu đã tồn tại !");
                        return Ok(def);
                    }
                    else if (restore == -1)
                    {
                        var restoreuser = RestoreUser(data.Username, data.Password, nameDB);
                        if (restoreuser == 1)
                        {
                            DropDatabase(data.Username, nameDB);
                            def.meta = new Meta(400, "Tên tài khoản đã tồn tại !");
                            return Ok(def);
                        }
                        if (restoreuser != -1)
                        {
                            DropDatabase(data.Username, nameDB);
                            def.meta = new Meta(400, "Lỗi dữ liệu!");
                            return Ok(def);
                        }
                    }
                    else
                    {
                        def.meta = new Meta(400, "Lỗi dữ liệu!");
                        return Ok(def);
                    }
                    // tạo Branch
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        Branch branch = new Branch();
                        branch.Code = data.Code;
                        branch.Name = data.Name;
                        branch.Avatar = data.Avatar;
                        branch.Email = data.Email;
                        branch.Phone = data.Phone;
                        branch.Address = data.Address;
                        branch.Contents = data.Contents;
                        branch.UserId = userId;
                        branch.Location = data.Location;
                        branch.Lat = data.Lat;
                        branch.Long = data.Long;
                        branch.CreatedAt = DateTime.Now;
                        branch.UpdatedAt = DateTime.Now;
                        branch.Status = (int)Const.Status.NORMAL;
                        branch.CompanyId = data.CompanyId;
                        branch.Dataname = nameDB;
                        branch.Username = data.Username;
                        branch.Password = data.Password;
                        branch.PMQLXe = data.PMQLXe;
                        branch.QLCamera = data.QLCamera;
                        branch.TypeTram = data.TypeTram;
                        db.Branch.Add(branch);

                        try
                        {
                            await db.SaveChangesAsync();
                            data.BranchId = branch.BranchId;

                            if (data.BranchId > 0)
                            {
                                transaction.Commit();
                                def.meta = new Meta(200, "Thêm mới thành công!");
                                def.data = data;
                                return Ok(def);
                            }
                            transaction.Rollback();
                            def.meta = new Meta(400, "Lỗi dữ liệu!");
                            return Ok(def);

                        }
                        catch (DbUpdateException e)
                        {
                            log.Error("DbUpdateException:" + e);
                            transaction.Rollback();
                            if (BranchExists(branch.BranchId))
                            {
                                def.meta = new Meta(211, "Mã trạm trộn đã tồn tại!");
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
                def.meta = new Meta(500, "Lỗi máy chủ!");
                return Ok(def);
            }
        }

        // DELETE: api/Branch/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            int userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.DELETED))
            {
                def.meta = new Meta(222, "Bạn không có quyền xóa trạm trộn!");
                return Ok(def);
            }
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    Branch data = await db.Branch.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Không tìm thấy trạm trộn");
                        return Ok(def);
                    }
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        data.UserId = userId;
                        data.UpdatedAt = DateTime.Now;
                        data.Status = (int)Const.Status.DELETED;
                        db.Entry(data).State = EntityState.Modified;
                        var users = db.User.Where(c => c.BranchId == data.BranchId.ToString()).Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                        foreach (var item in users)
                        {
                            item.UserEditId = userId;
                            item.UpdatedAt = DateTime.Now;
                            item.Status = (int)Const.Status.DELETED;
                            db.Entry(item).State = EntityState.Modified;
                            var userRoles = db.UserRole.Where(e => e.UserId == item.UserId && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item1 in userRoles)
                            {
                                item1.Status = (int)Const.Status.DELETED;
                                db.Entry(item1).State = EntityState.Modified;
                            }

                            //delete function role
                            var functionRoles = db.FunctionRole.Where(e => e.TargetId == item.UserId
                            && e.Type == (int)Const.TypeFunction.FUNCTION_USER
                            && e.Status != (int)Const.Status.DELETED).ToList();
                            foreach (var item2 in functionRoles)
                            {
                                item2.Status = (int)Const.Status.DELETED;
                                item2.UpdatedAt = DateTime.Now;
                                db.Entry(item2).State = EntityState.Modified;
                            }

                        }

                        try
                        {
                            var nameDB = data.Username;
                            if (data.TypeTram == 2)
                                nameDB += "_tramcan_online";
                            else
                                nameDB += "_online";

                            await db.SaveChangesAsync();
                            if (data.BranchId > 0)
                            {
                                DropDatabase(data.Username, nameDB);
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                            def.meta = new Meta(200, "Success");
                            def.data = data;
                            return Ok(def);
                        }
                        catch (DbUpdateException e)
                        {
                            transaction.Rollback();
                            log.Error("DbUpdateException:" + e);
                            if (!BranchExists(data.BranchId))
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

        private bool BranchExists(int id)
        {
            using (var db = new CNTTVNWebContext())
            {
                return db.Branch.Count(e => e.BranchId == id) > 0;
            }
        }

        private int RestoreDatabase(string name, int? typeTram)
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string sqlStmt3 = "";
            try
            {
                var isname = isduplicatedatabase(name);
                if (isname)
                {
                    return 1;
                }
                using (var db = new CNTTVNWebContext())
                {
                    //string dbPath = System.IO.Directory.GetCurrentDirectory();

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    log.Info("webRootPath:" + webRootPath);
                    log.Info("applicationPath:" + applicationPath);
                    string[] strArray = new string[] { webRootPath, @"\", typeTram == 1 ? "TRAMTRON_DNP_Online.bak" : "TRAMCAN_iBas_Online.bak" };
                    string filePath = string.Concat(strArray);

                    //string filePath = dbPath +"/TRAMTRON_Online.bak";

                    string savePath = webRootPath + @"\Data";


                    //string savePath = @"D:\DB test\";

                    if (typeTram == 2)
                        sqlStmt3 = string.Format("RESTORE DATABASE " + name + " FROM DISK = '" + filePath + "'" + @" WITH MOVE 'QUANLYTRAMCAN_TC_Local' TO '" + savePath + name + ".mdf', MOVE 'QUANLYTRAMCAN_TC_Local_log' TO '" + savePath + name + "_log.mdf';");
                    else
                        sqlStmt3 = string.Format("RESTORE DATABASE " + name + " FROM DISK = '" + filePath + "'" + @" WITH MOVE 'QUANLYTAITRAM' TO '" + savePath + name + "_online" + ".mdf', MOVE 'QUANLYTAITRAM_Log' TO '" + savePath + name + "_log.mdf';");


                    //var cmd = String.Format("USE master restore DATABASE QUANLYTAITRAM_Local from DISK='{0}' WITH REPLACE;", dbPath);
                    var data = db.Database.ExecuteSqlCommand(sqlStmt3);
                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                log.Error("applicationPath:" + applicationPath.ToString());
                log.Error("sqlStmt3:" + sqlStmt3.ToString());
                return 0;
            }
        }
        private int RestoreUser(string user, string pass, string dbName)
        {
            try
            {
                var isname = isduplicateuser(user);
                if (isname)
                {
                    return 1;
                }
                using (var db = new CNTTVNWebContext())
                {
                    string sqlStmt4 = string.Format("CREATE LOGIN " + user + " WITH PASSWORD = '" + pass + "';  USE " + dbName + "; CREATE USER " + user + " FOR LOGIN " + user + "; ALTER ROLE db_owner ADD MEMBER " + user + " ;");
                    var data = db.Database.ExecuteSqlCommand(sqlStmt4);
                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                return 0;
            }
        }

        private int DropDatabase(string user, string dbName)
        {
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    string sqlStmt = string.Format("DROP LOGIN " + user + " ; EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'" + dbName + "'  ALTER DATABASE " + dbName + " SET  SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE " + dbName);
                    var data = db.Database.ExecuteSqlCommand(sqlStmt);
                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                return 0;
            }
        }

        public static bool isduplicatedatabase(string name)
        {
            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT name from sys.databases";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        if ((string)result["name"] == name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool isduplicateuser(string name)
        {
            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT name FROM sys.server_principals";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        if ((string)result["name"] == name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public int changepassuser(string dataname, string user, string pass, string passold)
        {
            try
            {
                using (var db = new CNTTVNWebContext())
                {
                    string sqlStmt = "USE " + dataname + "_online" + " ALTER LOGIN " + user + " WITH PASSWORD = '" + pass + "' OLD_PASSWORD = '" + passold + "'";
                    var data = db.Database.ExecuteSqlCommand(sqlStmt);
                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error:" + e);
                return 0;
            }
        }

    }
}


