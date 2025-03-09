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
using System.Web.Http;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("error", "error");
        private static string functionCode = "BCDH";

        // GET: api/OrderItem
        [HttpGet("GetByPage")]
        public IActionResult GetByPageAsync([FromQuery] FilteredPagination paging)
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
                using (var context = new CNTTVNWebContext())
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {

                    List<rpdonhangBranchDTO> rpdonhang = new List<rpdonhangBranchDTO>();
                    command.CommandText = "  SELECT METKHOIDATHANG,METKHOITICHLUY,Name FROM ";
                    command.CommandText += "(";
                    if (paging.Branchlist != "" && paging.Branchlist != null)
                    {
                        var arrListStr = paging.Branchlist.Split(',');
                        int i = 0;
                        foreach (var item in arrListStr)
                        {
                            if (item != "")
                            {
                                Branch branch =  context.Branch.Find(Convert.ToInt32(item));
                                if (i == 0)
                                {
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                            }
                            ++i;
                        }
                    }
                    else
                    {
                        if (paging.companyid == 0)
                        {
                            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).ToList();
                            if (branchlist.Count() == 0)
                            {
                                def.data = null;
                                def.metadata = 0;
                                def.meta = new Meta(200, "Success");
                                return Ok(def);
                            }
                            int j = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (j == 0)
                                {
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                                ++j;
                            }
                        }
                        else
                        {
                            List<Branch> branchlist = context.Branch.Where(c => c.Status != (int)Const.Status.DELETED).Where(x => x.CompanyId == paging.companyid).ToList();
                            if (branchlist.Count() == 0)
                            {
                                def.data = null;
                                def.metadata = 0;
                                def.meta = new Meta(200, "Success");
                                return Ok(def);
                            }
                            int k = 0;
                            foreach (var item in branchlist)
                            {
                                Branch branch = context.Branch.Find(item.BranchId);
                                if (k == 0)
                                {
                                    command.CommandText += "SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                                else
                                {
                                    command.CommandText += " UNION ALL SELECT SUM(sa.[METKHOIDATHANG]) as METKHOIDATHANG,SUM(sa.METKHOITICHLUY) as METKHOITICHLUY,br.Name FROM [" + branch.Dataname + "].[dbo].[DATHANG] sa INNER JOIN  Branch br ON br.Dataname = '" + branch.Dataname + "' GROUP BY br.Name";
                                }
                                ++k;
                            }
                        }

                    }
                    command.CommandText += ") rpdonhang";
                    if (paging.query != null)
                    {
                        command.CommandText += " WHERE " + HttpUtility.UrlDecode(paging.query);
                    }
                    context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            rpdonhangBranchDTO item = new rpdonhangBranchDTO();
                            item.METKHOIDATHANG = (Single)result["METKHOIDATHANG"];
                            item.METKHOITICHLUY = (Single)result["METKHOITICHLUY"];
                            item.Name = (string)result["Name"];
                            rpdonhang.Add(item);
                        }

                        def.data = rpdonhang;
                    }
                    def.meta = new Meta(200, "Success");
                    return Ok(def);
                }
            }
            else
            {
                def.meta = new Meta(400, "Bad Request");
                return Ok(def);
            }
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItem(int id)
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
                using (var db=new CNTTVNWebContext())
                {
                    OrderItem data = await db.OrderItem.FindAsync(id);

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

        // PUT: api/OrderItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItem data)
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

                using(var db=new CNTTVNWebContext())
                {
                    using(var transaction = db.Database.BeginTransaction())
                    {
                        db.Entry(data).State = EntityState.Modified;
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderItemId > 0)
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
                            if (!OrderItemExists(data.OrderItemId))
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

        // POST: api/OrderItem
        [HttpPost]
        public async Task<IActionResult> PostOrderItem(OrderItem data)
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

                //if (data.OrderId == null || data.OrderId == "")
                //{
                //    def.meta = new Meta(211, "OrderId Null!");
                //    return Ok(def);
                //}

                //if (data.ProductId == null || data.ProductId == "")
                //{
                //    def.meta = new Meta(211, "ProductId Null!");
                //    return Ok(def);
                //}

                //if (data.Quantity == null || data.Quantity == "")
                //{
                //    def.meta = new Meta(211, "Quantity Null!");
                //    return Ok(def);
                //}

                //if (data.Price == null || data.Price == "")
                //{
                //    def.meta = new Meta(211, "Price Null!");
                //    return Ok(def);
                //}
                using (var db=new CNTTVNWebContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.OrderItem.Add(data);

                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderItemId > 0)
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
                            if (OrderItemExists(data.OrderItemId))
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

        // DELETE: api/OrderItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
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
                using(var db=new CNTTVNWebContext())
                {
                    OrderItem data = await db.OrderItem.FindAsync(id);
                    if (data == null)
                    {
                        def.meta = new Meta(404, "Not Found");
                        return Ok(def);
                    }

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.OrderItem.Remove(data);
                        try
                        {
                            await db.SaveChangesAsync();

                            if (data.OrderItemId > 0)
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
                            if (!OrderItemExists(data.OrderItemId))
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

        private bool OrderItemExists(int id)
        {
            using(var db=new CNTTVNWebContext())
            {
                return db.OrderItem.Count(e => e.OrderItemId == id) > 0;
            }
        }
    }
}


