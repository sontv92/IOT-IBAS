using IOITWebApp;
using IOITWebApp.Models;
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
    public class BankController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("bank", "bank");
        private static string functionCode = "QLX";

        // GET: api/Bank
        [HttpGet("GetByPage")]
        public IActionResult GetByPage(int userid)
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
            if (userid > 0)
            {
                using (var db = new CNTTVNWebContext())
                {
                    def.meta = new Meta(200, "Success");
                    var user = db.User.Find(userid);
                    var UserRole = db.UserRole.Where(c => c.UserId == userid).FirstOrDefault();
                    if (UserRole.RoleId == 3)
                    {
                        var branch = db.Branch.Where(c => c.CompanyId == user.CompanyId).Where(e => e.Status == (int)Const.Status.NORMAL);
                        def.data = branch.ToList();
                        return Ok(def);
                    }
                    else
                    {
                        if(user.BranchId != null && user.BranchId != "")
                        {
                            var branchs = new List<Branch>();
                            string[] arrListStr = user.BranchId.Split(',');
                            foreach (var item in arrListStr)
                            {
                                var branch = db.Branch.Where(c => c.BranchId == Convert.ToInt32(item)).Where(e => e.Status == (int)Const.Status.NORMAL).FirstOrDefault();
                                if (branch != null)
                                {
                                    branchs.Add(branch);
                                }
                               
                            }

                            def.data = branchs;
                            return Ok(def);
                        }
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
    }
    public class BranchIdDTO
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string PMQLXe { get; set; }
        public string QLCamera { get; set; }
    }
}


