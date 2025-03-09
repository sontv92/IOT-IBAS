using IOITWebApp.Models;
using IOITWebApp.Models.Data;
using IOITWebApp.Models.EF;
using IOITWebApp.Models.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IOITWebApp.ApiCMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CuaVLController : ControllerBase
    {
        private static readonly ILog log = LogMaster.GetLogger("slide", "slide");
        private static string functionCode = "QLTT";
        private IHostingEnvironment _hostingEnvironment;

        public CuaVLController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("{branchid}")]
        public IActionResult GetTenCuaVatLieu(int branchid)
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

            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<VatLieuDTO> rpdonhang = new List<VatLieuDTO>();
                Branch branch = context.Branch.Find(branchid);
                command.CommandText += "SELECT dh.TENCUAVL, dh.MACUAVL, dh.TRANGTHAI FROM [" + branch.Dataname + "].[dbo].[CUAVL] dh";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    var j = 1;
                    while (result.Read())
                    {
                        VatLieuDTO item = new VatLieuDTO();
                        item.STT = (long)j;
                        item.MACUAVL = (int)result["MACUAVL"];
                        item.TRANGTHAI = (bool)result["TRANGTHAI"];
                        if (!item.TRANGTHAI)
                        {
                            item.VALUE = 0;
                        }
                        if (result["TENCUAVL"] is System.DBNull)
                        {
                            item.TENCUAVL = "";

                        }
                        else
                        {
                            item.TENCUAVL = (string)result["TENCUAVL"];
                        }
                        rpdonhang.Add(item);
                        j++;
                    }
                    def.data = rpdonhang;
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }
        [HttpGet("gettencuavlactive/{branchid}")]
        public IActionResult GetTenCuaVatLieuActive(int branchid)
        {
            DefaultResponse def = new DefaultResponse();
            if (branchid == null)
            {
                def.meta = new Meta(222, "Vui lòng chọn trạm");
                return Ok(def);
            }
            //check role
            var identity = (ClaimsIdentity)User.Identity;
            string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)Const.Action.VIEW))
            {
                def.meta = new Meta(222, "No permission");
                return Ok(def);
            }

            using (var context = new CNTTVNWebContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                List<VatLieuDTO> rpdonhang = new List<VatLieuDTO>();
                Branch branch = context.Branch.Find(branchid);
                command.CommandText += "SELECT dh.TENCUAVL, dh.MACUAVL FROM [" + branch.Dataname + "].[dbo].[CUAVL] dh WHERE dh.TRANGTHAI = 1";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        VatLieuDTO item = new VatLieuDTO()
                        {
                            TENCUAVL = result["TENCUAVL"] != DBNull.Value ? (string)result["TENCUAVL"] : "",
                            MACUAVL = (int)result["MACUAVL"]

                        };
                        rpdonhang.Add(item);
                    }
                    def.data = rpdonhang;
                }
                def.meta = new Meta(200, "Success");
                return Ok(def);
            }
        }
    }
}
